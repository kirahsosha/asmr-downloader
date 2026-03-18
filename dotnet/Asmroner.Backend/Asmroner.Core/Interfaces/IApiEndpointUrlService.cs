using Asmroner.Core.Api;

namespace Asmroner.Core.Interfaces;

public interface IApiEndpointUrlService
{
    Task<string> GetCurrentBaseUrlAsync(CancellationToken cancellationToken = default);

    Task<EndpointDiscoveryResult> DiscoverAndPersistAsync(CancellationToken cancellationToken = default);
}
