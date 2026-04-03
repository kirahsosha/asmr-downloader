using System.Text.Json;
using Asmroner.Core.Configuration;
using Asmroner.Core.Constants;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Sync;
using Microsoft.Data.Sqlite;

namespace Asmroner.Infrastructure.Services;

public sealed class ConfigurationService : IConfigurationService
{
    private const string AppConfigTableName = AsmronerConstants.Storage.AppConfig.TableName;
    private const string AppConfigLegacyTableName = AsmronerConstants.Storage.AppConfig.LegacyTableName;
    private const string AppConfigKeyColumn = AsmronerConstants.Storage.AppConfig.KeyColumn;
    private const string AppConfigJsonColumn = AsmronerConstants.Storage.AppConfig.JsonColumn;

    private const string UserSectionKey = AsmronerConstants.Storage.AppConfig.SectionKeys.User;
    private const string DownloaderSectionKey = AsmronerConstants.Storage.AppConfig.SectionKeys.Downloader;
    private const string LimitSectionKey = AsmronerConstants.Storage.AppConfig.SectionKeys.Limit;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IAppPathService _appPathService;

    public ConfigurationService(IAppPathService appPathService)
    {
        _appPathService = appPathService;
    }

    public bool Exists()
    {
        return ExistsInSqlite() || File.Exists(_appPathService.DefaultConfigFilePath);
    }

    public async Task<AppConfig?> LoadAsync(CancellationToken cancellationToken = default)
    {
        var sqliteConfig = await LoadFromSqliteAsync(cancellationToken);
        if (sqliteConfig is not null)
        {
            return sqliteConfig;
        }

        return await LoadFromDefaultConfigFileAsync(cancellationToken);
    }

    public async Task SaveAsync(AppConfig config, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(config);

        _appPathService.EnsureMetadataDirectory();

        var persisted = CloneForStorage(config);

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await EnsureConfigTableAsync(connection, cancellationToken);

        await using var transaction = connection.BeginTransaction();

        await UpsertSectionAsync(
            connection,
            transaction,
            UserSectionKey,
            JsonSerializer.Serialize(persisted.User, JsonOptions),
            cancellationToken);
        await UpsertSectionAsync(
            connection,
            transaction,
            DownloaderSectionKey,
            JsonSerializer.Serialize(persisted.Downloader, JsonOptions),
            cancellationToken);
        await UpsertSectionAsync(
            connection,
            transaction,
            LimitSectionKey,
            JsonSerializer.Serialize(persisted.Limit, JsonOptions),
            cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }

    public IReadOnlyList<string> Validate(AppConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(config.User.Account))
        {
            errors.Add("账号不能为空。");
        }

        if (string.IsNullOrWhiteSpace(config.User.Password))
        {
            errors.Add("密码不能为空。");
        }

        if (config.Downloader.MaxWorkers <= 0)
        {
            errors.Add("并发工作数必须大于 0。");
        }

        if (config.Downloader.MaxRetries < 0)
        {
            errors.Add("重试次数不能小于 0。");
        }

        if (string.IsNullOrWhiteSpace(config.Downloader.SyncDataFolder))
        {
            errors.Add("同步目录不能为空。");
        }

        try
        {
            SyncSizeText.ParseBytes(config.Downloader.SyncWantedSize);
        }
        catch (ArgumentException ex)
        {
            errors.Add(ex.Message);
        }

        if (config.Limit.SyncQps <= 0 || config.Limit.DownloadQps <= 0)
        {
            errors.Add("QPS 必须大于 0。");
        }

        if (config.Limit.SyncJitterMin > config.Limit.SyncJitterMax)
        {
            errors.Add("同步抖动最小值不能大于最大值。");
        }

        if (config.Limit.DownloadJitterMin > config.Limit.DownloadJitterMax)
        {
            errors.Add("下载抖动最小值不能大于最大值。");
        }

        return errors;
    }

