using System.Globalization;
using Asmroner.Core.Constants;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Sync;
using Asmroner.Core.Utils;
using Microsoft.Data.Sqlite;

namespace Asmroner.Infrastructure.Services;

public sealed class MetadataSyncStore : IMetadataSyncStore
{
    private readonly IAppPathService _appPathService;

    public MetadataSyncStore(IAppPathService appPathService)
    {
        _appPathService = appPathService;
    }

    public async Task<MetadataSyncSnapshot> GetMetadataSnapshotAsync(CancellationToken cancellationToken = default)
    {
        _appPathService.EnsureMetadataDirectory();

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await EnsureTablesAsync(connection, cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = $"""
        SELECT
            COUNT(1),
            COALESCE(SUM(CASE WHEN HasSubtitle = 1 THEN 1 ELSE 0 END), 0),
            MAX({AsmronerConstants.Storage.Sync.MetadataWork.UpdatedAtColumn})
        FROM {AsmronerConstants.Storage.Sync.MetadataWork.TableName};
        """;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return new MetadataSyncSnapshot();
        }

        return new MetadataSyncSnapshot
        {
            LocalTotalCount = Convert.ToInt32(reader.GetValue(0), CultureInfo.InvariantCulture),
            LocalSubtitleCount = Convert.ToInt32(reader.GetValue(1), CultureInfo.InvariantCulture),
            LastUpdatedAt = reader.IsDBNull(2)
                ? null
                : ParseTimestamp(reader.GetString(2)),
        };
    }

