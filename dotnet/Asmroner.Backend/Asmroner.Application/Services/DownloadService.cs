using System.Text;
using System.Collections.Concurrent;
using System.Globalization;
using Asmroner.Core.Api;
using Asmroner.Core.Download;
using Asmroner.Core.Interfaces;

namespace Asmroner.Application.Services;

public sealed class DownloadService : IDownloadService
{
    private static readonly string[] TextSidecarExtensions = [".txt", ".lrc", ".ass"];

    private readonly IAsmrApiClient _apiClient;
    private readonly IConfigurationService _configurationService;
    private readonly ISearchStateStore _searchStateStore;
    private readonly IAppPathService _appPathService;
    private readonly IRateLimiterService _rateLimiterService;
    private readonly IWorkInfoCache _workInfoCache;
    private readonly object _stateLock = new();
    private readonly List<DownloadTaskItem> _tasks = new();
    private readonly Dictionary<Guid, CancellationTokenSource> _taskCancellationSources = new();

    public DownloadService(
        IAsmrApiClient apiClient,
        IConfigurationService configurationService,
        ISearchStateStore searchStateStore,
        IAppPathService appPathService,
        IRateLimiterService rateLimiterService,
        IWorkInfoCache workInfoCache)
    {
        _apiClient = apiClient;
        _configurationService = configurationService;
        _searchStateStore = searchStateStore;
        _appPathService = appPathService;
        _rateLimiterService = rateLimiterService;
        _workInfoCache = workInfoCache;
    }

    public async Task<IReadOnlyList<DownloadTaskItem>> RunQueuedAsync(string? fileFilter = null, bool hdAudioOnly = false, CancellationToken cancellationToken = default)
    {
        var sourceIds = _searchStateStore.GetQueuedSourceIds();
        if (sourceIds.Count == 0)
        {
            return Array.Empty<DownloadTaskItem>();
        }

        var config = await _configurationService.LoadAsync(cancellationToken);
        var preferExtensions = DownloadFilterParser.ParsePreferExtensions(config?.Downloader);
        var parsedFilter = DownloadFilterParser.ParseFileFilter(fileFilter);
        var targetRoot = ResolveTargetRoot(config?.Downloader.SyncDataFolder);
        var maxWorkers = NormalizeMaxWorkers(config?.Downloader.MaxWorkers ?? 4);

        var created = new List<DownloadTaskItem>(sourceIds.Count);

        foreach (var sourceId in sourceIds)
        {
            cancellationToken.ThrowIfCancellationRequested();

            lock (_stateLock)
            {
                var task = ResolveStartTargetTask(sourceId, preferredTaskId: null);
                if (!_tasks.Contains(task))
                {
                    _tasks.Add(task);
                }

                created.Add(task);
            }
        }

        using var workerLimiter = new SemaphoreSlim(maxWorkers);
        var jobs = created.Select(async task =>
        {
            await workerLimiter.WaitAsync(cancellationToken);
            try
            {
                if (task.Status == DownloadTaskStatus.Canceled)
                {
                    return;
                }

                await RunSingleAsync(task, targetRoot, preferExtensions, parsedFilter, hdAudioOnly, config, cancellationToken);
            }
            finally
            {
                workerLimiter.Release();
            }
        });

        await Task.WhenAll(jobs);

        _searchStateStore.RemoveFromQueue(sourceIds);
        return created;
    }

