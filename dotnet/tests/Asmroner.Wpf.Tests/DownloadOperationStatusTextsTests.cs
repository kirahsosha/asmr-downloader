using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class DownloadOperationStatusTextsTests
{
    [Fact]
    public void BuildBatchEnqueueResult_ShouldIncludeResolvedSuffix_WhenPartialResolved()
    {
        var message = DownloadOperationStatusTexts.BuildBatchEnqueueResult(6, 4);

        Assert.Equal("已加入批量下载：6 个任务（4 个已更新作品信息）。", message);
    }

    [Fact]
    public void BuildRunQueueResult_ShouldShowEmptyMessage_WhenNoTaskCreated()
    {
        var message = DownloadOperationStatusTexts.BuildRunQueueResult(0);

        Assert.Equal("下载队列为空，无需执行。", message);
    }

    [Fact]
    public void BuildCancelResult_ShouldShowSummary_WhenAnyTaskCanceled()
    {
        var message = DownloadOperationStatusTexts.BuildCancelResult(2, 3);

        Assert.Equal("已取消 2/3 个任务。", message);
    }

    [Fact]
    public void BuildRetryResult_ShouldReturnUnsupportedMessage_WhenNotRetried()
    {
        var message = DownloadOperationStatusTexts.BuildRetryResult(false, "RJ3001");

        Assert.Equal("仅失败状态任务支持重试。", message);
    }

    [Fact]
    public void BuildRetryAllResult_ShouldContainRetriedSummary()
    {
        var message = DownloadOperationStatusTexts.BuildRetryAllResult(3, 5);

        Assert.Equal("批量重试完成：成功触发 3/5。", message);
    }

    [Fact]
    public void BuildStartSelectedResult_ShouldReturnNoStartMessage_WhenNoneStarted()
    {
        var message = DownloadOperationStatusTexts.BuildStartSelectedResult(0, 2);

        Assert.Equal("没有可立即下载的任务。", message);
    }
}
