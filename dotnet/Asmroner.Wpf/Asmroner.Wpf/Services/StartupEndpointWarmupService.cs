using Asmroner.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Asmroner.Wpf.Services;

public sealed class StartupEndpointWarmupService
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);

    private readonly IApiEndpointUrlService _apiEndpointUrlService;
    private readonly ILogger<StartupEndpointWarmupService> _logger;
    private readonly TimeSpan _timeout;

    public StartupEndpointWarmupService(
        IApiEndpointUrlService apiEndpointUrlService,
        ILogger<StartupEndpointWarmupService> logger,
        TimeSpan? timeout = null)
    {
        _apiEndpointUrlService = apiEndpointUrlService;
        _logger = logger;
        var effectiveTimeout = timeout ?? DefaultTimeout;
        _timeout = effectiveTimeout > TimeSpan.Zero ? effectiveTimeout : DefaultTimeout;
    }

    public Task StartInBackgroundAsync(CancellationToken cancellationToken = default)
        => Task.Run(() => WarmupAsync(cancellationToken), CancellationToken.None);

    private async Task WarmupAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var timeoutCancellation = new CancellationTokenSource(_timeout);
            using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCancellation.Token);

            var endpoint = await _apiEndpointUrlService.DiscoverAndPersistAsync(linkedCancellation.Token).ConfigureAwait(false);
            _logger.LogInformation("Startup endpoint warmup succeeded: {BaseUrl} ({LatencyMs} ms).", endpoint.BaseUrl, endpoint.LatencyMs);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Startup endpoint warmup timed out after {TimeoutMs} ms; app will continue with configured API url.", (long)_timeout.TotalMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Startup endpoint warmup failed; app will continue with configured API url.");
        }
    }
}