namespace Asmroner.Wpf.Services;

internal static class SyncShellStatusRelayPolicy
{
    public static bool ShouldPublishMetadata(
        bool isViewVisible,
        bool isMetadataSyncRunning,
        bool isMetadataStopRequested,
        bool isDownloadSyncRunning,
        bool isDownloadStopRequested,
        bool isRetryFailedRunning,
        bool isExportRunning,
        bool isRefreshRunning)
    {
        if (!isViewVisible)
        {
            return false;
        }

        if (isDownloadSyncRunning || isDownloadStopRequested || isRetryFailedRunning || isExportRunning)
        {
            return false;
        }

        if (isMetadataSyncRunning || isMetadataStopRequested || isRefreshRunning)
        {
            return true;
        }

        return true;
    }

    public static bool ShouldPublishDownload(
        bool isViewVisible,
        bool isDownloadSyncRunning,
        bool isDownloadStopRequested,
        bool isRetryFailedRunning,
        bool isExportRunning)
    {
        return isViewVisible
            && (isDownloadSyncRunning
                || isDownloadStopRequested
                || isRetryFailedRunning
                || isExportRunning);
    }
}