using Asmroner.Core.Api;
using Asmroner.Infrastructure.Services;

namespace Asmroner.Infrastructure.Tests;

public class TokenStoreTests
{
    [Fact]
    public async Task TokenStore_ShouldRoundTripToken()
    {
        var sut = new TokenStore();
        var token = new ApiToken
        {
            AccessToken = "abc123",
        };

        await sut.SetAsync(token);
        var loaded = await sut.GetAsync();

        Assert.NotNull(loaded);
        Assert.Equal("abc123", loaded!.AccessToken);
    }
}