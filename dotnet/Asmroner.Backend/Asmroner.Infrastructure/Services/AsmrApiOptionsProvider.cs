using Asmroner.Core.Api;
using Asmroner.Core.Interfaces;

namespace Asmroner.Infrastructure.Services;

public sealed class AsmrApiOptionsProvider : IAsmrApiOptionsProvider
{
    private readonly IConfigurationService _configurationService;

    public AsmrApiOptionsProvider(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }

    public async Task<AsmrApiOptions> GetOptionsAsync(CancellationToken cancellationToken = default)
    {
        var config = await _configurationService.LoadAsync(cancellationToken) ?? new();
        var configuredBaseUrl = NormalizeBaseUrl(config.Downloader.ApiUrl);

        var candidates = new List<string>();
        if (!string.IsNullOrWhiteSpace(configuredBaseUrl))
        {
            candidates.Add(configuredBaseUrl);
        }

        candidates.Add(AsmrApiPaths.DefaultBaseUrl);
        candidates.Add(AsmrApiPaths.FallbackBaseUrl);

        return new AsmrApiOptions
        {
            BaseUrl = configuredBaseUrl ?? AsmrApiPaths.DefaultBaseUrl,
            ProxyUrl = string.IsNullOrWhiteSpace(config.Downloader.ProxyUrl) ? null : config.Downloader.ProxyUrl.Trim(),
            Timeout = TimeSpan.FromSeconds(10),
            CandidateBaseUrls = candidates.Distinct(StringComparer.OrdinalIgnoreCase).ToArray(),
        };
    }

    private static string? NormalizeBaseUrl(string? baseUrl)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            return null;
        }

        return baseUrl.Trim().TrimEnd('/');
    }
}