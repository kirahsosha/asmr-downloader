using Asmroner.Core.Interfaces;
using Microsoft.Data.Sqlite;

namespace Asmroner.Infrastructure.Services;

public sealed class DatabaseInitializer : IDatabaseInitializer
{
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

        await using var command = connection.CreateCommand();
        command.CommandText =
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
            """;

        await command.ExecuteNonQueryAsync(cancellationToken);
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