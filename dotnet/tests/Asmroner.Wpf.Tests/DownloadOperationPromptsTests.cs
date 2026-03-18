using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class DownloadOperationPromptsTests
{
    [Fact]
    public void BuildCancelConfirmMessage_ShouldContainCancelableCount()
    {
        var message = DownloadOperationPrompts.BuildCancelConfirmMessage(3);

        Assert.Equal("将取消 3 个任务，是否继续？", message);
    }

    [Fact]
    public void BuildRetryAllConfirmMessage_ShouldContainPreviewAndEllipsis_WhenExceedingPreviewLimit()
    {
        var sourceIds = new[] { "RJ1001", "RJ1002", "RJ1003", "RJ1004", "RJ1005", "RJ1006" };

        var message = DownloadOperationPrompts.BuildRetryAllConfirmMessage(2, sourceIds);

        Assert.Equal("将按最多 2 并发重试 6 个失败任务：RJ1001, RJ1002, RJ1003, RJ1004, RJ1005 ...\n是否继续？", message);
    }

    [Fact]
    public void BuildRetryAllConfirmMessage_ShouldNotUseEllipsis_WhenWithinPreviewLimit()
    {
        var sourceIds = new[] { "RJ2001", "RJ2002" };

        var message = DownloadOperationPrompts.BuildRetryAllConfirmMessage(4, sourceIds);

        Assert.Equal("将按最多 4 并发重试 2 个失败任务：RJ2001, RJ2002\n是否继续？", message);
    }
}
