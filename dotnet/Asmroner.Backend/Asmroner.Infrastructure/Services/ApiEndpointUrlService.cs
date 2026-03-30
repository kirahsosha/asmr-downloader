using Asmroner.Core.Api;
using Asmroner.Core.Configuration;
using Asmroner.Core.Interfaces;
using NLog;

namespace Asmroner.Infrastructure.Services;

public sealed class ApiEndpointUrlService : IApiEndpointUrlService
{
    private static readonly DownloaderOptions Defaults = new();
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private readonly IConfigurationService _configurationService;
    private readonly IEndpointDiscoveryService _endpointDiscoveryService;

    public ApiEndpointUrlService(
        IConfigurationService configurationService,
        IEndpointDiscoveryService endpointDiscoveryService)
    {
        _configurationService = configurationService;
        _endpointDiscoveryService = endpointDiscoveryService;
    }

    public async Task<string> GetCurrentBaseUrlAsync(CancellationToken cancellationToken = default)
    {
        var config = await _configurationService.LoadAsync(cancellationToken);
        var configuredBaseUrl = NormalizeBaseUrl(config?.Downloader.ApiUrl);
        if (!string.IsNullOrWhiteSpace(configuredBaseUrl))
        {
            return configuredBaseUrl;
        }

        return Defaults.ApiUrl.Trim().TrimEnd('/');
    }

    public async Task<EndpointDiscoveryResult> DiscoverAndPersistAsync(CancellationToken cancellationToken = default)
    {
        var result = await _endpointDiscoveryService.DiscoverAsync(cancellationToken);
        var discoveredBaseUrl = NormalizeBaseUrl(result.BaseUrl) ?? await GetCurrentBaseUrlAsync(cancellationToken);

        var config = await _configurationService.LoadAsync(cancellationToken);
        if (config is null)
        {
            _logger.Info("Skip persisting discovered api url because configuration file does not exist.");
            return CreateResultWithBaseUrl(result, discoveredBaseUrl);
        }

        var currentBaseUrl = NormalizeBaseUrl(config.Downloader.ApiUrl);
        if (string.Equals(currentBaseUrl, discoveredBaseUrl, StringComparison.OrdinalIgnoreCase))
        {
            return CreateResultWithBaseUrl(result, discoveredBaseUrl);
        }

        config.Downloader.ApiUrl = discoveredBaseUrl;
        await _configurationService.SaveAsync(config, cancellationToken);
        _logger.Info("Persisted discovered api url {BaseUrl}.", discoveredBaseUrl);

        return CreateResultWithBaseUrl(result, discoveredBaseUrl);
    }

    private static EndpointDiscoveryResult CreateResultWithBaseUrl(EndpointDiscoveryResult source, string baseUrl)
    {
        return new EndpointDiscoveryResult
        {
            BaseUrl = baseUrl,
            LatencyMs = source.LatencyMs,
            Candidates = source.Candidates,
        };
    }

    private static string? NormalizeBaseUrl(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim().TrimEnd('/');
        if (!Uri.TryCreate(normalized, UriKind.Absolute, out var uri))
        {
            return null;
        }

        return uri.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
            || uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)
            ? normalized
            : null;
    }
}
