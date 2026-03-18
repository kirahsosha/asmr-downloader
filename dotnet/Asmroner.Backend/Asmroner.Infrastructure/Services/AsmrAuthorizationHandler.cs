using Asmroner.Core.Interfaces;

namespace Asmroner.Infrastructure.Services;

public sealed class AsmrAuthorizationHandler : DelegatingHandler
{
    private readonly ITokenStore _tokenStore;

    public AsmrAuthorizationHandler(ITokenStore tokenStore)
    {
        _tokenStore = tokenStore;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenStore.GetAsync(cancellationToken);
        if (token is not null)
        {
            request.Headers.Remove("Authorization");
            request.Headers.TryAddWithoutValidation("Authorization", token.ToAuthorizationHeader());
        }

        return await base.SendAsync(request, cancellationToken);
    }
}