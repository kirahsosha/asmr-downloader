using Asmroner.Core.Configuration;
using System.Net;
using System.Text;
using Asmroner.Core.Api;
using Asmroner.Infrastructure.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace Asmroner.Infrastructure.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task AuthService_ShouldRejectLogin_WhenAccountOrPasswordEmpty()
    {
        var factory = new RecordingHttpClientFactory();
        var sut = new AuthService(
            factory,
            new StubApiEndpointUrlService("https://api.example.com"),
            new StubInfrastructureConfigurationService(new AppConfig
            {
                User = new UserOptions
                {
                    Account = string.Empty,
                    Password = string.Empty,
                },
            }),
            new TokenStore(),
            NullLogger<AuthService>.Instance);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.LoginAsync());

        Assert.Contains("缺少账号或密码", ex.Message);
    }

    [Fact]
    public async Task AuthService_ShouldStoreTokenAfterLogin()
    {
        var factory = new RecordingHttpClientFactory();
        factory.Register("AsmrApi", new RecordingHttpMessageHandler(_ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"token\":\"jwt-token\"}", Encoding.UTF8, "application/json"),
        })));
        factory.Register("AsmrProbe", new RecordingHttpMessageHandler(_ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK))));

        var tokenStore = new TokenStore();
        var sut = new AuthService(
            factory,
            new StubApiEndpointUrlService("https://api.example.com"),
            new StubInfrastructureConfigurationService(new AppConfig
            {
                User = new UserOptions
                {
                    Account = "tester",
                    Password = "secret",
                },
            }),
            tokenStore,
            NullLogger<AuthService>.Instance);

        var token = await sut.LoginAsync();
        var stored = await tokenStore.GetAsync();

        Assert.Equal("jwt-token", token.AccessToken);
        Assert.NotNull(stored);
        Assert.Equal("jwt-token", stored!.AccessToken);
    }

    [Fact]
    public async Task AuthService_ShouldThrowReadableErrorOnFailure()
    {
        var factory = new RecordingHttpClientFactory();
        factory.Register("AsmrApi", new RecordingHttpMessageHandler(_ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Unauthorized)
        {
            Content = new StringContent("unauthorized", Encoding.UTF8, "text/plain"),
        })));

        var sut = new AuthService(
            factory,
            new StubApiEndpointUrlService("https://api.example.com"),
            new StubInfrastructureConfigurationService(new AppConfig
            {
                User = new UserOptions
                {
                    Account = "tester",
                    Password = "bad",
                },
            }),
            new TokenStore(),
            NullLogger<AuthService>.Instance);

        var ex = await Assert.ThrowsAsync<AsmrApiException>(() => sut.LoginAsync());

        Assert.Equal("auth_login_failed", ex.Error.Code);
        Assert.Equal(401, ex.Error.HttpStatus);
    }

    [Fact]
    public async Task AuthService_ShouldUseCurrentBaseUrlService_WithoutDiscovery()
    {
        var endpointService = new StubApiEndpointUrlService("https://api.example.com", throwOnDiscover: true);
        var factory = new RecordingHttpClientFactory();
        factory.Register("AsmrApi", new RecordingHttpMessageHandler(_ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"token\":\"jwt-token\"}", Encoding.UTF8, "application/json"),
        })));

        var sut = new AuthService(
            factory,
            endpointService,
            new StubInfrastructureConfigurationService(new AppConfig
            {
                User = new UserOptions
                {
                    Account = "tester",
                    Password = "secret",
                },
            }),
            new TokenStore(),
            NullLogger<AuthService>.Instance);

        _ = await sut.LoginAsync();

        Assert.Equal(1, endpointService.GetCurrentBaseUrlCallCount);
        Assert.Equal(0, endpointService.DiscoverAndPersistCallCount);
    }
}
