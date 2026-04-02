using Asmroner.Core.Constants;
using Asmroner.Core.Favorites;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Utils;
using Microsoft.Data.Sqlite;

namespace Asmroner.Infrastructure.Services;

public sealed class FavoriteStore : IFavoriteStore
{
    private readonly IAppPathService _appPathService;

    public FavoriteStore(IAppPathService appPathService)
    {
        _appPathService = appPathService;
    }

    public async Task<IReadOnlyList<string>> LoadFavoriteFolderTitlesAsync(CancellationToken cancellationToken = default)
    {
        _appPathService.EnsureMetadataDirectory();

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await EnsureTableAsync(connection, cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = AsmronerConstants.SqliteQueries.BuildSelectFavoriteFolderTitles();

        var titles = new List<string>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var title = NormalizeFolderTitle(reader.GetString(0));
            if (!string.IsNullOrWhiteSpace(title))
            {
                titles.Add(title);
            }
        }

        return titles;
    }

    public async Task<IReadOnlyList<FavoriteWorkItem>> LoadFavoriteFolderItemsAsync(string folderTitle, CancellationToken cancellationToken = default)
    {
        var normalizedFolderTitle = NormalizeFolderTitle(folderTitle);
        if (string.IsNullOrWhiteSpace(normalizedFolderTitle))
        {
            return Array.Empty<FavoriteWorkItem>();
        }

        _appPathService.EnsureMetadataDirectory();

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await EnsureTableAsync(connection, cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = AsmronerConstants.SqliteQueries.BuildSelectFavoriteWorkItemsByFolder();
        command.Parameters.AddWithValue("@folderTitle", normalizedFolderTitle);

        var items = new List<FavoriteWorkItem>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            items.Add(new FavoriteWorkItem
            {
                SourceId = reader.GetString(0),
                WorkId = reader.GetInt32(1),
                Title = reader.GetString(2),
            });
        }

        return items;
    }

    public async Task<FavoriteSaveResult> SaveFavoriteFolderItemsAsync(
        string folderTitle,
        IReadOnlyCollection<FavoriteWorkItem> items,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(items);

        var normalizedFolderTitle = NormalizeFolderTitle(folderTitle);
        if (string.IsNullOrWhiteSpace(normalizedFolderTitle))
        {
            throw new ArgumentException("收藏夹标题不能为空。", nameof(folderTitle));
        }

        var rawItems = items
            .Select(static item => new FavoriteWorkItem
            {
                SourceId = SourceIdNormalizer.Normalize(item.SourceId),
                WorkId = item.WorkId,
                Title = item.Title.Trim(),
            })
            .Where(static item => !string.IsNullOrWhiteSpace(item.SourceId))
            .ToArray();

        if (rawItems.Length == 0)
        {
            return new FavoriteSaveResult();
        }

        var uniqueItems = new Dictionary<string, FavoriteWorkItem>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in rawItems)
        {
            uniqueItems[item.SourceId] = item;
        }

        _appPathService.EnsureMetadataDirectory();

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await EnsureTableAsync(connection, cancellationToken);

        var existingSourceIds = await LoadExistingSourceIdsAsync(
            connection,
            normalizedFolderTitle,
            uniqueItems.Keys.ToArray(),
            cancellationToken);

        foreach (var item in uniqueItems.Values)
        {
            await using var command = connection.CreateCommand();
            command.CommandText = AsmronerConstants.SqliteQueries.BuildUpsertFavoriteWork();
            command.Parameters.AddWithValue("@folderTitle", normalizedFolderTitle);
            command.Parameters.AddWithValue("@sourceId", item.SourceId);
            command.Parameters.AddWithValue("@workId", item.WorkId);
            command.Parameters.AddWithValue("@title", item.Title);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        return new FavoriteSaveResult
        {
            AddedCount = uniqueItems.Count - existingSourceIds.Count,
            SkippedCount = existingSourceIds.Count + (rawItems.Length - uniqueItems.Count),
        };
    }

    private async Task<HashSet<string>> LoadExistingSourceIdsAsync(
        SqliteConnection connection,
        string folderTitle,
        IReadOnlyList<string> sourceIds,
        CancellationToken cancellationToken)
    {
        var existing = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (sourceIds.Count == 0)
        {
            return existing;
        }

        await using var command = connection.CreateCommand();
        var parameterNames = new List<string>(sourceIds.Count);
        for (var index = 0; index < sourceIds.Count; index++)
        {
            var parameterName = $"@sourceId{index}";
            parameterNames.Add(parameterName);
            command.Parameters.AddWithValue(parameterName, sourceIds[index]);
        }

        command.CommandText = $"""
        SELECT {AsmronerConstants.Storage.Favorites.SourceIdColumn}
        FROM {AsmronerConstants.Storage.Favorites.TableName}
        WHERE {AsmronerConstants.Storage.Favorites.FolderTitleColumn} = @folderTitle
          AND {AsmronerConstants.Storage.Favorites.SourceIdColumn} IN ({string.Join(", ", parameterNames)});
        """;
        command.Parameters.AddWithValue("@folderTitle", folderTitle);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            existing.Add(reader.GetString(0));
        }

        return existing;
    }

    private static async Task EnsureTableAsync(SqliteConnection connection, CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = AsmronerConstants.SqliteQueries.BuildCreateFavoriteWorkTable();
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

    private static string NormalizeFolderTitle(string? folderTitle)
    {
        return (folderTitle ?? string.Empty).Trim();
    }
}