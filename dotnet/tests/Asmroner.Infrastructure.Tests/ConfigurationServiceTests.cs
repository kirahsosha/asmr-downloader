using System.Text.Json;
using Asmroner.Core.Configuration;
using Asmroner.Infrastructure.Services;
using Microsoft.Data.Sqlite;

namespace Asmroner.Infrastructure.Tests;

public class ConfigurationServiceTests
{
    [Fact]
    public void ConfigurationService_ShouldReturnValidationErrors_WhenRequiredFieldsMissing()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), $"asmroner-batch-config-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempRoot);

        var sut = new ConfigurationService(new AppPathService(tempRoot));
        var config = new AppConfig
        {
            User = new UserOptions
            {
                Account = string.Empty,
                Password = string.Empty,
            },
            Downloader = new DownloaderOptions
            {
                SyncDataFolder = string.Empty,
                MaxWorkers = 0,
                MaxRetries = -1,
            },
            Limit = new LimitOptions
            {
                SyncQps = 0,
                DownloadQps = 0,
                SyncJitterMin = 20,
                SyncJitterMax = 10,
                DownloadJitterMin = 20,
                DownloadJitterMax = 10,
            },
        };

        var errors = sut.Validate(config);

        Assert.Contains("账号不能为空。", errors);
        Assert.Contains("密码不能为空。", errors);
        Assert.Contains("并发工作数必须大于 0。", errors);
        Assert.Contains("重试次数不能小于 0。", errors);
        Assert.Contains("同步目录不能为空。", errors);
        Assert.Contains("QPS 必须大于 0。", errors);
        Assert.Contains("同步抖动最小值不能大于最大值。", errors);
        Assert.Contains("下载抖动最小值不能大于最大值。", errors);
    }

    [Fact]
    public async Task ConfigurationService_ShouldSaveAndLoadConfig_FromSplitSqliteSections()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var pathService = new AppPathService(tempRoot);
            var sut = new ConfigurationService(pathService);
            var config = new AppConfig
            {
                User = new UserOptions
                {
                    Account = "tester",
                    Password = "secret",
                },
                Downloader = new DownloaderOptions
                {
                    ApiUrl = "https://api-custom.example.com",
                    ApiCandidateUrls = "https://api-custom.example.com;https://api-fallback.example.com",
                    PublishSourceUrls = "https://publish-a.example.com;https://publish-b.example.com",
                    WorkPageUrlTemplate = "https://example.com/work/{RJID}",
                    SyncDataFolder = Path.Combine(tempRoot, "sync"),
                    MaxWorkers = 8,
                    MaxRetries = 5,
                    PreferFormats = "mp3,m4a",
                    HdAudioOnly = false,
                },
                Limit = new LimitOptions
                {
                    SyncQps = 6,
                    DownloadQps = 4,
                },
            };

            await sut.SaveAsync(config);
            var loaded = await sut.LoadAsync();

            Assert.NotNull(loaded);
            Assert.Equal("tester", loaded!.User.Account);
            Assert.Equal("https://api-custom.example.com", loaded.Downloader.ApiUrl);
            Assert.Equal("https://api-custom.example.com;https://api-fallback.example.com", loaded.Downloader.ApiCandidateUrls);
            Assert.Equal("https://publish-a.example.com;https://publish-b.example.com", loaded.Downloader.PublishSourceUrls);
            Assert.Equal("https://example.com/work/{RJID}", loaded.Downloader.WorkPageUrlTemplate);
            Assert.Equal(8, loaded.Downloader.MaxWorkers);
            Assert.Equal(5, loaded.Downloader.MaxRetries);
            Assert.Equal("mp3,m4a", loaded.Downloader.PreferFormats);
            Assert.False(loaded.Downloader.HdAudioOnly);

            var sectionCount = await CountSplitConfigSectionsAsync(pathService.DatabaseFilePath);
            Assert.Equal(3, sectionCount);
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    [Fact]
    public async Task ConfigurationService_ShouldLoadFromDefaultConfigJson_WhenSqliteMissing()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var pathService = new AppPathService(tempRoot);
            var defaults = new AppConfig
            {
                User = new UserOptions
                {
                    Account = "default-user",
                    Password = "default-pass",
                },
                Downloader = new DownloaderOptions
                {
                    ApiUrl = "https://defaults.example.com",
                    SyncDataFolder = Path.Combine(tempRoot, "sync-default"),
                    MaxWorkers = 7,
                    MaxRetries = 1,
                    HdAudioOnly = false,
                },
                Limit = new LimitOptions
                {
                    SyncQps = 4,
                    DownloadQps = 2,
                },
            };

            var json = JsonSerializer.Serialize(defaults, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            await File.WriteAllTextAsync(pathService.DefaultConfigFilePath, json);

            var sut = new ConfigurationService(pathService);
            var loaded = await sut.LoadAsync();

            Assert.NotNull(loaded);
            Assert.Equal("default-user", loaded!.User.Account);
            Assert.Equal("https://defaults.example.com", loaded.Downloader.ApiUrl);
            Assert.Equal(7, loaded.Downloader.MaxWorkers);
            Assert.False(loaded.Downloader.HdAudioOnly);
            Assert.Equal(4, loaded.Limit.SyncQps);
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    [Fact]
    public async Task ConfigurationService_ShouldPreferSqliteOverDefaultConfigJson()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var pathService = new AppPathService(tempRoot);
            var defaultConfig = new AppConfig
            {
                User = new UserOptions
                {
                    Account = "default-account",
                    Password = "default-password",
                },
                Downloader = new DownloaderOptions
                {
                    ApiUrl = "https://defaults.example.com",
                    SyncDataFolder = Path.Combine(tempRoot, "sync-default"),
                },
            };
            await File.WriteAllTextAsync(
                pathService.DefaultConfigFilePath,
                JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions(JsonSerializerDefaults.Web)));

            var sut = new ConfigurationService(pathService);
            await sut.SaveAsync(new AppConfig
            {
                User = new UserOptions
                {
                    Account = "sqlite-account",
                    Password = "sqlite-password",
                },
                Downloader = new DownloaderOptions
                {
                    ApiUrl = "https://sqlite.example.com",
                    SyncDataFolder = Path.Combine(tempRoot, "sync-sqlite"),
                    MaxWorkers = 9,
                },
                Limit = new LimitOptions
                {
                    SyncQps = 9,
                    DownloadQps = 5,
                },
            });

            var loaded = await sut.LoadAsync();

            Assert.NotNull(loaded);
            Assert.Equal("sqlite-account", loaded!.User.Account);
            Assert.Equal("https://sqlite.example.com", loaded.Downloader.ApiUrl);
            Assert.Equal(9, loaded.Downloader.MaxWorkers);
            Assert.Equal(9, loaded.Limit.SyncQps);
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    [Fact]
    public async Task ConfigurationService_ShouldMigrateLegacySingleRowAppConfig()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var pathService = new AppPathService(tempRoot);
            pathService.EnsureMetadataDirectory();

            var legacy = new AppConfig
            {
                User = new UserOptions
                {
                    Account = "legacy-user",
                    Password = "legacy-pass",
                },
                Downloader = new DownloaderOptions
                {
                    ApiUrl = "https://legacy.example.com",
                    SyncDataFolder = Path.Combine(tempRoot, "legacy-sync"),
                    PreferFormats = "wav,flac",
                    HdAudioOnly = false,
                },
                Limit = new LimitOptions
                {
                    SyncQps = 3,
                    DownloadQps = 2,
                },
            };

            await SeedLegacySingleRowAsync(pathService.DatabaseFilePath, legacy);

            var sut = new ConfigurationService(pathService);
            var loaded = await sut.LoadAsync();

            Assert.NotNull(loaded);
            Assert.Equal("legacy-user", loaded!.User.Account);
            Assert.Equal("https://legacy.example.com", loaded.Downloader.ApiUrl);
            Assert.Equal("wav,flac", loaded.Downloader.PreferFormats);

            Assert.True(await TableHasColumnAsync(pathService.DatabaseFilePath, "AppConfig", "ConfigKey"));
            Assert.False(await TableHasColumnAsync(pathService.DatabaseFilePath, "AppConfig", "Id"));
            Assert.Equal(3, await CountSplitConfigSectionsAsync(pathService.DatabaseFilePath));
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    private static async Task SeedLegacySingleRowAsync(string databaseFilePath, AppConfig config)
    {
        await using var connection = new SqliteConnection(
            new SqliteConnectionStringBuilder
            {
                DataSource = databaseFilePath,
                Mode = SqliteOpenMode.ReadWriteCreate,
                Pooling = false,
            }.ToString());

        await connection.OpenAsync();

        await using (var create = connection.CreateCommand())
        {
            create.CommandText =
                """
                CREATE TABLE IF NOT EXISTS AppConfig (
                    Id INTEGER PRIMARY KEY CHECK (Id = 1),
                    JsonValue TEXT NOT NULL,
                    UpdatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
                );
                """;
            await create.ExecuteNonQueryAsync();
        }

        await using (var insert = connection.CreateCommand())
        {
            insert.CommandText =
                """
                INSERT INTO AppConfig (Id, JsonValue, UpdatedAt)
                VALUES (1, @json, CURRENT_TIMESTAMP)
                ON CONFLICT(Id) DO UPDATE SET
                    JsonValue = excluded.JsonValue,
                    UpdatedAt = CURRENT_TIMESTAMP;
                """;
            insert.Parameters.AddWithValue("@json", JsonSerializer.Serialize(config, new JsonSerializerOptions(JsonSerializerDefaults.Web)));
            await insert.ExecuteNonQueryAsync();
        }
    }

    private static async Task<bool> TableHasColumnAsync(string databaseFilePath, string tableName, string columnName)
    {
        await using var connection = new SqliteConnection(
            new SqliteConnectionStringBuilder
            {
                DataSource = databaseFilePath,
                Mode = SqliteOpenMode.ReadWriteCreate,
                Pooling = false,
            }.ToString());

        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = $"PRAGMA table_info({tableName});";

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            if (string.Equals(reader.GetString(1), columnName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static async Task<int> CountSplitConfigSectionsAsync(string databaseFilePath)
    {
        await using var connection = new SqliteConnection(
            new SqliteConnectionStringBuilder
            {
                DataSource = databaseFilePath,
                Mode = SqliteOpenMode.ReadWriteCreate,
                Pooling = false,
            }.ToString());

        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(1) FROM AppConfig WHERE ConfigKey IN ('user','downloader','limit');";
        return Convert.ToInt32(await command.ExecuteScalarAsync());
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
}
