using Asmroner.Core.Configuration;
using Asmroner.Infrastructure.Services;

namespace Asmroner.Infrastructure.Tests;

public class AsmrApiOptionsProviderTests
{
    [Fact]
    public async Task AsmrApiOptionsProvider_ShouldParseConfiguredUrlLists()
    {
        var sut = new AsmrApiOptionsProvider(new StubInfrastructureConfigurationService(new AppConfig
        {
            Downloader = new DownloaderOptions
            {
                ApiUrl = "https://custom-api.example.com/",
                ApiCandidateUrls = "https://api-a.example.com;https://api-b.example.com,https://api-a.example.com",
                PublishSourceUrls = "https://publish-a.example.com; https://publish-b.example.com",
            },
        }));

        var options = await sut.GetOptionsAsync();

        Assert.Equal("https://custom-api.example.com", options.BaseUrl);
        Assert.Equal(
            new[]
            {
                "https://custom-api.example.com",
                "https://api-a.example.com",
                "https://api-b.example.com",
            },
            options.CandidateBaseUrls);
        Assert.Equal(
            new[]
            {
                "https://publish-a.example.com",
                "https://publish-b.example.com",
            },
            options.PublishSourceUrls);
    }

    [Fact]
    public async Task AsmrApiOptionsProvider_ShouldFallbackToDefaults_WhenConfiguredUrlsMissing()
    {
        var sut = new AsmrApiOptionsProvider(new StubInfrastructureConfigurationService(new AppConfig
        {
            Downloader = new DownloaderOptions
            {
                ApiUrl = " ",
                ApiCandidateUrls = " ; , ",
                PublishSourceUrls = string.Empty,
            },
        }));

        var options = await sut.GetOptionsAsync();

        Assert.Equal("https://api.asmr-300.com", options.BaseUrl);
        Assert.Equal(
            new[]
            {
                "https://api.asmr-300.com",
                "https://api.asmr-200.com",
                "https://api.asmr-100.com",
                "https://api.asmr.one",
            },
            options.CandidateBaseUrls);
        Assert.Equal(
            new[]
            {
                "https://as.mr",
                "https://as.131433.xyz",
            },
            options.PublishSourceUrls);
    }

    [Fact]
    public async Task AsmrApiOptionsProvider_ShouldExpandLegacyBuiltInCandidateSubset()
    {
        var sut = new AsmrApiOptionsProvider(new StubInfrastructureConfigurationService(new AppConfig
        {
            Downloader = new DownloaderOptions
            {
                ApiUrl = "https://api.asmr-300.com",
                ApiCandidateUrls = "https://api.asmr-300.com;https://api.asmr.one",
            },
        }));

        var options = await sut.GetOptionsAsync();

        Assert.Equal(
            new[]
            {
                "https://api.asmr-300.com",
                "https://api.asmr-200.com",
                "https://api.asmr-100.com",
                "https://api.asmr.one",
            },
            options.CandidateBaseUrls);
    }

    [Fact]
    public async Task AsmrApiOptionsProvider_ShouldIgnoreInvalidUrls_InConfiguredLists()
    {
        var sut = new AsmrApiOptionsProvider(new StubInfrastructureConfigurationService(new AppConfig
        {
            Downloader = new DownloaderOptions
            {
                ApiUrl = "https://api-custom.example.com",
                ApiCandidateUrls = "invalid-value;https://api-ok.example.com",
                PublishSourceUrls = "not-a-url,https://publish-ok.example.com",
            },
        }));

        var options = await sut.GetOptionsAsync();

        Assert.Equal(
            new[]
            {
                "https://api-custom.example.com",
                "https://api-ok.example.com",
            },
            options.CandidateBaseUrls);
        Assert.Equal(
            new[]
            {
                "https://publish-ok.example.com",
            },
            options.PublishSourceUrls);
    }
}
