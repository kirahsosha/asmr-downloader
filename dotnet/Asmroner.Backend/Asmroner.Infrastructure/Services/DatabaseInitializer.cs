using System.Text.Json;
using Asmroner.Core.Configuration;
using Asmroner.Core.Interfaces;
using Microsoft.Data.Sqlite;

namespace Asmroner.Infrastructure.Services;

public sealed class DatabaseInitializer : IDatabaseInitializer
{
    private const string AppConfigTableName = "AppConfig";
    private const string AppConfigLegacyTableName = "AppConfigLegacy";
    private const string AppConfigKeyColumn = "ConfigKey";
    private const string AppConfigJsonColumn = "JsonValue";
    private const string UiStateTableName = "UiState";

    private const string UserSectionKey = "user";
    private const string DownloaderSectionKey = "downloader";
    private const string LimitSectionKey = "limit";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IAppPathService _appPathService;

    public DatabaseInitializer(IAppPathService appPathService)
    {
        _appPathService = appPathService;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _appPathService.EnsureMetadataDirectory();

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await DropUnusedLegacyTablesAsync(connection, cancellationToken);
        await EnsureAppConfigTableAsync(connection, cancellationToken);
        await EnsureUiStateTableAsync(connection, cancellationToken);
    }

    public async Task<bool> CanConnectAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1;";
            var result = await command.ExecuteScalarAsync(cancellationToken);
            return Convert.ToInt32(result) == 1;
        }
        catch
        {
            return false;
        }
    }

    private static async Task DropUnusedLegacyTablesAsync(SqliteConnection connection, CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            DROP TABLE IF EXISTS MetadataWork;
            DROP TABLE IF EXISTS WorkSyncInfo;
            """;

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task EnsureAppConfigTableAsync(SqliteConnection connection, CancellationToken cancellationToken)
    {
        var tableExists = await TableExistsAsync(connection, AppConfigTableName, cancellationToken);
        if (!tableExists)
        {
            await CreateAppConfigTableAsync(connection, transaction: null, cancellationToken);
            return;
        }

        var columns = await ReadTableColumnsAsync(connection, AppConfigTableName, cancellationToken);
        if (columns.Contains(AppConfigKeyColumn, StringComparer.OrdinalIgnoreCase))
        {
            return;
        }

        if (columns.Contains("Id", StringComparer.OrdinalIgnoreCase))
        {
            await MigrateLegacyAppConfigTableAsync(connection, cancellationToken);
            return;
        }

        await using var resetTransaction = connection.BeginTransaction();
        await ExecuteNonQueryAsync(
            connection,
            $"DROP TABLE IF EXISTS {AppConfigTableName};",
            resetTransaction,
            cancellationToken);
        await CreateAppConfigTableAsync(connection, resetTransaction, cancellationToken);
        await resetTransaction.CommitAsync(cancellationToken);
    }

    private static async Task EnsureUiStateTableAsync(SqliteConnection connection, CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText =
            $"""
            CREATE TABLE IF NOT EXISTS {UiStateTableName} (
                StateKey TEXT PRIMARY KEY,
                JsonValue TEXT NOT NULL,
                UpdatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
            );
            """;

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task MigrateLegacyAppConfigTableAsync(SqliteConnection connection, CancellationToken cancellationToken)
    {
        AppConfig? legacyConfig = null;

        await using (var readCommand = connection.CreateCommand())
        {
            readCommand.CommandText = $"SELECT {AppConfigJsonColumn} FROM {AppConfigTableName} WHERE Id = 1 LIMIT 1;";
            var legacyJson = await readCommand.ExecuteScalarAsync(cancellationToken) as string;
            if (!string.IsNullOrWhiteSpace(legacyJson))
            {
                try
                {
                    legacyConfig = JsonSerializer.Deserialize<AppConfig>(legacyJson, JsonOptions);
                    if (legacyConfig is not null)
                    {
                        MergeLegacyPreferFormatsIfNeeded(legacyConfig, legacyJson);
                    }
                }
                catch
                {
                    legacyConfig = null;
                }
            }
        }

        await using var transaction = connection.BeginTransaction();

        await ExecuteNonQueryAsync(
            connection,
            $"DROP TABLE IF EXISTS {AppConfigLegacyTableName};",
            transaction,
            cancellationToken);
        await ExecuteNonQueryAsync(
            connection,
            $"ALTER TABLE {AppConfigTableName} RENAME TO {AppConfigLegacyTableName};",
            transaction,
            cancellationToken);

        await CreateAppConfigTableAsync(connection, transaction, cancellationToken);

        if (legacyConfig is not null)
        {
            var normalized = NormalizeForStorage(legacyConfig);

            await UpsertAppConfigSectionAsync(
                connection,
                transaction,
                UserSectionKey,
                JsonSerializer.Serialize(normalized.User, JsonOptions),
                cancellationToken);
            await UpsertAppConfigSectionAsync(
                connection,
                transaction,
                DownloaderSectionKey,
                JsonSerializer.Serialize(normalized.Downloader, JsonOptions),
                cancellationToken);
            await UpsertAppConfigSectionAsync(
                connection,
                transaction,
                LimitSectionKey,
                JsonSerializer.Serialize(normalized.Limit, JsonOptions),
                cancellationToken);
        }

        await ExecuteNonQueryAsync(
            connection,
            $"DROP TABLE IF EXISTS {AppConfigLegacyTableName};",
            transaction,
            cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }

    private static AppConfig NormalizeForStorage(AppConfig config)
    {
        return new AppConfig
        {
            User = new UserOptions
            {
                Account = config.User.Account,
                Password = config.User.Password,
            },
            Downloader = new DownloaderOptions
            {
                ApiUrl = config.Downloader.ApiUrl,
                ApiCandidateUrls = config.Downloader.ApiCandidateUrls,
                PublishSourceUrls = config.Downloader.PublishSourceUrls,
                WorkPageUrlTemplate = config.Downloader.WorkPageUrlTemplate,
                ProxyUrl = config.Downloader.ProxyUrl,
                MaxWorkers = config.Downloader.MaxWorkers,
                MaxRetries = config.Downloader.MaxRetries,
                SyncDataFolder = config.Downloader.SyncDataFolder,
                SyncWantedSize = config.Downloader.SyncWantedSize,
                PreferFormats = config.Downloader.PreferFormats,
                HdAudioOnly = config.Downloader.HdAudioOnly,
            },
            Limit = new LimitOptions
            {
                SyncQps = config.Limit.SyncQps,
                SyncJitterMin = config.Limit.SyncJitterMin,
                SyncJitterMax = config.Limit.SyncJitterMax,
                DownloadQps = config.Limit.DownloadQps,
                DownloadJitterMin = config.Limit.DownloadJitterMin,
                DownloadJitterMax = config.Limit.DownloadJitterMax,
            },
        };
    }

    private static void MergeLegacyPreferFormatsIfNeeded(AppConfig config, string legacyJson)
    {
        try
        {
            using var document = JsonDocument.Parse(legacyJson);
            if (!TryGetPropertyIgnoreCase(document.RootElement, "downloader", out var downloaderElement) ||
                downloaderElement.ValueKind != JsonValueKind.Object)
            {
                return;
            }

            if (TryGetPropertyIgnoreCase(downloaderElement, "preferFormats", out var preferFormatsElement))
            {
                config.Downloader.PreferFormats = preferFormatsElement.GetString() ?? string.Empty;
                return;
            }

            var legacyParts = new[]
            {
                ReadStringPropertyIgnoreCase(downloaderElement, "preferMedia"),
                ReadStringPropertyIgnoreCase(downloaderElement, "preferImage"),
                ReadStringPropertyIgnoreCase(downloaderElement, "preferVideo"),
            };

            var merged = string.Join(",", legacyParts.Where(static part => !string.IsNullOrWhiteSpace(part)));
            if (!string.IsNullOrWhiteSpace(merged))
            {
                config.Downloader.PreferFormats = merged;
            }
        }
        catch (JsonException)
        {
        }
    }

    private static string ReadStringPropertyIgnoreCase(JsonElement element, string propertyName)
    {
        if (!TryGetPropertyIgnoreCase(element, propertyName, out var property))
        {
            return string.Empty;
        }

        return property.GetString() ?? string.Empty;
    }

    private static bool TryGetPropertyIgnoreCase(JsonElement element, string propertyName, out JsonElement property)
    {
        foreach (var candidate in element.EnumerateObject())
        {
            if (string.Equals(candidate.Name, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                property = candidate.Value;
                return true;
            }
        }

        property = default;
        return false;
    }

    private static async Task CreateAppConfigTableAsync(
        SqliteConnection connection,
        SqliteTransaction? transaction,
        CancellationToken cancellationToken)
    {
        var sql =
            $"""
            CREATE TABLE IF NOT EXISTS {AppConfigTableName} (
                {AppConfigKeyColumn} TEXT PRIMARY KEY,
                {AppConfigJsonColumn} TEXT NOT NULL,
                UpdatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
            );
            """;

        await ExecuteNonQueryAsync(connection, sql, transaction, cancellationToken);
    }

    private static async Task UpsertAppConfigSectionAsync(
        SqliteConnection connection,
        SqliteTransaction transaction,
        string sectionKey,
        string sectionJson,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText =
            $"""
            INSERT INTO {AppConfigTableName} ({AppConfigKeyColumn}, {AppConfigJsonColumn}, UpdatedAt)
            VALUES (@key, @json, CURRENT_TIMESTAMP)
            ON CONFLICT({AppConfigKeyColumn}) DO UPDATE SET
                {AppConfigJsonColumn} = excluded.{AppConfigJsonColumn},
                UpdatedAt = CURRENT_TIMESTAMP;
            """;
        command.Parameters.AddWithValue("@key", sectionKey);
        command.Parameters.AddWithValue("@json", sectionJson);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task ExecuteNonQueryAsync(
        SqliteConnection connection,
        string sql,
        SqliteTransaction? transaction,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task<bool> TableExistsAsync(
        SqliteConnection connection,
        string tableName,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(1) FROM sqlite_master WHERE type='table' AND name=@name;";
        command.Parameters.AddWithValue("@name", tableName);
        return Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken)) > 0;
    }

    private static async Task<HashSet<string>> ReadTableColumnsAsync(
        SqliteConnection connection,
        string tableName,
        CancellationToken cancellationToken)
    {
        var columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        await using var command = connection.CreateCommand();
        command.CommandText = $"PRAGMA table_info({tableName});";
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            if (reader[1] is string name)
            {
                columns.Add(name);
            }
        }

        return columns;
    }

    private SqliteConnection CreateConnection()
    {
        var connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = _appPathService.DatabaseFilePath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Pooling = false,
        }.ToString();

        return new SqliteConnection(connectionString);
    }
}
