using Asmroner.Core.Api;

namespace Asmroner.Core.Interfaces;

public interface ITokenStore
{
    Task<ApiToken?> GetAsync(CancellationToken cancellationToken = default);

    Task SetAsync(ApiToken token, CancellationToken cancellationToken = default);

    Task ClearAsync(CancellationToken cancellationToken = default);
}