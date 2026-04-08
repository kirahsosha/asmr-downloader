using System.Diagnostics;
using System.Text.RegularExpressions;
using Asmroner.Core.Api;
using Asmroner.Core.Interfaces;

namespace Asmroner.Infrastructure.Services;

public sealed class EndpointDiscoveryService : IEndpointDiscoveryService
{
    private const string PublishSourceUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/135.0.0.0 Safari/537.36";
    private const string ProbeRequestPath = AsmrApiPaths.Works;

    private static readonly Regex ScriptTagRegex = new("<script\\b[^>]*\\bsrc=[\"'](?<path>[^\"']+)[\"'][^>]*>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex LinkRegex = new("link\\s*:\\s*[\"'](?<url>[^\"']+)[\"']", RegexOptions.Compiled | RegexOptions.IgnoreCase);

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
        candidates.AddRange(await GetPublishedCandidatesAsync(options, cancellationToken));

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

    private async Task<IReadOnlyList<string>> GetPublishedCandidatesAsync(AsmrApiOptions options, CancellationToken cancellationToken)
    {
        var probeClient = _httpClientFactory.CreateClient("AsmrProbe");
        foreach (var publishUrl in options.PublishSourceUrls)
        {
            var html = await GetStringWithTimeoutAsync(probeClient, publishUrl, options.Timeout, cancellationToken);
            if (string.IsNullOrWhiteSpace(html))
            {
                continue;
            }

            foreach (var scriptUrl in ExtractEntryScriptUrls(publishUrl, html))
            {
                var scriptText = await GetStringWithTimeoutAsync(probeClient, scriptUrl, options.Timeout, cancellationToken);
                if (string.IsNullOrWhiteSpace(scriptText))
                {
                    continue;
                }

                var publishedCandidates = ExtractPublishedCandidates(scriptText);
                if (publishedCandidates.Count == 0)
                {
                    continue;
                }

                return publishedCandidates;
            }
        }

        return Array.Empty<string>();
    }

    private static async Task<long> MeasureLatencyAsync(HttpClient client, string baseUrl, TimeSpan timeout, CancellationToken cancellationToken)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, baseUrl.TrimEnd('/') + ProbeRequestPath);
            request.Headers.TryAddWithoutValidation("User-Agent", PublishSourceUserAgent);
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

    private static async Task<string?> GetStringWithTimeoutAsync(HttpClient client, string requestUri, TimeSpan timeout, CancellationToken cancellationToken)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.TryAddWithoutValidation("User-Agent", PublishSourceUserAgent);

            using var timeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCancellation.CancelAfter(timeout);

            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead, timeoutCancellation.Token);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync(timeoutCancellation.Token);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return null;
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    private static string EnsureTrailingSlash(string url)
    {
        return url.EndsWith("/", StringComparison.Ordinal) ? url : url + '/';
    }

    private static IReadOnlyList<string> ExtractEntryScriptUrls(string publishUrl, string html)
    {
        if (!Uri.TryCreate(EnsureTrailingSlash(publishUrl), UriKind.Absolute, out var publishBaseUri))
        {
            return Array.Empty<string>();
        }

        return ScriptTagRegex.Matches(html)
            .Select(static match => match.Groups["path"].Value)
            .Where(IsEntryScriptPath)
            .Select(path => ResolveScriptUrl(publishBaseUri, path))
            .OfType<string>()
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static bool IsEntryScriptPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        var normalizedPath = TrimPathSuffix(path);
        var lastSlashIndex = normalizedPath.LastIndexOf('/');
        var fileName = lastSlashIndex >= 0
            ? normalizedPath[(lastSlashIndex + 1)..]
            : normalizedPath;

        return fileName.EndsWith(".js", StringComparison.OrdinalIgnoreCase)
            && fileName.Contains("index", StringComparison.OrdinalIgnoreCase);
    }

    private static string? ResolveScriptUrl(Uri publishBaseUri, string path)
    {
        var trimmedPath = path.Trim();
        return Uri.TryCreate(publishBaseUri, trimmedPath, out var scriptUri)
            ? scriptUri.ToString()
            : null;
    }

    private static IReadOnlyList<string> ExtractPublishedCandidates(string scriptText)
    {
        return LinkRegex.Matches(scriptText)
            .Select(static match => match.Groups["url"].Value)
            .Where(static item => item.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            .Select(ToApiBaseUrl)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static string TrimPathSuffix(string path)
    {
        var trimmedPath = path.Trim();
        var queryIndex = trimmedPath.IndexOf('?');
        var fragmentIndex = trimmedPath.IndexOf('#');

        if (queryIndex < 0 && fragmentIndex < 0)
        {
            return trimmedPath;
        }

        if (queryIndex < 0)
        {
            return trimmedPath[..fragmentIndex];
        }

        if (fragmentIndex < 0)
        {
            return trimmedPath[..queryIndex];
        }

        return trimmedPath[..Math.Min(queryIndex, fragmentIndex)];
    }

    private static string ToApiBaseUrl(string siteUrl)
    {
        var normalized = siteUrl.TrimEnd('/');
        return normalized.StartsWith("https://api.", StringComparison.OrdinalIgnoreCase)
            ? normalized
            : normalized.Replace("https://", "https://api.", StringComparison.OrdinalIgnoreCase);
    }
}