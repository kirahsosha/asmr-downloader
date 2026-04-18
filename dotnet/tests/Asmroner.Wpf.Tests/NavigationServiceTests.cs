using Asmroner.Wpf.Services;
using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class NavigationServiceTests
{
    [Fact]
    public void NavigateTo_ShouldUpdateCurrentPageAndSelectedIndex()
    {
        var viewModel = new ShellViewModel();
        var service = new NavigationService(viewModel);

        service.NavigateTo(ShellPage.Library);

        Assert.Equal(ShellPage.Library, service.CurrentPage);
        Assert.Equal((int)ShellPage.Library, viewModel.SelectedPageIndex);
    }

    [Fact]
    public void SetPrimaryPagesEnabled_ShouldToggleContentTabsAndFallbackToSettings()
    {
        var viewModel = new ShellViewModel();
        var service = new NavigationService(viewModel);

        service.SetPrimaryPagesEnabled(true);
        service.NavigateTo(ShellPage.Search);
        service.SetPrimaryPagesEnabled(false);

        Assert.False(viewModel.IsSearchEnabled);
        Assert.False(viewModel.IsDownloadEnabled);
        Assert.False(viewModel.IsLibraryEnabled);
        Assert.False(viewModel.IsSyncEnabled);
        Assert.Equal(ShellPage.Settings, service.CurrentPage);
        Assert.True(service.IsPageEnabled(ShellPage.Settings));
    }
}