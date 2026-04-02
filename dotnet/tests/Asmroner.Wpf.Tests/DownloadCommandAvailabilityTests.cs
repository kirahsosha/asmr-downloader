using Asmroner.Core.Download;
using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class DownloadCommandAvailabilityTests
{
    [Fact]
    public void Evaluate_ShouldToggleCommandAvailability_ByTaskState()
    {
        var allTasks = new[]
        {
            CreateTask(DownloadTaskStatus.Running),
            CreateTask(DownloadTaskStatus.Failed),
            CreateTask(DownloadTaskStatus.Completed),
        };

        var runningSelection = DownloadCommandAvailability.Evaluate(
            new[] { CreateRow(DownloadTaskStatus.Running) },
            allTasks);
        var failedSelection = DownloadCommandAvailability.Evaluate(
            new[] { CreateRow(DownloadTaskStatus.Failed, taskId: Guid.NewGuid()) },
            allTasks);
        var mixedEndedSelection = DownloadCommandAvailability.Evaluate(
            new[] { CreateRow(DownloadTaskStatus.Completed), CreateRow(DownloadTaskStatus.Canceled) },
            allTasks);

        Assert.True(runningSelection.CanCancel);
        Assert.False(runningSelection.CanRetry);
        Assert.False(runningSelection.CanStartImmediate);

        Assert.False(failedSelection.CanCancel);
        Assert.True(failedSelection.CanRetry);
        Assert.True(failedSelection.CanStartImmediate);

        Assert.False(mixedEndedSelection.CanCancel);
        Assert.False(mixedEndedSelection.CanRetry);
        Assert.True(mixedEndedSelection.CanStartImmediate);
    }

    [Fact]
    public void Evaluate_ShouldAllowCancel_WhenQueuedTaskSelected()
    {
        var allTasks = new[]
        {
            CreateTask(DownloadTaskStatus.Failed),
        };

        var queuedSelection = DownloadCommandAvailability.Evaluate(
            new[] { CreateRow(DownloadTaskStatus.Queued) },
            allTasks);

        Assert.True(queuedSelection.CanCancel);
        Assert.False(queuedSelection.CanRetry);
        Assert.False(queuedSelection.CanStartImmediate);
    }

    [Fact]
    public void Evaluate_ShouldAllowCancelAndImmediateStart_WhenPendingTaskSelected()
    {
        var availability = DownloadCommandAvailability.Evaluate(
            new[] { CreateRow(DownloadTaskStatus.Pending) },
            new[] { CreateTask(DownloadTaskStatus.Failed) });

        Assert.True(availability.CanCancel);
        Assert.False(availability.CanRetry);
        Assert.True(availability.CanStartImmediate);
    }

    [Fact]
    public void Evaluate_ShouldDisableRetry_ForFailedPlaceholderRow_WhenSelectionExists()
    {
        var availability = DownloadCommandAvailability.Evaluate(
            new[] { CreateRow(DownloadTaskStatus.Failed) },
            new[] { CreateTask(DownloadTaskStatus.Failed) });

        Assert.False(availability.CanRetry);
        Assert.True(availability.CanStartImmediate);
    }

    [Fact]
    public void Evaluate_ShouldAllowRetryWithoutSelection_WhenAnyFailedTaskExists()
    {
        var availability = DownloadCommandAvailability.Evaluate(
            Array.Empty<DownloadTaskRowViewModel>(),
            new[] { CreateTask(DownloadTaskStatus.Failed) });

        Assert.True(availability.CanRetry);
    }

    [Fact]
    public void Evaluate_ShouldAllowRetry_WhenSelectionContainsRetryableFailedTask_AndIgnoreOtherStatuses()
    {
        var availability = DownloadCommandAvailability.Evaluate(
            new[]
            {
                CreateRow(DownloadTaskStatus.Completed),
                CreateRow(DownloadTaskStatus.Failed, Guid.NewGuid()),
            },
            new[] { CreateTask(DownloadTaskStatus.Failed) });

        Assert.True(availability.CanRetry);
    }

    private static DownloadTaskItem CreateTask(DownloadTaskStatus status)
    {
        return new DownloadTaskItem
        {
            SourceId = $"RJ-{status}",
            Status = status,
        };
    }

    private static DownloadTaskRowViewModel CreateRow(DownloadTaskStatus status, Guid? taskId = null)
    {
        return new DownloadTaskRowViewModel
        {
            TaskId = taskId ?? Guid.Empty,
            SourceId = $"RJ-{status}",
            Status = status,
        };
    }
}
