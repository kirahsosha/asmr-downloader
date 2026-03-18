using System.Net.Http.Json;
using System.Text.Json;
using Asmroner.Core.Api;
using Asmroner.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Asmroner.Infrastructure.Services;

public sealed class AuthService : IAuthService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(10);

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IApiEndpointUrlService _apiEndpointUrlService;
    private readonly IConfigurationService _configurationService;
    private readonly ITokenStore _tokenStore;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IHttpClientFactory httpClientFactory,
        IApiEndpointUrlService apiEndpointUrlService,
        IConfigurationService configurationService,
        ITokenStore tokenStore,
        ILogger<AuthService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _apiEndpointUrlService = apiEndpointUrlService;
        _configurationService = configurationService;
        _tokenStore = tokenStore;
        _logger = logger;
    }

    public bool IsAuthenticated => _tokenStore.GetAsync().GetAwaiter().GetResult() is not null;

    public Task<ApiToken?> GetCurrentTokenAsync(CancellationToken cancellationToken = default)
        => _tokenStore.GetAsync(cancellationToken);

    public async Task<ApiToken> LoginAsync(CancellationToken cancellationToken = default)
    {
        var config = await _configurationService.LoadAsync(cancellationToken) ?? new();
        if (string.IsNullOrWhiteSpace(config.User.Account) || string.IsNullOrWhiteSpace(config.User.Password))
        {
            throw new InvalidOperationException("缺少账号或密码，无法执行登录。");
        }

        var currentBaseUrl = await _apiEndpointUrlService.GetCurrentBaseUrlAsync(cancellationToken);
        var client = _httpClientFactory.CreateClient("AsmrApi");

        var request = new AuthLoginRequest
        {
            Name = config.User.Account,
            Password = config.User.Password,
        };

        var requestUri = BuildRequestUri(currentBaseUrl, AsmrApiPaths.AuthLogin);
        using var httpRequest = CreateRequest(HttpMethod.Post, requestUri);
        httpRequest.Content = JsonContent.Create(request, options: JsonOptions);
        using var timeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCancellation.CancelAfter(RequestTimeout);

        using var response = await client.SendAsync(httpRequest, timeoutCancellation.Token);
        if (!response.IsSuccessStatusCode)
        {
            throw await CreateExceptionAsync(response, "auth_login_failed", timeoutCancellation.Token);
        }

        var payload = await response.Content.ReadFromJsonAsync<AuthLoginResponse>(JsonOptions, timeoutCancellation.Token);
        if (payload is null || string.IsNullOrWhiteSpace(payload.Token))
        {
            throw new AsmrApiException(new ApiError
            {
                Code = "auth_token_missing",
                HttpStatus = (int)response.StatusCode,
                Message = "登录成功但响应中未找到 token。",
            });
        }

        var token = new ApiToken
        {
            AccessToken = payload.Token,
        };

        await _tokenStore.SetAsync(token, cancellationToken);
        _logger.LogInformation("Auth login succeeded against {BaseUrl}.", currentBaseUrl);
        return token;
    }

    public Task LogoutAsync(CancellationToken cancellationToken = default)
        => _tokenStore.ClearAsync(cancellationToken);

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
}