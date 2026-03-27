using Asmroner.Core.Download;
using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class DownloadTaskRowViewModelTests
{
    [Theory]
    [InlineData(DownloadTaskStatus.Completed)]
    [InlineData(DownloadTaskStatus.Running)]
    [InlineData(DownloadTaskStatus.Queued)]
    [InlineData(DownloadTaskStatus.Pending)]
    [InlineData(DownloadTaskStatus.Failed)]
    [InlineData(DownloadTaskStatus.Canceled)]
    public void From_ShouldSetStatusSortOrder_MatchingGetSortOrder(DownloadTaskStatus status)
    {
        var item = new DownloadTaskItem
        {
            SourceId = "RJ001",
            Status = status,
        };

        var vm = DownloadTaskRowViewModel.From(item);

        Assert.Equal(status.GetSortOrder(), vm.StatusSortOrder);
    }

    [Theory]
    [InlineData(DownloadTaskStatus.Queued)]
    [InlineData(DownloadTaskStatus.Pending)]
    [InlineData(DownloadTaskStatus.Canceled)]
    public void CreatePending_ShouldSetStatusSortOrder_MatchingGetSortOrder(DownloadTaskStatus status)
    {
        var vm = DownloadTaskRowViewModel.CreatePending("RJ001", "テスト", status);

        Assert.Equal(status.GetSortOrder(), vm.StatusSortOrder);
    }
}