    public async Task<int> UpsertMetadataWorksAsync(IReadOnlyCollection<MetadataWorkItem> works, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(works);

        var normalizedWorks = works
            .Where(static work => work.Id > 0)
            .Select(static work => new MetadataWorkItem
            {
                Id = work.Id,
                Title = work.Title.Trim(),
                CircleId = work.CircleId,
                CircleName = work.CircleName.Trim(),
                Nsfw = work.Nsfw,
                Release = work.Release.Trim(),
                DownloadCount = work.DownloadCount,
                Price = work.Price,
                ReviewCount = work.ReviewCount,
                RateCount = work.RateCount,
                RateAverage = work.RateAverage,
                HasSubtitle = work.HasSubtitle,
                CreateDate = work.CreateDate.Trim(),
                Vas = work.Vas.Trim(),
                Tags = work.Tags.Trim(),
                Duration = work.Duration,
                SourceType = work.SourceType.Trim(),
                SourceId = SourceIdNormalizer.Normalize(work.SourceId),
                UpdatedAt = work.UpdatedAt == default ? DateTime.UtcNow : work.UpdatedAt,
            })
            .Where(static work => !string.IsNullOrWhiteSpace(work.SourceId))
            .GroupBy(static work => work.Id)
            .Select(static group => group.Last())
            .ToArray();

        if (normalizedWorks.Length == 0)
        {
            return 0;
        }

        _appPathService.EnsureMetadataDirectory();

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await EnsureTablesAsync(connection, cancellationToken);

        var existingIds = await LoadExistingIdsAsync(connection, normalizedWorks.Select(static work => work.Id).ToArray(), cancellationToken);

        await using var transaction = connection.BeginTransaction();
        foreach (var work in normalizedWorks)
        {
            await using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = $"""
            INSERT INTO {AsmronerConstants.Storage.Sync.MetadataWork.TableName} (
                Id,
                Title,
                CircleId,
                CircleName,
                Nsfw,
                Release,
                DownloadCount,
                Price,
                ReviewCount,
                RateCount,
                RateAverage,
                HasSubtitle,
                CreateDate,
                Vas,
                Tags,
                Duration,
                SourceType,
                {AsmronerConstants.Storage.Sync.MetadataWork.SourceIdColumn},
                {AsmronerConstants.Storage.Sync.MetadataWork.UpdatedAtColumn})
            VALUES (
                @id,
                @title,
                @circleId,
                @circleName,
                @nsfw,
                @release,
                @downloadCount,
                @price,
                @reviewCount,
                @rateCount,
                @rateAverage,
                @hasSubtitle,
                @createDate,
                @vas,
                @tags,
                @duration,
                @sourceType,
                @sourceId,
                @updatedAt)
            ON CONFLICT(Id) DO UPDATE SET
                Title = excluded.Title,
                CircleId = excluded.CircleId,
                CircleName = excluded.CircleName,
                Nsfw = excluded.Nsfw,
                Release = excluded.Release,
                DownloadCount = excluded.DownloadCount,
                Price = excluded.Price,
                ReviewCount = excluded.ReviewCount,
                RateCount = excluded.RateCount,
                RateAverage = excluded.RateAverage,
                HasSubtitle = excluded.HasSubtitle,
                CreateDate = excluded.CreateDate,
                Vas = excluded.Vas,
                Tags = excluded.Tags,
                Duration = excluded.Duration,
                SourceType = excluded.SourceType,
                {AsmronerConstants.Storage.Sync.MetadataWork.SourceIdColumn} = excluded.{AsmronerConstants.Storage.Sync.MetadataWork.SourceIdColumn},
                {AsmronerConstants.Storage.Sync.MetadataWork.UpdatedAtColumn} = excluded.{AsmronerConstants.Storage.Sync.MetadataWork.UpdatedAtColumn};
            """;
            command.Parameters.AddWithValue("@id", work.Id);
            command.Parameters.AddWithValue("@title", work.Title);
            command.Parameters.AddWithValue("@circleId", work.CircleId);
            command.Parameters.AddWithValue("@circleName", work.CircleName);
            command.Parameters.AddWithValue("@nsfw", work.Nsfw ? 1 : 0);
            command.Parameters.AddWithValue("@release", work.Release);
            command.Parameters.AddWithValue("@downloadCount", work.DownloadCount);
            command.Parameters.AddWithValue("@price", work.Price);
            command.Parameters.AddWithValue("@reviewCount", work.ReviewCount);
            command.Parameters.AddWithValue("@rateCount", work.RateCount);
            command.Parameters.AddWithValue("@rateAverage", work.RateAverage);
            command.Parameters.AddWithValue("@hasSubtitle", work.HasSubtitle ? 1 : 0);
            command.Parameters.AddWithValue("@createDate", work.CreateDate);
            command.Parameters.AddWithValue("@vas", work.Vas);
            command.Parameters.AddWithValue("@tags", work.Tags);
            command.Parameters.AddWithValue("@duration", work.Duration);
            command.Parameters.AddWithValue("@sourceType", work.SourceType);
            command.Parameters.AddWithValue("@sourceId", work.SourceId);
            command.Parameters.AddWithValue("@updatedAt", work.UpdatedAt.ToString("O", CultureInfo.InvariantCulture));
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);
        return normalizedWorks.Count(work => !existingIds.Contains(work.Id));
    }

    public async Task<IReadOnlyDictionary<string, MetadataWorkItem>> GetMetadataWorksBySourceIdsAsync(IReadOnlyCollection<string> sourceIds, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sourceIds);

        var normalizedSourceIds = sourceIds
            .Select(SourceIdNormalizer.Normalize)
            .Where(static sourceId => !string.IsNullOrWhiteSpace(sourceId))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (normalizedSourceIds.Length == 0)
        {
            return new Dictionary<string, MetadataWorkItem>(StringComparer.OrdinalIgnoreCase);
        }

        _appPathService.EnsureMetadataDirectory();

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await EnsureTablesAsync(connection, cancellationToken);

        await using var command = connection.CreateCommand();
        var parameterNames = new List<string>(normalizedSourceIds.Length);
        for (var index = 0; index < normalizedSourceIds.Length; index++)
        {
            var parameterName = $"@sourceId{index}";
            parameterNames.Add(parameterName);
            command.Parameters.AddWithValue(parameterName, normalizedSourceIds[index]);
        }

