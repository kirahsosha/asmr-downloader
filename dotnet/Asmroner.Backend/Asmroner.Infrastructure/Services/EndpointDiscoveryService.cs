using System.Diagnostics;
using System.Text.RegularExpressions;
using Asmroner.Core.Api;
using Asmroner.Core.Interfaces;

namespace Asmroner.Infrastructure.Services;

public sealed class EndpointDiscoveryService : IEndpointDiscoveryService
{
    private static readonly Regex ScriptRegex = new("<script type=\"module\" crossorigin src=\"(?<path>/assets/index\\.[a-f0-9]+\\.js)\"></script>", RegexOptions.Compiled);
    private static readonly Regex LinkRegex = new("link:\\s*\"(?<url>[^\"]+)\"", RegexOptions.Compiled);

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IAsmrApiOptionsProvider _optionsProvider;

    public EndpointDiscoveryService(IHttpClientFactory httpClientFactory, IAsmrApiOptionsProvider optionsProvider)
    {
        _httpClientFactory = httpClientFactory;
        _optionsProvider = optionsProvider;
    }

    public async Task<EndpointDiscoveryResult> DiscoverAsync(CancellationToken cancellationToken = default)
    {
        var options = await _optionsProvider.GetOptionsAsync(cancellationToken);
        var candidates = new List<string>(options.CandidateBaseUrls);
        candidates.AddRange(await GetPublishedCandidatesAsync(cancellationToken));

        var uniqueCandidates = candidates
            .Where(static item => !string.IsNullOrWhiteSpace(item))
            .Select(static item => item.Trim().TrimEnd('/'))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var probeClient = _httpClientFactory.CreateClient("AsmrProbe");

        string bestUrl = options.BaseUrl;
        long bestLatency = long.MaxValue;

        foreach (var candidate in uniqueCandidates)
        {
            var latency = await MeasureLatencyAsync(probeClient, candidate, options.Timeout, cancellationToken);
            if (latency >= 0 && latency < bestLatency)
            {
                bestLatency = latency;
                bestUrl = candidate;
            }
        }

        if (bestLatency == long.MaxValue)
        {
            bestLatency = -1;
        }

        return new EndpointDiscoveryResult
        {
            BaseUrl = bestUrl,
            LatencyMs = bestLatency,
            Candidates = uniqueCandidates,
        };
    }

    private async Task<IReadOnlyList<string>> GetPublishedCandidatesAsync(CancellationToken cancellationToken)
    {
        var options = await _optionsProvider.GetOptionsAsync(cancellationToken);
        var probeClient = _httpClientFactory.CreateClient("AsmrProbe");
        foreach (var publishUrl in new[] { AsmrApiPaths.LatestPublishUrl, AsmrApiPaths.LatestPublishProxyUrl })
        {
            try
            {
                var html = await GetStringWithTimeoutAsync(probeClient, publishUrl, options.Timeout, cancellationToken);
                var scriptMatch = ScriptRegex.Match(html);
                if (!scriptMatch.Success)
                {
                    continue;
                }

                var scriptUrl = publishUrl.TrimEnd('/') + scriptMatch.Groups["path"].Value;
                var scriptText = await GetStringWithTimeoutAsync(probeClient, scriptUrl, options.Timeout, cancellationToken);
                var matches = LinkRegex.Matches(scriptText);
                if (matches.Count == 0)
                {
                    continue;
                }

                return matches
                    .Select(static match => match.Groups["url"].Value)
                    .Where(static item => item.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                    .Select(ToApiBaseUrl)
                    .ToArray();
            }
            catch
            {
                // Ignore publish source failures and fall back to configured/default candidates.
            }
        }

        return Array.Empty<string>();
    }

    private static async Task<long> MeasureLatencyAsync(HttpClient client, string baseUrl, TimeSpan timeout, CancellationToken cancellationToken)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, baseUrl.TrimEnd('/') + AsmrApiPaths.Popular);
            var watch = Stopwatch.StartNew();
            using var timeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCancellation.CancelAfter(timeout);
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, timeoutCancellation.Token);
            watch.Stop();
            return response.IsSuccessStatusCode ? watch.ElapsedMilliseconds : -1;
        }
        catch
        {
            return -1;
        }
    }

    private static async Task<string> GetStringWithTimeoutAsync(HttpClient client, string requestUri, TimeSpan timeout, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        using var timeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCancellation.CancelAfter(timeout);
        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, timeoutCancellation.Token);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(timeoutCancellation.Token);
    }

    private static string ToApiBaseUrl(string siteUrl)
    {
        var normalized = siteUrl.TrimEnd('/');
        return normalized.StartsWith("https://api.", StringComparison.OrdinalIgnoreCase)
            ? normalized
            : normalized.Replace("https://", "https://api.", StringComparison.OrdinalIgnoreCase);
    }
}