using Asmroner.Core.Download;
using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class DownloadUnfinishedQueueSnapshotPolicyTests
{
    [Fact]
    public void BuildSnapshot_ShouldIncludePendingQueuedFailedAndQueuedSourceIds()
    {
        var activeTasks = new[]
        {
            CreateTask("RJ5003", DownloadTaskStatus.Pending),
            CreateTask("RJ5001", DownloadTaskStatus.Queued),
            CreateTask("RJ5005", DownloadTaskStatus.Failed),
            CreateTask("RJ5007", DownloadTaskStatus.Running),
            CreateTask("RJ5008", DownloadTaskStatus.Completed),
            CreateTask("RJ5009", DownloadTaskStatus.Canceled),
        };

        var queuedSourceIds = new[]
        {
            "RJ5002",
            "rj5002",
            "RJ5007",
            " ",
        };

        var snapshot = DownloadUnfinishedQueueSnapshotPolicy.BuildSnapshot(activeTasks, queuedSourceIds);

        Assert.Equal(new[] { "RJ5001", "RJ5002", "RJ5003", "RJ5005", "RJ5007" }, snapshot);
    }

    [Fact]
    public void BuildSnapshot_ShouldReturnEmpty_WhenNoActiveAndQueueEmpty()
    {
        var snapshot = DownloadUnfinishedQueueSnapshotPolicy.BuildSnapshot(
            Array.Empty<DownloadTaskItem>(),
            Array.Empty<string>());

        Assert.Empty(snapshot);
    }

    [Fact]
    public void BuildSnapshot_ShouldDeduplicateCaseInsensitiveAcrossSources()
    {
        var activeTasks = new[]
        {
            CreateTask("rj7001", DownloadTaskStatus.Pending),
        };

        var queuedSourceIds = new[]
        {
            "RJ7001",
            "rj7002",
        };

        var snapshot = DownloadUnfinishedQueueSnapshotPolicy.BuildSnapshot(activeTasks, queuedSourceIds);

        Assert.Equal(2, snapshot.Count);
        Assert.Contains("RJ7001", snapshot, StringComparer.OrdinalIgnoreCase);
        Assert.Contains("RJ7002", snapshot, StringComparer.OrdinalIgnoreCase);
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