    private async Task<AppConfig?> LoadFromSqliteAsync(CancellationToken cancellationToken)
    {
        _appPathService.EnsureMetadataDirectory();

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await EnsureConfigTableAsync(connection, cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = AsmronerConstants.SqliteQueries.BuildSelectAppConfigSections(
            AppConfigTableName,
            AppConfigKeyColumn,
            AppConfigJsonColumn);
        command.Parameters.AddWithValue("@user", UserSectionKey);
        command.Parameters.AddWithValue("@downloader", DownloaderSectionKey);
        command.Parameters.AddWithValue("@limit", LimitSectionKey);

        var sections = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var key = reader.GetString(0);
            var json = reader.GetString(1);
            sections[key] = json;
        }

        if (sections.Count == 0)
        {
            return null;
        }

        var loaded = new AppConfig();
        var hasAnySection = false;

        if (TryDeserializeSection(sections, UserSectionKey, out UserOptions user))
        {
            loaded.User = user;
            hasAnySection = true;
        }

        if (TryDeserializeSection(sections, DownloaderSectionKey, out DownloaderOptions downloader))
        {
            loaded.Downloader = downloader;
            hasAnySection = true;
        }

        if (TryDeserializeSection(sections, LimitSectionKey, out LimitOptions limit))
        {
            loaded.Limit = limit;
            hasAnySection = true;
        }

        return hasAnySection ? loaded : null;
    }

