using Asmroner.Core.Download;
using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class DownloadTaskListComposerTests
{
    [Fact]
    public void ComposeRows_ShouldIncludeQueuedPendingAndCanceledRows_BeforeActiveRows()
    {
        var activeTasks = new[]
        {
            CreateTask("RJ5001", DownloadTaskStatus.Running, title: "active-1"),
        };

        var queuedSourceIds = new[] { "RJ5002" };
        var titles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["RJ5002"] = "queued-title",
            ["RJ5003"] = "canceled-title",
        };
        var overrides = new Dictionary<string, DownloadTaskStatus>(StringComparer.OrdinalIgnoreCase)
        {
            ["RJ5003"] = DownloadTaskStatus.Canceled,
        };
        var errorMessages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var rows = DownloadTaskListComposer.ComposeRows(activeTasks, queuedSourceIds, titles, overrides, errorMessages);

        Assert.Equal(3, rows.Count);
        // Rows are now sorted by status order (Running=1, Pending=3, Canceled=5), then by SourceId
        Assert.Equal("RJ5001", rows[0].SourceId);
        Assert.Equal("下载中", rows[0].StatusText);

        Assert.Equal("RJ5002", rows[1].SourceId);
        Assert.Equal("未下载", rows[1].StatusText);
        Assert.Equal("queued-title", rows[1].Title);

        Assert.Equal("RJ5003", rows[2].SourceId);
        Assert.Equal("已取消", rows[2].StatusText);
        Assert.Equal("任务已取消。", rows[2].ErrorMessage);
    }

    [Fact]
    public void From_ShouldMapCompletedStatusAndProgressText()
    {
        var item = new DownloadTaskItem
        {
            SourceId = "RJ6001",
            Title = "demo",
            Status = DownloadTaskStatus.Completed,
            CompletedFiles = 5,
            TotalFiles = 5,
            ProgressPercent = 100,
            TargetDirectory = "c:/temp",
        };

        var row = DownloadTaskRowViewModel.From(item);

        Assert.Equal("已完成", row.StatusText);
        Assert.Equal("100% (5/5)", row.ProgressText);
        Assert.Equal("c:/temp", row.TargetDirectory);
    }

    [Fact]
    public void ComposeRows_ShouldResetCanceledOverrideToPending_WhenSourceRequeued()
    {
        var activeTasks = Array.Empty<DownloadTaskItem>();
        var queuedSourceIds = new[] { "RJ7001" };
        var titles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["RJ7001"] = "queued-again",
        };
        var overrides = new Dictionary<string, DownloadTaskStatus>(StringComparer.OrdinalIgnoreCase)
        {
            ["RJ7001"] = DownloadTaskStatus.Canceled,
        };
        var errorMessages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var rows = DownloadTaskListComposer.ComposeRows(activeTasks, queuedSourceIds, titles, overrides, errorMessages);

        var row = Assert.Single(rows);
        Assert.Equal("RJ7001", row.SourceId);
        Assert.Equal("未下载", row.StatusText);
    }

    [Fact]
    public void ComposeRows_ShouldFillActiveTitleFromQueuedTitleCache_WhenTaskTitleIsEmpty()
    {
        var activeTasks = new[]
        {
            CreateTask("RJ7101", DownloadTaskStatus.Pending, title: string.Empty),
        };

        var queuedSourceIds = Array.Empty<string>();
        var titles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["RJ7101"] = "prefetched-title",
        };
        var overrides = new Dictionary<string, DownloadTaskStatus>(StringComparer.OrdinalIgnoreCase);
        var errorMessages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var rows = DownloadTaskListComposer.ComposeRows(activeTasks, queuedSourceIds, titles, overrides, errorMessages);

        var row = Assert.Single(rows);
        Assert.Equal("RJ7101", row.SourceId);
        Assert.Equal("prefetched-title", row.Title);
    }

    [Fact]
    public void ComposeRows_ShouldOrderByStatusAscending_ThenBySourceIdAscending()
    {
        var activeTasks = new[]
        {
            CreateTask("RJ8003", DownloadTaskStatus.Failed, title: "failed"),
            CreateTask("RJ8001", DownloadTaskStatus.Completed, title: "completed-1"),
            CreateTask("RJ8002", DownloadTaskStatus.Completed, title: "completed-2"),
        };

        var queuedSourceIds = new[] { "RJ8004", "RJ8005" };
        var titles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["RJ8004"] = "pending-1",
            ["RJ8005"] = "pending-2",
        };
        var overrides = new Dictionary<string, DownloadTaskStatus>(StringComparer.OrdinalIgnoreCase);
        var errorMessages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var rows = DownloadTaskListComposer.ComposeRows(activeTasks, queuedSourceIds, titles, overrides, errorMessages);

        // Expected order by status sort order: Completed(0) < Pending(3) < Failed(4)
        // Within same status, by SourceId ascending
        Assert.Equal(5, rows.Count);

        // Completed tasks first (RJ8001 < RJ8002)
        Assert.Equal("RJ8001", rows[0].SourceId);
        Assert.Equal("已完成", rows[0].StatusText);
        Assert.Equal("RJ8002", rows[1].SourceId);
        Assert.Equal("已完成", rows[1].StatusText);

        // Pending tasks next (RJ8004 < RJ8005)
        Assert.Equal("RJ8004", rows[2].SourceId);
        Assert.Equal("未下载", rows[2].StatusText);
        Assert.Equal("RJ8005", rows[3].SourceId);
        Assert.Equal("未下载", rows[3].StatusText);

        // Failed tasks last
        Assert.Equal("RJ8003", rows[4].SourceId);
        Assert.Equal("已失败", rows[4].StatusText);
    }

    [Fact]
    public void ComposeRows_ShouldUseQueuedFailureErrorMessage_ForPlaceholderRow()
    {
        var titles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["BJ02370869"] = "bj-title",
        };
        var overrides = new Dictionary<string, DownloadTaskStatus>(StringComparer.OrdinalIgnoreCase)
        {
            ["BJ02370869"] = DownloadTaskStatus.Failed,
        };
        var errorMessages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["BJ02370869"] = "API 调用失败: 404 Not Found",
        };

        var rows = DownloadTaskListComposer.ComposeRows(
            Array.Empty<DownloadTaskItem>(),
            new[] { "BJ02370869" },
            titles,
            overrides,
            errorMessages);

        var row = Assert.Single(rows);
        Assert.Equal(DownloadTaskStatus.Failed, row.Status);
        Assert.Equal("API 调用失败: 404 Not Found", row.ErrorMessage);
    }

    private static DownloadTaskItem CreateTask(string sourceId, DownloadTaskStatus status, string title)
    {
        return new DownloadTaskItem
        {
            SourceId = sourceId,
            Status = status,
            Title = title,
            CompletedFiles = 1,
            TotalFiles = 2,
            ProgressPercent = 50,
        };
    }
}
