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
    public void CheckRetry_ShouldReturnSelectedFailedTargets_AndIgnoreOtherStatuses()
    {
        var selected = new[]
        {
            CreateRow("RJ8001", DownloadTaskStatus.Failed, Guid.NewGuid()),
            CreateRow("RJ8002", DownloadTaskStatus.Completed, Guid.NewGuid()),
            CreateRow("RJ8003", DownloadTaskStatus.Failed, Guid.NewGuid()),
        };

        var result = DownloadOperationPrecheckPolicy.CheckRetry(selected, Array.Empty<DownloadTaskItem>());

        Assert.True(result.CanProceed);
        Assert.True(result.UsesSelection);
        Assert.Equal(2, result.Targets.Count);
        Assert.Contains(result.Targets, item => item.SourceId == "RJ8001");
        Assert.Contains(result.Targets, item => item.SourceId == "RJ8003");
    }

    [Fact]
    public void CheckRetry_ShouldRejectWhenSelectionHasNoRetryableFailedTargets()
    {
        var selected = new[]
        {
            CreateRow("RJ8001", DownloadTaskStatus.Failed, Guid.Empty),
            CreateRow("RJ8002", DownloadTaskStatus.Completed, Guid.NewGuid()),
        };

        var result = DownloadOperationPrecheckPolicy.CheckRetry(selected, Array.Empty<DownloadTaskItem>());

        Assert.False(result.CanProceed);
        Assert.Equal("选中项中没有可重试的失败任务。", result.StatusText);
        Assert.True(result.UsesSelection);
        Assert.Empty(result.Targets);
    }

    [Fact]
    public void CheckRetry_ShouldFallbackToAllFailed_WhenSelectionEmpty()
    {
        var result = DownloadOperationPrecheckPolicy.CheckRetry(
            Array.Empty<DownloadTaskRowViewModel>(),
            new[]
            {
                CreateTask("RJ8011", DownloadTaskStatus.Failed, Guid.NewGuid()),
                CreateTask("RJ8012", DownloadTaskStatus.Completed, Guid.NewGuid()),
            });

        Assert.True(result.CanProceed);
        Assert.False(result.UsesSelection);
        var target = Assert.Single(result.Targets);
        Assert.Equal("RJ8011", target.SourceId);
    }

    [Fact]
    public void CheckRetry_ShouldRejectWhenSelectionEmpty_AndNoFailedTasksExist()
    {
        var result = DownloadOperationPrecheckPolicy.CheckRetry(
            Array.Empty<DownloadTaskRowViewModel>(),
            Array.Empty<DownloadTaskItem>());

        Assert.False(result.CanProceed);
        Assert.Equal("当前没有失败任务可重试。", result.StatusText);
        Assert.False(result.UsesSelection);
        Assert.Empty(result.Targets);
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

    private static DownloadTaskItem CreateTask(string sourceId, DownloadTaskStatus status, Guid taskId)
    {
        return new DownloadTaskItem
        {
            SourceId = sourceId,
            Status = status,
            TaskId = taskId,
        };
    }
}
