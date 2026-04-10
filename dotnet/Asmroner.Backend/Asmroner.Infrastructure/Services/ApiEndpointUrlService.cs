using Asmroner.Core.Api;
using Asmroner.Core.Configuration;
using Asmroner.Core.Interfaces;
using NLog;

namespace Asmroner.Infrastructure.Services;

public sealed class ApiEndpointUrlService : IApiEndpointUrlService
{
    private static readonly DownloaderOptions Defaults = new();
    private static readonly char[] UrlListSeparators = { ';', ',', '\r', '\n' };
    private static readonly IReadOnlyList<string> BuiltInCandidateBaseUrls = ParseUrlList(Defaults.ApiCandidateUrls);
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
            return CreateResult(discoveredBaseUrl, result.LatencyMs, result.Candidates);
        }

        var currentBaseUrl = NormalizeBaseUrl(config.Downloader.ApiUrl);
        var currentCandidates = ParseUrlList(config.Downloader.ApiCandidateUrls);
        var persistedCandidates = BuildPersistedCandidateBaseUrls(
            currentCandidates,
            currentBaseUrl,
            discoveredBaseUrl,
            result.Candidates);

        var baseUrlChanged = !string.Equals(currentBaseUrl, discoveredBaseUrl, StringComparison.OrdinalIgnoreCase);
        var candidatesChanged = !AreSameUrlList(currentCandidates, persistedCandidates);
        if (!baseUrlChanged && !candidatesChanged)
        {
            return CreateResult(discoveredBaseUrl, result.LatencyMs, persistedCandidates);
        }

        config.Downloader.ApiUrl = discoveredBaseUrl;
        config.Downloader.ApiCandidateUrls = string.Join(';', persistedCandidates);
        await _configurationService.SaveAsync(config, cancellationToken);
        _logger.Info("Persisted discovered api url {BaseUrl}.", discoveredBaseUrl);

        return CreateResult(discoveredBaseUrl, result.LatencyMs, persistedCandidates);
    }

    private static bool AreSameUrlList(IReadOnlyList<string> first, IReadOnlyList<string> second)
    {
        if (first.Count != second.Count)
        {
            return false;
        }

        for (var index = 0; index < first.Count; index++)
        {
            if (!string.Equals(first[index], second[index], StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }

    private static IReadOnlyList<string> BuildPersistedCandidateBaseUrls(
        IReadOnlyList<string> currentCandidates,
        string? currentBaseUrl,
        string discoveredBaseUrl,
        IReadOnlyList<string> discoveredCandidates)
    {
        var normalizedDiscoveredCandidates = NormalizeUrlList(discoveredCandidates);
        if (ShouldPersistDiscoveredCandidates(currentCandidates, currentBaseUrl, discoveredBaseUrl))
        {
            return EnsureBaseUrlIncluded(normalizedDiscoveredCandidates, discoveredBaseUrl);
        }

        if (currentCandidates.Count == 0)
        {
            return new[] { discoveredBaseUrl };
        }

        return EnsureBaseUrlIncluded(currentCandidates, discoveredBaseUrl);
    }

    private static EndpointDiscoveryResult CreateResult(string baseUrl, long latencyMs, IReadOnlyList<string> candidates)
    {
        return new EndpointDiscoveryResult
        {
            BaseUrl = baseUrl,
            LatencyMs = latencyMs,
            Candidates = candidates,
        };
    }

    private static IReadOnlyList<string> EnsureBaseUrlIncluded(IReadOnlyList<string> candidates, string baseUrl)
    {
        var normalizedCandidates = NormalizeUrlList(candidates);
        if (normalizedCandidates.Count == 0)
        {
            return new[] { baseUrl };
        }

        if (normalizedCandidates.Contains(baseUrl, StringComparer.OrdinalIgnoreCase))
        {
            if (string.Equals(normalizedCandidates[0], baseUrl, StringComparison.OrdinalIgnoreCase))
            {
                return normalizedCandidates;
            }

            var reordered = new List<string>(normalizedCandidates.Count) { baseUrl };
            reordered.AddRange(normalizedCandidates.Where(candidate => !string.Equals(candidate, baseUrl, StringComparison.OrdinalIgnoreCase)));
            return reordered;
        }

        var expanded = new List<string>(normalizedCandidates.Count + 1) { baseUrl };
        expanded.AddRange(normalizedCandidates);
        return expanded;
    }

    private static IReadOnlyList<string> NormalizeUrlList(IEnumerable<string> values)
    {
        var normalized = new List<string>();
        foreach (var value in values)
        {
            var baseUrl = NormalizeBaseUrl(value);
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                continue;
            }

            if (!normalized.Contains(baseUrl, StringComparer.OrdinalIgnoreCase))
            {
                normalized.Add(baseUrl);
            }
        }

        return normalized;
    }

    private static IReadOnlyList<string> ParseUrlList(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Array.Empty<string>();
        }

        return NormalizeUrlList(value.Split(UrlListSeparators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
    }

    private static bool ShouldPersistDiscoveredCandidates(
        IReadOnlyList<string> currentCandidates,
        string? currentBaseUrl,
        string discoveredBaseUrl)
    {
        var comparisonBaseUrl = currentBaseUrl ?? discoveredBaseUrl;
        if (!BuiltInCandidateBaseUrls.Contains(comparisonBaseUrl, StringComparer.OrdinalIgnoreCase))
        {
            return false;
        }

        return currentCandidates.Count == 0
            || currentCandidates.All(candidate => BuiltInCandidateBaseUrls.Contains(candidate, StringComparer.OrdinalIgnoreCase));
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
