using Asmroner.Core.Configuration;
using Asmroner.Core.Initialization;
using Asmroner.Core.Interfaces;

namespace Asmroner.Application.Services;

public sealed class ApplicationBootstrapper : IApplicationBootstrapper
{
    private readonly IAppPathService _appPathService;
    private readonly IConfigurationService _configurationService;
    private readonly IDatabaseInitializer _databaseInitializer;
    private readonly IFirstRunService _firstRunService;

    public ApplicationBootstrapper(
        IAppPathService appPathService,
        IConfigurationService configurationService,
        IDatabaseInitializer databaseInitializer,
        IFirstRunService firstRunService)
    {
        _appPathService = appPathService;
        _configurationService = configurationService;
        _databaseInitializer = databaseInitializer;
        _firstRunService = firstRunService;
    }

    public async Task<BootstrapResult> InitializeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _appPathService.EnsureMetadataDirectory();
            await _databaseInitializer.InitializeAsync(cancellationToken);

            var config = await _configurationService.LoadAsync(cancellationToken);
            if (config is null)
            {
                return BootstrapResult.SetupRequired(
                    CreateDefaultConfig(),
                    new[] { "配置文件不存在，请先完成设置。" });
            }

            if (string.IsNullOrWhiteSpace(config.Downloader.DownloadDataFolder))
            {
                config.Downloader.DownloadDataFolder = _appPathService.DefaultDownloadDataDirectory;
            }

            if (string.IsNullOrWhiteSpace(config.Downloader.SyncDataFolder))
            {
                config.Downloader.SyncDataFolder = _appPathService.DefaultSyncDataDirectory;
            }

            var validationErrors = _configurationService.Validate(config);
            if (_firstRunService.RequiresSetup(config, validationErrors))
            {
                return BootstrapResult.SetupRequired(config, validationErrors);
            }

            Directory.CreateDirectory(config.Downloader.DownloadDataFolder);
            Directory.CreateDirectory(config.Downloader.SyncDataFolder);
            return BootstrapResult.Success(config);
        }
        catch (Exception ex)
        {
            return BootstrapResult.Failed($"初始化失败: {ex.Message}");
        }
    }

    private AppConfig CreateDefaultConfig()
    {
        return new AppConfig
        {
            Downloader =
            {
                DownloadDataFolder = _appPathService.DefaultDownloadDataDirectory,
                SyncDataFolder = _appPathService.DefaultSyncDataDirectory,
            },
        };
    }
}