using System.Net.Http.Json;
using System.Text.Json;
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

    public async Task<WorkInfoDto> GetWorkInfoAsync(string id, CancellationToken cancellationToken = default)
        => await GetAsync<WorkInfoDto>(AsmrApiPaths.Work + ToApiNumericId(id), cancellationToken);

    public async Task<IReadOnlyList<TrackDto>> GetTracksAsync(string id, CancellationToken cancellationToken = default)
        => await GetAsync<IReadOnlyList<TrackDto>>(AsmrApiPaths.Tracks + ToApiNumericId(id), cancellationToken) ?? Array.Empty<TrackDto>();

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

        var currentBaseUrl = await _apiEndpointUrlService.GetCurrentBaseUrlAsync(cancellationToken);
        var client = _httpClientFactory.CreateClient("AsmrApi");
        var requestUri = BuildRequestUri(currentBaseUrl, path);
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

    private static HttpRequestMessage CreateRequest(HttpMethod method, string requestUri)
    {
        var request = new HttpRequestMessage(method, requestUri);
        request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36");
        request.Headers.Accept.ParseAdd("application/json");
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

    // asmr.one /api/work/{id} 和 /api/tracks/{id} 仅接受纯数字 id，不含 "RJ" 前缀。
    private static string ToApiNumericId(string raw)
    {
        return SourceIdNormalizer.ToApiNumericId(raw);
    }
}