using System.Windows.Controls;
using System.Windows;
using Asmroner.Core.Api;
using Asmroner.Core.Configuration;
using Asmroner.Core.Initialization;
using Asmroner.Core.Interfaces;
using Asmroner.Wpf.Services;
using NLog;

namespace Asmroner.Wpf.Views;

public partial class SettingsView : UserControl
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private readonly IConfigurationService _configurationService;
    private readonly IApplicationBootstrapper _bootstrapper;
    private readonly IAppPathService _appPathService;
    private readonly IConnectivityProbeService _connectivityProbeService;

    private string _apiCandidateUrls = new DownloaderOptions().ApiCandidateUrls;
    private string _publishSourceUrls = new DownloaderOptions().PublishSourceUrls;
    private string _workPageUrlTemplate = new DownloaderOptions().WorkPageUrlTemplate;

    public SettingsView(
        IConfigurationService configurationService,
        IApplicationBootstrapper bootstrapper,
        IAppPathService appPathService,
        IConnectivityProbeService connectivityProbeService)
    {
        _configurationService = configurationService;
        _bootstrapper = bootstrapper;
        _appPathService = appPathService;
        _connectivityProbeService = connectivityProbeService;

        InitializeComponent();
        Loaded += OnLoaded;
    }

    public event EventHandler<BootstrapResult>? InitializationCompleted;

    /// <summary>
    /// 处理 Settings 页面加载并填充当前配置。
    /// </summary>
    private async void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
    {
        VersionTextBlock.Text = AppVersionInfo.BuildSettingsVersionText();

        var config = await _configurationService.LoadAsync() ?? new AppConfig
        {
            Downloader =
            {
                DownloadDataFolder = _appPathService.DefaultDownloadDataDirectory,
                SyncDataFolder = _appPathService.DefaultSyncDataDirectory,
            },
        };

        _apiCandidateUrls = config.Downloader.ApiCandidateUrls;
        _publishSourceUrls = config.Downloader.PublishSourceUrls;
        _workPageUrlTemplate = config.Downloader.WorkPageUrlTemplate;

        FillForm(config);
    }

    /// <summary>
    /// 处理“保存并重新初始化”按钮点击。
    /// </summary>
    private async void OnSaveAndInitializeClicked(object sender, System.Windows.RoutedEventArgs e)
    {
        await ExecuteConfigActionAsync(
            runningStatus: "正在保存配置并执行初始化...",
            failureStatusPrefix: "保存失败",
            errorLogMessage: "Saving configuration failed.",
            async config =>
            {
                await _configurationService.SaveAsync(config);
                var bootstrapResult = await _bootstrapper.InitializeAsync();

                StatusTextBlock.Text = bootstrapResult.IsSuccess
                    ? "初始化成功，已可进入主页面。"
                    : bootstrapResult.ErrorMessage ?? "初始化失败，请检查配置。";

                InitializationCompleted?.Invoke(this, bootstrapResult);
            });
    }

    /// <summary>
    /// 处理“测试连接”按钮点击。
    /// </summary>
    private async void OnTestConnectionClicked(object sender, RoutedEventArgs e)
    {
        await ExecuteConfigActionAsync(
            runningStatus: "正在测试 API 连通性与登录状态...",
            failureStatusPrefix: "测试失败",
            errorLogMessage: "Connectivity probe failed.",
            async config =>
            {
                await _configurationService.SaveAsync(config);
                var result = await _connectivityProbeService.ProbeAsync();
                await RefreshDiscoveryStateAsync();
                ApplyConnectivityProbeResult(result);
            });
    }

    private async Task ExecuteConfigActionAsync(
        string runningStatus,
        string failureStatusPrefix,
        string errorLogMessage,
        Func<AppConfig, Task> action)
    {
        SetPrimaryActionButtonsEnabled(false);
        StatusTextBlock.Text = runningStatus;

        try
        {
            var config = TryBuildValidatedConfig();
            if (config is null)
            {
                return;
            }

            await action(config);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, errorLogMessage);
            StatusTextBlock.Text = $"{failureStatusPrefix}: {ex.Message}";
        }
        finally
        {
            SetPrimaryActionButtonsEnabled(true);
        }
    }

    private AppConfig? TryBuildValidatedConfig()
    {
        var config = BuildConfigFromForm();
        var validationErrors = _configurationService.Validate(config);
        if (validationErrors.Count == 0)
        {
            return config;
        }

        StatusTextBlock.Text = string.Join("; ", validationErrors);
        return null;
    }

    private void ApplyConnectivityProbeResult(ConnectivityProbeResult result)
    {
        if (result.IsReachable)
        {
            ApiUrlTextBox.Text = result.BaseUrl;
        }

        StatusTextBlock.Text = result.IsReachable
            ? $"连接成功: {result.BaseUrl} | 延迟 {result.LatencyMs} ms | 鉴权 {(result.IsAuthenticated ? "成功" : "失败")}"
            : $"连接失败: {result.Message}";
    }

    private void SetPrimaryActionButtonsEnabled(bool isEnabled)
    {
        SaveButton.IsEnabled = isEnabled;
        TestConnectionButton.IsEnabled = isEnabled;
    }

    private AppConfig BuildConfigFromForm()
    {
        if (!int.TryParse(MaxWorkersTextBox.Text, out var maxWorkers))
        {
            throw new InvalidOperationException("并发工作数必须是整数。");
        }

        if (!int.TryParse(MaxRetriesTextBox.Text, out var maxRetries))
        {
            throw new InvalidOperationException("重试次数必须是整数。");
        }

        if (!int.TryParse(MetadataValidityDaysTextBox.Text, out var metadataValidityDays))
        {
            throw new InvalidOperationException("元数据有效期必须是整数。");
        }

        if (!double.TryParse(SyncQpsTextBox.Text, out var syncQps))
        {
            throw new InvalidOperationException("同步 QPS 必须是数字。");
        }

        if (!int.TryParse(SyncJitterMinTextBox.Text, out var syncJitterMin))
        {
            throw new InvalidOperationException("同步抖动最小值必须是整数。");
        }

        if (!int.TryParse(SyncJitterMaxTextBox.Text, out var syncJitterMax))
        {
            throw new InvalidOperationException("同步抖动最大值必须是整数。");
        }

        if (!double.TryParse(DownloadQpsTextBox.Text, out var downloadQps))
        {
            throw new InvalidOperationException("下载 QPS 必须是数字。");
        }

        if (!int.TryParse(DownloadJitterMinTextBox.Text, out var downloadJitterMin))
        {
            throw new InvalidOperationException("下载抖动最小值必须是整数。");
        }

        if (!int.TryParse(DownloadJitterMaxTextBox.Text, out var downloadJitterMax))
        {
            throw new InvalidOperationException("下载抖动最大值必须是整数。");
        }

        return new AppConfig
        {
            User = new UserOptions
            {
                Account = AccountTextBox.Text.Trim(),
                Password = PasswordBox.Password.Trim(),
            },
            Downloader = new DownloaderOptions
            {
                ApiUrl = ApiUrlTextBox.Text.Trim(),
                ApiCandidateUrls = _apiCandidateUrls,
                PublishSourceUrls = _publishSourceUrls,
                WorkPageUrlTemplate = _workPageUrlTemplate,
                ProxyUrl = ProxyUrlTextBox.Text.Trim(),
                MaxWorkers = maxWorkers,
                MaxRetries = maxRetries,
                DownloadDataFolder = DownloadDataFolderTextBox.Text.Trim(),
                SyncDataFolder = SyncDataFolderTextBox.Text.Trim(),
                MetadataValidityDays = metadataValidityDays,
                SyncWantedSize = SyncWantedSizeTextBox.Text.Trim(),
                PreferFormats = PreferFormatsTextBox.Text.Trim(),
            },
            Limit = new LimitOptions
            {
                SyncQps = syncQps,
                SyncJitterMin = syncJitterMin,
                SyncJitterMax = syncJitterMax,
                DownloadQps = downloadQps,
                DownloadJitterMin = downloadJitterMin,
                DownloadJitterMax = downloadJitterMax,
            },
        };
    }

    private void FillForm(AppConfig config)
    {
        AccountTextBox.Text = config.User.Account;
        PasswordBox.Password = config.User.Password;
        ApiUrlTextBox.Text = config.Downloader.ApiUrl;
        ProxyUrlTextBox.Text = config.Downloader.ProxyUrl;
        MaxWorkersTextBox.Text = config.Downloader.MaxWorkers.ToString();
        MaxRetriesTextBox.Text = config.Downloader.MaxRetries.ToString();
        DownloadDataFolderTextBox.Text = string.IsNullOrWhiteSpace(config.Downloader.DownloadDataFolder)
            ? _appPathService.DefaultDownloadDataDirectory
            : config.Downloader.DownloadDataFolder;
        SyncDataFolderTextBox.Text = string.IsNullOrWhiteSpace(config.Downloader.SyncDataFolder)
            ? _appPathService.DefaultSyncDataDirectory
            : config.Downloader.SyncDataFolder;
        MetadataValidityDaysTextBox.Text = config.Downloader.MetadataValidityDays.ToString();
        SyncWantedSizeTextBox.Text = config.Downloader.SyncWantedSize;
        PreferFormatsTextBox.Text = BuildPreferFormatsForDisplay(config.Downloader);
        SyncQpsTextBox.Text = config.Limit.SyncQps.ToString();
        SyncJitterMinTextBox.Text = config.Limit.SyncJitterMin.ToString();
        SyncJitterMaxTextBox.Text = config.Limit.SyncJitterMax.ToString();
        DownloadQpsTextBox.Text = config.Limit.DownloadQps.ToString();
        DownloadJitterMinTextBox.Text = config.Limit.DownloadJitterMin.ToString();
        DownloadJitterMaxTextBox.Text = config.Limit.DownloadJitterMax.ToString();
    }

    private async Task RefreshDiscoveryStateAsync()
    {
        var config = await _configurationService.LoadAsync();
        if (config is null)
        {
            return;
        }

        _apiCandidateUrls = config.Downloader.ApiCandidateUrls;
        _publishSourceUrls = config.Downloader.PublishSourceUrls;
        _workPageUrlTemplate = config.Downloader.WorkPageUrlTemplate;
    }

    private static string BuildPreferFormatsForDisplay(DownloaderOptions downloader)
    {
        return downloader.PreferFormats;
    }
}