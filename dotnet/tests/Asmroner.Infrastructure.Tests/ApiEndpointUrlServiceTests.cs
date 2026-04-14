using Asmroner.Core.Api;
using Asmroner.Core.Configuration;
using Asmroner.Core.Interfaces;
using Asmroner.Infrastructure.Services;


namespace Asmroner.Infrastructure.Tests;

public class ApiEndpointUrlServiceTests
{
    [Fact]
    public async Task DiscoverAndPersistAsync_ShouldUpdateApiUrl_AndCandidateUrls_WhenConfigExists()
    {
        var configService = new RecordingConfigurationService(new AppConfig
        {
            Downloader = new DownloaderOptions
            {
                ApiUrl = "https://api.asmr-300.com",
                ApiCandidateUrls = "https://api.asmr-300.com;https://api.asmr.one",
            },
        });

        var sut = new ApiEndpointUrlService(
            configService,
            new StubInfrastructureEndpointDiscoveryService(
                "https://api.asmr-200.com",
                new[]
                {
                    "https://api.asmr-300.com",
                    "https://api.asmr-200.com",
                    "https://api.asmr-100.com",
                    "https://api.asmr.one",
                }));

        var result = await sut.DiscoverAndPersistAsync();

        Assert.Equal("https://api.asmr-200.com", result.BaseUrl);
        Assert.NotNull(configService.CurrentConfig);
        Assert.Equal("https://api.asmr-200.com", configService.CurrentConfig!.Downloader.ApiUrl);
        Assert.Equal(
            "https://api.asmr-200.com;https://api.asmr-300.com;https://api.asmr-100.com;https://api.asmr.one",
            configService.CurrentConfig.Downloader.ApiCandidateUrls);
        Assert.Equal(1, configService.SaveCallCount);
    }

    [Fact]
    public async Task DiscoverAndPersistAsync_ShouldMergeDiscoveredAndSavedCandidates_WhenMergedCountIncreases()
    {
        var configService = new RecordingConfigurationService(new AppConfig
        {
            Downloader = new DownloaderOptions
            {
                ApiUrl = "https://api.asmr-300.com",
                ApiCandidateUrls = "https://api.asmr-300.com;https://api.asmr.one",
            },
        });

        var sut = new ApiEndpointUrlService(
            configService,
            new StubInfrastructureEndpointDiscoveryService(
                "https://api.asmr-200.com",
                new[]
                {
                    "https://api.asmr-200.com",
                    "https://api.asmr-300.com",
                }));

        var result = await sut.DiscoverAndPersistAsync();

        Assert.Equal("https://api.asmr-200.com", result.BaseUrl);
        Assert.NotNull(configService.CurrentConfig);
        Assert.Equal(
            "https://api.asmr-200.com;https://api.asmr-300.com;https://api.asmr.one",
            configService.CurrentConfig!.Downloader.ApiCandidateUrls);
        Assert.Equal(1, configService.SaveCallCount);
    }

    [Fact]
    public async Task DiscoverAndPersistAsync_ShouldSkipSave_WhenConfigMissing()
    {
        var configService = new RecordingConfigurationService(initialConfig: null);
        var sut = new ApiEndpointUrlService(
            configService,
            new StubInfrastructureEndpointDiscoveryService("https://new.example.com"));

        var result = await sut.DiscoverAndPersistAsync();

        Assert.Equal("https://new.example.com", result.BaseUrl);
        Assert.Equal(0, configService.SaveCallCount);
    }

    [Fact]
    public async Task GetCurrentBaseUrlAsync_ShouldReturnDefault_WhenConfigMissingOrEmpty()
    {
        var sut = new ApiEndpointUrlService(
            new RecordingConfigurationService(new AppConfig
            {
                Downloader = new DownloaderOptions
                {
                    ApiUrl = " ",
                },
            }),
            new StubInfrastructureEndpointDiscoveryService("https://unused.example.com"));

        var baseUrl = await sut.GetCurrentBaseUrlAsync();

        Assert.Equal("https://api.asmr-300.com", baseUrl);
    }