    public async Task<DownloadTaskItem?> StartAsync(string sourceId, string? fileFilter = null, Guid? preferredTaskId = null, bool hdAudioOnly = false, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sourceId))
        {
            return null;
        }

        var config = await _configurationService.LoadAsync(cancellationToken);
        var preferExtensions = DownloadFilterParser.ParsePreferExtensions(config?.Downloader);
        var parsedFilter = DownloadFilterParser.ParseFileFilter(fileFilter);
        var targetRoot = ResolveTargetRoot(config?.Downloader.SyncDataFolder);

        DownloadTaskItem task;
        var normalizedSourceId = sourceId.Trim();

        lock (_stateLock)
        {
            task = ResolveStartTargetTask(normalizedSourceId, preferredTaskId);
            if (!_tasks.Contains(task))
            {
                _tasks.Add(task);
            }
        }

        await RunSingleAsync(task, targetRoot, preferExtensions, parsedFilter, hdAudioOnly, config, cancellationToken);
        _searchStateStore.RemoveFromQueue(new[] { task.SourceId });
        return task;
    }

    public IReadOnlyList<DownloadTaskItem> GetTasks()
    {
        lock (_stateLock)
        {
            return _tasks
                .OrderByDescending(static t => t.StartedAt)
                .ToArray();
        }
    }

    public Task<bool> CancelAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_stateLock)
        {
            var task = _tasks.FirstOrDefault(t => t.TaskId != Guid.Empty && t.TaskId == taskId);
            if (task is null)
            {
                return Task.FromResult(false);
            }

            if (task.Status is DownloadTaskStatus.Completed or DownloadTaskStatus.Failed or DownloadTaskStatus.Canceled)
            {
                return Task.FromResult(false);
            }

            if (task.Status == DownloadTaskStatus.Queued)
            {
                MarkTaskCanceled(task);
                return Task.FromResult(true);
            }

            if (_taskCancellationSources.TryGetValue(task.TaskId, out var cts))
            {
                cts.Cancel();
                return Task.FromResult(true);
            }

            MarkTaskCanceled(task);
            return Task.FromResult(true);
        }
    }

    public async Task<DownloadTaskItem?> RetryFailedAsync(Guid taskId, string? fileFilter = null, bool hdAudioOnly = false, CancellationToken cancellationToken = default)
    {
        DownloadTaskItem? task;

        lock (_stateLock)
        {
            task = _tasks.FirstOrDefault(t => t.TaskId != Guid.Empty && t.TaskId == taskId);
            if (task is null || task.Status != DownloadTaskStatus.Failed)
            {
                return null;
            }

            task.RetryCount++;
            ResetTaskForRestart(task);
        }

        var config = await _configurationService.LoadAsync(cancellationToken);
        var preferExtensions = DownloadFilterParser.ParsePreferExtensions(config?.Downloader);
        var parsedFilter = DownloadFilterParser.ParseFileFilter(fileFilter);
        var targetRoot = ResolveTargetRoot(config?.Downloader.SyncDataFolder);

        await RunSingleAsync(task, targetRoot, preferExtensions, parsedFilter, hdAudioOnly, config, cancellationToken);
        return task;
    }

    public Task ClearAllTasksAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        List<CancellationTokenSource> cancellationSources;
        lock (_stateLock)
        {
            cancellationSources = _taskCancellationSources.Values.ToList();
            foreach (var source in cancellationSources)
            {
                source.Cancel();
            }

            _taskCancellationSources.Clear();
            _tasks.Clear();
        }

        foreach (var source in cancellationSources)
        {
            source.Dispose();
        }

        var queuedSourceIds = _searchStateStore.GetQueuedSourceIds();
        if (queuedSourceIds.Count > 0)
        {
            _searchStateStore.RemoveFromQueue(queuedSourceIds);
        }

        return Task.CompletedTask;
    }

    public void UpsertPrefetchedWorkInfo(IReadOnlyDictionary<string, WorkInfoDto> workInfos, WorkInfoCacheEntryLevel cacheLevel = WorkInfoCacheEntryLevel.Summary)
    {
        ArgumentNullException.ThrowIfNull(workInfos);

        _workInfoCache.SetMany(workInfos, cacheLevel);
    }

    public IReadOnlyDictionary<string, WorkInfoDto> GetPrefetchedWorkInfoSnapshot()
    {
        return _workInfoCache.GetSnapshot();
    }

    private async Task RunSingleAsync(
        DownloadTaskItem task,
        string targetRoot,
        IReadOnlyList<string> preferExtensions,
        IReadOnlyList<(string Term, bool IsExclude)> fileFilter,
        bool hdAudioOnly,
        Core.Configuration.AppConfig? config,
        CancellationToken cancellationToken)
    {
        using var taskCancellation = new CancellationTokenSource();
        CancellationTokenSource? previous;
        lock (_stateLock)
        {
            _taskCancellationSources.TryGetValue(task.TaskId, out previous);
            _taskCancellationSources[task.TaskId] = taskCancellation;
        }

        previous?.Dispose();
        using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, taskCancellation.Token);
        var runCancellationToken = linkedCancellation.Token;

        lock (_stateLock)
        {
            task.Status = DownloadTaskStatus.Running;
            task.StartedAt = DateTimeOffset.Now;
        }

        try
        {
            runCancellationToken.ThrowIfCancellationRequested();

            if (!_workInfoCache.TryGet(task.SourceId, WorkInfoCacheRequirement.Full, out var workInfo) || workInfo is null)
            {
                workInfo = await _apiClient.GetWorkInfoAsync(task.SourceId, runCancellationToken);
                _workInfoCache.Set(task.SourceId, workInfo, WorkInfoCacheEntryLevel.Full);
            }

            lock (_stateLock)
            {
                task.Title = workInfo.Title;
            }

            var folderName = BuildFolderName(workInfo);
            var targetDirectory = Path.Combine(targetRoot, folderName);
            Directory.CreateDirectory(targetDirectory);
            lock (_stateLock)
            {
                task.TargetDirectory = targetDirectory;
            }

            var trackLookupId = workInfo.Id > 0
                ? workInfo.Id.ToString(CultureInfo.InvariantCulture)
                : task.SourceId;
            var tracks = await _apiClient.GetTracksAsync(trackLookupId, runCancellationToken);
            var allEntries = FlattenTracksWithPath(tracks)
                .Where(static item => !string.IsNullOrWhiteSpace(item.Url))
                .Where(item => IsPreferred(item.Url, preferExtensions))
                .Where(item => DownloadFilterParser.MatchesFileFilter(
                    string.IsNullOrEmpty(item.RelativePath)
                        ? item.Title
                        : Path.Combine(item.RelativePath, item.Title),
                    fileFilter))
                .ToArray();

            var mediaEntries = DownloadFilterParser.FilterHdAudioOnly(
                allEntries,
                static item => item.Url,
                hdAudioOnly);

            if (hdAudioOnly)
            {
                mediaEntries = FilterTextSidecarsForRemovedMp3(allEntries, mediaEntries);
            }

            lock (_stateLock)
            {
                task.TotalFiles = mediaEntries.Count;
                task.CompletedFiles = 0;
            }

            if (mediaEntries.Count == 0)
            {
                lock (_stateLock)
                {
                    task.Status = DownloadTaskStatus.Failed;
                    task.ErrorMessage = "未找到符合偏好格式的媒体链接。";
                }
                return;
            }

            for (var index = 0; index < mediaEntries.Count; index++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                runCancellationToken.ThrowIfCancellationRequested();

                await _rateLimiterService.WaitAsync(
                    config?.Limit.DownloadQps ?? 3,
                    config?.Limit.DownloadJitterMin ?? 50,
                    config?.Limit.DownloadJitterMax ?? 300,
                    runCancellationToken);

                var item = mediaEntries[index];
                var extension = ReadExtension(item.Url);
                var fileName = BuildOutputFileName(item.Title, extension);

                var subDir = string.IsNullOrEmpty(item.RelativePath)
                    ? targetDirectory
                    : Path.Combine(targetDirectory, item.RelativePath);
                Directory.CreateDirectory(subDir);

                var outputPath = Path.Combine(subDir, fileName);
                var content = $"source={task.SourceId}{Environment.NewLine}title={item.Title}{Environment.NewLine}url={item.Url}{Environment.NewLine}";
                await File.WriteAllTextAsync(outputPath, content, Encoding.UTF8, runCancellationToken);

                lock (_stateLock)
                {
                    task.CurrentFile = string.IsNullOrEmpty(item.RelativePath)
                        ? fileName
                        : Path.Combine(item.RelativePath, fileName);
                    task.CompletedFiles++;
                    task.ProgressPercent = task.TotalFiles == 0 ? 0 : task.CompletedFiles * 100d / task.TotalFiles;
                }
            }

            lock (_stateLock)
            {
                task.Status = DownloadTaskStatus.Completed;
            }
        }
        catch (OperationCanceledException) when (runCancellationToken.IsCancellationRequested)
        {
            lock (_stateLock)
            {
                MarkTaskCanceled(task);
            }
        }
        catch (Exception ex)
        {
            lock (_stateLock)
            {
                task.Status = DownloadTaskStatus.Failed;
                task.ErrorMessage = ex.Message;
            }
        }
        finally
        {
            lock (_stateLock)
            {
                task.FinishedAt = DateTimeOffset.Now;
                _taskCancellationSources.Remove(task.TaskId);
            }
        }
    }

    private static IReadOnlyList<(string Title, string Url, string RelativePath)> FilterTextSidecarsForRemovedMp3(
        IReadOnlyList<(string Title, string Url, string RelativePath)> originalEntries,
        IReadOnlyList<(string Title, string Url, string RelativePath)> filteredEntries)
    {
        if (originalEntries.Count == 0 || filteredEntries.Count == 0)
        {
            return filteredEntries;
        }

        var remainingMp3Keys = filteredEntries
            .Where(static entry => IsExtension(entry.Url, ".mp3"))
            .Select(static entry => BuildEntryKey(entry.RelativePath, entry.Title))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var removedMp3Keys = originalEntries
            .Where(static entry => IsExtension(entry.Url, ".mp3"))
            .Select(static entry => BuildEntryKey(entry.RelativePath, entry.Title))
            .Where(key => !remainingMp3Keys.Contains(key))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (removedMp3Keys.Count == 0)
        {
            return filteredEntries;
        }

        return filteredEntries
            .Where(entry =>
                !TextSidecarExtensions.Contains(ReadExtension(entry.Url), StringComparer.OrdinalIgnoreCase)
                || !removedMp3Keys.Contains(BuildEntryKey(entry.RelativePath, entry.Title)))
            .ToArray();
    }

    private static bool IsExtension(string url, string extension)
    {
        return ReadExtension(url).Equals(extension, StringComparison.OrdinalIgnoreCase);
    }

    private static string BuildOutputFileName(string title, string extension)
    {
        var sanitizedTitle = SanitizePathPart(title);
        return sanitizedTitle.EndsWith(extension, StringComparison.OrdinalIgnoreCase)
            ? sanitizedTitle
            : sanitizedTitle + extension;
    }

    private static string BuildEntryKey(string relativePath, string title)
    {
        var normalizedPath = relativePath ?? string.Empty;
        var normalizedTitle = SanitizePathPart(title);
        return string.Concat(normalizedPath, "|", normalizedTitle);
    }

    private string ResolveTargetRoot(string? configured)
    {
        var root = string.IsNullOrWhiteSpace(configured)
            ? _appPathService.DefaultSyncDataDirectory
            : configured.Trim();

        Directory.CreateDirectory(root);
        return root;
    }

    private static int NormalizeMaxWorkers(int configured)
    {
        return Math.Clamp(configured, 1, 32);
    }

    private DownloadTaskItem ResolveStartTargetTask(string sourceId, Guid? preferredTaskId)
    {
        if (preferredTaskId is Guid taskId && taskId != Guid.Empty)
        {
            var preferred = _tasks.FirstOrDefault(item => item.TaskId == taskId);
            if (preferred is not null && preferred.Status is DownloadTaskStatus.Failed or DownloadTaskStatus.Canceled)
            {
                ResetTaskForRestart(preferred);
                return preferred;
            }
        }

        var reusable = _tasks
            .Where(item => item.SourceId.Equals(sourceId, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(static item => item.StartedAt)
            .FirstOrDefault(item => item.Status is DownloadTaskStatus.Failed or DownloadTaskStatus.Canceled);

        if (reusable is not null)
        {
            ResetTaskForRestart(reusable);
            return reusable;
        }

        return new DownloadTaskItem
        {
            SourceId = sourceId,
            Status = DownloadTaskStatus.Queued,
        };
    }

    private static void ResetTaskForRestart(DownloadTaskItem task)
    {
        task.Status = DownloadTaskStatus.Queued;
        task.TotalFiles = 0;
        task.CompletedFiles = 0;
        task.ProgressPercent = 0;
        task.CurrentFile = string.Empty;
        task.ErrorMessage = string.Empty;
        task.FinishedAt = null;
        task.TargetDirectory = string.Empty;
    }

    private static void MarkTaskCanceled(DownloadTaskItem task)
    {
        task.Status = DownloadTaskStatus.Canceled;
        task.ErrorMessage = "任务已取消。";
        task.FinishedAt = DateTimeOffset.Now;
    }

    private static string BuildFolderName(WorkInfoDto workInfo)
    {
        var title = SanitizePathPart(workInfo.Title);
        return $"[{workInfo.SourceId}]{title}";
    }

    private static bool IsPreferred(string url, IReadOnlyList<string> preferMedia)
    {
        if (preferMedia.Count == 0)
        {
            return true;
        }

        var ext = ReadExtension(url);
        return preferMedia.Contains(ext, StringComparer.OrdinalIgnoreCase);
    }

    private static string ReadExtension(string url)
    {
        try
        {
            var uri = new Uri(url, UriKind.Absolute);
            var ext = Path.GetExtension(uri.AbsolutePath);
            return string.IsNullOrWhiteSpace(ext) ? ".bin" : ext.ToLowerInvariant();
        }
        catch
        {
            return ".bin";
        }
    }

    private static string SanitizePathPart(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "untitled";
        }

        var invalid = Path.GetInvalidFileNameChars();
        var chars = value
            .Select(ch => invalid.Contains(ch) ? '_' : ch)
            .ToArray();

        return new string(chars).Trim();
    }

    private static IEnumerable<(string Title, string Url, string RelativePath)> FlattenTracksWithPath(
        IEnumerable<TrackDto> tracks, string currentPath = "")
    {
        foreach (var track in tracks)
        {
            if (!string.IsNullOrWhiteSpace(track.MediaDownloadUrl))
            {
                yield return (track.Title, track.MediaDownloadUrl, currentPath);
            }

            if (track.Children.Count > 0)
            {
                var folderName = SanitizePathPart(track.Title);
                var childPath = string.IsNullOrEmpty(currentPath)
                    ? folderName
                    : Path.Combine(currentPath, folderName);
                foreach (var child in FlattenTracksWithPath(track.Children, childPath))
                {
                    yield return child;
                }
            }
        }
    }
}
