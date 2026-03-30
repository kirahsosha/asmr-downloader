using System.Windows.Controls;
using System.Windows;
using Asmroner.Core.Configuration;
using Asmroner.Core.Initialization;
using Asmroner.Core.Interfaces;
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

    private async void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
    {
        var config = await _configurationService.LoadAsync() ?? new AppConfig
        {
            Downloader =
            {
                SyncDataFolder = _appPathService.DefaultSyncDataDirectory,
            },
        };

        _apiCandidateUrls = config.Downloader.ApiCandidateUrls;
        _publishSourceUrls = config.Downloader.PublishSourceUrls;
        _workPageUrlTemplate = config.Downloader.WorkPageUrlTemplate;

        FillForm(config);
    }

    private async void OnSaveAndInitializeClicked(object sender, System.Windows.RoutedEventArgs e)
    {
        SaveButton.IsEnabled = false;
        TestConnectionButton.IsEnabled = false;
        StatusTextBlock.Text = "正在保存配置并执行初始化...";

        try
        {
            var config = BuildConfigFromForm();
            var validationErrors = _configurationService.Validate(config);
            if (validationErrors.Count > 0)
            {
                StatusTextBlock.Text = string.Join("; ", validationErrors);
                return;
            }

            await _configurationService.SaveAsync(config);
            var bootstrapResult = await _bootstrapper.InitializeAsync();

            StatusTextBlock.Text = bootstrapResult.IsSuccess
                ? "初始化成功，已可进入主页面。"
                : bootstrapResult.ErrorMessage ?? "初始化失败，请检查配置。";

            InitializationCompleted?.Invoke(this, bootstrapResult);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Saving configuration failed.");
            StatusTextBlock.Text = $"保存失败: {ex.Message}";
        }
        finally
        {
            SaveButton.IsEnabled = true;
            TestConnectionButton.IsEnabled = true;
        }
    }

    private async void OnTestConnectionClicked(object sender, RoutedEventArgs e)
    {
        SaveButton.IsEnabled = false;
        TestConnectionButton.IsEnabled = false;
        StatusTextBlock.Text = "正在测试 API 连通性与登录状态...";

        try
        {
            var config = BuildConfigFromForm();
            var validationErrors = _configurationService.Validate(config);
            if (validationErrors.Count > 0)
            {
                StatusTextBlock.Text = string.Join("; ", validationErrors);
                return;
            }

            await _configurationService.SaveAsync(config);
            var result = await _connectivityProbeService.ProbeAsync();

            if (result.IsReachable)
            {
                ApiUrlTextBox.Text = result.BaseUrl;
            }

            StatusTextBlock.Text = result.IsReachable
                ? $"连接成功: {result.BaseUrl} | 延迟 {result.LatencyMs} ms | 鉴权 {(result.IsAuthenticated ? "成功" : "失败")}"
                : $"连接失败: {result.Message}";
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Connectivity probe failed.");
            StatusTextBlock.Text = $"测试失败: {ex.Message}";
        }
        finally
        {
            SaveButton.IsEnabled = true;
            TestConnectionButton.IsEnabled = true;
        }
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
                SyncDataFolder = SyncDataFolderTextBox.Text.Trim(),
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
        SyncDataFolderTextBox.Text = string.IsNullOrWhiteSpace(config.Downloader.SyncDataFolder)
            ? _appPathService.DefaultSyncDataDirectory
            : config.Downloader.SyncDataFolder;
        SyncWantedSizeTextBox.Text = config.Downloader.SyncWantedSize;
        PreferFormatsTextBox.Text = BuildPreferFormatsForDisplay(config.Downloader);
        SyncQpsTextBox.Text = config.Limit.SyncQps.ToString();
        SyncJitterMinTextBox.Text = config.Limit.SyncJitterMin.ToString();
        SyncJitterMaxTextBox.Text = config.Limit.SyncJitterMax.ToString();
        DownloadQpsTextBox.Text = config.Limit.DownloadQps.ToString();
        DownloadJitterMinTextBox.Text = config.Limit.DownloadJitterMin.ToString();
        DownloadJitterMaxTextBox.Text = config.Limit.DownloadJitterMax.ToString();
    }

    private static string BuildPreferFormatsForDisplay(DownloaderOptions downloader)
    {
        return downloader.PreferFormats;
    }
}