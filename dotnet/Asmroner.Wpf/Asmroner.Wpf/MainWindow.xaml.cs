using System.Windows;
using Asmroner.Core.Initialization;
using Asmroner.Core.Interfaces;
using Asmroner.Wpf.Services;
using Asmroner.Wpf.ViewModels;
using Asmroner.Wpf.Views;
using NLog;

namespace Asmroner.Wpf;

public partial class MainWindow : Window
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private readonly IApplicationBootstrapper _bootstrapper;
    private readonly INavigationService _navigationService;
    private readonly IUiMessageService _uiMessageService;
    private readonly DownloadView _downloadView;
    private readonly LibraryView _libraryView;
    private readonly SyncView _syncView;
    private readonly SettingsView _settingsView;

    public MainWindow(
        SearchView searchView,
        DownloadView downloadView,
        LibraryView libraryView,
        SyncView syncView,
        SettingsView settingsView,
        IApplicationBootstrapper bootstrapper,
        ShellViewModel shellViewModel,
        INavigationService navigationService,
        IUiMessageService uiMessageService)
    {
        InitializeComponent();

        _bootstrapper = bootstrapper;
        _navigationService = navigationService;
        _uiMessageService = uiMessageService;
        _downloadView = downloadView;
        _libraryView = libraryView;
        _syncView = syncView;
        _settingsView = settingsView;

        DataContext = shellViewModel;

        SearchHost.Content = searchView;
        DownloadHost.Content = downloadView;
        LibraryHost.Content = libraryView;
        SyncHost.Content = syncView;
        SettingsHost.Content = settingsView;
        _settingsView.InitializationCompleted += (_, result) => ApplyBootstrapResult(result, navigateToSearchOnSuccess: false);

        Loaded += OnLoaded;

        _logger.Info("MainWindow initialized.");
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        var result = await _bootstrapper.InitializeAsync();
        ApplyBootstrapResult(result, navigateToSearchOnSuccess: true);
    }

    private void ApplyBootstrapResult(BootstrapResult result, bool navigateToSearchOnSuccess)
    {
        if (result.IsSuccess)
        {
            _navigationService.SetPrimaryPagesEnabled(true);
            _navigationService.NavigateTo(navigateToSearchOnSuccess ? ShellPage.Search : ShellPage.Settings);
            _uiMessageService.ShowInfo("初始化完成，可进入主页面。");
            _downloadView.StartUnfinishedQueueMetadataRefreshInBackground();
            _logger.Info("Bootstrap succeeded.");
            return;
        }

        _navigationService.SetPrimaryPagesEnabled(false);
        _navigationService.NavigateTo(ShellPage.Settings);

        if (result.RequiresSetup)
        {
            _uiMessageService.ShowWarning(string.IsNullOrWhiteSpace(result.ErrorMessage)
                ? "检测到配置缺失，请先完成设置。"
                : $"请先完成设置: {result.ErrorMessage}");
            _logger.Warn("Bootstrap requires setup: {Reason}", result.ErrorMessage);
            return;
        }

        _uiMessageService.ShowError(string.IsNullOrWhiteSpace(result.ErrorMessage)
            ? "初始化失败，请检查设置页后重试。"
            : result.ErrorMessage);
        _logger.Error("Bootstrap failed: {Reason}", result.ErrorMessage);

    }
}