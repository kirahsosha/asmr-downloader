using Asmroner.Core.Api;
using Asmroner.Core.Interfaces;
using NLog;

namespace Asmroner.Infrastructure.Services;

public sealed class ConnectivityProbeService : IConnectivityProbeService
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private readonly IApiEndpointUrlService _apiEndpointUrlService;
    private readonly IAuthService _authService;

    public ConnectivityProbeService(
        IApiEndpointUrlService apiEndpointUrlService,
        IAuthService authService)
    {
        _apiEndpointUrlService = apiEndpointUrlService;
        _authService = authService;
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

            _logger.Info("Connectivity probe succeeded for {BaseUrl} with latency {LatencyMs} ms.", result.BaseUrl, result.LatencyMs);
            return result;
        }
        catch (Exception ex)
        {
            _logger.Warn(ex, "Connectivity probe failed.");
            return new ConnectivityProbeResult
            {
                IsReachable = false,
                IsAuthenticated = false,
                Message = ex.Message,
            };
        }
    }
}