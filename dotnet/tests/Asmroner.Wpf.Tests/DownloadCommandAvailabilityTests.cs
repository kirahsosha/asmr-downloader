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
            new[] { DownloadTaskStatus.Running },
            allTasks);
        var failedSelection = DownloadCommandAvailability.Evaluate(
            new[] { DownloadTaskStatus.Failed },
            allTasks);
        var mixedEndedSelection = DownloadCommandAvailability.Evaluate(
            new[] { DownloadTaskStatus.Completed, DownloadTaskStatus.Canceled },
            allTasks);

        Assert.True(runningSelection.CanCancel);
        Assert.False(runningSelection.CanRetry);
        Assert.True(runningSelection.CanRetryAllFailed);
        Assert.False(runningSelection.CanStartImmediate);

        Assert.False(failedSelection.CanCancel);
        Assert.True(failedSelection.CanRetry);
        Assert.True(failedSelection.CanRetryAllFailed);
        Assert.True(failedSelection.CanStartImmediate);

        Assert.False(mixedEndedSelection.CanCancel);
        Assert.False(mixedEndedSelection.CanRetry);
        Assert.True(mixedEndedSelection.CanRetryAllFailed);
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
            new[] { DownloadTaskStatus.Queued },
            allTasks);

        Assert.True(queuedSelection.CanCancel);
        Assert.False(queuedSelection.CanRetry);
        Assert.True(queuedSelection.CanRetryAllFailed);
        Assert.False(queuedSelection.CanStartImmediate);
    }

    [Fact]
    public void Evaluate_ShouldAllowCancelAndImmediateStart_WhenPendingTaskSelected()
    {
        var availability = DownloadCommandAvailability.Evaluate(
            new[] { DownloadTaskStatus.Pending },
            new[] { CreateTask(DownloadTaskStatus.Failed) });

        Assert.True(availability.CanCancel);
        Assert.False(availability.CanRetry);
        Assert.True(availability.CanRetryAllFailed);
        Assert.True(availability.CanStartImmediate);
    }

    private static DownloadTaskItem CreateTask(DownloadTaskStatus status)
    {
        return new DownloadTaskItem
        {
            SourceId = $"RJ-{status}",
            Status = status,
        };
    }
}
