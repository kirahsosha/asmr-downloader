using System.Net.Http.Json;
using System.Text.Json;
using System.Globalization;
using Asmroner.Core.Api;
using Asmroner.Core.Constants;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Utils;

namespace Asmroner.Infrastructure.Services;

public sealed class AsmrApiClient : IAsmrApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly TimeSpan RequestTimeout = AsmronerConstants.Api.RequestTimeout;

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IApiEndpointUrlService _apiEndpointUrlService;
    private readonly IAuthService _authService;

    public AsmrApiClient(
        IHttpClientFactory httpClientFactory,
        IApiEndpointUrlService apiEndpointUrlService,
        IAuthService authService)
    {
        _httpClientFactory = httpClientFactory;
        _apiEndpointUrlService = apiEndpointUrlService;
        _authService = authService;
    }

    public Task<MetadataSyncPageDto> GetMetadataWorksAsync(int page, int pageSize, bool subtitleOnly = false, CancellationToken cancellationToken = default)
    {
        var normalizedPage = Math.Max(1, page);
        var normalizedPageSize = Math.Max(1, pageSize);
        return GetAsync<MetadataSyncPageDto>(AsmrApiPaths.BuildWorksQuery(normalizedPage, normalizedPageSize, subtitleOnly), cancellationToken);
    }

    public async Task<WorkInfoDto> GetWorkInfoAsync(string id, CancellationToken cancellationToken = default)
    {
        var numericId = await ResolveApiNumericIdAsync(id, cancellationToken);
        return await GetAsync<WorkInfoDto>(AsmrApiPaths.Work + numericId, cancellationToken);
    }

    public async Task<IReadOnlyList<TrackDto>> GetTracksAsync(string id, CancellationToken cancellationToken = default)
    {
        var numericId = await ResolveApiNumericIdAsync(id, cancellationToken);
        return await GetAsync<IReadOnlyList<TrackDto>>(AsmrApiPaths.Tracks + numericId, cancellationToken) ?? Array.Empty<TrackDto>();
    }

    public Task<SearchResultDto> SearchAsync(string query, CancellationToken cancellationToken = default)
        => GetAsync<SearchResultDto>(AsmrApiPaths.Search + query, cancellationToken);

    public async Task<IReadOnlyList<SearchWorkDto>> GetPopularAsync(CancellationToken cancellationToken = default)
    {
        // Go 版本使用 POST /api/recommender/popular，GET 会返回 404。
        var payload = new
        {
            keyword = string.Empty,
            page = 1,
            pageSize = 100,
            subtitle = 0,
            localSubtitledWorks = Array.Empty<object>(),
            withPlaylistStatus = Array.Empty<object>(),
        };

        var result = await PostAsync<SearchResultDto>(AsmrApiPaths.Popular, payload, cancellationToken);
        return result.Works;
    }

    public async Task DownloadFileAsync(string url, string destinationPath, CancellationToken cancellationToken = default)
    {
        await EnsureAuthenticatedAsync(cancellationToken);
        var token = await _authService.GetCurrentTokenAsync(cancellationToken);
        var requestUri = await ResolveRequestUriAsync(url, cancellationToken);
        var client = _httpClientFactory.CreateClient("AsmrApi");

        using var request = CreateRequest(HttpMethod.Get, requestUri, acceptHeader: "*/*");
        if (token is not null)
        {
            request.Headers.Remove("Authorization");
            request.Headers.TryAddWithoutValidation("Authorization", token.ToAuthorizationHeader());
        }

        using var timeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCancellation.CancelAfter(RequestTimeout);

        using var response = await client.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            timeoutCancellation.Token);
        if (!response.IsSuccessStatusCode)
        {
            throw await CreateExceptionAsync(response, AsmronerConstants.Api.ErrorCodes.RequestFailed, timeoutCancellation.Token);
        }

        var directory = Path.GetDirectoryName(destinationPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var responseStream = await response.Content.ReadAsStreamAsync(timeoutCancellation.Token);
        await using var outputStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await responseStream.CopyToAsync(outputStream, timeoutCancellation.Token);
    }

    private async Task<T> GetAsync<T>(string path, CancellationToken cancellationToken)
    {
        return await SendAsync<T>(HttpMethod.Get, path, body: null, cancellationToken);
    }

    private async Task<T> PostAsync<T>(string path, object body, CancellationToken cancellationToken)
    {
        return await SendAsync<T>(HttpMethod.Post, path, body, cancellationToken);
    }

    private async Task<T> SendAsync<T>(HttpMethod method, string path, object? body, CancellationToken cancellationToken)
    {
        await EnsureAuthenticatedAsync(cancellationToken);
        var token = await _authService.GetCurrentTokenAsync(cancellationToken);

        var client = _httpClientFactory.CreateClient("AsmrApi");
        var requestUri = await ResolveRequestUriAsync(path, cancellationToken);
        using var request = CreateRequest(method, requestUri);
        if (token is not null)
        {
            request.Headers.Remove("Authorization");
            request.Headers.TryAddWithoutValidation("Authorization", token.ToAuthorizationHeader());
        }

        if (body is not null)
        {
            request.Content = JsonContent.Create(body, options: JsonOptions);
        }

        using var timeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCancellation.CancelAfter(RequestTimeout);

        using var response = await client.SendAsync(request, timeoutCancellation.Token);
        if (!response.IsSuccessStatusCode)
        {
            throw await CreateExceptionAsync(response, AsmronerConstants.Api.ErrorCodes.RequestFailed, timeoutCancellation.Token);
        }

        return await ReadAsRequiredAsync<T>(response, timeoutCancellation.Token);
    }

    private async Task EnsureAuthenticatedAsync(CancellationToken cancellationToken)
    {
        var token = await _authService.GetCurrentTokenAsync(cancellationToken);
        if (token is null || token.IsExpired())
        {
            await _authService.LoginAsync(cancellationToken);
        }
    }

    private async Task<string> ResolveRequestUriAsync(string pathOrAbsoluteUrl, CancellationToken cancellationToken)
    {
        if (Uri.TryCreate(pathOrAbsoluteUrl, UriKind.Absolute, out var absoluteUri))
        {
            return absoluteUri.ToString();
        }

        var currentBaseUrl = await _apiEndpointUrlService.GetCurrentBaseUrlAsync(cancellationToken);
        return BuildRequestUri(currentBaseUrl, pathOrAbsoluteUrl);
    }

    private static HttpRequestMessage CreateRequest(HttpMethod method, string requestUri, string acceptHeader = "application/json")
    {
        var request = new HttpRequestMessage(method, requestUri);
        request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36");
        request.Headers.Accept.ParseAdd(acceptHeader);
        return request;
    }

    private static string BuildRequestUri(string baseUrl, string path)
    {
        var baseUri = new Uri(baseUrl.TrimEnd('/') + "/", UriKind.Absolute);
        return new Uri(baseUri, path.TrimStart('/')).ToString();
    }

    private static async Task<AsmrApiException> CreateExceptionAsync(HttpResponseMessage response, string code, CancellationToken cancellationToken)
    {
        var rawBody = await response.Content.ReadAsStringAsync(cancellationToken);
        return new AsmrApiException(new ApiError
        {
            Code = code,
            HttpStatus = (int)response.StatusCode,
            Message = $"API 调用失败: {(int)response.StatusCode} {response.ReasonPhrase}",
            RawBody = rawBody,
        });
    }

    private static async Task<T> ReadAsRequiredAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var result = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
        if (result is null)
        {
            throw new AsmrApiException(new ApiError
            {
                Code = AsmronerConstants.Api.ErrorCodes.ResponseEmpty,
                HttpStatus = (int)response.StatusCode,
                Message = "API 返回为空。",
            });
        }

        return result;
    }

    private async Task<string> ResolveApiNumericIdAsync(string raw, CancellationToken cancellationToken)
    {
        var numericId = ToApiNumericId(raw);
        if (string.IsNullOrWhiteSpace(numericId) || numericId.All(char.IsDigit))
        {
            return numericId;
        }

        var sourceId = SourceIdNormalizer.Normalize(raw);
        if (string.IsNullOrWhiteSpace(sourceId))
        {
            return numericId;
        }

        var query = AsmrApiPaths.BuildQuery(
            Uri.EscapeDataString(sourceId),
            new Dictionary<string, object?>
            {
                ["order"] = "id",
                ["sort"] = "desc",
                ["page"] = 1,
                ["pageSize"] = 20,
                ["subtitle"] = 0,
                ["includeTranslationWorks"] = true,
            });
        var result = await SearchAsync(query, cancellationToken);
        var matched = result.Works.FirstOrDefault(work =>
            string.Equals(SourceIdNormalizer.Normalize(work.SourceId), sourceId, StringComparison.OrdinalIgnoreCase));

        return matched?.Id > 0
            ? matched.Id.ToString(CultureInfo.InvariantCulture)
            : numericId;
    }

    // asmr.one /api/work/{id} 和 /api/tracks/{id} 仅接受纯数字 id，不含 "RJ" 前缀。
    private static string ToApiNumericId(string raw)
    {
        return SourceIdNormalizer.ToApiNumericId(raw);
    }
}