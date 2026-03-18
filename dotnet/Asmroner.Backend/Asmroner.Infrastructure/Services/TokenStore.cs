using Asmroner.Core.Api;
using Asmroner.Core.Interfaces;

namespace Asmroner.Infrastructure.Services;

public sealed class TokenStore : ITokenStore
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private ApiToken? _token;

    public async Task<ApiToken?> GetAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            if (_token?.IsExpired() == true)
            {
                _token = null;
            }

            return _token;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task SetAsync(ApiToken token, CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            _token = token;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            _token = null;
        }
        finally
        {
            _gate.Release();
        }
    }
}