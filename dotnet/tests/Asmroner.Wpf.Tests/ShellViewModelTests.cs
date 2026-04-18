using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class ShellViewModelTests
{
    [Fact]
    public void DefaultState_ShouldStartOnSettingsWithInitializingMessage()
    {
        var viewModel = new ShellViewModel();

        Assert.Equal(ShellPage.Settings, viewModel.SelectedPage);
        Assert.Equal((int)ShellPage.Settings, viewModel.SelectedPageIndex);
        Assert.Equal("初始化中...", viewModel.StatusMessage);
        Assert.False(viewModel.IsSearchEnabled);
        Assert.False(viewModel.IsDownloadEnabled);
        Assert.False(viewModel.IsLibraryEnabled);
        Assert.False(viewModel.IsSyncEnabled);
    }

    [Fact]
    public void SelectedPageIndex_ShouldClampToValidRange()
    {
        var viewModel = new ShellViewModel();

        viewModel.SelectedPageIndex = 99;
        Assert.Equal(ShellPage.Settings, viewModel.SelectedPage);

        viewModel.SelectedPageIndex = -5;
        Assert.Equal(ShellPage.Search, viewModel.SelectedPage);
    }
}