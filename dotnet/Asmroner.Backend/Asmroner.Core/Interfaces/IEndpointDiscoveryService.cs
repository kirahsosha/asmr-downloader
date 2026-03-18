using Asmroner.Core.Api;

namespace Asmroner.Core.Interfaces;

public interface IEndpointDiscoveryService
{
    Task<EndpointDiscoveryResult> DiscoverAsync(CancellationToken cancellationToken = default);
}