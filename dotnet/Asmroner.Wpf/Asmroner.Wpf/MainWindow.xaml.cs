using System.Windows;
using Asmroner.Core.Initialization;
using Asmroner.Core.Interfaces;
using Asmroner.Wpf.Views;
using NLog;

namespace Asmroner.Wpf;

public partial class MainWindow : Window
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private readonly IApplicationBootstrapper _bootstrapper;
    private readonly DownloadView _downloadView;
    private readonly SettingsView _settingsView;

    public MainWindow(
        SearchView searchView,
        DownloadView downloadView,
        SettingsView settingsView,
        IApplicationBootstrapper bootstrapper)
    {
        InitializeComponent();

        _bootstrapper = bootstrapper;
        _downloadView = downloadView;
        _settingsView = settingsView;

        SearchHost.Content = searchView;
        DownloadHost.Content = downloadView;
        SettingsHost.Content = settingsView;
        _settingsView.InitializationCompleted += (_, result) => ApplyBootstrapResult(result, navigateToSearchOnSuccess: false);

        Loaded += OnLoaded;

        _logger.Info("MainWindow initialized with phase 1 workflow.");
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
            SearchTab.IsEnabled = true;
            DownloadTab.IsEnabled = true;
            MainTabControl.SelectedItem = navigateToSearchOnSuccess ? SearchTab : SettingsTab;
            StatusTextBlock.Text = "初始化完成，可进入主页面。";
            _downloadView.StartUnfinishedQueueMetadataRefreshInBackground();
            _logger.Info("Bootstrap succeeded.");
            return;
        }

        MainTabControl.SelectedItem = SettingsTab;

        if (result.RequiresSetup)
        {
            SearchTab.IsEnabled = false;
            DownloadTab.IsEnabled = false;
            StatusTextBlock.Text = string.IsNullOrWhiteSpace(result.ErrorMessage)
                ? "检测到配置缺失，请先完成设置。"
                : $"请先完成设置: {result.ErrorMessage}";
            _logger.Warn("Bootstrap requires setup: {Reason}", result.ErrorMessage);
            return;
        }

        SearchTab.IsEnabled = false;
        DownloadTab.IsEnabled = false;
        StatusTextBlock.Text = string.IsNullOrWhiteSpace(result.ErrorMessage)
            ? "初始化失败，请检查设置页后重试。"
            : result.ErrorMessage;
        _logger.Error("Bootstrap failed: {Reason}", result.ErrorMessage);

    }
}