using Asmroner.Core.Api;
using Asmroner.Core.Configuration;
using Asmroner.Core.Download;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Sync;

namespace Asmroner.Application.Services;

public sealed class SyncDownloadService
{
    private readonly IConfigurationService _configurationService;
    private readonly IMetadataSyncStore _metadataSyncStore;
    private readonly ISyncWorkInfoResolver _syncWorkInfoResolver;
    private readonly IDownloadService _downloadService;
    private readonly IAppPathService _appPathService;
    private readonly IUiStateStore _uiStateStore;

    public SyncDownloadService(
        IConfigurationService configurationService,
        IMetadataSyncStore metadataSyncStore,
        ISyncWorkInfoResolver syncWorkInfoResolver,
        IDownloadService downloadService,
        IAppPathService appPathService,
        IUiStateStore uiStateStore)
    {
        _configurationService = configurationService;
        _metadataSyncStore = metadataSyncStore;
        _syncWorkInfoResolver = syncWorkInfoResolver;
        _downloadService = downloadService;
        _appPathService = appPathService;
        _uiStateStore = uiStateStore;
    }

    public Task<SyncDownloadSnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default)
    {
        return _metadataSyncStore.GetDownloadSnapshotAsync(cancellationToken);
    }

    public Task<SyncDownloadProgressState> GetProgressAsync(CancellationToken cancellationToken = default)
    {
        return _uiStateStore.LoadSyncDownloadProgressAsync(cancellationToken);
    }

    public Task RequestStopAsync(CancellationToken cancellationToken = default)
    {
        return _uiStateStore.RequestStopSyncDownloadAsync(cancellationToken);
    }

    public async Task<SyncDownloadRunResult> SyncDownloadAsync(CancellationToken cancellationToken = default)
    {
        var progress = await _uiStateStore.LoadSyncDownloadProgressAsync(cancellationToken);
        var resumedFromProgress = ShouldResume(progress);
        var config = await _configurationService.LoadAsync(cancellationToken) ?? new AppConfig();
        var sizeLimitBytes = SyncSizeText.ParseBytes(config.Downloader.SyncWantedSize);
        var targetRoot = ResolveTargetRoot(config);
        var downloadLookupRoot = ResolveDownloadLookupRoot(config);

        await _metadataSyncStore.CleanupPendingSyncDownloadsAsync(cancellationToken);

        var before = await _metadataSyncStore.GetDownloadSnapshotAsync(cancellationToken);
        var allWorkInfos = await _syncWorkInfoResolver.ResolveAllAsync(cancellationToken);
        var syncInfoMap = new Dictionary<int, WorkSyncInfoItem>(await _metadataSyncStore.GetWorkSyncInfoMapAsync(cancellationToken));
        var isRescanMode = ShouldRescanAllMetadata(progress, resumedFromProgress, before, allWorkInfos.Count);
        var candidates = BuildProcessingCandidates(allWorkInfos, syncInfoMap, isRescanMode, resumedFromProgress, progress.LastProcessedSourceId);
        var baseDownloadOptions = new DownloadStartOptions
        {
            TargetRoot = targetRoot,
            LookupRoots = string.IsNullOrWhiteSpace(downloadLookupRoot)
                ? Array.Empty<string>()
                : new[] { downloadLookupRoot },
        };
        var startedAt = resumedFromProgress
            ? progress.StartedAt ?? DateTime.UtcNow
            : DateTime.UtcNow;
        var cumulativeProcessedCount = resumedFromProgress ? Math.Max(0, progress.ProcessedCount) : 0;
        var cumulativeCompletedCount = resumedFromProgress ? Math.Max(0, progress.CompletedCount) : 0;
        var cumulativeFailedCount = resumedFromProgress ? Math.Max(0, progress.FailedCount) : 0;
        var cumulativeSizeBefore = resumedFromProgress && progress.CompletedSizeBytesBefore > 0
            ? progress.CompletedSizeBytesBefore
            : before.CompletedSizeBytes;
        var completedSizeBytes = before.CompletedSizeBytes;
        var remainingMetadataCount = candidates.Count;
        var lastProcessedSourceId = resumedFromProgress
            ? progress.LastProcessedSourceId
            : string.Empty;

        if (!resumedFromProgress && !isRescanMode && before.CompletedSizeBytes >= sizeLimitBytes)
        {
            await _uiStateStore.SaveSyncDownloadProgressAsync(
                BuildProgressState(
                    SyncProgressStatuses.Completed,
                    lastProcessedSourceId: string.Empty,
                    processedCount: 0,
                    completedCount: 0,
                    failedCount: 0,
                    remainingMetadataCountAfter: before.RemainingMetadataCount,
                    completedSizeBytesBefore: before.CompletedSizeBytes,
                    completedSizeBytesAfter: before.CompletedSizeBytes,
                    sizeLimitBytes,
                    startedAt,
                    updatedAt: DateTime.UtcNow),
                cancellationToken);

            return BuildNoOpResult(
                before,
                sizeLimitBytes,
                "已达到同步容量上限，无需继续同步下载。",
                reachedSizeLimit: true,
                resumedFromProgress: false,
                lastProcessedSourceId: string.Empty);
        }

        await _uiStateStore.SaveSyncDownloadProgressAsync(
            BuildProgressState(
                SyncProgressStatuses.Running,
                lastProcessedSourceId,
                processedCount: cumulativeProcessedCount,
                completedCount: cumulativeCompletedCount,
                failedCount: cumulativeFailedCount,
                remainingMetadataCountAfter: remainingMetadataCount,
                completedSizeBytesBefore: cumulativeSizeBefore,
                completedSizeBytesAfter: completedSizeBytes,
                sizeLimitBytes,
                startedAt,
                updatedAt: DateTime.UtcNow),
            cancellationToken);

        var processedCount = 0;
        var completedCount = 0;
        var failedCount = 0;
        var reachedSizeLimit = false;

        foreach (var candidate in candidates)
        {
            if (!isRescanMode && completedSizeBytes >= sizeLimitBytes)
            {
                reachedSizeLimit = true;
                break;
            }

            cancellationToken.ThrowIfCancellationRequested();

            processedCount++;
            cumulativeProcessedCount++;
            remainingMetadataCount = Math.Max(0, remainingMetadataCount - 1);
            lastProcessedSourceId = candidate.SourceId;
            var expectedPath = SyncDownloadPathPolicy.BuildTargetDirectory(targetRoot, candidate.SourceId, candidate.Title);
            syncInfoMap.TryGetValue(candidate.Id, out var existingSyncInfo);
            var pendingItem = await CreateOrUpdatePendingSyncInfoAsync(existingSyncInfo, candidate, expectedPath, cancellationToken);
            var downloadOptions = new DownloadStartOptions
            {
                TargetRoot = baseDownloadOptions.TargetRoot,
                LookupRoots = baseDownloadOptions.LookupRoots,
                WorkId = candidate.Id,
            };
            var task = await _downloadService.StartAsync(
                candidate.SourceId,
                hdAudioOnly: config.Downloader.HdAudioOnly,
                options: downloadOptions,
                cancellationToken: cancellationToken);

            var updatedItem = BuildCompletedSyncInfo(pendingItem, candidate, task, expectedPath);
            await _metadataSyncStore.UpdateWorkSyncInfoAsync(updatedItem, cancellationToken);
            syncInfoMap[candidate.Id] = updatedItem;

            var previousCompletedSize = existingSyncInfo is not null && string.Equals(existingSyncInfo.Status, "COMPLETED", StringComparison.Ordinal)
                ? existingSyncInfo.DirSize
                : 0L;
            var currentCompletedSize = string.Equals(updatedItem.Status, "COMPLETED", StringComparison.Ordinal)
                ? updatedItem.DirSize
                : 0L;
            completedSizeBytes += currentCompletedSize - previousCompletedSize;

            if (updatedItem.Status == "COMPLETED")
            {
                completedCount++;
                cumulativeCompletedCount++;
            }
            else
            {
                failedCount++;
                cumulativeFailedCount++;
            }

            if (!isRescanMode && completedSizeBytes >= sizeLimitBytes)
            {
                reachedSizeLimit = true;
                break;
            }

            await _uiStateStore.SaveSyncDownloadProgressAsync(
                BuildProgressState(
                    SyncProgressStatuses.Running,
                    lastProcessedSourceId,
                    processedCount: cumulativeProcessedCount,
                    completedCount: cumulativeCompletedCount,
                    failedCount: cumulativeFailedCount,
                    remainingMetadataCountAfter: remainingMetadataCount,
                    completedSizeBytesBefore: cumulativeSizeBefore,
                    completedSizeBytesAfter: completedSizeBytes,
                    sizeLimitBytes,
                    startedAt,
                    updatedAt: DateTime.UtcNow),
                cancellationToken);

            var currentProgress = await _uiStateStore.LoadSyncDownloadProgressAsync(cancellationToken);
            if (currentProgress.StopRequested)
            {
                await _uiStateStore.SaveSyncDownloadProgressAsync(
                    BuildProgressState(
                        SyncProgressStatuses.Stopped,
                        lastProcessedSourceId,
                        processedCount: cumulativeProcessedCount,
                        completedCount: cumulativeCompletedCount,
                        failedCount: cumulativeFailedCount,
                        remainingMetadataCountAfter: remainingMetadataCount,
                        completedSizeBytesBefore: cumulativeSizeBefore,
                        completedSizeBytesAfter: completedSizeBytes,
                        sizeLimitBytes,
                        startedAt,
                        updatedAt: DateTime.UtcNow),
                    cancellationToken);

                return new SyncDownloadRunResult
                {
                    ProcessedCount = processedCount,
                    CompletedCount = completedCount,
                    FailedCount = failedCount,
                    RemainingMetadataCountAfter = remainingMetadataCount,
                    CompletedSizeBytesBefore = before.CompletedSizeBytes,
                    CompletedSizeBytesAfter = completedSizeBytes,
                    SizeLimitBytes = sizeLimitBytes,
                    ReachedSizeLimit = false,
                    LastProcessedSourceId = lastProcessedSourceId,
                    Message = isRescanMode
                        ? $"同步下载校验已按请求停止：当前作品已完成，成功 {completedCount} 项，失败 {failedCount} 项，剩余待校验 {remainingMetadataCount} 项。"
                        : $"同步下载已按请求停止：当前作品已完成，成功 {completedCount} 项，失败 {failedCount} 项，剩余待同步 {remainingMetadataCount} 项。",
                    WasStopped = true,
                    ResumedFromProgress = resumedFromProgress,
                };
            }
        }

        if (processedCount == 0)
        {
            await _uiStateStore.SaveSyncDownloadProgressAsync(
                BuildProgressState(
                    SyncProgressStatuses.Completed,
                    lastProcessedSourceId,
                    processedCount: cumulativeProcessedCount,
                    completedCount: cumulativeCompletedCount,
                    failedCount: cumulativeFailedCount,
                    remainingMetadataCountAfter: remainingMetadataCount,
                    completedSizeBytesBefore: cumulativeSizeBefore,
                    completedSizeBytesAfter: completedSizeBytes,
                    sizeLimitBytes,
                    startedAt,
                    updatedAt: DateTime.UtcNow),
                cancellationToken);

            return BuildNoOpResult(
                new SyncDownloadSnapshot
                {
                    PendingCount = before.PendingCount,
                    CompletedCount = before.CompletedCount,
                    FailedCount = before.FailedCount,
                    RemainingMetadataCount = remainingMetadataCount,
                    CompletedSizeBytes = completedSizeBytes,
                    LastUpdatedAt = before.LastUpdatedAt,
                },
                sizeLimitBytes,
                isRescanMode ? "没有可校验的同步下载作品。" : "没有待同步下载的新作品。",
                reachedSizeLimit: false,
                resumedFromProgress,
                lastProcessedSourceId);
        }

        await _uiStateStore.SaveSyncDownloadProgressAsync(
            BuildProgressState(
                SyncProgressStatuses.Completed,
                lastProcessedSourceId,
                processedCount: cumulativeProcessedCount,
                completedCount: cumulativeCompletedCount,
                failedCount: cumulativeFailedCount,
                remainingMetadataCountAfter: remainingMetadataCount,
                completedSizeBytesBefore: cumulativeSizeBefore,
                completedSizeBytesAfter: completedSizeBytes,
                sizeLimitBytes,
                startedAt,
                updatedAt: DateTime.UtcNow),
            cancellationToken);

        return new SyncDownloadRunResult
        {
            ProcessedCount = processedCount,
            CompletedCount = completedCount,
            FailedCount = failedCount,
            RemainingMetadataCountAfter = remainingMetadataCount,
            CompletedSizeBytesBefore = before.CompletedSizeBytes,
            CompletedSizeBytesAfter = completedSizeBytes,
            SizeLimitBytes = sizeLimitBytes,
            ReachedSizeLimit = reachedSizeLimit,
            LastProcessedSourceId = lastProcessedSourceId,
            Message = isRescanMode
                ? $"同步下载校验完成：成功 {completedCount} 项，失败 {failedCount} 项。"
                : reachedSizeLimit
                    ? $"同步下载完成：成功 {completedCount} 项，失败 {failedCount} 项，已达到容量上限 {SyncSizeText.FormatBytes(sizeLimitBytes)}。"
                    : $"同步下载完成：成功 {completedCount} 项，失败 {failedCount} 项，剩余待同步 {remainingMetadataCount} 项。",
            WasStopped = false,
            ResumedFromProgress = resumedFromProgress,
        };
    }

    public async Task<SyncRetryRunResult> RetryFailedAsync(CancellationToken cancellationToken = default)
    {
        var config = await _configurationService.LoadAsync(cancellationToken) ?? new AppConfig();
        var before = await _metadataSyncStore.GetDownloadSnapshotAsync(cancellationToken);
        var failedItems = await _metadataSyncStore.GetFailedSyncDownloadsAsync(cancellationToken);
        var baseRetryOptions = new DownloadStartOptions
        {
            TargetRoot = ResolveTargetRoot(config),
            LookupRoots = string.IsNullOrWhiteSpace(ResolveDownloadLookupRoot(config))
                ? Array.Empty<string>()
                : new[] { ResolveDownloadLookupRoot(config) },
        };

        if (failedItems.Count == 0)
        {
            return BuildRetryNoOpResult(before, "没有需要重试的失败同步下载记录。");
        }

        var recoveredCount = 0;
        var failedAgainCount = 0;

        foreach (var failedItem in failedItems)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var updatedItem = await RetryFailedItemAsync(
                failedItem,
                config.Downloader.HdAudioOnly,
                baseRetryOptions,
                cancellationToken);
            await _metadataSyncStore.UpdateWorkSyncInfoAsync(updatedItem, cancellationToken);

            if (updatedItem.Status == "COMPLETED")
            {
                recoveredCount++;
            }
            else
            {
                failedAgainCount++;
            }
        }

        var after = await _metadataSyncStore.GetDownloadSnapshotAsync(cancellationToken);
        return new SyncRetryRunResult
        {
            RetriedCount = failedItems.Count,
            RecoveredCount = recoveredCount,
            FailedAgainCount = failedAgainCount,
            FailedCountBefore = before.FailedCount,
            FailedCountAfter = after.FailedCount,
            CompletedCountAfter = after.CompletedCount,
            Message = after.FailedCount == 0
                ? $"失败重试完成：已恢复 {recoveredCount} 项，当前无失败项。"
                : $"失败重试完成：已恢复 {recoveredCount} 项，仍失败 {after.FailedCount} 项。",
        };
    }

    private string ResolveTargetRoot(AppConfig config)
    {
        var targetRoot = string.IsNullOrWhiteSpace(config.Downloader.SyncDataFolder)
            ? _appPathService.DefaultSyncDataDirectory
            : config.Downloader.SyncDataFolder.Trim();
        Directory.CreateDirectory(targetRoot);
        return targetRoot;
    }

    private string ResolveDownloadLookupRoot(AppConfig config)
    {
        return string.IsNullOrWhiteSpace(config.Downloader.DownloadDataFolder)
            ? _appPathService.DefaultDownloadDataDirectory
            : config.Downloader.DownloadDataFolder.Trim();
    }

    private static SyncDownloadRunResult BuildNoOpResult(
        SyncDownloadSnapshot snapshot,
        long sizeLimitBytes,
        string message,
        bool reachedSizeLimit,
        bool resumedFromProgress,
        string lastProcessedSourceId)
    {
        return new SyncDownloadRunResult
        {
            ProcessedCount = 0,
            CompletedCount = 0,
            FailedCount = 0,
            RemainingMetadataCountAfter = snapshot.RemainingMetadataCount,
            CompletedSizeBytesBefore = snapshot.CompletedSizeBytes,
            CompletedSizeBytesAfter = snapshot.CompletedSizeBytes,
            SizeLimitBytes = sizeLimitBytes,
            ReachedSizeLimit = reachedSizeLimit,
            LastProcessedSourceId = lastProcessedSourceId,
            Message = message,
            WasStopped = false,
            ResumedFromProgress = resumedFromProgress,
        };
    }

    private static SyncRetryRunResult BuildRetryNoOpResult(SyncDownloadSnapshot snapshot, string message)
    {
        return new SyncRetryRunResult
        {
            RetriedCount = 0,
            RecoveredCount = 0,
            FailedAgainCount = 0,
            FailedCountBefore = snapshot.FailedCount,
            FailedCountAfter = snapshot.FailedCount,
            CompletedCountAfter = snapshot.CompletedCount,
            Message = message,
        };
    }

    private async Task<WorkSyncInfoItem> RetryFailedItemAsync(
        WorkSyncInfoItem failedItem,
        bool hdAudioOnly,
        DownloadStartOptions options,
        CancellationToken cancellationToken)
    {
        try
        {
            DeleteDirectoryIfExists(failedItem.FilePath);

            var task = await _downloadService.StartAsync(
                failedItem.SourceId,
                hdAudioOnly: hdAudioOnly,
                options: new DownloadStartOptions
                {
                    TargetRoot = options.TargetRoot,
                    LookupRoots = options.LookupRoots,
                    WorkId = failedItem.MetadataWorkId,
                },
                cancellationToken: cancellationToken);

            return BuildRetriedSyncInfo(failedItem, task);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return BuildRetryFailureSyncInfo(
                failedItem,
                failedItem.FilePath,
                $"同步失败重试失败: {ex.Message}");
        }
    }

    private static bool ShouldRescanAllMetadata(
        SyncDownloadProgressState progress,
        bool resumedFromProgress,
        SyncDownloadSnapshot snapshot,
        int metadataCount)
    {
        if (metadataCount == 0)
        {
            return false;
        }

        if (resumedFromProgress)
        {
            return snapshot.RemainingMetadataCount == 0;
        }

        return string.Equals(progress.Status, SyncProgressStatuses.Completed, StringComparison.Ordinal);
    }

    private static IReadOnlyList<WorkInfoDto> BuildProcessingCandidates(
        IReadOnlyList<WorkInfoDto> allWorkInfos,
        IReadOnlyDictionary<int, WorkSyncInfoItem> syncInfoMap,
        bool isRescanMode,
        bool resumedFromProgress,
        string lastProcessedSourceId)
    {
        IEnumerable<WorkInfoDto> candidates = allWorkInfos
            .OrderBy(static work => work.Id);

        if (!isRescanMode)
        {
            candidates = candidates.Where(work => !syncInfoMap.ContainsKey(work.Id));
        }

        var orderedCandidates = candidates.ToArray();
        if (!resumedFromProgress || string.IsNullOrWhiteSpace(lastProcessedSourceId))
        {
            return orderedCandidates;
        }

        var startIndex = Array.FindIndex(
            orderedCandidates,
            work => string.Equals(work.SourceId, lastProcessedSourceId, StringComparison.OrdinalIgnoreCase));
        return startIndex < 0
            ? orderedCandidates
            : orderedCandidates[(startIndex + 1)..];
    }

    private async Task<WorkSyncInfoItem> CreateOrUpdatePendingSyncInfoAsync(
        WorkSyncInfoItem? existingItem,
        WorkInfoDto work,
        string filePath,
        CancellationToken cancellationToken)
    {
        if (existingItem is null)
        {
            return await _metadataSyncStore.CreatePendingWorkSyncInfoAsync(
                MetadataWorkSummaryMapper.MergeFromWorkInfo(work, existing: null, updatedAt: DateTime.UtcNow),
                filePath,
                cancellationToken);
        }

        var pendingItem = BuildPendingSyncInfo(existingItem, work, filePath);
        await _metadataSyncStore.UpdateWorkSyncInfoAsync(pendingItem, cancellationToken);
        return pendingItem;
    }

    private static WorkSyncInfoItem BuildPendingSyncInfo(WorkSyncInfoItem existingItem, WorkInfoDto work, string filePath)
    {
        return new WorkSyncInfoItem
        {
            Id = existingItem.Id,
            MetadataWorkId = work.Id,
            SourceId = work.SourceId,
            HasSubtitle = work.HasSubtitle,
            DirSize = existingItem.DirSize,
            Status = "PENDING",
            FilePath = filePath,
            FailReason = string.Empty,
            RetryCount = existingItem.RetryCount,
            UpdatedAt = DateTime.UtcNow,
            FailedAt = null,
        };
    }

    private static WorkSyncInfoItem BuildCompletedSyncInfo(
        WorkSyncInfoItem pendingItem,
        WorkInfoDto workInfo,
        DownloadTaskItem? task,
        string expectedPath)
    {
        var targetPath = string.IsNullOrWhiteSpace(task?.TargetDirectory)
            ? expectedPath
            : task.TargetDirectory;
        var status = task?.Status switch
        {
            DownloadTaskStatus.Completed => "COMPLETED",
            DownloadTaskStatus.Failed => "FAILED",
            DownloadTaskStatus.Canceled => "FAILED",
            _ => "FAILED",
        };
        var dirSize = status == "COMPLETED"
            ? MeasureDirectorySize(targetPath)
            : 0L;

        return new WorkSyncInfoItem
        {
            Id = pendingItem.Id,
            MetadataWorkId = workInfo.Id,
            SourceId = workInfo.SourceId,
            HasSubtitle = workInfo.HasSubtitle,
            DirSize = dirSize,
            Status = status,
            FilePath = targetPath,
            FailReason = status == "FAILED"
                ? task?.ErrorMessage ?? "同步下载失败。"
                : string.Empty,
            RetryCount = pendingItem.RetryCount,
            UpdatedAt = DateTime.UtcNow,
            FailedAt = status == "FAILED" ? DateTime.UtcNow : null,
        };
    }

    private static WorkSyncInfoItem BuildRetriedSyncInfo(WorkSyncInfoItem failedItem, DownloadTaskItem? task)
    {
        var targetPath = string.IsNullOrWhiteSpace(task?.TargetDirectory)
            ? failedItem.FilePath
            : task.TargetDirectory;
        var status = task?.Status switch
        {
            DownloadTaskStatus.Completed => "COMPLETED",
            DownloadTaskStatus.Failed => "FAILED",
            DownloadTaskStatus.Canceled => "FAILED",
            _ => "FAILED",
        };
        var dirSize = status == "COMPLETED"
            ? MeasureDirectorySize(targetPath)
            : 0L;

        return new WorkSyncInfoItem
        {
            Id = failedItem.Id,
            MetadataWorkId = failedItem.MetadataWorkId,
            SourceId = failedItem.SourceId,
            HasSubtitle = failedItem.HasSubtitle,
            DirSize = dirSize,
            Status = status,
            FilePath = targetPath,
            FailReason = status == "FAILED"
                ? task?.ErrorMessage ?? "同步失败重试失败。"
                : string.Empty,
            RetryCount = failedItem.RetryCount + 1,
            UpdatedAt = DateTime.UtcNow,
            FailedAt = status == "FAILED" ? DateTime.UtcNow : null,
        };
    }

    private static WorkSyncInfoItem BuildRetryFailureSyncInfo(
        WorkSyncInfoItem failedItem,
        string filePath,
        string failReason)
    {
        return new WorkSyncInfoItem
        {
            Id = failedItem.Id,
            MetadataWorkId = failedItem.MetadataWorkId,
            SourceId = failedItem.SourceId,
            HasSubtitle = failedItem.HasSubtitle,
            DirSize = 0,
            Status = "FAILED",
            FilePath = filePath,
            FailReason = failReason,
            RetryCount = failedItem.RetryCount + 1,
            UpdatedAt = DateTime.UtcNow,
            FailedAt = DateTime.UtcNow,
        };
    }

    private static void DeleteDirectoryIfExists(string path)
    {
        if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }

    private static long MeasureDirectorySize(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
        {
            return 0;
        }

        return Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
            .Select(filePath => new FileInfo(filePath).Length)
            .Sum();
    }

    private static bool ShouldResume(SyncDownloadProgressState state)
    {
        return !string.Equals(state.Status, SyncProgressStatuses.Completed, StringComparison.Ordinal)
            && !string.Equals(state.Status, SyncProgressStatuses.Idle, StringComparison.Ordinal);
    }

    private static SyncDownloadProgressState BuildProgressState(
        string status,
        string lastProcessedSourceId,
        int processedCount,
        int completedCount,
        int failedCount,
        int remainingMetadataCountAfter,
        long completedSizeBytesBefore,
        long completedSizeBytesAfter,
        long sizeLimitBytes,
        DateTime startedAt,
        DateTime updatedAt)
    {
        return new SyncDownloadProgressState
        {
            Status = status,
            LastProcessedSourceId = lastProcessedSourceId,
            ProcessedCount = processedCount,
            CompletedCount = completedCount,
            FailedCount = failedCount,
            RemainingMetadataCountAfter = remainingMetadataCountAfter,
            CompletedSizeBytesBefore = completedSizeBytesBefore,
            CompletedSizeBytesAfter = completedSizeBytesAfter,
            SizeLimitBytes = sizeLimitBytes,
            StartedAt = startedAt,
            UpdatedAt = updatedAt,
        };
    }
}