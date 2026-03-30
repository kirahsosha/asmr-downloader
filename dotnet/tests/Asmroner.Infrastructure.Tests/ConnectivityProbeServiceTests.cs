using Asmroner.Core.Api;
using Asmroner.Core.Interfaces;
using Asmroner.Infrastructure.Services;


namespace Asmroner.Infrastructure.Tests;

public class ConnectivityProbeServiceTests
{
    [Fact]
    public async Task ProbeAsync_ShouldDiscoverAndAuthenticate_WhenDependenciesSucceed()
    {
        var endpointService = new RecordingApiEndpointUrlService("https://api.example.com", latencyMs: 23);
        var authService = new RecordingAuthService(new ApiToken { AccessToken = "jwt-token" });
        var sut = new ConnectivityProbeService(endpointService, authService);

        var result = await sut.ProbeAsync();

        Assert.True(result.IsReachable);
        Assert.True(result.IsAuthenticated);
        Assert.Equal("https://api.example.com", result.BaseUrl);
        Assert.Equal(23, result.LatencyMs);
        Assert.Equal(1, endpointService.DiscoverCallCount);
        Assert.Equal(1, authService.LoginCallCount);
    }

    [Fact]
    public async Task ProbeAsync_ShouldReturnFailureResult_WhenAuthenticationThrows()
    {
        var endpointService = new RecordingApiEndpointUrlService("https://api.example.com", latencyMs: 5);
        var authService = new RecordingAuthService(token: null, throwOnLogin: true);
        var sut = new ConnectivityProbeService(endpointService, authService);

        var result = await sut.ProbeAsync();

        Assert.False(result.IsReachable);
        Assert.False(result.IsAuthenticated);
        Assert.Contains("forced login failure", result.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(1, endpointService.DiscoverCallCount);
        Assert.Equal(1, authService.LoginCallCount);
    }

    private sealed class RecordingApiEndpointUrlService : IApiEndpointUrlService
    {
        private readonly string _baseUrl;
        private readonly long _latencyMs;

        public RecordingApiEndpointUrlService(string baseUrl, long latencyMs)
        {
            _baseUrl = baseUrl;
            _latencyMs = latencyMs;
        }

        public int DiscoverCallCount { get; private set; }

        public Task<string> GetCurrentBaseUrlAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(_baseUrl);

        public Task<EndpointDiscoveryResult> DiscoverAndPersistAsync(CancellationToken cancellationToken = default)
        {
            DiscoverCallCount++;
            return Task.FromResult(new EndpointDiscoveryResult
            {
                BaseUrl = _baseUrl,
                LatencyMs = _latencyMs,
                Candidates = new[] { _baseUrl },
            });
        }
    }

    private sealed class RecordingAuthService : IAuthService
    {
        private readonly ApiToken? _token;
        private readonly bool _throwOnLogin;

        public RecordingAuthService(ApiToken? token, bool throwOnLogin = false)
        {
            _token = token;
            _throwOnLogin = throwOnLogin;
        }

        public int LoginCallCount { get; private set; }

        public bool IsAuthenticated => _token is not null;

        public Task<ApiToken?> GetCurrentTokenAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(_token);

        public Task<ApiToken> LoginAsync(CancellationToken cancellationToken = default)
        {
            LoginCallCount++;
            if (_throwOnLogin)
            {
                throw new InvalidOperationException("forced login failure");
            }

            return Task.FromResult(_token ?? throw new InvalidOperationException("missing token"));
        }

        public Task LogoutAsync(CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
