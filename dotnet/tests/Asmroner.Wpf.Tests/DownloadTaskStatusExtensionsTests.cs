using Asmroner.Core.Download;

namespace Asmroner.Wpf.Tests;

public class DownloadTaskStatusExtensionsTests
{
    [Theory]
    [InlineData(DownloadTaskStatus.Pending, "未下载")]
    [InlineData(DownloadTaskStatus.Queued, "待下载")]
    [InlineData(DownloadTaskStatus.Running, "下载中")]
    [InlineData(DownloadTaskStatus.Completed, "已完成")]
    [InlineData(DownloadTaskStatus.Failed, "已失败")]
    [InlineData(DownloadTaskStatus.Canceled, "已取消")]
    public void GetDisplayName_ReturnsCorrectChineseLabel(DownloadTaskStatus status, string expectedName)
    {
        var displayName = status.GetDisplayName();
        Assert.Equal(expectedName, displayName);
    }

    [Theory]
    [InlineData(DownloadTaskStatus.Completed, 0)]
    [InlineData(DownloadTaskStatus.Running, 1)]
    [InlineData(DownloadTaskStatus.Queued, 2)]
    [InlineData(DownloadTaskStatus.Pending, 3)]
    [InlineData(DownloadTaskStatus.Failed, 4)]
    [InlineData(DownloadTaskStatus.Canceled, 5)]
    public void GetSortOrder_ReturnsCorrectSortOrder(DownloadTaskStatus status, int expectedOrder)
    {
        var sortOrder = status.GetSortOrder();
        Assert.Equal(expectedOrder, sortOrder);
    }

    [Fact]
    public void GetSortOrder_OrdersStatusesCorrectly()
    {
        var statuses = new[]
        {
            DownloadTaskStatus.Canceled,
            DownloadTaskStatus.Failed,
            DownloadTaskStatus.Pending,
            DownloadTaskStatus.Queued,
            DownloadTaskStatus.Running,
            DownloadTaskStatus.Completed,
        };

        var sortedStatuses = statuses
            .OrderBy(s => s.GetSortOrder())
            .ToArray();

        var expectedOrder = new[]
        {
            DownloadTaskStatus.Completed,
            DownloadTaskStatus.Running,
            DownloadTaskStatus.Queued,
            DownloadTaskStatus.Pending,
            DownloadTaskStatus.Failed,
            DownloadTaskStatus.Canceled,
        };

        Assert.Equal(expectedOrder, sortedStatuses);
    }
}
