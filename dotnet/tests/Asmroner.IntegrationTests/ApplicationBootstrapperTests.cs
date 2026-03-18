using Asmroner.Application.Services;
using Asmroner.Core.Configuration;
using Asmroner.Core.Interfaces;
using Asmroner.Infrastructure.Services;

namespace Asmroner.IntegrationTests;

public class ApplicationBootstrapperTests
{
    [Fact]
    public async Task Bootstrapper_ShouldFailGracefully_WhenDatabaseInitThrows()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var pathService = new AppPathService(tempRoot);
            var sut = new ApplicationBootstrapper(
                pathService,
                new StubConfigurationService(new AppConfig
                {
                    User = new UserOptions
                    {
                        Account = "tester",
                        Password = "secret",
                    },
                    Downloader = new DownloaderOptions
                    {
                        SyncDataFolder = Path.Combine(tempRoot, "sync-data"),
                    },
                    Limit = new LimitOptions
                    {
                        SyncQps = 1,
                        DownloadQps = 1,
                    },
                }),
                new ThrowingDatabaseInitializer("mock database init failure"),
                new FirstRunService());

            var result = await sut.InitializeAsync();

            Assert.False(result.IsSuccess);
            Assert.False(result.RequiresSetup);
            Assert.NotNull(result.ErrorMessage);
            Assert.Contains("初始化失败", result.ErrorMessage);
            Assert.Contains("mock database init failure", result.ErrorMessage);
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    [Fact]
    public async Task Bootstrapper_ShouldRequireSetup_WhenConfigMissing()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var pathService = new AppPathService(tempRoot);
            var configService = new ConfigurationService(pathService);
            var databaseInitializer = new DatabaseInitializer(pathService);
            var firstRunService = new FirstRunService();
            var sut = new ApplicationBootstrapper(pathService, configService, databaseInitializer, firstRunService);

            var result = await sut.InitializeAsync();

            Assert.True(result.RequiresSetup);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Config);
            Assert.True(File.Exists(pathService.DatabaseFilePath));
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    [Fact]
    public async Task Bootstrapper_ShouldSucceed_WhenConfigValid()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var pathService = new AppPathService(tempRoot);
            var configService = new ConfigurationService(pathService);
            var databaseInitializer = new DatabaseInitializer(pathService);
            var firstRunService = new FirstRunService();
            var sut = new ApplicationBootstrapper(pathService, configService, databaseInitializer, firstRunService);

            var syncFolder = Path.Combine(tempRoot, "data", "sync");
            await configService.SaveAsync(new AppConfig
            {
                User = new UserOptions
                {
                    Account = "tester",
                    Password = "password",
                },
                Downloader = new DownloaderOptions
                {
                    SyncDataFolder = syncFolder,
                    MaxWorkers = 6,
                    MaxRetries = 2,
                },
                Limit = new LimitOptions
                {
                    SyncQps = 4,
                    DownloadQps = 2,
                },
            });

            var result = await sut.InitializeAsync();

            Assert.True(result.IsSuccess);
            Assert.False(result.RequiresSetup);
            Assert.True(Directory.Exists(syncFolder));
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    private static string CreateTempRoot()
    {
        var path = Path.Combine(Path.GetTempPath(), "asmroner-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }

    private static void CleanupTempRoot(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }

    private sealed class ThrowingDatabaseInitializer : IDatabaseInitializer
    {
        private readonly string _message;

        public ThrowingDatabaseInitializer(string message)
        {
            _message = message;
        }

        public Task InitializeAsync(CancellationToken cancellationToken = default)
            => throw new InvalidOperationException(_message);

        public Task<bool> CanConnectAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(false);
    }

    private sealed class StubConfigurationService : IConfigurationService
    {
        private readonly AppConfig _config;

        public StubConfigurationService(AppConfig config)
        {
            _config = config;
        }

        public bool Exists() => true;

        public Task<AppConfig?> LoadAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<AppConfig?>(_config);

        public Task SaveAsync(AppConfig config, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public IReadOnlyList<string> Validate(AppConfig config)
            => Array.Empty<string>();
    }
}

