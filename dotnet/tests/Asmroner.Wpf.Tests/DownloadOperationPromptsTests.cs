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
    public void BuildRetryConfirmMessage_ShouldContainSelectedScopePreviewAndEllipsis_WhenExceedingPreviewLimit()
    {
        var sourceIds = new[] { "RJ1001", "RJ1002", "RJ1003", "RJ1004", "RJ1005", "RJ1006" };

        var message = DownloadOperationPrompts.BuildRetryConfirmMessage(2, sourceIds, usesSelection: true);

        Assert.Equal("将按最多 2 并发重试选中的 6 个失败任务：RJ1001, RJ1002, RJ1003, RJ1004, RJ1005 ...\n是否继续？", message);
    }

    [Fact]
    public void BuildRetryConfirmMessage_ShouldContainAllScopeWithoutEllipsis_WhenWithinPreviewLimit()
    {
        var sourceIds = new[] { "RJ2001", "RJ2002" };

        var message = DownloadOperationPrompts.BuildRetryConfirmMessage(4, sourceIds, usesSelection: false);

        Assert.Equal("将按最多 4 并发重试全部 2 个失败任务：RJ2001, RJ2002\n是否继续？", message);
    }
}
