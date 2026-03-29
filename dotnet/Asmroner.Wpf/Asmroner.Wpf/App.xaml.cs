using Asmroner.Core.Interfaces;
using Asmroner.Application.Services;
using Asmroner.Infrastructure.Services;
using Asmroner.Wpf.Services;
using Asmroner.Wpf.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace Asmroner.Wpf;

public partial class App : System.Windows.Application
{
    private IHost? _host;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _host = Host.CreateDefaultBuilder()
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddDebug();
                logging.AddConsole();
            })
            .ConfigureServices(services =>
            {
                services.AddSingleton<IAppPathService, AppPathService>();
                services.AddSingleton<IConfigurationService, Asmroner.Infrastructure.Services.ConfigurationService>();
                services.AddSingleton<IUiStateStore, UiStateStore>();
                services.AddSingleton<IDatabaseInitializer, DatabaseInitializer>();
                services.AddSingleton<IFirstRunService, FirstRunService>();
                services.AddSingleton<IApplicationBootstrapper, ApplicationBootstrapper>();

                services.AddTransient<AsmrAuthorizationHandler>();
                services.AddHttpClient("AsmrProbe");
                services.AddHttpClient("AsmrApi")
                    .AddHttpMessageHandler<AsmrAuthorizationHandler>();

                services.TryAddSingleton<IAsmrApiOptionsProvider, AsmrApiOptionsProvider>();
                services.TryAddSingleton<IEndpointDiscoveryService, EndpointDiscoveryService>();
                services.TryAddSingleton<IApiEndpointUrlService, ApiEndpointUrlService>();
                services.TryAddSingleton<ITokenStore, TokenStore>();
                services.TryAddSingleton<IAuthService, Asmroner.Infrastructure.Services.AuthService>();
                services.TryAddSingleton<IAsmrApiClient, AsmrApiClient>();
                services.TryAddSingleton<IConnectivityProbeService, ConnectivityProbeService>();

                services.AddSingleton<IQueryParserService, QueryParserService>();
                services.AddSingleton<ISearchService, Asmroner.Application.Services.SearchService>();
                services.AddSingleton<ISearchExportService, SearchExportService>();
                services.AddSingleton<ISearchImportService, SearchImportService>();
                services.AddSingleton<ISearchStateStore, SearchStateStore>();
                services.AddSingleton<IRateLimiterService, RateLimiterService>();
                services.AddSingleton<IDownloadService, Asmroner.Application.Services.DownloadService>();
                services.AddSingleton<ISyncService, Asmroner.Wpf.Services.SyncService>();
                services.AddSingleton<StartupEndpointWarmupService>();

                services.AddSingleton<DashboardView>();
                services.AddSingleton<DownloadView>();
                services.AddSingleton<SettingsView>();
                services.AddSingleton<MainWindow>();
            })
            .Build();

        _host.Start();

        var logger = _host.Services.GetRequiredService<ILogger<App>>();
        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        var startupEndpointWarmupService = _host.Services.GetRequiredService<StartupEndpointWarmupService>();
        _ = startupEndpointWarmupService.StartInBackgroundAsync();

        logger.LogInformation("Asmroner v0.4.2 startup completed.");
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            _host.StopAsync().GetAwaiter().GetResult();
            _host.Dispose();
        }

        base.OnExit(e);
    }
}

