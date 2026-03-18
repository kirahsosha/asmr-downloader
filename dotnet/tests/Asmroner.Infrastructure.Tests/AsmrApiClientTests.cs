using System.Net;
using System.Text;
using System.Text.Json;
using Asmroner.Core.Api;
using Asmroner.Infrastructure.Services;

namespace Asmroner.Infrastructure.Tests;

public class AsmrApiClientTests
{
    [Fact]
    public async Task AsmrApiClient_ShouldMapHttpErrors()
    {
        var factory = new RecordingHttpClientFactory();
        factory.Register("AsmrApi", new RecordingHttpMessageHandler(_ =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("boom", Encoding.UTF8, "text/plain"),
            })));

        var sut = new AsmrApiClient(
            factory,
            new StubApiEndpointUrlService("https://api.example.com"),
            new StubAuthService(new ApiToken { AccessToken = "jwt-token" }));

        var ex = await Assert.ThrowsAsync<AsmrApiException>(() => sut.GetWorkInfoAsync("RJ123"));

        Assert.Equal("api_request_failed", ex.Error.Code);
        Assert.Equal(500, ex.Error.HttpStatus);
    }

    [Fact]
    public async Task GetPopularAsync_ShouldUsePostAndMapWorks()
    {
        HttpRequestMessage? capturedRequest = null;
        string? capturedPayload = null;

        var factory = new RecordingHttpClientFactory();
        factory.Register("AsmrApi", new RecordingHttpMessageHandler(async request =>
        {
            capturedRequest = request;
            capturedPayload = request.Content is null ? null : await request.Content.ReadAsStringAsync();

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"works\":[{\"id\":42,\"title\":\"Popular\",\"source_id\":\"RJ42\",\"download_count\":9}],\"pagination\":{\"currentPage\":1,\"pageSize\":100,\"totalCount\":1}}", Encoding.UTF8, "application/json"),
            };
        }));

        var sut = new AsmrApiClient(
            factory,
            new StubApiEndpointUrlService("https://api.example.com"),
            new StubAuthService(new ApiToken { AccessToken = "jwt-token" }));

        var result = await sut.GetPopularAsync();

        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Post, capturedRequest!.Method);
        Assert.Equal("/api/recommender/popular", capturedRequest.RequestUri!.AbsolutePath);

        Assert.False(string.IsNullOrWhiteSpace(capturedPayload));
        using var payload = JsonDocument.Parse(capturedPayload!);
        Assert.Equal(1, payload.RootElement.GetProperty("page").GetInt32());
        Assert.Equal(100, payload.RootElement.GetProperty("pageSize").GetInt32());

        var item = Assert.Single(result);
        Assert.Equal("RJ42", item.SourceId);
        Assert.Equal("Popular", item.Title);
    }

    [Fact]
    public async Task AsmrApiClient_SearchAsync_ShouldNotDoubleEncodeQuery()
    {
        HttpRequestMessage? capturedRequest = null;
        var factory = new RecordingHttpClientFactory();
        factory.Register("AsmrApi", new RecordingHttpMessageHandler(request =>
        {
            capturedRequest = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"works\":[],\"pagination\":{\"currentPage\":1,\"pageSize\":20,\"totalCount\":0}}", Encoding.UTF8, "application/json"),
            });
        }));

        var sut = new AsmrApiClient(
            factory,
            new StubApiEndpointUrlService("https://api.example.com"),
            new StubAuthService(new ApiToken { AccessToken = "jwt-token" }));

        _ = await sut.SearchAsync("%20%24age%3Ageneral%24?order=release&sort=desc&page=1&pageSize=20");

        Assert.NotNull(capturedRequest?.RequestUri);
        Assert.Contains("%20%24age%3Ageneral%24", capturedRequest!.RequestUri!.AbsoluteUri);
        Assert.DoesNotContain("%2520%2524age%253Ageneral%2524", capturedRequest.RequestUri.AbsoluteUri);
    }

    [Fact]
    public async Task AsmrApiClient_ShouldAttachBearerToken_OnAuthorizedCalls()
    {
        HttpRequestMessage? capturedRequest = null;
        var factory = new RecordingHttpClientFactory();
        factory.Register("AsmrApi", new RecordingHttpMessageHandler(request =>
        {
            capturedRequest = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"id\":123,\"title\":\"Sample\",\"release\":\"2026-03-15\",\"has_subtitle\":true,\"source_id\":\"RJ7301\"}", Encoding.UTF8, "application/json"),
            });
        }));

        var sut = new AsmrApiClient(
            factory,
            new StubApiEndpointUrlService("https://api.example.com"),
            new StubAuthService(new ApiToken { AccessToken = "jwt-token" }));

        var result = await sut.GetWorkInfoAsync("RJ7301");

        Assert.NotNull(capturedRequest);
        Assert.NotNull(capturedRequest!.Headers.Authorization);
        Assert.Equal("Bearer", capturedRequest.Headers.Authorization!.Scheme);
        Assert.Equal("jwt-token", capturedRequest.Headers.Authorization.Parameter);
        Assert.Equal("RJ7301", result.SourceId);
    }

    [Fact]
    public async Task AsmrApiClient_ShouldNormalizeWorkUrlInput_ToWorkEndpointPath()
    {
        HttpRequestMessage? capturedRequest = null;
        var factory = new RecordingHttpClientFactory();
        factory.Register("AsmrApi", new RecordingHttpMessageHandler(request =>
        {
            capturedRequest = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"id\":123,\"title\":\"Sample\",\"release\":\"2026-03-15\",\"has_subtitle\":true,\"source_id\":\"RJ7301\"}", Encoding.UTF8, "application/json"),
            });
        }));

        var sut = new AsmrApiClient(
            factory,
            new StubApiEndpointUrlService("https://api.example.com"),
            new StubAuthService(new ApiToken { AccessToken = "jwt-token" }));

        // asmr.one API 端点仅接受纯数字 id，传入包含前缀的 "RJ7301" 会返回 400 Bad Request
        _ = await sut.GetWorkInfoAsync("https://www.asmr.one/work/rj7301");

        Assert.NotNull(capturedRequest?.RequestUri);
        Assert.Equal("/api/work/7301", capturedRequest!.RequestUri!.AbsolutePath);
    }

    [Theory]
    [InlineData("RJ-7301")]
    [InlineData("7301")]
    [InlineData("RJ7301")]
    public async Task AsmrApiClient_ShouldNormalizeNonCanonicalSourceId_ToNumericApiPath(string sourceId)
    {
        HttpRequestMessage? capturedRequest = null;
        var factory = new RecordingHttpClientFactory();
        factory.Register("AsmrApi", new RecordingHttpMessageHandler(request =>
        {
            capturedRequest = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"id\":123,\"title\":\"Sample\",\"release\":\"2026-03-15\",\"has_subtitle\":true,\"source_id\":\"RJ7301\"}", Encoding.UTF8, "application/json"),
            });
        }));

        var sut = new AsmrApiClient(
            factory,
            new StubApiEndpointUrlService("https://api.example.com"),
            new StubAuthService(new ApiToken { AccessToken = "jwt-token" }));

        _ = await sut.GetWorkInfoAsync(sourceId);

        // asmr.one /api/work/{id} 和 /api/tracks/{id} 仅接受纯数字 id
        // 传入包含 "RJ" 前缀的 id 将返回 400 Bad Request
        Assert.NotNull(capturedRequest?.RequestUri);
        Assert.Equal("/api/work/7301", capturedRequest!.RequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task AsmrApiClient_ShouldUseCurrentBaseUrlService_WithoutDiscovery()
    {
        var endpointService = new StubApiEndpointUrlService("https://api.example.com", throwOnDiscover: true);
        var factory = new RecordingHttpClientFactory();
        factory.Register("AsmrApi", new RecordingHttpMessageHandler(_ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"id\":123,\"title\":\"Sample\",\"release\":\"2026-03-15\",\"has_subtitle\":true,\"source_id\":\"RJ7301\"}", Encoding.UTF8, "application/json"),
        })));

        var sut = new AsmrApiClient(
            factory,
            endpointService,
            new StubAuthService(new ApiToken { AccessToken = "jwt-token" }));

        _ = await sut.GetWorkInfoAsync("RJ7301");

        Assert.Equal(1, endpointService.GetCurrentBaseUrlCallCount);
        Assert.Equal(0, endpointService.DiscoverAndPersistCallCount);
    }
}