    private async Task<AppConfig?> LoadFromDefaultConfigFileAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_appPathService.DefaultConfigFilePath))
        {
            return null;
        }

        var content = await File.ReadAllTextAsync(_appPathService.DefaultConfigFilePath, cancellationToken);

        try
        {
            var parsed = JsonSerializer.Deserialize<AppConfig>(content, JsonOptions);
            if (parsed is null)
            {
                throw new InvalidOperationException("默认配置文件格式无效: 根对象不能为空。");
            }

            return NormalizeLoadedConfig(parsed);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"默认配置文件格式无效: {ex.Message}", ex);
        }
    }

    private bool ExistsInSqlite()
    {
        try
        {
            if (!File.Exists(_appPathService.DatabaseFilePath))
            {
                return false;
            }

            using var connection = CreateConnection();
            connection.Open();

            using var tableCheck = connection.CreateCommand();
            tableCheck.CommandText = AsmronerConstants.SqliteQueries.CountTableByName;
            tableCheck.Parameters.AddWithValue("@name", AppConfigTableName);
            if (Convert.ToInt32(tableCheck.ExecuteScalar()) <= 0)
            {
                return false;
            }

            using var pragma = connection.CreateCommand();
            pragma.CommandText = AsmronerConstants.SqliteQueries.BuildPragmaTableInfo(AppConfigTableName);

            var hasConfigKey = false;
            var hasLegacyId = false;

            using (var reader = pragma.ExecuteReader())
            {
                while (reader.Read())
                {
                    var column = reader.GetString(1);
                    if (string.Equals(column, AppConfigKeyColumn, StringComparison.OrdinalIgnoreCase))
                    {
                        hasConfigKey = true;
                    }

                    if (string.Equals(column, AsmronerConstants.Storage.Columns.LegacyId, StringComparison.OrdinalIgnoreCase))
                    {
                        hasLegacyId = true;
                    }
                }
            }

            using var rowCheck = connection.CreateCommand();
            if (hasConfigKey)
            {
                rowCheck.CommandText = AsmronerConstants.SqliteQueries.BuildCountAppConfigSections(
                    AppConfigTableName,
                    AppConfigKeyColumn);
                return Convert.ToInt32(rowCheck.ExecuteScalar()) > 0;
            }

            if (hasLegacyId)
            {
                rowCheck.CommandText = AsmronerConstants.SqliteQueries.BuildCountLegacyConfigRows(AppConfigTableName);
                return Convert.ToInt32(rowCheck.ExecuteScalar()) > 0;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    private static bool TryDeserializeSection<T>(
        IReadOnlyDictionary<string, string> sections,
        string sectionKey,
        out T section)
        where T : class, new()
    {
        section = new T();

        if (!sections.TryGetValue(sectionKey, out var json) || string.IsNullOrWhiteSpace(json))
        {
            return false;
        }

        try
        {
            var parsed = JsonSerializer.Deserialize<T>(json, JsonOptions);
            if (parsed is null)
            {
                return false;
            }

            section = parsed;
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static AppConfig NormalizeLoadedConfig(AppConfig config)
    {
        return new AppConfig
        {
            User = config.User ?? new UserOptions(),
            Downloader = config.Downloader ?? new DownloaderOptions(),
            Limit = config.Limit ?? new LimitOptions(),
        };
    }

    private static AppConfig CloneForStorage(AppConfig config)
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

    private static async Task EnsureConfigTableAsync(SqliteConnection connection, CancellationToken cancellationToken)
    {
        var tableExists = await TableExistsAsync(connection, AppConfigTableName, cancellationToken);
        if (!tableExists)
        {
            await CreateConfigTableAsync(connection, transaction: null, cancellationToken);
            return;
        }

        var columns = await ReadTableColumnsAsync(connection, AppConfigTableName, cancellationToken);
        if (columns.Contains(AppConfigKeyColumn, StringComparer.OrdinalIgnoreCase))
        {
            return;
        }

        if (columns.Contains(AsmronerConstants.Storage.Columns.LegacyId, StringComparer.OrdinalIgnoreCase))
        {
            await MigrateLegacyConfigTableAsync(connection, cancellationToken);
            return;
        }

        await using var resetTransaction = connection.BeginTransaction();
        await ExecuteNonQueryAsync(
            connection,
            AsmronerConstants.SqliteQueries.BuildDropTable(AppConfigTableName),
            resetTransaction,
            cancellationToken);
        await CreateConfigTableAsync(connection, resetTransaction, cancellationToken);
        await resetTransaction.CommitAsync(cancellationToken);
    }

    private static async Task MigrateLegacyConfigTableAsync(SqliteConnection connection, CancellationToken cancellationToken)
    {
        AppConfig? legacyConfig = null;

        await using (var readCommand = connection.CreateCommand())
        {
            readCommand.CommandText = AsmronerConstants.SqliteQueries.BuildSelectLegacyConfigJson(
                AppConfigTableName,
                AppConfigJsonColumn);
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
            AsmronerConstants.SqliteQueries.BuildDropTable(AppConfigLegacyTableName),
            transaction,
            cancellationToken);
        await ExecuteNonQueryAsync(
            connection,
            AsmronerConstants.SqliteQueries.BuildRenameTable(AppConfigTableName, AppConfigLegacyTableName),
            transaction,
            cancellationToken);

        await CreateConfigTableAsync(connection, transaction, cancellationToken);

        if (legacyConfig is not null)
        {
            var normalized = CloneForStorage(legacyConfig);

            await UpsertSectionAsync(
                connection,
                transaction,
                UserSectionKey,
                JsonSerializer.Serialize(normalized.User, JsonOptions),
                cancellationToken);
            await UpsertSectionAsync(
                connection,
                transaction,
                DownloaderSectionKey,
                JsonSerializer.Serialize(normalized.Downloader, JsonOptions),
                cancellationToken);
            await UpsertSectionAsync(
                connection,
                transaction,
                LimitSectionKey,
                JsonSerializer.Serialize(normalized.Limit, JsonOptions),
                cancellationToken);
        }

        await ExecuteNonQueryAsync(
            connection,
            AsmronerConstants.SqliteQueries.BuildDropTable(AppConfigLegacyTableName),
            transaction,
            cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }

    private static async Task CreateConfigTableAsync(
        SqliteConnection connection,
        SqliteTransaction? transaction,
        CancellationToken cancellationToken)
    {
        var sql = AsmronerConstants.SqliteQueries.BuildCreateAppConfigTable(
            AppConfigTableName,
            AppConfigKeyColumn,
            AppConfigJsonColumn);

        await ExecuteNonQueryAsync(connection, sql, transaction, cancellationToken);
    }

    private static async Task UpsertSectionAsync(
        SqliteConnection connection,
        SqliteTransaction transaction,
        string key,
        string json,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = AsmronerConstants.SqliteQueries.BuildUpsertAppConfigSection(
            AppConfigTableName,
            AppConfigKeyColumn,
            AppConfigJsonColumn);
        command.Parameters.AddWithValue("@key", key);
        command.Parameters.AddWithValue("@json", json);
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
        command.CommandText = AsmronerConstants.SqliteQueries.CountTableByName;
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
        command.CommandText = AsmronerConstants.SqliteQueries.BuildPragmaTableInfo(tableName);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            if (reader[1] is string column)
            {
                columns.Add(column);
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
