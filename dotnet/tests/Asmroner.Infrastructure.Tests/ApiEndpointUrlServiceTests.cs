using Asmroner.Core.Api;
using Asmroner.Core.Configuration;
using Asmroner.Core.Interfaces;
using Asmroner.Infrastructure.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace Asmroner.Infrastructure.Tests;

public class ApiEndpointUrlServiceTests
{
    [Fact]
    public async Task DiscoverAndPersistAsync_ShouldUpdateApiUrl_WhenConfigExists()
    {
        var configService = new RecordingConfigurationService(new AppConfig
        {
            Downloader = new DownloaderOptions
            {
                ApiUrl = "https://old.example.com",
            },
        });

        var sut = new ApiEndpointUrlService(
            configService,
            new StubInfrastructureEndpointDiscoveryService("https://new.example.com"),
            NullLogger<ApiEndpointUrlService>.Instance);

        var result = await sut.DiscoverAndPersistAsync();

        Assert.Equal("https://new.example.com", result.BaseUrl);
        Assert.NotNull(configService.CurrentConfig);
        Assert.Equal("https://new.example.com", configService.CurrentConfig!.Downloader.ApiUrl);
        Assert.Equal(1, configService.SaveCallCount);
    }

    [Fact]
    public async Task DiscoverAndPersistAsync_ShouldSkipSave_WhenConfigMissing()
    {
        var configService = new RecordingConfigurationService(initialConfig: null);
        var sut = new ApiEndpointUrlService(
            configService,
            new StubInfrastructureEndpointDiscoveryService("https://new.example.com"),
            NullLogger<ApiEndpointUrlService>.Instance);

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
            new StubInfrastructureEndpointDiscoveryService("https://unused.example.com"),
            NullLogger<ApiEndpointUrlService>.Instance);

        var baseUrl = await sut.GetCurrentBaseUrlAsync();

        Assert.Equal("https://api.asmr-300.com", baseUrl);
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
