using Asmroner.Core.Api;

namespace Asmroner.Core.Interfaces;

public interface IAuthService
{
    bool IsAuthenticated { get; }

    Task<ApiToken?> GetCurrentTokenAsync(CancellationToken cancellationToken = default);

    Task<ApiToken> LoginAsync(CancellationToken cancellationToken = default);

    Task LogoutAsync(CancellationToken cancellationToken = default);
}