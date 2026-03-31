namespace Asmroner.Core.Constants;

public static class AsmronerConstants
{
    public static class Application
    {
        public const string Name = "Asmroner";
        public const string SettingsVersionPrefix = "版本：v";
    }

    public static class Api
    {
        public static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(10);

        public static class ErrorCodes
        {
            public const string RequestFailed = "api_request_failed";
            public const string ResponseEmpty = "api_response_empty";
        }
    }

    public static class Download
    {
        public const int RetryAllMaxConcurrency = 2;
        public const int WorkInfoFetchMaxConcurrency = 4;
        public static readonly TimeSpan StartupMetadataRefreshTimeout = TimeSpan.FromSeconds(15);
    }

    public static class Storage
    {
        public static class AppConfig
        {
            public const string TableName = "AppConfig";
            public const string LegacyTableName = "AppConfigLegacy";
            public const string KeyColumn = "ConfigKey";
            public const string JsonColumn = "JsonValue";

            public static class SectionKeys
            {
                public const string User = "user";
                public const string Downloader = "downloader";
                public const string Limit = "limit";
            }
        }

        public static class UiState
        {
            public const string TableName = "UiState";
            public const string KeyColumn = "StateKey";
            public const string JsonColumn = "JsonValue";

            public static class StateKeys
            {
                public const string Search = "search";
                public const string Download = "download";
                public const string UnfinishedQueue = "unfinished_queue";
            }
        }

        public static class Columns
        {
            public const string LegacyId = "Id";
            public const string UpdatedAt = "UpdatedAt";
        }

        public static class LegacyTables
        {
            public const string MetadataWork = "MetadataWork";
            public const string WorkSyncInfo = "WorkSyncInfo";
        }
    }

    public static class SqliteQueries
    {
        public const string CountTableByName = "SELECT COUNT(1) FROM sqlite_master WHERE type='table' AND name=@name;";

        public static string BuildPragmaTableInfo(string tableName)
        {
            return $"PRAGMA table_info({tableName});";
        }

        public static string BuildDropTable(string tableName)
        {
            return $"DROP TABLE IF EXISTS {tableName};";
        }

        public static string BuildRenameTable(string sourceTableName, string targetTableName)
        {
            return $"ALTER TABLE {sourceTableName} RENAME TO {targetTableName};";
        }

        public static string BuildDropLegacySyncTables()
        {
            return $"""
            DROP TABLE IF EXISTS {Storage.LegacyTables.MetadataWork};
            DROP TABLE IF EXISTS {Storage.LegacyTables.WorkSyncInfo};
            """;
        }

        public static string BuildSelectLegacyConfigJson(string tableName, string jsonColumn)
        {
            return $"SELECT {jsonColumn} FROM {tableName} WHERE {Storage.Columns.LegacyId} = 1 LIMIT 1;";
        }

        public static string BuildCountLegacyConfigRows(string tableName)
        {
            return $"SELECT COUNT(1) FROM {tableName} WHERE {Storage.Columns.LegacyId} = 1;";
        }

        public static string BuildCountAppConfigSections(string tableName, string keyColumn)
        {
            return $"SELECT COUNT(1) FROM {tableName} WHERE {keyColumn} IN ('{Storage.AppConfig.SectionKeys.User}','{Storage.AppConfig.SectionKeys.Downloader}','{Storage.AppConfig.SectionKeys.Limit}');";
        }

        public static string BuildSelectAppConfigSections(string tableName, string keyColumn, string jsonColumn)
        {
            return $"""
            SELECT {keyColumn}, {jsonColumn}
            FROM {tableName}
            WHERE {keyColumn} IN (@user, @downloader, @limit);
            """;
        }

        public static string BuildCreateAppConfigTable(string tableName, string keyColumn, string jsonColumn)
        {
            return $"""
            CREATE TABLE IF NOT EXISTS {tableName} (
                {keyColumn} TEXT PRIMARY KEY,
                {jsonColumn} TEXT NOT NULL,
                {Storage.Columns.UpdatedAt} TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
            );
            """;
        }

        public static string BuildUpsertAppConfigSection(string tableName, string keyColumn, string jsonColumn)
        {
            return $"""
            INSERT INTO {tableName} ({keyColumn}, {jsonColumn}, {Storage.Columns.UpdatedAt})
            VALUES (@key, @json, CURRENT_TIMESTAMP)
            ON CONFLICT({keyColumn}) DO UPDATE SET
                {jsonColumn} = excluded.{jsonColumn},
                {Storage.Columns.UpdatedAt} = CURRENT_TIMESTAMP;
            """;
        }

        public static string BuildCreateUiStateTable()
        {
            return $"""
            CREATE TABLE IF NOT EXISTS {Storage.UiState.TableName} (
                {Storage.UiState.KeyColumn} TEXT PRIMARY KEY,
                {Storage.UiState.JsonColumn} TEXT NOT NULL,
                {Storage.Columns.UpdatedAt} TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
            );
            """;
        }

        public static string BuildDeleteUiStateByKey()
        {
            return $"DELETE FROM {Storage.UiState.TableName} WHERE {Storage.UiState.KeyColumn} = @key;";
        }

        public static string BuildSelectUiStateJson()
        {
            return $"SELECT {Storage.UiState.JsonColumn} FROM {Storage.UiState.TableName} WHERE {Storage.UiState.KeyColumn} = @key LIMIT 1;";
        }

        public static string BuildUpsertUiState()
        {
            return $"""
            INSERT INTO {Storage.UiState.TableName} ({Storage.UiState.KeyColumn}, {Storage.UiState.JsonColumn}, {Storage.Columns.UpdatedAt})
            VALUES (@key, @json, CURRENT_TIMESTAMP)
            ON CONFLICT({Storage.UiState.KeyColumn}) DO UPDATE SET
                {Storage.UiState.JsonColumn} = excluded.{Storage.UiState.JsonColumn},
                {Storage.Columns.UpdatedAt} = CURRENT_TIMESTAMP;
            """;
        }
    }
}