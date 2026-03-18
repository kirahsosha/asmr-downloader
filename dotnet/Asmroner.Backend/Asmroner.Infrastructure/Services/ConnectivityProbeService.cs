using Asmroner.Core.Api;
using Asmroner.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Asmroner.Infrastructure.Services;

public sealed class ConnectivityProbeService : IConnectivityProbeService
{
    private readonly IApiEndpointUrlService _apiEndpointUrlService;
    private readonly IAuthService _authService;
    private readonly ILogger<ConnectivityProbeService> _logger;

    public ConnectivityProbeService(
        IApiEndpointUrlService apiEndpointUrlService,
        IAuthService authService,
        ILogger<ConnectivityProbeService> logger)
    {
        _apiEndpointUrlService = apiEndpointUrlService;
        _authService = authService;
        _logger = logger;
    }

    public async Task<ConnectivityProbeResult> ProbeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var endpoint = await _apiEndpointUrlService.DiscoverAndPersistAsync(cancellationToken);
            var token = await _authService.LoginAsync(cancellationToken);

            var result = new ConnectivityProbeResult
            {
                IsReachable = true,
                IsAuthenticated = !string.IsNullOrWhiteSpace(token.AccessToken),
                BaseUrl = endpoint.BaseUrl,
                LatencyMs = endpoint.LatencyMs,
                Message = "连接正常。",
            };

            _logger.LogInformation("Connectivity probe succeeded for {BaseUrl} with latency {LatencyMs} ms.", result.BaseUrl, result.LatencyMs);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Connectivity probe failed.");
            return new ConnectivityProbeResult
            {
                IsReachable = false,
                IsAuthenticated = false,
                Message = ex.Message,
            };
        }
    }
}