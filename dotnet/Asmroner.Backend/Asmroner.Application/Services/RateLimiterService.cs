using Asmroner.Core.Interfaces;

namespace Asmroner.Application.Services;

public sealed class RateLimiterService : IRateLimiterService
{
    private readonly Random _random = new();

    public async Task WaitAsync(double qps, int jitterMin, int jitterMax, CancellationToken cancellationToken = default)
    {
        var minQps = qps <= 0 ? 1 : qps;
        var baseDelayMs = (int)Math.Ceiling(1000d / minQps);
        var minJitter = Math.Max(0, Math.Min(jitterMin, jitterMax));
        var maxJitter = Math.Max(0, Math.Max(jitterMin, jitterMax));
        var jitter = maxJitter == minJitter ? minJitter : _random.Next(minJitter, maxJitter + 1);

        await Task.Delay(baseDelayMs + jitter, cancellationToken);
    }
}
