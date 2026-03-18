using Asmroner.Core.Api;

namespace Asmroner.Core.Interfaces;

public interface IConnectivityProbeService
{
    Task<ConnectivityProbeResult> ProbeAsync(CancellationToken cancellationToken = default);
}