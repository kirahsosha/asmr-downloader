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
    public async Task EndpointDiscoveryService_ShouldProbeCandidates_UsingHealthEndpoint()
    {
        var requestedPaths = new List<string>();
        var factory = new RecordingHttpClientFactory();
        factory.Register("AsmrProbe", new RecordingHttpMessageHandler(request =>
        {
            var uri = request.RequestUri!;
            requestedPaths.Add(uri.PathAndQuery);

            if (uri.Host.Equals("healthy.example.com", StringComparison.OrdinalIgnoreCase)
                && uri.PathAndQuery == AsmrApiPaths.Health)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }));

        var sut = new EndpointDiscoveryService(
            factory,
            new StubInfrastructureOptionsProvider(new AsmrApiOptions
            {
                BaseUrl = "https://fallback.example.com",
                CandidateBaseUrls = new[]
                {
                    "https://fallback.example.com",
                    "https://healthy.example.com",
                },
            }));

        var result = await sut.DiscoverAsync();

        Assert.Equal("https://healthy.example.com", result.BaseUrl);
        Assert.Contains(AsmrApiPaths.Health, requestedPaths);
        Assert.DoesNotContain(AsmrApiPaths.Popular, requestedPaths);
        Assert.DoesNotContain(AsmrApiPaths.Works, requestedPaths);
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
                && uri.PathAndQuery == AsmrApiPaths.Health)
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

    [Fact]
    public async Task EndpointDiscoveryService_ShouldExtractPublishedCandidatesFromHtmlText_AndSkipEntryScriptFetch()
    {
        var requestedPaths = new List<string>();
        var factory = new RecordingHttpClientFactory();
        factory.Register("AsmrProbe", new RecordingHttpMessageHandler(request =>
        {
            var uri = request.RequestUri!;
            requestedPaths.Add(uri.AbsolutePath);

            if (uri.Host.Equals("publish.example.com", StringComparison.OrdinalIgnoreCase)
                && uri.AbsolutePath == "/")
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("ASMR Online 最新域名asmr-300.com 随缘墙224 ms\nasmr-200.com 随缘墙205 ms\nasmr-100.com 国内墙连接失败\nasmr.one 国内墙连接失败"),
                });
            }

            if (uri.Host.Equals("api.asmr-200.com", StringComparison.OrdinalIgnoreCase)
                && uri.PathAndQuery == AsmrApiPaths.Health)
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
                CandidateBaseUrls = Array.Empty<string>(),
                PublishSourceUrls = new[]
                {
                    "https://publish.example.com",
                },
            }));

        var result = await sut.DiscoverAsync();

        Assert.Equal("https://api.asmr-200.com", result.BaseUrl);
        Assert.Equal(
            new[]
            {
                "https://api.asmr-300.com",
                "https://api.asmr-200.com",
                "https://api.asmr-100.com",
                "https://api.asmr.one",
            },
            result.Candidates);
        Assert.DoesNotContain("/assets/index.abc123.js", requestedPaths);
    }

    [Fact]
    public async Task EndpointDiscoveryService_ShouldFallbackToConfiguredCandidate_WhenPublishAssetRequestFails()
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
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            if (uri.Host.Equals("configured.example.com", StringComparison.OrdinalIgnoreCase)
                && uri.PathAndQuery == AsmrApiPaths.Health)
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

        Assert.Equal("https://configured.example.com", result.BaseUrl);
    }

    [Fact]
    public async Task EndpointDiscoveryService_ShouldParseScriptTag_WhenAttributesUseDifferentOrder()
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
                    Content = new StringContent("<html><script crossorigin data-entry=\"main\" src=\"/assets/index.abc123.js\" type=\"module\"></script></html>"),
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
                && uri.PathAndQuery == AsmrApiPaths.Health)
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
                CandidateBaseUrls = Array.Empty<string>(),
                PublishSourceUrls = new[]
                {
                    "https://publish.example.com",
                },
            }));

        var result = await sut.DiscoverAsync();

        Assert.Equal("https://api.mirror.example.com", result.BaseUrl);
    }

    [Fact]
    public async Task EndpointDiscoveryService_ShouldParseRelativeEntryScript_WithQuerySuffix_AndSingleQuotedLink()
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
                    Content = new StringContent("<html><script defer src='./assets/index-AbC123.js?v=1' data-entry='main'></script></html>"),
                });
            }

            if (uri.Host.Equals("publish.example.com", StringComparison.OrdinalIgnoreCase)
                && uri.AbsolutePath == "/assets/index-AbC123.js")
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("const cfg={link:'https://mirror.example.com'};"),
                });
            }

            if (uri.Host.Equals("api.mirror.example.com", StringComparison.OrdinalIgnoreCase)
                && uri.PathAndQuery == AsmrApiPaths.Health)
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
                CandidateBaseUrls = Array.Empty<string>(),
                PublishSourceUrls = new[]
                {
                    "https://publish.example.com",
                },
            }));

        var result = await sut.DiscoverAsync();

        Assert.Equal("https://api.mirror.example.com", result.BaseUrl);
    }

    [Fact]
    public async Task EndpointDiscoveryService_ShouldIgnorePublishSourceTimeoutAndHttpFailures_AndFallbackToConfiguredCandidate()
    {
        var factory = new RecordingHttpClientFactory();
        factory.Register("AsmrProbe", new RecordingHttpMessageHandler(request =>
        {
            var uri = request.RequestUri!;

            if (uri.Host.Equals("publish-timeout.example.com", StringComparison.OrdinalIgnoreCase)
                && uri.AbsolutePath == "/")
            {
                throw new OperationCanceledException("publish source timeout");
            }

            if (uri.Host.Equals("publish-error.example.com", StringComparison.OrdinalIgnoreCase)
                && uri.AbsolutePath == "/")
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("<html><script src='/assets/index.abc123.js'></script></html>"),
                });
            }

            if (uri.Host.Equals("publish-error.example.com", StringComparison.OrdinalIgnoreCase)
                && uri.AbsolutePath == "/assets/index.abc123.js")
            {
                throw new HttpRequestException("The SSL connection could not be established, see inner exception.");
            }

            if (uri.Host.Equals("configured.example.com", StringComparison.OrdinalIgnoreCase)
                && uri.PathAndQuery == AsmrApiPaths.Health)
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
                    "https://publish-timeout.example.com",
                    "https://publish-error.example.com",
                },
            }));

        var result = await sut.DiscoverAsync();

        Assert.Equal("https://configured.example.com", result.BaseUrl);
    }

    [Fact]
    public async Task EndpointDiscoveryService_ShouldSendProbeUserAgent_OnPublishAndHealthRequests()
    {
        var observedUserAgents = new List<string>();
        var factory = new RecordingHttpClientFactory();
        factory.Register(EndpointDiscoveryHttpTransport.ProbeClientName, new RecordingHttpMessageHandler(request =>
        {
            observedUserAgents.Add(request.Headers.UserAgent.ToString());

            var uri = request.RequestUri!;
            if (uri.Host.Equals("publish.example.com", StringComparison.OrdinalIgnoreCase)
                && uri.AbsolutePath == "/")
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("asmr-200.com"),
                });
            }

            if (uri.Host.Equals("api.asmr-200.com", StringComparison.OrdinalIgnoreCase)
                && uri.PathAndQuery == AsmrApiPaths.Health)
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
                CandidateBaseUrls = Array.Empty<string>(),
                PublishSourceUrls = new[]
                {
                    "https://publish.example.com",
                },
            }));

        _ = await sut.DiscoverAsync();

        Assert.True(observedUserAgents.Count >= 2);
        Assert.All(observedUserAgents, userAgent => Assert.Equal(EndpointDiscoveryHttpTransport.ProbeUserAgent, userAgent));
    }
}
