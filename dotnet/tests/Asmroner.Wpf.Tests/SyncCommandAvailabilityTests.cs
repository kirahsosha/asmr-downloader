using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class SyncCommandAvailabilityTests
{
    [Fact]
    public void Evaluate_ShouldExposeStartTexts_WhenIdle()
    {
        var actual = SyncCommandAvailability.Evaluate(
            isMetadataSyncRunning: false,
            isMetadataStopRequested: false,
            isDownloadSyncRunning: false,
            isDownloadStopRequested: false,
            isExclusiveOperationRunning: false,
            isRefreshRunning: false);

        Assert.True(actual.CanMetadataAction);
        Assert.Equal("开始同步元数据", actual.MetadataActionText);
        Assert.True(actual.CanDownloadAction);
        Assert.Equal("开始同步下载", actual.DownloadActionText);
        Assert.True(actual.CanRefresh);
    }

    [Fact]
    public void Evaluate_ShouldKeepRefreshEnabled_WhenMetadataSyncIsRunning()
    {
        var actual = SyncCommandAvailability.Evaluate(
            isMetadataSyncRunning: true,
            isMetadataStopRequested: false,
            isDownloadSyncRunning: false,
            isDownloadStopRequested: false,
            isExclusiveOperationRunning: false,
            isRefreshRunning: false);

        Assert.True(actual.CanMetadataAction);
        Assert.Equal("停止同步元数据", actual.MetadataActionText);
        Assert.False(actual.CanDownloadAction);
        Assert.False(actual.CanRetryFailed);
        Assert.False(actual.CanExportFailed);
        Assert.False(actual.CanExportCompleted);
        Assert.True(actual.CanRefresh);
    }

    [Fact]
    public void Evaluate_ShouldShowStoppingText_WhenMetadataStopAlreadyRequested()
    {
        var actual = SyncCommandAvailability.Evaluate(
            isMetadataSyncRunning: true,
            isMetadataStopRequested: true,
            isDownloadSyncRunning: false,
            isDownloadStopRequested: false,
            isExclusiveOperationRunning: false,
            isRefreshRunning: false);

        Assert.False(actual.CanMetadataAction);
        Assert.Equal("正在停止元数据...", actual.MetadataActionText);
        Assert.False(actual.CanDownloadAction);
        Assert.True(actual.CanRefresh);
    }

    [Fact]
    public void Evaluate_ShouldShowStoppingText_WhenDownloadStopAlreadyRequested()
    {
        var actual = SyncCommandAvailability.Evaluate(
            isMetadataSyncRunning: false,
            isMetadataStopRequested: false,
            isDownloadSyncRunning: true,
            isDownloadStopRequested: true,
            isExclusiveOperationRunning: false,
            isRefreshRunning: false);

        Assert.False(actual.CanMetadataAction);
        Assert.False(actual.CanDownloadAction);
        Assert.Equal("正在停止下载...", actual.DownloadActionText);
        Assert.True(actual.CanRefresh);
    }

    [Fact]
    public void Evaluate_ShouldDisableRefreshWhileRefreshIsRunning_ButKeepStopAvailable()
    {
        var actual = SyncCommandAvailability.Evaluate(
            isMetadataSyncRunning: true,
            isMetadataStopRequested: false,
            isDownloadSyncRunning: false,
            isDownloadStopRequested: false,
            isExclusiveOperationRunning: false,
            isRefreshRunning: true);

        Assert.True(actual.CanMetadataAction);
        Assert.Equal("停止同步元数据", actual.MetadataActionText);
        Assert.False(actual.CanDownloadAction);
        Assert.False(actual.CanRetryFailed);
        Assert.False(actual.CanExportFailed);
        Assert.False(actual.CanExportCompleted);
        Assert.False(actual.CanRefresh);
    }

    [Fact]
    public void Evaluate_ShouldDisableAllActions_WhenExclusiveOperationRunning()
    {
        var actual = SyncCommandAvailability.Evaluate(
            isMetadataSyncRunning: false,
            isMetadataStopRequested: false,
            isDownloadSyncRunning: false,
            isDownloadStopRequested: false,
            isExclusiveOperationRunning: true,
            isRefreshRunning: false);

        Assert.False(actual.CanMetadataAction);
        Assert.False(actual.CanDownloadAction);
        Assert.False(actual.CanRetryFailed);
        Assert.False(actual.CanExportFailed);
        Assert.False(actual.CanExportCompleted);
        Assert.False(actual.CanRefresh);
    }
}