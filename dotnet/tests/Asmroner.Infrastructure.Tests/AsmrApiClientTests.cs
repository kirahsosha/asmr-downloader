using System.Net;
using System.Text;
using System.Text.Json;
using Asmroner.Core.Api;
using Asmroner.Infrastructure.Services;

namespace Asmroner.Infrastructure.Tests;

public class AsmrApiClientTests
{
    [Fact]
    public async Task GetMetadataWorksAsync_ShouldUseWorksEndpoint_AndSubtitleFlag()
    {
        HttpRequestMessage? capturedRequest = null;

        var factory = new RecordingHttpClientFactory();
        factory.Register("AsmrApi", new RecordingHttpMessageHandler(request =>
        {
            capturedRequest = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"works\":[{\"id\":501,\"title\":\"Sync Title\",\"circle_id\":8,\"name\":\"Circle\",\"nsfw\":false,\"release\":\"2026-04-02\",\"dl_count\":15,\"price\":100,\"review_count\":2,\"rate_count\":3,\"rate_average_2dp\":4.5,\"has_subtitle\":true,\"create_date\":\"2026-04-02\",\"vas\":[{\"id\":\"1\",\"name\":\"VA\"}],\"tags\":[{\"id\":1,\"name\":\"tag\"}],\"duration\":1200,\"source_type\":\"DLSITE\",\"source_id\":\"RJ501\"}],\"pagination\":{\"currentPage\":2,\"pageSize\":50,\"totalCount\":120}}", Encoding.UTF8, "application/json"),
            });
        }));

        var sut = new AsmrApiClient(
            factory,
            new StubApiEndpointUrlService("https://api.example.com"),
            new StubAuthService(new ApiToken { AccessToken = "jwt-token" }));

        var result = await sut.GetMetadataWorksAsync(page: 2, pageSize: 50, subtitleOnly: true);

        Assert.NotNull(capturedRequest?.RequestUri);
        Assert.Equal("/api/works?order=release&sort=desc&page=2&pageSize=50&subtitle=1", capturedRequest!.RequestUri!.PathAndQuery);
        Assert.Equal(120, result.Pagination.TotalCount);
        Assert.Equal("RJ501", Assert.Single(result.Works).SourceId);
    }

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
    public async Task AsmrApiClient_ShouldDeserializeTranslationMetadata_OnWorkInfoResponse()
    {
        var factory = new RecordingHttpClientFactory();
        factory.Register("AsmrApi", new RecordingHttpMessageHandler(_ =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"id\":123,\"title\":\"Sample\",\"release\":\"2026-03-15\",\"has_subtitle\":true,\"source_id\":\"RJ7301\",\"work_attributes\":\"RG01020616,JPN,DLP\",\"other_language_editions_in_db\":[{\"id\":1,\"lang\":\"简体中文\",\"title\":\"中文版\",\"source_id\":\"RJ7302\",\"is_original\":false,\"source_type\":\"DLSITE\"}],\"translation_info\":{\"lang\":null,\"is_original\":true},\"language_editions\":[{\"lang\":\"JPN\",\"label\":\"日本語\",\"workno\":\"RJ7301\",\"display_order\":1}]}", Encoding.UTF8, "application/json"),
            })));

        var sut = new AsmrApiClient(
            factory,
            new StubApiEndpointUrlService("https://api.example.com"),
            new StubAuthService(new ApiToken { AccessToken = "jwt-token" }));

        var result = await sut.GetWorkInfoAsync("RJ7301");

        Assert.Equal("RG01020616,JPN,DLP", result.WorkAttributes);
        var otherEdition = Assert.Single(result.OtherLanguageEditionsInDb);
        Assert.Equal("RJ7302", otherEdition.SourceId);
        Assert.Equal("简体中文", otherEdition.Lang);
        Assert.True(result.TranslationInfo.IsOriginal);
        var languageEdition = Assert.Single(result.LanguageEditions);
        Assert.Equal("JPN", languageEdition.Lang);
    }

    [Fact]
    public async Task AsmrApiClient_ShouldResolveNonNumericSourceId_ToNumericWorkEndpointPath()
    {
        var requestedPaths = new List<string>();
        var factory = new RecordingHttpClientFactory();
        factory.Register("AsmrApi", new RecordingHttpMessageHandler(request =>
        {
            requestedPaths.Add(request.RequestUri!.PathAndQuery);

            if (request.RequestUri.AbsolutePath.StartsWith("/api/search/", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"works\":[{\"id\":100000062,\"title\":\"BJ\",\"source_id\":\"BJ02370869\"}],\"pagination\":{\"currentPage\":1,\"pageSize\":20,\"totalCount\":1}}", Encoding.UTF8, "application/json"),
                });
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"id\":100000062,\"title\":\"BJ\",\"release\":\"2026-04-01\",\"has_subtitle\":false,\"source_id\":\"BJ02370869\"}", Encoding.UTF8, "application/json"),
            });
        }));

        var sut = new AsmrApiClient(
            factory,
            new StubApiEndpointUrlService("https://api.example.com"),
            new StubAuthService(new ApiToken { AccessToken = "jwt-token" }));

        var result = await sut.GetWorkInfoAsync("BJ02370869");

        Assert.Equal("BJ02370869", result.SourceId);
        Assert.Equal("/api/search/BJ02370869?order=id&sort=desc&page=1&pageSize=20&subtitle=0&includeTranslationWorks=true", requestedPaths[0]);
        Assert.Equal("/api/work/100000062", requestedPaths[1]);
    }

    [Fact]
    public async Task AsmrApiClient_GetTracksAsync_ShouldResolveNonNumericSourceId_ToNumericTracksEndpointPath()
    {
        var requestedPaths = new List<string>();
        var factory = new RecordingHttpClientFactory();
        factory.Register("AsmrApi", new RecordingHttpMessageHandler(request =>
        {
            requestedPaths.Add(request.RequestUri!.PathAndQuery);

            if (request.RequestUri.AbsolutePath.StartsWith("/api/search/", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"works\":[{\"id\":100000062,\"title\":\"BJ\",\"source_id\":\"BJ02370869\"}],\"pagination\":{\"currentPage\":1,\"pageSize\":20,\"totalCount\":1}}", Encoding.UTF8, "application/json"),
                });
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[]", Encoding.UTF8, "application/json"),
            });
        }));

        var sut = new AsmrApiClient(
            factory,
            new StubApiEndpointUrlService("https://api.example.com"),
            new StubAuthService(new ApiToken { AccessToken = "jwt-token" }));

        var result = await sut.GetTracksAsync("BJ02370869");

        Assert.Empty(result);
        Assert.Equal("/api/search/BJ02370869?order=id&sort=desc&page=1&pageSize=20&subtitle=0&includeTranslationWorks=true", requestedPaths[0]);
        Assert.Equal("/api/tracks/100000062", requestedPaths[1]);
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