    [Fact]
    public async Task DiscoverAndPersistAsync_ShouldKeepCustomCandidateList_WhenDiscoveryAddsPublicCandidates()
    {
        var configService = new RecordingConfigurationService(new AppConfig
        {
            Downloader = new DownloaderOptions
            {
                ApiUrl = "https://custom-a.example.com",
                ApiCandidateUrls = "https://custom-a.example.com;https://custom-b.example.com",
            },
        });

        var sut = new ApiEndpointUrlService(
            configService,
            new StubInfrastructureEndpointDiscoveryService(
                "https://custom-b.example.com",
                new[]
                {
                    "https://api.asmr-300.com",
                    "https://api.asmr-200.com",
                    "https://custom-b.example.com",
                    "https://custom-a.example.com",
                }));

        var result = await sut.DiscoverAndPersistAsync();

        Assert.Equal("https://custom-b.example.com", result.BaseUrl);
        Assert.NotNull(configService.CurrentConfig);
        Assert.Equal(
            "https://custom-b.example.com;https://custom-a.example.com",
            configService.CurrentConfig!.Downloader.ApiCandidateUrls);
        Assert.Equal(1, configService.SaveCallCount);
    }

    [Fact]
    public async Task DiscoverAndPersistAsync_ShouldSkipSave_WhenBaseUrlAndCandidatesAreUnchanged()
    {
        var configService = new RecordingConfigurationService(new AppConfig
        {
            Downloader = new DownloaderOptions
            {
                ApiUrl = "https://api.asmr-300.com",
                ApiCandidateUrls = "https://api.asmr-300.com;https://api.asmr-200.com",
            },
        });

        var sut = new ApiEndpointUrlService(
            configService,
            new StubInfrastructureEndpointDiscoveryService(
                "https://api.asmr-300.com",
                new[]
                {
                    "https://api.asmr-300.com",
                    "https://api.asmr-200.com",
                }));

        var result = await sut.DiscoverAndPersistAsync();

        Assert.Equal("https://api.asmr-300.com", result.BaseUrl);
        Assert.Equal(0, configService.SaveCallCount);
    }

    [Fact]
    public async Task DiscoverAndPersistAsync_ShouldKeepSavedCandidateList_WhenMergedCountDoesNotIncrease()
    {
        var configService = new RecordingConfigurationService(new AppConfig
        {
            Downloader = new DownloaderOptions
            {
                ApiUrl = "https://api.asmr-300.com",
                ApiCandidateUrls = "https://api.asmr-300.com;https://api.asmr-200.com;https://api.asmr.one",
            },
        });

        var sut = new ApiEndpointUrlService(
            configService,
            new StubInfrastructureEndpointDiscoveryService(
                "https://api.asmr-300.com",
                new[]
                {
                    "https://api.asmr-300.com",
                    "https://api.asmr-200.com",
                }));

        var result = await sut.DiscoverAndPersistAsync();

        Assert.Equal("https://api.asmr-300.com", result.BaseUrl);
        Assert.Equal(
            new[]
            {
                "https://api.asmr-300.com",
                "https://api.asmr-200.com",
                "https://api.asmr.one",
            },
            result.Candidates);
        Assert.Equal(0, configService.SaveCallCount);
    }

    private sealed class RecordingConfigurationService : IConfigurationService
    {
        public AppConfig? CurrentConfig { get; private set; }

        public int SaveCallCount { get; private set; }

        public RecordingConfigurationService(AppConfig? initialConfig)
        {
            CurrentConfig = initialConfig;
        }

        public bool Exists() => CurrentConfig is not null;

        public Task<AppConfig?> LoadAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(CurrentConfig);

        public Task SaveAsync(AppConfig config, CancellationToken cancellationToken = default)
        {
            SaveCallCount++;
            CurrentConfig = config;
            return Task.CompletedTask;
        }

        public IReadOnlyList<string> Validate(AppConfig config)
            => Array.Empty<string>();
    }
}
