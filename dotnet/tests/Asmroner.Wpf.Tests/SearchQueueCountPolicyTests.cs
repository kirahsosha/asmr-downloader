using Asmroner.Core.Download;
using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class SearchQueueCountPolicyTests
{
    [Fact]
    public void Build_ShouldCountSkippedFromExistingQueuedAndInputDuplicates()
    {
        var sourceIds = new[] { "RJ1", "rj1", "RJ2", "RJ3", "RJ4", "RJ4" };
        var existingTasks = new[]
        {
            new DownloadTaskItem { SourceId = "RJ3", Status = DownloadTaskStatus.Pending },
        };
        var queued = new[] { "RJ2" };

        var plan = SearchQueueCountPolicy.Build(sourceIds, existingTasks, queued);

        Assert.Equal(new[] { "RJ1", "RJ4" }, plan.ToEnqueue);
        Assert.Equal(4, plan.SkippedCount);
    }

    [Fact]
    public void Build_ShouldSkipQueuedItems_WhenTaskListEmpty()
    {
        var sourceIds = new[] { "RJ1001", "RJ1002" };
        var queued = new[] { "RJ1001", "RJ1002" };

        var plan = SearchQueueCountPolicy.Build(sourceIds, Array.Empty<DownloadTaskItem>(), queued);

        Assert.Empty(plan.ToEnqueue);
        Assert.Equal(2, plan.SkippedCount);
    }

    [Fact]
    public void Build_ShouldReturnEmpty_WhenInputInvalid()
    {
        var sourceIds = new[] { "", "   " };
        var plan = SearchQueueCountPolicy.Build(sourceIds, Array.Empty<DownloadTaskItem>(), Array.Empty<string>());

        Assert.Empty(plan.ToEnqueue);
        Assert.Equal(0, plan.SkippedCount);
    }
}
