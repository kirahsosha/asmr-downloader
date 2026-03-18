using Asmroner.Core.Download;
using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class DownloadTaskSelectionPolicyTests
{
    [Fact]
    public void GetCancelable_ShouldReturnOnlyPendingQueuedRunning()
    {
        var selected = new[]
        {
            CreateRow("RJ7001", DownloadTaskStatus.Pending),
            CreateRow("RJ7002", DownloadTaskStatus.Queued),
            CreateRow("RJ7003", DownloadTaskStatus.Running),
            CreateRow("RJ7004", DownloadTaskStatus.Failed),
            CreateRow("RJ7005", DownloadTaskStatus.Completed),
            CreateRow("RJ7006", DownloadTaskStatus.Canceled),
        };

        var cancelable = DownloadTaskSelectionPolicy.GetCancelable(selected);

        Assert.Equal(3, cancelable.Count);
        Assert.All(cancelable, item => Assert.Contains(item.Status, new[]
        {
            DownloadTaskStatus.Pending,
            DownloadTaskStatus.Queued,
            DownloadTaskStatus.Running,
        }));
    }

    [Fact]
    public void GetImmediateStartTargets_ShouldFilterStatusAndDeduplicateBySourceId()
    {
        var selected = new[]
        {
            CreateRow("RJ7101", DownloadTaskStatus.Pending),
            CreateRow("RJ7101", DownloadTaskStatus.Failed),
            CreateRow("RJ7102", DownloadTaskStatus.Canceled),
            CreateRow("RJ7103", DownloadTaskStatus.Running),
            CreateRow("RJ7104", DownloadTaskStatus.Completed),
        };

        var targets = DownloadTaskSelectionPolicy.GetImmediateStartTargets(selected);

        Assert.Equal(2, targets.Count);
        Assert.Contains(targets, item => item.SourceId == "RJ7101");
        Assert.Contains(targets, item => item.SourceId == "RJ7102");
    }

    private static DownloadTaskRowViewModel CreateRow(string sourceId, DownloadTaskStatus status)
    {
        return new DownloadTaskRowViewModel
        {
            SourceId = sourceId,
            Status = status,
        };
    }
}
