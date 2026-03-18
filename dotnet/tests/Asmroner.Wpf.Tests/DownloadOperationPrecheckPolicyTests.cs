using Asmroner.Core.Download;
using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class DownloadOperationPrecheckPolicyTests
{
    [Fact]
    public void CheckCancel_ShouldReturnPrompt_WhenSelectionEmpty()
    {
        var result = DownloadOperationPrecheckPolicy.CheckCancel(Array.Empty<DownloadTaskRowViewModel>());

        Assert.False(result.CanProceed);
        Assert.Equal("请先选择要取消的任务。", result.StatusText);
        Assert.Empty(result.Cancelable);
    }

    [Fact]
    public void CheckRetrySingle_ShouldRejectMultipleSelection()
    {
        var selected = new[]
        {
            CreateRow("RJ8001", DownloadTaskStatus.Failed, Guid.NewGuid()),
            CreateRow("RJ8002", DownloadTaskStatus.Failed, Guid.NewGuid()),
        };

        var result = DownloadOperationPrecheckPolicy.CheckRetrySingle(selected);

        Assert.False(result.CanProceed);
        Assert.Equal("重试仅支持单个失败任务，请只选择一条记录。", result.StatusText);
        Assert.Null(result.Target);
    }

    [Fact]
    public void CheckRetrySingle_ShouldRejectPlaceholderTask()
    {
        var selected = new[]
        {
            CreateRow("RJ8001", DownloadTaskStatus.Failed, Guid.Empty),
        };

        var result = DownloadOperationPrecheckPolicy.CheckRetrySingle(selected);

        Assert.False(result.CanProceed);
        Assert.Equal("该任务尚未开始执行，无需重试。", result.StatusText);
    }

    [Fact]
    public void CheckRetryAllFailed_ShouldRejectWhenEmpty()
    {
        var result = DownloadOperationPrecheckPolicy.CheckRetryAllFailed(Array.Empty<DownloadTaskItem>());

        Assert.False(result.CanProceed);
        Assert.Equal("当前没有失败任务可重试。", result.StatusText);
        Assert.Empty(result.FailedTasks);
    }

    [Fact]
    public void CheckStartImmediate_ShouldReturnDeduplicatedTargets()
    {
        var selected = new[]
        {
            CreateRow("RJ8101", DownloadTaskStatus.Pending, Guid.Empty),
            CreateRow("RJ8101", DownloadTaskStatus.Failed, Guid.NewGuid()),
            CreateRow("RJ8102", DownloadTaskStatus.Canceled, Guid.NewGuid()),
            CreateRow("RJ8103", DownloadTaskStatus.Running, Guid.NewGuid()),
        };

        var result = DownloadOperationPrecheckPolicy.CheckStartImmediate(selected);

        Assert.True(result.CanProceed);
        Assert.Equal(2, result.Targets.Count);
        Assert.Contains(result.Targets, item => item.SourceId == "RJ8101");
        Assert.Contains(result.Targets, item => item.SourceId == "RJ8102");
    }

    private static DownloadTaskRowViewModel CreateRow(string sourceId, DownloadTaskStatus status, Guid taskId)
    {
        return new DownloadTaskRowViewModel
        {
            SourceId = sourceId,
            Status = status,
            TaskId = taskId,
        };
    }
}
