using Asmroner.Core.Sync;
using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class SyncStatusTextBuilderTests
{
    [Fact]
    public void BuildMetadataStatus_ShouldDescribeRunningProgress()
    {
        var progress = new MetadataSyncProgressState
        {
            Status = SyncProgressStatuses.Running,
            NextPage = 3,
            ProcessedPageCount = 2,
            TotalPageCount = 5,
            ProcessedWorkCount = 180,
            LocalTotalCount = 240,
        };

        var text = SyncStatusTextBuilder.BuildMetadataStatus(progress);

        Assert.Equal("同步状态：元数据同步进行中：已处理 2/5 页，累计处理 180 条，本地现有 240 条，下次页码 3。", text);
    }

    [Fact]
    public void BuildMetadataStatus_ShouldDescribeStoppedProgress()
    {
        var progress = new MetadataSyncProgressState
        {
            Status = SyncProgressStatuses.Stopped,
            NextPage = 4,
            ProcessedWorkCount = 260,
            LocalTotalCount = 400,
        };

        var text = SyncStatusTextBuilder.BuildMetadataStatus(progress);

        Assert.Equal("同步状态：检测到未完成元数据同步进度，累计处理 260 条，本地现有 400 条，下次将从第 4 页继续。", text);
    }

    [Fact]
    public void BuildMetadataStatus_ShouldDescribeCompletedProgressWithoutNextPageHint()
    {
        var progress = new MetadataSyncProgressState
        {
            Status = SyncProgressStatuses.Completed,
            ProcessedPageCount = 10,
            TotalPageCount = 10,
            ProcessedWorkCount = 640,
            LocalTotalCount = 640,
        };

        var text = SyncStatusTextBuilder.BuildMetadataStatus(progress);

        Assert.Equal("同步状态：元数据同步已完成：累计处理 640 条，本地现有 640 条。", text);
    }

    [Fact]
    public void BuildDownloadStatus_ShouldDescribeStoppingProgress()
    {
        var progress = new SyncDownloadProgressState
        {
            Status = SyncProgressStatuses.Running,
            StopRequested = true,
            ProcessedCount = 7,
        };

        var text = SyncStatusTextBuilder.BuildDownloadStatus(progress);

        Assert.Equal("下载状态：同步下载正在停止：已处理 7 项，等待当前作品完成。", text);
    }

    [Fact]
    public void BuildDownloadStatus_ShouldDescribeCompletedProgress()
    {
        var progress = new SyncDownloadProgressState
        {
            Status = SyncProgressStatuses.Completed,
            ProcessedCount = 12,
            CompletedCount = 10,
            FailedCount = 2,
        };

        var text = SyncStatusTextBuilder.BuildDownloadStatus(progress);

        Assert.Equal("下载状态：同步下载已完成：已处理 12 项，成功 10 项，失败 2 项。", text);
    }
}