        command.CommandText = $"""
        SELECT Id, Title, CircleId, CircleName, Nsfw, Release, DownloadCount, Price, ReviewCount,
               RateCount, RateAverage, HasSubtitle, CreateDate, Vas, Tags, Duration, SourceType,
               {AsmronerConstants.Storage.Sync.MetadataWork.SourceIdColumn},
               {AsmronerConstants.Storage.Sync.MetadataWork.UpdatedAtColumn}
        FROM {AsmronerConstants.Storage.Sync.MetadataWork.TableName}
        WHERE {AsmronerConstants.Storage.Sync.MetadataWork.SourceIdColumn} IN ({string.Join(", ", parameterNames)});
        """;

        var items = new Dictionary<string, MetadataWorkItem>(StringComparer.OrdinalIgnoreCase);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var item = MapMetadataWork(reader);
            items[item.SourceId] = item;
        }

        return items;
    }

    public async Task<IReadOnlyList<int>> GetExpiredMetadataWorkIdsAsync(DateTime updatedBefore, CancellationToken cancellationToken = default)
    {
        _appPathService.EnsureMetadataDirectory();

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await EnsureTablesAsync(connection, cancellationToken);

        var items = new List<int>();
        await using var command = connection.CreateCommand();
        command.CommandText = $"""
        SELECT Id
        FROM {AsmronerConstants.Storage.Sync.MetadataWork.TableName}
        WHERE {AsmronerConstants.Storage.Sync.MetadataWork.UpdatedAtColumn} < @updatedBefore
        ORDER BY Id;
        """;
        command.Parameters.AddWithValue("@updatedBefore", updatedBefore.ToString("O", CultureInfo.InvariantCulture));

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            items.Add(reader.GetInt32(0));
        }

        return items;
    }

    public async Task<IReadOnlyList<MetadataWorkItem>> GetAllMetadataWorksAsync(CancellationToken cancellationToken = default)
    {
        _appPathService.EnsureMetadataDirectory();

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await EnsureTablesAsync(connection, cancellationToken);

        var items = new List<MetadataWorkItem>();
        await using var command = connection.CreateCommand();
        command.CommandText = $"""
        SELECT Id, Title, CircleId, CircleName, Nsfw, Release, DownloadCount, Price, ReviewCount,
               RateCount, RateAverage, HasSubtitle, CreateDate, Vas, Tags, Duration, SourceType,
               {AsmronerConstants.Storage.Sync.MetadataWork.SourceIdColumn},
               {AsmronerConstants.Storage.Sync.MetadataWork.UpdatedAtColumn}
        FROM {AsmronerConstants.Storage.Sync.MetadataWork.TableName}
        ORDER BY Id;
        """;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            items.Add(MapMetadataWork(reader));
        }

        return items;
    }

    public async Task<SyncDownloadSnapshot> GetDownloadSnapshotAsync(CancellationToken cancellationToken = default)
    {
        _appPathService.EnsureMetadataDirectory();

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await EnsureTablesAsync(connection, cancellationToken);

        var snapshot = new SyncDownloadSnapshot();

        await using (var command = connection.CreateCommand())
        {
            command.CommandText = $"""
            SELECT
                COALESCE(SUM(CASE WHEN {AsmronerConstants.Storage.Sync.WorkSyncInfo.StatusColumn} = 'PENDING' THEN 1 ELSE 0 END), 0),
                COALESCE(SUM(CASE WHEN {AsmronerConstants.Storage.Sync.WorkSyncInfo.StatusColumn} = 'COMPLETED' THEN 1 ELSE 0 END), 0),
                COALESCE(SUM(CASE WHEN {AsmronerConstants.Storage.Sync.WorkSyncInfo.StatusColumn} = 'FAILED' THEN 1 ELSE 0 END), 0),
                COALESCE(SUM(CASE WHEN {AsmronerConstants.Storage.Sync.WorkSyncInfo.StatusColumn} = 'COMPLETED' THEN DirSize ELSE 0 END), 0),
                MAX({AsmronerConstants.Storage.Sync.WorkSyncInfo.UpdatedAtColumn})
            FROM {AsmronerConstants.Storage.Sync.WorkSyncInfo.TableName};
            """;

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                snapshot = new SyncDownloadSnapshot
                {
                    PendingCount = Convert.ToInt32(reader.GetValue(0), CultureInfo.InvariantCulture),
                    CompletedCount = Convert.ToInt32(reader.GetValue(1), CultureInfo.InvariantCulture),
                    FailedCount = Convert.ToInt32(reader.GetValue(2), CultureInfo.InvariantCulture),
                    CompletedSizeBytes = Convert.ToInt64(reader.GetValue(3), CultureInfo.InvariantCulture),
                    LastUpdatedAt = reader.IsDBNull(4)
                        ? null
                        : ParseTimestamp(reader.GetString(4)),
                };
            }
        }

        await using (var command = connection.CreateCommand())
        {
            command.CommandText = $"""
            SELECT COUNT(1)
            FROM {AsmronerConstants.Storage.Sync.MetadataWork.TableName} metadata
            WHERE NOT EXISTS (
                SELECT 1
                FROM {AsmronerConstants.Storage.Sync.WorkSyncInfo.TableName} sync
                WHERE sync.{AsmronerConstants.Storage.Sync.WorkSyncInfo.MetadataWorkIdColumn} = metadata.Id);
            """;

            var remaining = await command.ExecuteScalarAsync(cancellationToken);
            return new SyncDownloadSnapshot
            {
                PendingCount = snapshot.PendingCount,
                CompletedCount = snapshot.CompletedCount,
                FailedCount = snapshot.FailedCount,
                CompletedSizeBytes = snapshot.CompletedSizeBytes,
                LastUpdatedAt = snapshot.LastUpdatedAt,
                RemainingMetadataCount = Convert.ToInt32(remaining ?? 0, CultureInfo.InvariantCulture),
            };
        }
    }

    public async Task<IReadOnlyDictionary<int, WorkSyncInfoItem>> GetWorkSyncInfoMapAsync(CancellationToken cancellationToken = default)
    {
        _appPathService.EnsureMetadataDirectory();

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await EnsureTablesAsync(connection, cancellationToken);

        var items = new Dictionary<int, WorkSyncInfoItem>();
        await using var command = connection.CreateCommand();
        command.CommandText = $"""
        SELECT Id, {AsmronerConstants.Storage.Sync.WorkSyncInfo.MetadataWorkIdColumn}, SourceId, HasSubtitle, DirSize,
               {AsmronerConstants.Storage.Sync.WorkSyncInfo.StatusColumn}, FilePath, FailReason, RetryCount,
               {AsmronerConstants.Storage.Sync.WorkSyncInfo.UpdatedAtColumn}, FailedAt
        FROM {AsmronerConstants.Storage.Sync.WorkSyncInfo.TableName};
        """;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var item = MapWorkSyncInfo(reader);
            items[item.MetadataWorkId] = item;
        }

        return items;
    }

    public async Task<int> CleanupPendingSyncDownloadsAsync(CancellationToken cancellationToken = default)
    {
        _appPathService.EnsureMetadataDirectory();

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await EnsureTablesAsync(connection, cancellationToken);

        var pendingItems = new List<WorkSyncInfoItem>();
        await using (var readCommand = connection.CreateCommand())
        {
            readCommand.CommandText = $"""
            SELECT Id, {AsmronerConstants.Storage.Sync.WorkSyncInfo.MetadataWorkIdColumn}, SourceId, HasSubtitle, DirSize,
                   {AsmronerConstants.Storage.Sync.WorkSyncInfo.StatusColumn}, FilePath, FailReason, RetryCount,
                   {AsmronerConstants.Storage.Sync.WorkSyncInfo.UpdatedAtColumn}, FailedAt
            FROM {AsmronerConstants.Storage.Sync.WorkSyncInfo.TableName}
            WHERE {AsmronerConstants.Storage.Sync.WorkSyncInfo.StatusColumn} = 'PENDING';
            """;

            await using var reader = await readCommand.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                pendingItems.Add(MapWorkSyncInfo(reader));
            }
        }

        foreach (var item in pendingItems)
        {
            if (!string.IsNullOrWhiteSpace(item.FilePath) && Directory.Exists(item.FilePath))
            {
                Directory.Delete(item.FilePath, recursive: true);
            }
        }

        await using var deleteCommand = connection.CreateCommand();
        deleteCommand.CommandText = $"""
        DELETE FROM {AsmronerConstants.Storage.Sync.WorkSyncInfo.TableName}
        WHERE {AsmronerConstants.Storage.Sync.WorkSyncInfo.StatusColumn} = 'PENDING';
        """;
        await deleteCommand.ExecuteNonQueryAsync(cancellationToken);
        return pendingItems.Count;
    }

    public async Task<IReadOnlyList<MetadataWorkItem>> GetSyncDownloadCandidatesAsync(int count, CancellationToken cancellationToken = default)
    {
        if (count <= 0)
        {
            return Array.Empty<MetadataWorkItem>();
        }

        _appPathService.EnsureMetadataDirectory();

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await EnsureTablesAsync(connection, cancellationToken);

        var items = new List<MetadataWorkItem>(count);
        await using var command = connection.CreateCommand();
        command.CommandText = $"""
        SELECT Id, Title, CircleId, CircleName, Nsfw, Release, DownloadCount, Price, ReviewCount,
               RateCount, RateAverage, HasSubtitle, CreateDate, Vas, Tags, Duration, SourceType,
               {AsmronerConstants.Storage.Sync.MetadataWork.SourceIdColumn},
               {AsmronerConstants.Storage.Sync.MetadataWork.UpdatedAtColumn}
        FROM {AsmronerConstants.Storage.Sync.MetadataWork.TableName} metadata
        WHERE NOT EXISTS (
            SELECT 1
            FROM {AsmronerConstants.Storage.Sync.WorkSyncInfo.TableName} sync
            WHERE sync.{AsmronerConstants.Storage.Sync.WorkSyncInfo.MetadataWorkIdColumn} = metadata.Id)
        ORDER BY metadata.Id
        LIMIT @count;
        """;
        command.Parameters.AddWithValue("@count", count);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            items.Add(new MetadataWorkItem
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                CircleId = reader.GetInt32(2),
                CircleName = reader.GetString(3),
                Nsfw = reader.GetInt64(4) != 0,
                Release = reader.GetString(5),
                DownloadCount = reader.GetInt32(6),
                Price = reader.GetInt32(7),
                ReviewCount = reader.GetInt32(8),
                RateCount = reader.GetInt32(9),
                RateAverage = reader.GetDouble(10),
                HasSubtitle = reader.GetInt64(11) != 0,
                CreateDate = reader.GetString(12),
                Vas = reader.GetString(13),
                Tags = reader.GetString(14),
                Duration = reader.GetInt32(15),
                SourceType = reader.GetString(16),
                SourceId = reader.GetString(17),
                UpdatedAt = ParseTimestamp(reader.GetString(18)) ?? DateTime.UtcNow,
            });
        }

        return items;
    }

    public async Task<IReadOnlyList<WorkSyncInfoItem>> GetFailedSyncDownloadsAsync(CancellationToken cancellationToken = default)
    {
        return await GetSyncDownloadsByStatusAsync("FAILED", cancellationToken);
    }

    public async Task<IReadOnlyList<WorkSyncInfoItem>> GetSyncDownloadsByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(status);

        _appPathService.EnsureMetadataDirectory();

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await EnsureTablesAsync(connection, cancellationToken);

        var items = new List<WorkSyncInfoItem>();
        await using var command = connection.CreateCommand();
        command.CommandText = $"""
        SELECT Id, {AsmronerConstants.Storage.Sync.WorkSyncInfo.MetadataWorkIdColumn}, SourceId, HasSubtitle, DirSize,
               {AsmronerConstants.Storage.Sync.WorkSyncInfo.StatusColumn}, FilePath, FailReason, RetryCount,
               {AsmronerConstants.Storage.Sync.WorkSyncInfo.UpdatedAtColumn}, FailedAt
        FROM {AsmronerConstants.Storage.Sync.WorkSyncInfo.TableName}
         WHERE {AsmronerConstants.Storage.Sync.WorkSyncInfo.StatusColumn} = @status
        ORDER BY COALESCE(FailedAt, {AsmronerConstants.Storage.Sync.WorkSyncInfo.UpdatedAtColumn}), Id;
        """;
        command.Parameters.AddWithValue("@status", status.Trim().ToUpperInvariant());

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            items.Add(MapWorkSyncInfo(reader));
        }

        return items;
    }

    public async Task<WorkSyncInfoItem> CreatePendingWorkSyncInfoAsync(MetadataWorkItem work, string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(work);

        _appPathService.EnsureMetadataDirectory();

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await EnsureTablesAsync(connection, cancellationToken);

        var updatedAt = DateTime.UtcNow;
        await using var command = connection.CreateCommand();
        command.CommandText = $"""
        INSERT INTO {AsmronerConstants.Storage.Sync.WorkSyncInfo.TableName} (
            {AsmronerConstants.Storage.Sync.WorkSyncInfo.MetadataWorkIdColumn},
            SourceId,
            HasSubtitle,
            DirSize,
            {AsmronerConstants.Storage.Sync.WorkSyncInfo.StatusColumn},
            FilePath,
            FailReason,
            RetryCount,
            {AsmronerConstants.Storage.Sync.WorkSyncInfo.UpdatedAtColumn},
            FailedAt)
        VALUES (
            @metadataWorkId,
            @sourceId,
            @hasSubtitle,
            0,
            'PENDING',
            @filePath,
            '',
            0,
            @updatedAt,
            NULL);
        SELECT last_insert_rowid();
        """;
        command.Parameters.AddWithValue("@metadataWorkId", work.Id);
        command.Parameters.AddWithValue("@sourceId", SourceIdNormalizer.Normalize(work.SourceId));
        command.Parameters.AddWithValue("@hasSubtitle", work.HasSubtitle ? 1 : 0);
        command.Parameters.AddWithValue("@filePath", filePath ?? string.Empty);
        command.Parameters.AddWithValue("@updatedAt", updatedAt.ToString("O", CultureInfo.InvariantCulture));

        var insertedId = await command.ExecuteScalarAsync(cancellationToken);
        return new WorkSyncInfoItem
        {
            Id = Convert.ToInt32(insertedId, CultureInfo.InvariantCulture),
            MetadataWorkId = work.Id,
            SourceId = SourceIdNormalizer.Normalize(work.SourceId),
            HasSubtitle = work.HasSubtitle,
            DirSize = 0,
            Status = "PENDING",
            FilePath = filePath ?? string.Empty,
            FailReason = string.Empty,
            RetryCount = 0,
            UpdatedAt = updatedAt,
            FailedAt = null,
        };
    }

    public async Task UpdateWorkSyncInfoAsync(WorkSyncInfoItem item, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);

        _appPathService.EnsureMetadataDirectory();

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await EnsureTablesAsync(connection, cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = $"""
        UPDATE {AsmronerConstants.Storage.Sync.WorkSyncInfo.TableName}
        SET SourceId = @sourceId,
            HasSubtitle = @hasSubtitle,
            DirSize = @dirSize,
            {AsmronerConstants.Storage.Sync.WorkSyncInfo.StatusColumn} = @status,
            FilePath = @filePath,
            FailReason = @failReason,
            RetryCount = @retryCount,
            {AsmronerConstants.Storage.Sync.WorkSyncInfo.UpdatedAtColumn} = @updatedAt,
            FailedAt = @failedAt
        WHERE Id = @id;
        """;
        command.Parameters.AddWithValue("@id", item.Id);
        command.Parameters.AddWithValue("@sourceId", SourceIdNormalizer.Normalize(item.SourceId));
        command.Parameters.AddWithValue("@hasSubtitle", item.HasSubtitle ? 1 : 0);
        command.Parameters.AddWithValue("@dirSize", item.DirSize);
        command.Parameters.AddWithValue("@status", item.Status);
        command.Parameters.AddWithValue("@filePath", item.FilePath);
        command.Parameters.AddWithValue("@failReason", item.FailReason);
        command.Parameters.AddWithValue("@retryCount", item.RetryCount);
        command.Parameters.AddWithValue("@updatedAt", item.UpdatedAt.ToString("O", CultureInfo.InvariantCulture));
        command.Parameters.AddWithValue("@failedAt", item.FailedAt?.ToString("O", CultureInfo.InvariantCulture) ?? (object)DBNull.Value);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private async Task<HashSet<int>> LoadExistingIdsAsync(SqliteConnection connection, IReadOnlyList<int> ids, CancellationToken cancellationToken)
    {
        var existingIds = new HashSet<int>();
        if (ids.Count == 0)
        {
            return existingIds;
        }

        await using var command = connection.CreateCommand();
        var parameters = new List<string>(ids.Count);
        for (var index = 0; index < ids.Count; index++)
        {
            var parameterName = $"@id{index}";
            parameters.Add(parameterName);
            command.Parameters.AddWithValue(parameterName, ids[index]);
        }

        command.CommandText = $"""
        SELECT Id
        FROM {AsmronerConstants.Storage.Sync.MetadataWork.TableName}
        WHERE Id IN ({string.Join(", ", parameters)});
        """;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            existingIds.Add(reader.GetInt32(0));
        }

        return existingIds;
    }

    private static async Task EnsureTablesAsync(SqliteConnection connection, CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = $"""
        {AsmronerConstants.SqliteQueries.BuildCreateMetadataWorkTable()}
        {AsmronerConstants.SqliteQueries.BuildCreateWorkSyncInfoTable()}
        """;
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

    private static DateTime? ParseTimestamp(string raw)
    {
        if (DateTime.TryParse(
            raw,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind,
            out var parsed))
        {
            return parsed;
        }

        return null;
    }

    private static WorkSyncInfoItem MapWorkSyncInfo(SqliteDataReader reader)
    {
        return new WorkSyncInfoItem
        {
            Id = reader.GetInt32(0),
            MetadataWorkId = reader.GetInt32(1),
            SourceId = reader.GetString(2),
            HasSubtitle = reader.GetInt64(3) != 0,
            DirSize = reader.GetInt64(4),
            Status = reader.GetString(5),
            FilePath = reader.GetString(6),
            FailReason = reader.GetString(7),
            RetryCount = reader.GetInt32(8),
            UpdatedAt = ParseTimestamp(reader.GetString(9)) ?? DateTime.UtcNow,
            FailedAt = reader.IsDBNull(10) ? null : ParseTimestamp(reader.GetString(10)),
        };
    }

    private static MetadataWorkItem MapMetadataWork(SqliteDataReader reader)
    {
        return new MetadataWorkItem
        {
            Id = reader.GetInt32(0),
            Title = reader.GetString(1),
            CircleId = reader.GetInt32(2),
            CircleName = reader.GetString(3),
            Nsfw = reader.GetInt64(4) != 0,
            Release = reader.GetString(5),
            DownloadCount = reader.GetInt32(6),
            Price = reader.GetInt32(7),
            ReviewCount = reader.GetInt32(8),
            RateCount = reader.GetInt32(9),
            RateAverage = reader.GetDouble(10),
            HasSubtitle = reader.GetInt64(11) != 0,
            CreateDate = reader.GetString(12),
            Vas = reader.GetString(13),
            Tags = reader.GetString(14),
            Duration = reader.GetInt32(15),
            SourceType = reader.GetString(16),
            SourceId = reader.GetString(17),
            UpdatedAt = ParseTimestamp(reader.GetString(18)) ?? DateTime.UtcNow,
        };
    }
}