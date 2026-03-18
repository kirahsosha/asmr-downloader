using System.Net;
using Asmroner.Core.Api;
using Asmroner.Infrastructure.Services;

namespace Asmroner.Infrastructure.Tests;

public class EndpointDiscoveryServiceTests
{
    [Fact]
    public async Task EndpointDiscoveryService_ShouldPickFastestReachableCandidate()
    {
        var factory = new RecordingHttpClientFactory();
        factory.Register("AsmrProbe", new RecordingHttpMessageHandler(request =>
        {
            var host = request.RequestUri!.GetLeftPart(UriPartial.Authority);
            return host.Contains("fast", StringComparison.OrdinalIgnoreCase)
                ? Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK))
                : Task.FromResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
        }));

        var sut = new EndpointDiscoveryService(
            factory,
            new StubInfrastructureOptionsProvider(new AsmrApiOptions
            {
                BaseUrl = "https://slow.example.com",
                CandidateBaseUrls = new[]
                {
                    "https://slow.example.com",
                    "https://fast.example.com",
                },
            }));

        var result = await sut.DiscoverAsync();

        Assert.Equal("https://fast.example.com", result.BaseUrl);
    }

    [Fact]
    public async Task EndpointDiscoveryService_ShouldFallbackToConfiguredBaseUrl_WhenAllCandidatesFail()
    {
        var factory = new RecordingHttpClientFactory();
        factory.Register("AsmrProbe", new RecordingHttpMessageHandler(_ =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable))));

        var sut = new EndpointDiscoveryService(
            factory,
            new StubInfrastructureOptionsProvider(new AsmrApiOptions
            {
                BaseUrl = "https://configured.example.com",
                CandidateBaseUrls = new[]
                {
                    "https://first.example.com",
                    "https://second.example.com",
                },
            }));

        var result = await sut.DiscoverAsync();

        Assert.Equal("https://configured.example.com", result.BaseUrl);
        Assert.Equal(-1, result.LatencyMs);
    }

    [Fact]
    public async Task EndpointDiscoveryService_ShouldUseConfiguredPublishSources_ForDynamicCandidates()
    {
        var factory = new RecordingHttpClientFactory();
        factory.Register("AsmrProbe", new RecordingHttpMessageHandler(request =>
        {
            var uri = request.RequestUri!;

            if (uri.Host.Equals("publish.example.com", StringComparison.OrdinalIgnoreCase)
                && uri.AbsolutePath == "/")
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("<html><script type=\"module\" crossorigin src=\"/assets/index.abc123.js\"></script></html>"),
                });
            }

            if (uri.Host.Equals("publish.example.com", StringComparison.OrdinalIgnoreCase)
                && uri.AbsolutePath == "/assets/index.abc123.js")
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("const cfg={link:\"https://mirror.example.com\"};"),
                });
            }

            if (uri.Host.Equals("api.mirror.example.com", StringComparison.OrdinalIgnoreCase)
                && uri.AbsolutePath == AsmrApiPaths.Popular)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
        }));

        var sut = new EndpointDiscoveryService(
            factory,
            new StubInfrastructureOptionsProvider(new AsmrApiOptions
            {
                BaseUrl = "https://configured.example.com",
                CandidateBaseUrls = new[]
                {
                    "https://configured.example.com",
                },
                PublishSourceUrls = new[]
                {
                    "https://publish.example.com",
                },
            }));

        var result = await sut.DiscoverAsync();

        Assert.Equal("https://api.mirror.example.com", result.BaseUrl);
    }
}
