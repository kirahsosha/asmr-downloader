namespace Asmroner.Core.Interfaces;

public interface IRateLimiterService
{
    Task WaitAsync(double qps, int jitterMin, int jitterMax, CancellationToken cancellationToken = default);
}
