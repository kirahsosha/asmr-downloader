using Asmroner.Core.Download;
using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class DownloadTaskSnapshotPolicyTests
{
    [Fact]
    public void GetFailedTasks_ShouldReturnOnlyFailedItems()
    {
        var tasks = new[]
        {
            CreateTask("RJ4101", DownloadTaskStatus.Pending),
            CreateTask("RJ4102", DownloadTaskStatus.Failed),
            CreateTask("RJ4103", DownloadTaskStatus.Completed),
            CreateTask("RJ4104", DownloadTaskStatus.Failed),
        };

        var failed = DownloadTaskSnapshotPolicy.GetFailedTasks(tasks);

        Assert.Equal(2, failed.Count);
        Assert.All(failed, item => Assert.Equal(DownloadTaskStatus.Failed, item.Status));
    }

    [Fact]
    public void GetActiveSourceIds_ShouldReturnCaseInsensitiveDeduplicatedSet()
    {
        var tasks = new[]
        {
            CreateTask("rj4201", DownloadTaskStatus.Pending),
            CreateTask("RJ4201", DownloadTaskStatus.Running),
            CreateTask("RJ4202", DownloadTaskStatus.Completed),
        };

        var sourceIds = DownloadTaskSnapshotPolicy.GetActiveSourceIds(tasks);

        Assert.Equal(2, sourceIds.Count);
        Assert.Contains("RJ4201", sourceIds);
        Assert.Contains("rj4202", sourceIds);
    }

    private static DownloadTaskItem CreateTask(string sourceId, DownloadTaskStatus status)
    {
        return new DownloadTaskItem
        {
            SourceId = sourceId,
            Status = status,
        };
    }
}
