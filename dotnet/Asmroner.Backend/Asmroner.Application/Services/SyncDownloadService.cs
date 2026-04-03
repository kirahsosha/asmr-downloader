using Asmroner.Core.Configuration;
using Asmroner.Core.Download;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Sync;

namespace Asmroner.Application.Services;

public sealed class SyncDownloadService
{
    private readonly IConfigurationService _configurationService;
    private readonly IMetadataSyncStore _metadataSyncStore;
    private readonly IDownloadService _downloadService;
    private readonly IAppPathService _appPathService;

    public SyncDownloadService(
        IConfigurationService configurationService,
        IMetadataSyncStore metadataSyncStore,
        IDownloadService downloadService,
        IAppPathService appPathService)
    {
        _configurationService = configurationService;
        _metadataSyncStore = metadataSyncStore;
        _downloadService = downloadService;
        _appPathService = appPathService;
    }

    public Task<SyncDownloadSnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default)
    {
        return _metadataSyncStore.GetDownloadSnapshotAsync(cancellationToken);
    }

    public async Task<SyncDownloadRunResult> SyncDownloadAsync(CancellationToken cancellationToken = default)
    {
        var config = await _configurationService.LoadAsync(cancellationToken) ?? new AppConfig();
        var sizeLimitBytes = SyncSizeText.ParseBytes(config.Downloader.SyncWantedSize);
        var targetRoot = ResolveTargetRoot(config);

        await _metadataSyncStore.CleanupPendingSyncDownloadsAsync(cancellationToken);

        var before = await _metadataSyncStore.GetDownloadSnapshotAsync(cancellationToken);
        if (before.CompletedSizeBytes >= sizeLimitBytes)
        {
            return BuildNoOpResult(
                before,
                sizeLimitBytes,
                "已达到同步容量上限，无需继续同步下载。",
                reachedSizeLimit: true);
        }

        var processedCount = 0;
        var completedCount = 0;
        var failedCount = 0;
        var completedSizeBytes = before.CompletedSizeBytes;
        var reachedSizeLimit = false;

        while (completedSizeBytes < sizeLimitBytes)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var candidate = (await _metadataSyncStore.GetSyncDownloadCandidatesAsync(1, cancellationToken)).SingleOrDefault();
            if (candidate is null)
            {
                break;
            }

            processedCount++;
            var expectedPath = SyncDownloadPathPolicy.BuildTargetDirectory(targetRoot, candidate.SourceId, candidate.Title);
            var pendingItem = await _metadataSyncStore.CreatePendingWorkSyncInfoAsync(candidate, expectedPath, cancellationToken);
            var task = await _downloadService.StartAsync(
                candidate.SourceId,
                hdAudioOnly: config.Downloader.HdAudioOnly,
                cancellationToken: cancellationToken);

            var updatedItem = BuildCompletedSyncInfo(pendingItem, candidate, task, expectedPath);
            await _metadataSyncStore.UpdateWorkSyncInfoAsync(updatedItem, cancellationToken);

            if (updatedItem.Status == "COMPLETED")
            {
                completedCount++;
                completedSizeBytes += updatedItem.DirSize;
            }
            else
            {
                failedCount++;
            }

            if (completedSizeBytes >= sizeLimitBytes)
            {
                reachedSizeLimit = true;
                break;
            }
        }

        var after = await _metadataSyncStore.GetDownloadSnapshotAsync(cancellationToken);
        if (processedCount == 0)
        {
            return BuildNoOpResult(
                after,
                sizeLimitBytes,
                "没有待同步下载的新作品。",
                reachedSizeLimit: false);
        }

        return new SyncDownloadRunResult
        {
            ProcessedCount = processedCount,
            CompletedCount = completedCount,
            FailedCount = failedCount,
            RemainingMetadataCountAfter = after.RemainingMetadataCount,
            CompletedSizeBytesBefore = before.CompletedSizeBytes,
            CompletedSizeBytesAfter = after.CompletedSizeBytes,
            SizeLimitBytes = sizeLimitBytes,
            ReachedSizeLimit = reachedSizeLimit,
            Message = reachedSizeLimit
                ? $"同步下载完成：成功 {completedCount} 项，失败 {failedCount} 项，已达到容量上限 {SyncSizeText.FormatBytes(sizeLimitBytes)}。"
                : $"同步下载完成：成功 {completedCount} 项，失败 {failedCount} 项，剩余待同步 {after.RemainingMetadataCount} 项。",
        };
    }

    public async Task<SyncRetryRunResult> RetryFailedAsync(CancellationToken cancellationToken = default)
    {
        var config = await _configurationService.LoadAsync(cancellationToken) ?? new AppConfig();
        var before = await _metadataSyncStore.GetDownloadSnapshotAsync(cancellationToken);
        var failedItems = await _metadataSyncStore.GetFailedSyncDownloadsAsync(cancellationToken);

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

    private static SyncDownloadRunResult BuildNoOpResult(
        SyncDownloadSnapshot snapshot,
        long sizeLimitBytes,
        string message,
        bool reachedSizeLimit)
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
            Message = message,
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
        CancellationToken cancellationToken)
    {
        try
        {
            DeleteDirectoryIfExists(failedItem.FilePath);

            var task = await _downloadService.StartAsync(
                failedItem.SourceId,
                hdAudioOnly: hdAudioOnly,
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

    private static WorkSyncInfoItem BuildCompletedSyncInfo(
        WorkSyncInfoItem pendingItem,
        MetadataWorkItem metadataWork,
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
            MetadataWorkId = metadataWork.Id,
            SourceId = metadataWork.SourceId,
            HasSubtitle = metadataWork.HasSubtitle,
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
}