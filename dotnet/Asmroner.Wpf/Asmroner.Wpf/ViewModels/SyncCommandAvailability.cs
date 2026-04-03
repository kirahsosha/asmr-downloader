namespace Asmroner.Wpf.ViewModels;

public sealed class SyncCommandAvailability
{
    public bool CanMetadataAction { get; init; }

    public string MetadataActionText { get; init; } = "开始同步元数据";

    public bool CanDownloadAction { get; init; }

    public string DownloadActionText { get; init; } = "开始同步下载";

    public bool CanRetryFailed { get; init; }

    public bool CanExportFailed { get; init; }

    public bool CanExportCompleted { get; init; }

    public bool CanRefresh { get; init; }

    public static SyncCommandAvailability Evaluate(
        bool isMetadataSyncRunning,
        bool isMetadataStopRequested,
        bool isDownloadSyncRunning,
        bool isDownloadStopRequested,
        bool isExclusiveOperationRunning,
        bool isRefreshRunning)
    {
        var hasRunningSync = isMetadataSyncRunning || isDownloadSyncRunning;
        var disableExclusiveActions = hasRunningSync || isExclusiveOperationRunning || isRefreshRunning;

        var canMetadataAction = isMetadataSyncRunning
            ? !isMetadataStopRequested
            : !hasRunningSync && !isExclusiveOperationRunning;
        var canDownloadAction = isDownloadSyncRunning
            ? !isDownloadStopRequested
            : !hasRunningSync && !isExclusiveOperationRunning;

        return new SyncCommandAvailability
        {
            CanMetadataAction = canMetadataAction,
            MetadataActionText = isMetadataSyncRunning
                ? isMetadataStopRequested ? "正在停止元数据..." : "停止同步元数据"
                : "开始同步元数据",
            CanDownloadAction = canDownloadAction,
            DownloadActionText = isDownloadSyncRunning
                ? isDownloadStopRequested ? "正在停止下载..." : "停止同步下载"
                : "开始同步下载",
            CanRetryFailed = !disableExclusiveActions,
            CanExportFailed = !disableExclusiveActions,
            CanExportCompleted = !disableExclusiveActions,
            CanRefresh = !isExclusiveOperationRunning && !isRefreshRunning,
        };
    }
}