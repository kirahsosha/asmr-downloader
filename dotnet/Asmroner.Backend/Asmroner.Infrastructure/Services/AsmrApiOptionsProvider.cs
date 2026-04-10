using Asmroner.Core.Api;
using Asmroner.Core.Configuration;
using Asmroner.Core.Interfaces;

namespace Asmroner.Infrastructure.Services;

public sealed class AsmrApiOptionsProvider : IAsmrApiOptionsProvider
{
    private static readonly char[] UrlListSeparators = { ';', ',', '\r', '\n' };

    private readonly IConfigurationService _configurationService;

    public AsmrApiOptionsProvider(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }

    public async Task<AsmrApiOptions> GetOptionsAsync(CancellationToken cancellationToken = default)
    {
        var config = await _configurationService.LoadAsync(cancellationToken) ?? new();
        var defaults = new DownloaderOptions();

        var configuredBaseUrl = NormalizeBaseUrl(config.Downloader.ApiUrl)
            ?? NormalizeBaseUrl(defaults.ApiUrl)
            ?? throw new InvalidOperationException("无可用的 API 基础地址配置。");

        var defaultCandidates = ParseUrlList(defaults.ApiCandidateUrls);
        var candidates = ParseUrlList(config.Downloader.ApiCandidateUrls);
        if (candidates.Count == 0)
        {
            candidates = new List<string>(defaultCandidates);
        }
        else if (ShouldAppendBuiltInCandidates(candidates, configuredBaseUrl, defaultCandidates))
        {
            candidates = new List<string>(defaultCandidates);
        }

        candidates = EnsureBaseUrlFirst(candidates, configuredBaseUrl);

        var publishSourceUrls = ParseUrlList(config.Downloader.PublishSourceUrls);
        if (publishSourceUrls.Count == 0)
        {
            publishSourceUrls = ParseUrlList(defaults.PublishSourceUrls);
        }

        return new AsmrApiOptions
        {
            BaseUrl = configuredBaseUrl,
            ProxyUrl = string.IsNullOrWhiteSpace(config.Downloader.ProxyUrl) ? null : config.Downloader.ProxyUrl.Trim(),
            Timeout = TimeSpan.FromSeconds(10),
            CandidateBaseUrls = candidates,
            PublishSourceUrls = publishSourceUrls,
        };
    }

    private static List<string> EnsureBaseUrlFirst(List<string> candidates, string configuredBaseUrl)
    {
        if (!candidates.Contains(configuredBaseUrl, StringComparer.OrdinalIgnoreCase))
        {
            candidates.Insert(0, configuredBaseUrl);
            return candidates;
        }

        if (string.Equals(candidates[0], configuredBaseUrl, StringComparison.OrdinalIgnoreCase))
        {
            return candidates;
        }

        candidates.RemoveAll(candidate => string.Equals(candidate, configuredBaseUrl, StringComparison.OrdinalIgnoreCase));
        candidates.Insert(0, configuredBaseUrl);
        return candidates;
    }

    private static bool ShouldAppendBuiltInCandidates(
        IReadOnlyList<string> candidates,
        string configuredBaseUrl,
        IReadOnlyList<string> defaultCandidates)
    {
        return defaultCandidates.Contains(configuredBaseUrl, StringComparer.OrdinalIgnoreCase)
            && candidates.All(candidate => defaultCandidates.Contains(candidate, StringComparer.OrdinalIgnoreCase));
    }

    private static string? NormalizeBaseUrl(string? baseUrl)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            return null;
        }

        var normalized = baseUrl.Trim().TrimEnd('/');
        if (!Uri.TryCreate(normalized, UriKind.Absolute, out var uri))
        {
            return null;
        }

        return uri.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
            || uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)
            ? normalized
            : null;
    }

    private static List<string> ParseUrlList(string? value)
    {
        var urls = new List<string>();
        if (string.IsNullOrWhiteSpace(value))
        {
            return urls;
        }

        foreach (var segment in value.Split(UrlListSeparators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var normalized = NormalizeBaseUrl(segment);
            if (string.IsNullOrWhiteSpace(normalized))
            {
                continue;
            }

            if (!urls.Contains(normalized, StringComparer.OrdinalIgnoreCase))
            {
                urls.Add(normalized);
            }
        }

        return urls;
    }
}