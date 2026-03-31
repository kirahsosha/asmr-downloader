using System.Text.Json;
using Asmroner.Core.Configuration;
using Asmroner.Core.Constants;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Utils;
using Microsoft.Data.Sqlite;

namespace Asmroner.Infrastructure.Services;

public sealed class UiStateStore : IUiStateStore
{
    private const string TableName = AsmronerConstants.Storage.UiState.TableName;
    private const string SearchStateKey = AsmronerConstants.Storage.UiState.StateKeys.Search;
    private const string DownloadStateKey = AsmronerConstants.Storage.UiState.StateKeys.Download;
    private const string UnfinishedQueueKey = AsmronerConstants.Storage.UiState.StateKeys.UnfinishedQueue;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IAppPathService _appPathService;

    public UiStateStore(IAppPathService appPathService)
    {
        _appPathService = appPathService;
    }

    public async Task<SearchUiState> LoadSearchUiStateAsync(CancellationToken cancellationToken = default)
    {
        var persisted = await LoadStateAsync<SearchUiState>(SearchStateKey, cancellationToken);
        if (persisted is not null)
        {
            return persisted;
        }

        return new SearchUiState();
    }

    public Task SaveSearchUiStateAsync(SearchUiState state, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(state);
        return SaveStateAsync(SearchStateKey, state, cancellationToken);
    }

    public async Task<DownloadUiState> LoadDownloadUiStateAsync(CancellationToken cancellationToken = default)
    {
        var persisted = await LoadStateAsync<DownloadUiState>(DownloadStateKey, cancellationToken);
        if (persisted is not null)
        {
            return persisted;
        }

        return new DownloadUiState();
    }

    public Task SaveDownloadUiStateAsync(DownloadUiState state, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(state);
        return SaveStateAsync(DownloadStateKey, state, cancellationToken);
    }

    public async Task<IReadOnlyList<string>> LoadUnfinishedQueueAsync(CancellationToken cancellationToken = default)
    {
        var persisted = await LoadStateAsync<UnfinishedQueueState>(UnfinishedQueueKey, cancellationToken)
            ?? new UnfinishedQueueState();

        return persisted.SourceIds
            .Select(SourceIdNormalizer.Normalize)
            .Where(static sourceId => !string.IsNullOrWhiteSpace(sourceId))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(static sourceId => sourceId)
            .ToArray();
    }

    public Task SaveUnfinishedQueueAsync(IReadOnlyCollection<string> sourceIds, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sourceIds);

        var normalized = sourceIds
            .Select(SourceIdNormalizer.Normalize)
            .Where(static sourceId => !string.IsNullOrWhiteSpace(sourceId))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(static sourceId => sourceId)
            .ToArray();

        var state = new UnfinishedQueueState
        {
            SourceIds = normalized,
        };

        return SaveStateAsync(UnfinishedQueueKey, state, cancellationToken);
    }

    public async Task ClearUnfinishedQueueAsync(CancellationToken cancellationToken = default)
    {
        _appPathService.EnsureMetadataDirectory();

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await EnsureTableAsync(connection, cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = AsmronerConstants.SqliteQueries.BuildDeleteUiStateByKey();
        command.Parameters.AddWithValue("@key", UnfinishedQueueKey);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private async Task SaveStateAsync<T>(string key, T state, CancellationToken cancellationToken)
    {
        _appPathService.EnsureMetadataDirectory();

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await EnsureTableAsync(connection, cancellationToken);

        var json = JsonSerializer.Serialize(state, JsonOptions);

        await using var command = connection.CreateCommand();
        command.CommandText = AsmronerConstants.SqliteQueries.BuildUpsertUiState();
        command.Parameters.AddWithValue("@key", key);
        command.Parameters.AddWithValue("@json", json);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private async Task<T?> LoadStateAsync<T>(string key, CancellationToken cancellationToken)
        where T : class
    {
        _appPathService.EnsureMetadataDirectory();

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await EnsureTableAsync(connection, cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = AsmronerConstants.SqliteQueries.BuildSelectUiStateJson();
        command.Parameters.AddWithValue("@key", key);

        var result = await command.ExecuteScalarAsync(cancellationToken) as string;
        if (string.IsNullOrWhiteSpace(result))
        {
            return null;
        }

        return JsonSerializer.Deserialize<T>(result, JsonOptions);
    }

    private static async Task EnsureTableAsync(SqliteConnection connection, CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = AsmronerConstants.SqliteQueries.BuildCreateUiStateTable();

        await command.ExecuteNonQueryAsync(cancellationToken);
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

    private sealed class UnfinishedQueueState
    {
        public string[] SourceIds { get; set; } = Array.Empty<string>();
    }
}
