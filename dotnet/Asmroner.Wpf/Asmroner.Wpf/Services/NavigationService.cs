using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Services;

public interface INavigationService
{
    ShellPage CurrentPage { get; }

    void NavigateTo(ShellPage page);

    void SetPrimaryPagesEnabled(bool isEnabled);

    void SetPageEnabled(ShellPage page, bool isEnabled);

    bool IsPageEnabled(ShellPage page);
}

public sealed class NavigationService : INavigationService
{
    private readonly ShellViewModel _shellViewModel;

    public NavigationService(ShellViewModel shellViewModel)
    {
        _shellViewModel = shellViewModel;
    }

    public ShellPage CurrentPage => _shellViewModel.SelectedPage;

    public void NavigateTo(ShellPage page)
    {
        _shellViewModel.SelectedPage = page;
    }

    public void SetPrimaryPagesEnabled(bool isEnabled)
    {
        _shellViewModel.IsSearchEnabled = isEnabled;
        _shellViewModel.IsDownloadEnabled = isEnabled;
        _shellViewModel.IsLibraryEnabled = isEnabled;
        _shellViewModel.IsSyncEnabled = isEnabled;

        if (!isEnabled && CurrentPage != ShellPage.Settings)
        {
            NavigateTo(ShellPage.Settings);
        }
    }

    public void SetPageEnabled(ShellPage page, bool isEnabled)
    {
        switch (page)
        {
            case ShellPage.Search:
                _shellViewModel.IsSearchEnabled = isEnabled;
                break;
            case ShellPage.Download:
                _shellViewModel.IsDownloadEnabled = isEnabled;
                break;
            case ShellPage.Library:
                _shellViewModel.IsLibraryEnabled = isEnabled;
                break;
            case ShellPage.Sync:
                _shellViewModel.IsSyncEnabled = isEnabled;
                break;
            case ShellPage.Settings:
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(page), page, null);
        }

        if (!isEnabled && CurrentPage == page)
        {
            NavigateTo(ShellPage.Settings);
        }
    }

    public bool IsPageEnabled(ShellPage page)
    {
        return page switch
        {
            ShellPage.Search => _shellViewModel.IsSearchEnabled,
            ShellPage.Download => _shellViewModel.IsDownloadEnabled,
            ShellPage.Library => _shellViewModel.IsLibraryEnabled,
            ShellPage.Sync => _shellViewModel.IsSyncEnabled,
            ShellPage.Settings => true,
            _ => throw new ArgumentOutOfRangeException(nameof(page), page, null),
        };
    }
}