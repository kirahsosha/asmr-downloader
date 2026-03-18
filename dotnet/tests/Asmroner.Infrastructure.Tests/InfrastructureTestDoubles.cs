using System.Net;
using Asmroner.Core.Api;
using Asmroner.Core.Configuration;
using Asmroner.Core.Interfaces;

namespace Asmroner.Infrastructure.Tests;

internal sealed class RecordingHttpClientFactory : IHttpClientFactory
{
    private readonly Dictionary<string, HttpClient> _clients = new(StringComparer.OrdinalIgnoreCase);

    public void Register(string name, HttpMessageHandler handler)
    {
        _clients[name] = new HttpClient(handler);
    }

    public HttpClient CreateClient(string name)
    {
        if (_clients.TryGetValue(name, out var client))
        {
            return client;
        }

        throw new InvalidOperationException($"No client registered for {name}.");
    }
}

internal sealed class RecordingHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _handler;

    public RecordingHttpMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler)
    {
        _handler = handler;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        => _handler(request);
}

internal sealed class StubInfrastructureConfigurationService : IConfigurationService
{
    private readonly AppConfig _config;

    public StubInfrastructureConfigurationService(AppConfig config)
    {
        _config = config;
    }

    public bool Exists() => true;

    public Task<AppConfig?> LoadAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<AppConfig?>(_config);

    public Task SaveAsync(AppConfig config, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public IReadOnlyList<string> Validate(AppConfig config)
        => Array.Empty<string>();
}

internal sealed class StubInfrastructureEndpointDiscoveryService : IEndpointDiscoveryService
{
    private readonly string _baseUrl;

    public StubInfrastructureEndpointDiscoveryService(string baseUrl)
    {
        _baseUrl = baseUrl;
    }

    public Task<EndpointDiscoveryResult> DiscoverAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(new EndpointDiscoveryResult
        {
            BaseUrl = _baseUrl,
            LatencyMs = 1,
            Candidates = new[] { _baseUrl },
        });
}

internal sealed class StubInfrastructureOptionsProvider : IAsmrApiOptionsProvider
{
    private readonly AsmrApiOptions _options;

    public StubInfrastructureOptionsProvider(AsmrApiOptions options)
    {
        _options = options;
    }

    public Task<AsmrApiOptions> GetOptionsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(_options);
}

internal sealed class StubAuthService : IAuthService
{
    private readonly ApiToken? _token;

    public StubAuthService(ApiToken? token)
    {
        _token = token;
    }

    public bool IsAuthenticated => _token is not null;

    public Task<ApiToken?> GetCurrentTokenAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(_token);

    public Task<ApiToken> LoginAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(_token ?? throw new InvalidOperationException("No token configured."));

    public Task LogoutAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
