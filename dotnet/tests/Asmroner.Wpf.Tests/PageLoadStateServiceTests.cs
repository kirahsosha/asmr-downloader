using Asmroner.Wpf.Services;

namespace Asmroner.Wpf.Tests;

public class PageLoadStateServiceTests
{
    [Fact]
    public void Create_ShouldReturnStateForRequestedPage()
    {
        var service = new PageLoadStateService();

        var state = service.Create("Search");

        Assert.Equal("Search", state.PageName);
        Assert.False(state.IsBusy);
        Assert.False(state.IsEmptyVisible);
    }

    [Fact]
    public void ShowBusy_AndHideBusy_ShouldKeepBusyUntilAllOperationsComplete()
    {
        var state = new PageLoadState("Download");

        state.ShowBusy("正在处理下载任务，请稍候...");
        state.ShowBusy("正在刷新下载统计，请稍候...");

        Assert.True(state.IsBusy);
        Assert.Equal("正在刷新下载统计，请稍候...", state.BusyMessage);

        state.HideBusy();

        Assert.True(state.IsBusy);
        Assert.Equal("Download 正在处理，请稍候...", state.BusyMessage);

        state.HideBusy();

        Assert.False(state.IsBusy);
        Assert.Equal(string.Empty, state.BusyMessage);
    }

    [Fact]
    public void ShowEmpty_AndClearEmpty_ShouldToggleEmptyState()
    {
        var state = new PageLoadState("Library");

        state.ShowEmpty("资源库为空", "请先执行下载或同步后再查看。");

        Assert.True(state.IsEmptyVisible);
        Assert.Equal("资源库为空", state.EmptyTitle);
        Assert.Equal("请先执行下载或同步后再查看。", state.EmptyDescription);

        state.ClearEmpty();

        Assert.False(state.IsEmptyVisible);
        Assert.Equal(string.Empty, state.EmptyTitle);
        Assert.Equal(string.Empty, state.EmptyDescription);
    }
}