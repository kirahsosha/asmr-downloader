using System.Windows;
using Asmroner.Core.Initialization;
using Asmroner.Core.Interfaces;
using Asmroner.Wpf.Views;
using Microsoft.Extensions.Logging;

namespace Asmroner.Wpf;

public partial class MainWindow : Window
{
    private readonly IApplicationBootstrapper _bootstrapper;
    private readonly SettingsView _settingsView;
    private readonly ILogger<MainWindow> _logger;

    public MainWindow(
        DashboardView dashboardView,
        DownloadView downloadView,
        SettingsView settingsView,
        IApplicationBootstrapper bootstrapper,
        ILogger<MainWindow> logger)
    {
        InitializeComponent();

        _bootstrapper = bootstrapper;
        _settingsView = settingsView;
        _logger = logger;

        DashboardHost.Content = dashboardView;
        DownloadHost.Content = downloadView;
        SettingsHost.Content = settingsView;
        _settingsView.InitializationCompleted += (_, result) => ApplyBootstrapResult(result);

        Loaded += OnLoaded;

        logger.LogInformation("MainWindow initialized with phase 1 workflow.");
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        var result = await _bootstrapper.InitializeAsync();
        ApplyBootstrapResult(result);
    }

    private void ApplyBootstrapResult(BootstrapResult result)
    {
        if (result.IsSuccess)
        {
            DashboardTab.IsEnabled = true;
            DownloadTab.IsEnabled = true;
            MainTabControl.SelectedItem = DashboardTab;
            StatusTextBlock.Text = "初始化完成，可进入主页面。";
            _logger.LogInformation("Bootstrap succeeded.");
            return;
        }

        MainTabControl.SelectedItem = SettingsTab;

        if (result.RequiresSetup)
        {
            DashboardTab.IsEnabled = false;
            DownloadTab.IsEnabled = false;
            StatusTextBlock.Text = string.IsNullOrWhiteSpace(result.ErrorMessage)
                ? "检测到配置缺失，请先完成设置。"
                : $"请先完成设置: {result.ErrorMessage}";
            _logger.LogWarning("Bootstrap requires setup: {Reason}", result.ErrorMessage);
            return;
        }

        DashboardTab.IsEnabled = false;
        DownloadTab.IsEnabled = false;
        StatusTextBlock.Text = string.IsNullOrWhiteSpace(result.ErrorMessage)
            ? "初始化失败，请检查设置页后重试。"
            : result.ErrorMessage;
        _logger.LogError("Bootstrap failed: {Reason}", result.ErrorMessage);

    }
}