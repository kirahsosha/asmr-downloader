using System.Text.Json;
using Asmroner.Core.Configuration;
using Asmroner.Core.Constants;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Sync;
using Asmroner.Core.Utils;
using Microsoft.Data.Sqlite;

namespace Asmroner.Infrastructure.Services;

public sealed class UiStateStore : IUiStateStore
{
    private const string TableName = AsmronerConstants.Storage.UiState.TableName;
    private const string SearchStateKey = AsmronerConstants.Storage.UiState.StateKeys.Search;
    private const string DownloadStateKey = AsmronerConstants.Storage.UiState.StateKeys.Download;
    private const string MetadataSyncProgressKey = AsmronerConstants.Storage.UiState.StateKeys.MetadataSyncProgress;
    private const string SyncDownloadProgressKey = AsmronerConstants.Storage.UiState.StateKeys.SyncDownloadProgress;
    private const string SyncUiStateKey = AsmronerConstants.Storage.UiState.StateKeys.Sync;
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

    public async Task<MetadataSyncProgressState> LoadMetadataSyncProgressAsync(CancellationToken cancellationToken = default)
    {
        var persisted = await LoadStateAsync<MetadataSyncProgressState>(MetadataSyncProgressKey, cancellationToken);
        return persisted ?? new MetadataSyncProgressState();
    }

    public async Task SaveMetadataSyncProgressAsync(MetadataSyncProgressState state, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(state);

        var persisted = await LoadStateAsync<MetadataSyncProgressState>(MetadataSyncProgressKey, cancellationToken);
        if (persisted?.StopRequested == true && ShouldPreserveStopRequested(state.Status))
        {
            state.StopRequested = true;
        }

        if (!ShouldPreserveStopRequested(state.Status))
        {
            state.StopRequested = false;
        }

        await SaveStateAsync(MetadataSyncProgressKey, state, cancellationToken);
    }

    public async Task RequestStopMetadataSyncAsync(CancellationToken cancellationToken = default)
    {
        var state = await LoadMetadataSyncProgressAsync(cancellationToken);
        if (!string.Equals(state.Status, SyncProgressStatuses.Running, StringComparison.Ordinal))
        {
            return;
        }

        state.StopRequested = true;
        state.UpdatedAt = DateTime.UtcNow;
        await SaveStateAsync(MetadataSyncProgressKey, state, cancellationToken);
    }

    public async Task<SyncDownloadProgressState> LoadSyncDownloadProgressAsync(CancellationToken cancellationToken = default)
    {
        var persisted = await LoadStateAsync<SyncDownloadProgressState>(SyncDownloadProgressKey, cancellationToken);
        return persisted ?? new SyncDownloadProgressState();
    }

    public async Task SaveSyncDownloadProgressAsync(SyncDownloadProgressState state, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(state);

        var persisted = await LoadStateAsync<SyncDownloadProgressState>(SyncDownloadProgressKey, cancellationToken);
        if (persisted?.StopRequested == true && ShouldPreserveStopRequested(state.Status))
        {
            state.StopRequested = true;
        }

        if (!ShouldPreserveStopRequested(state.Status))
        {
            state.StopRequested = false;
        }

        await SaveStateAsync(SyncDownloadProgressKey, state, cancellationToken);
    }

    public async Task<SyncUiState> LoadSyncUiStateAsync(CancellationToken cancellationToken = default)
    {
        var persisted = await LoadStateAsync<SyncUiState>(SyncUiStateKey, cancellationToken);
        return persisted ?? new SyncUiState();
    }

    public Task SaveSyncUiStateAsync(SyncUiState state, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(state);
        return SaveStateAsync(SyncUiStateKey, state, cancellationToken);
    }

    public async Task RequestStopSyncDownloadAsync(CancellationToken cancellationToken = default)
    {
        var state = await LoadSyncDownloadProgressAsync(cancellationToken);
        if (!string.Equals(state.Status, SyncProgressStatuses.Running, StringComparison.Ordinal))
        {
            return;
        }

        state.StopRequested = true;
        state.UpdatedAt = DateTime.UtcNow;
        await SaveStateAsync(SyncDownloadProgressKey, state, cancellationToken);
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

    private static bool ShouldPreserveStopRequested(string status)
    {
        return string.Equals(status, SyncProgressStatuses.Running, StringComparison.Ordinal);
    }

    private sealed class UnfinishedQueueState
    {
        public string[] SourceIds { get; set; } = Array.Empty<string>();
    }
}
