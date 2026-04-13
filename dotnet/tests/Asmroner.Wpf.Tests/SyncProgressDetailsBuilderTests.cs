using Asmroner.Core.Sync;
using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class SyncProgressDetailsBuilderTests
{
    [Fact]
    public void BuildRetryPendingDetails_ShouldDescribeRetryInFlight()
    {
        var text = SyncProgressDetailsBuilder.BuildRetryPendingDetails();

        Assert.Equal("正在重试失败同步下载，完成后这里会显示本次重试结果。", text);
    }

    [Fact]
    public void BuildPersistedProgressDetails_ShouldDescribeMetadataAndDownloadProgress()
    {
        var metadataProgress = new MetadataSyncProgressState
        {
            Status = SyncProgressStatuses.Running,
            StopRequested = false,
            NextPage = 3,
            ProcessedPageCount = 2,
            TotalPageCount = 5,
            LocalTotalCount = 240,
            LocalSubtitleCount = 120,
            ProcessedWorkCount = 180,
            InsertedCount = 60,
        };
        var downloadProgress = new SyncDownloadProgressState
        {
            Status = SyncProgressStatuses.Stopped,
            StopRequested = false,
            LastProcessedSourceId = "RJ123456",
            ProcessedCount = 7,
            CompletedCount = 4,
            FailedCount = 3,
            RemainingMetadataCountAfter = 11,
            CompletedSizeBytesAfter = 2048,
            SizeLimitBytes = 4096,
        };

        var text = SyncProgressDetailsBuilder.BuildPersistedProgressDetails(metadataProgress, downloadProgress);

        var expected = string.Join(Environment.NewLine,
        [
            "元数据进度状态：进行中",
            "元数据停止请求：否",
            "元数据下次页码：3",
            "元数据已处理分页：2/5",
            "元数据当前本地总量：240",
            "元数据当前本地字幕量：120",
            "元数据累计处理：180",
            "元数据累计新增：60",
            "元数据最近更新时间：暂无",
            string.Empty,
            "同步下载进度状态：已停止",
            "同步下载停止请求：否",
            "同步下载最近处理作品：RJ123456",
            "同步下载累计处理：7 项",
            "同步下载累计成功：4 项",
            "同步下载累计失败：3 项",
            "同步下载剩余待处理：11 项",
            "同步下载累计大小：2 KB / 4 KB",
            "同步下载最近更新时间：暂无",
        ]);

        Assert.Equal(expected, text);
    }

    [Fact]
    public void BuildRetryRefreshDetails_ShouldDescribeCurrentRetrySnapshot()
    {
        var report = new SyncReportSnapshot
        {
            CompletedCount = 8,
            FailedCount = 3,
            RemainingMetadataCount = 5,
            PendingCount = 2,
            CompletedSizeBytes = 2048,
        };

        var text = SyncProgressDetailsBuilder.BuildRetryRefreshDetails(report);

        var expected = string.Join(Environment.NewLine,
        [
            "当前正在重试失败同步下载。",
            "当前已完成总数：8 项",
            "当前失败总数：3 项",
            "当前待处理总数：5 项",
            "当前队列中数量：2 项",
            "当前已落盘大小：2 KB",
            "最近同步记录更新时间：暂无",
            "刷新统计已保留当前重试状态，完成后这里会显示本次重试结果。",
        ]);

        Assert.Equal(expected, text);
    }
}