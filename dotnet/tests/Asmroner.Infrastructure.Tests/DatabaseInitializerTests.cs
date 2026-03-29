using System.Text.Json;
using Asmroner.Core.Configuration;
using Asmroner.Infrastructure.Services;
using Microsoft.Data.Sqlite;

namespace Asmroner.Infrastructure.Tests;

public class DatabaseInitializerTests
{
    [Fact]
    public async Task DatabaseInitializer_ShouldCreateNewSchema_AndDropUnusedLegacyTables()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var pathService = new AppPathService(tempRoot);
            var sut = new DatabaseInitializer(pathService);

            await sut.InitializeAsync();
            var canConnect = await sut.CanConnectAsync();

            Assert.True(File.Exists(pathService.DatabaseFilePath));
            Assert.True(canConnect);

            await using var connection = CreateConnection(pathService.DatabaseFilePath);
            await connection.OpenAsync();

            Assert.False(await TableExistsAsync(connection, "MetadataWork"));
            Assert.False(await TableExistsAsync(connection, "WorkSyncInfo"));
            Assert.True(await TableExistsAsync(connection, "AppConfig"));
            Assert.True(await TableExistsAsync(connection, "UiState"));

            Assert.True(await TableHasColumnAsync(connection, "AppConfig", "ConfigKey"));
            Assert.True(await TableHasColumnAsync(connection, "AppConfig", "JsonValue"));
            Assert.False(await TableHasColumnAsync(connection, "AppConfig", "Id"));
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    [Fact]
    public async Task DatabaseInitializer_ShouldMigrateLegacySingleRowAppConfig_ToSplitSections()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var pathService = new AppPathService(tempRoot);
            pathService.EnsureMetadataDirectory();

            await SeedLegacySchemaAsync(pathService.DatabaseFilePath, new AppConfig
            {
                User = new UserOptions
                {
                    Account = "legacy-user",
                    Password = "legacy-pass",
                },
                Downloader = new DownloaderOptions
                {
                    ApiUrl = "https://legacy.example.com",
                    SyncDataFolder = Path.Combine(tempRoot, "sync"),
                },
                Limit = new LimitOptions
                {
                    SyncQps = 3,
                    DownloadQps = 2,
                },
            });

            var sut = new DatabaseInitializer(pathService);
            await sut.InitializeAsync();

            await using var connection = CreateConnection(pathService.DatabaseFilePath);
            await connection.OpenAsync();

            Assert.True(await TableHasColumnAsync(connection, "AppConfig", "ConfigKey"));
            Assert.False(await TableHasColumnAsync(connection, "AppConfig", "Id"));
            Assert.Equal(3, await CountSplitConfigRowsAsync(connection));
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    private static async Task SeedLegacySchemaAsync(string databaseFilePath, AppConfig legacyConfig)
    {
        await using var connection = CreateConnection(databaseFilePath);
        await connection.OpenAsync();

        await using (var create = connection.CreateCommand())
        {
            create.CommandText =
                """
                CREATE TABLE IF NOT EXISTS MetadataWork (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    SourceId TEXT NOT NULL,
                    Title TEXT NOT NULL,
                    CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
                );

                CREATE TABLE IF NOT EXISTS WorkSyncInfo (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    SourceId TEXT NOT NULL,
                    Status TEXT NOT NULL,
                    UpdatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
                );

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
            insert.Parameters.AddWithValue("@json", JsonSerializer.Serialize(legacyConfig, new JsonSerializerOptions(JsonSerializerDefaults.Web)));
            await insert.ExecuteNonQueryAsync();
        }
    }

    private static async Task<bool> TableExistsAsync(SqliteConnection connection, string tableName)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(1) FROM sqlite_master WHERE type='table' AND name=@name;";
        command.Parameters.AddWithValue("@name", tableName);
        return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
    }

    private static async Task<bool> TableHasColumnAsync(SqliteConnection connection, string tableName, string columnName)
    {
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

    private static async Task<int> CountSplitConfigRowsAsync(SqliteConnection connection)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(1) FROM AppConfig WHERE ConfigKey IN ('user','downloader','limit');";
        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    private static SqliteConnection CreateConnection(string databaseFilePath)
    {
        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = databaseFilePath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Pooling = false,
        };

        return new SqliteConnection(builder.ToString());
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
