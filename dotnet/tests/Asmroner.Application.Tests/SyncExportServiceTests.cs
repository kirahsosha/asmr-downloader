using Asmroner.Application.Services;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Sync;

namespace Asmroner.Application.Tests;

public class SyncExportServiceTests
{
    [Fact]
    public async Task ExportAsync_ShouldWriteCsvAndJson_ForRequestedStatus()
    {
        var sut = new SyncExportService(new StubMetadataSyncStore(
        [
            new WorkSyncInfoItem
            {
                Id = 1,
                MetadataWorkId = 101,
                SourceId = "RJF001",
                HasSubtitle = true,
                DirSize = 0,
                Status = "FAILED",
                FilePath = @"C:\sync\failed",
                FailReason = "network, \"reset\"",
                RetryCount = 2,
                UpdatedAt = new DateTime(2026, 4, 3, 1, 2, 3, DateTimeKind.Utc),
                FailedAt = new DateTime(2026, 4, 3, 1, 2, 4, DateTimeKind.Utc),
            },
            new WorkSyncInfoItem
            {
                Id = 2,
                MetadataWorkId = 102,
                SourceId = "RJC001",
                HasSubtitle = false,
                DirSize = 64,
                Status = "COMPLETED",
                FilePath = @"C:\sync\completed",
                FailReason = string.Empty,
                RetryCount = 0,
                UpdatedAt = new DateTime(2026, 4, 3, 1, 3, 0, DateTimeKind.Utc),
                FailedAt = null,
            },
        ]));

        var tempDir = Path.Combine(Path.GetTempPath(), $"asmroner-sync-export-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);

        var failedPath = Path.Combine(tempDir, "failed.csv");
        var completedPath = Path.Combine(tempDir, "completed.json");

        var failedResult = await sut.ExportAsync(SyncExportStatus.Failed, failedPath);
        var completedResult = await sut.ExportAsync(SyncExportStatus.Completed, completedPath);

        Assert.True(File.Exists(failedPath));
        Assert.True(File.Exists(completedPath));
        Assert.Equal(1, failedResult.ExportedCount);
        Assert.Equal(1, completedResult.ExportedCount);
        Assert.Equal("csv", failedResult.Format);
        Assert.Equal("json", completedResult.Format);

        var csvContent = await File.ReadAllTextAsync(failedPath);
        var jsonContent = await File.ReadAllTextAsync(completedPath);
        Assert.Contains("metadata_work_id", csvContent, StringComparison.Ordinal);
        Assert.Contains("RJF001", csvContent, StringComparison.Ordinal);
        Assert.Contains("\"network, \"\"reset\"\"\"", csvContent, StringComparison.Ordinal);
        Assert.DoesNotContain("RJC001", csvContent, StringComparison.Ordinal);
        Assert.Contains("\"source_id\": \"RJC001\"", jsonContent, StringComparison.Ordinal);
        Assert.DoesNotContain("RJF001", jsonContent, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ExportAsync_ShouldReturnNoOp_WhenNoItemsMatchStatus()
    {
        var sut = new SyncExportService(new StubMetadataSyncStore(
        [
            new WorkSyncInfoItem
            {
                Id = 5,
                MetadataWorkId = 205,
                SourceId = "RJC205",
                Status = "COMPLETED",
                FilePath = @"C:\sync\completed",
                UpdatedAt = DateTime.UtcNow,
            },
        ]));
        var tempDir = Path.Combine(Path.GetTempPath(), $"asmroner-sync-export-empty-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);
        var outputPath = Path.Combine(tempDir, "failed.csv");

        var result = await sut.ExportAsync(SyncExportStatus.Failed, outputPath);

        Assert.Equal(0, result.ExportedCount);
        Assert.False(File.Exists(outputPath));
        Assert.Contains("没有可导出的失败同步记录", result.Message, StringComparison.Ordinal);
    }

    private sealed class StubMetadataSyncStore : IMetadataSyncStore
    {
        private readonly IReadOnlyList<WorkSyncInfoItem> _items;

        public StubMetadataSyncStore(IReadOnlyList<WorkSyncInfoItem> items)
        {
            _items = items;
        }

        public Task<MetadataSyncSnapshot> GetMetadataSnapshotAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpsertMetadataWorksAsync(IReadOnlyCollection<MetadataWorkItem> works, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<SyncDownloadSnapshot> GetDownloadSnapshotAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> CleanupPendingSyncDownloadsAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<MetadataWorkItem>> GetSyncDownloadCandidatesAsync(int count, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<WorkSyncInfoItem>> GetFailedSyncDownloadsAsync(CancellationToken cancellationToken = default)
        {
            return GetSyncDownloadsByStatusAsync("FAILED", cancellationToken);
        }

        public Task<IReadOnlyList<WorkSyncInfoItem>> GetSyncDownloadsByStatusAsync(string status, CancellationToken cancellationToken = default)
        {
            var items = _items
                .Where(item => string.Equals(item.Status, status, StringComparison.OrdinalIgnoreCase))
                .ToArray();
            return Task.FromResult<IReadOnlyList<WorkSyncInfoItem>>(items);
        }

        public Task<WorkSyncInfoItem> CreatePendingWorkSyncInfoAsync(MetadataWorkItem work, string filePath, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateWorkSyncInfoAsync(WorkSyncInfoItem item, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}