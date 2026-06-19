using Asmroner.Wpf.Services;

namespace Asmroner.Wpf.Tests;

public class SyncShellStatusRelayPolicyTests
{
    [Fact]
    public void ShouldPublishMetadata_ShouldReturnFalse_WhenViewIsHidden()
    {
        var result = SyncShellStatusRelayPolicy.ShouldPublishMetadata(
            isViewVisible: false,
            isMetadataSyncRunning: true,
            isMetadataStopRequested: false,
            isDownloadSyncRunning: false,
            isDownloadStopRequested: false,
            isRetryFailedRunning: false,
            isExportRunning: false,
            isRefreshRunning: false);

        Assert.False(result);
    }

    [Fact]
    public void ShouldPublishMetadata_ShouldReturnFalse_WhenDownloadFlowOwnsStatus()
    {
        var result = SyncShellStatusRelayPolicy.ShouldPublishMetadata(
            isViewVisible: true,
            isMetadataSyncRunning: true,
            isMetadataStopRequested: false,
            isDownloadSyncRunning: true,
            isDownloadStopRequested: false,
            isRetryFailedRunning: false,
            isExportRunning: false,
            isRefreshRunning: false);

        Assert.False(result);
    }

    [Fact]
    public void ShouldPublishMetadata_ShouldReturnTrue_WhenViewVisibleAndNoDownloadConflict()
    {
        var result = SyncShellStatusRelayPolicy.ShouldPublishMetadata(
            isViewVisible: true,
            isMetadataSyncRunning: false,
            isMetadataStopRequested: false,
            isDownloadSyncRunning: false,
            isDownloadStopRequested: false,
            isRetryFailedRunning: false,
            isExportRunning: false,
            isRefreshRunning: false);

        Assert.True(result);
    }

    [Fact]
    public void ShouldPublishDownload_ShouldReturnTrue_WhenDownloadRunning()
    {
        var result = SyncShellStatusRelayPolicy.ShouldPublishDownload(
            isViewVisible: true,
            isDownloadSyncRunning: true,
            isDownloadStopRequested: false,
            isRetryFailedRunning: false,
            isExportRunning: false);

        Assert.True(result);
    }

    [Fact]
    public void ShouldPublishDownload_ShouldReturnFalse_WhenViewHidden()
    {
        var result = SyncShellStatusRelayPolicy.ShouldPublishDownload(
            isViewVisible: false,
            isDownloadSyncRunning: true,
            isDownloadStopRequested: false,
            isRetryFailedRunning: false,
            isExportRunning: false);

        Assert.False(result);
    }
}
