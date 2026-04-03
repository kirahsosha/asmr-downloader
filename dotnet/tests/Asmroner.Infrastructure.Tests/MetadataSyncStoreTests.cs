using Asmroner.Core.Sync;
using Asmroner.Infrastructure.Services;

namespace Asmroner.Infrastructure.Tests;

public class MetadataSyncStoreTests
{
    [Fact]
    public async Task MetadataSyncStore_ShouldTrackSyncDownloadSnapshot_AndCleanupPendingRows()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var pathService = new AppPathService(tempRoot);
            var sut = new MetadataSyncStore(pathService);
            var works = new[]
            {
                new MetadataWorkItem { Id = 1, SourceId = "RJ001", Title = "Title 1", UpdatedAt = DateTime.UtcNow },
                new MetadataWorkItem { Id = 2, SourceId = "RJ002", Title = "Title 2", UpdatedAt = DateTime.UtcNow },
            };

            await sut.UpsertMetadataWorksAsync(works);

            var candidates = await sut.GetSyncDownloadCandidatesAsync(10);
            Assert.Equal(2, candidates.Count);

            var completedPath = Path.Combine(tempRoot, "sync-completed");
            Directory.CreateDirectory(completedPath);
            await File.WriteAllBytesAsync(Path.Combine(completedPath, "payload.bin"), new byte[40]);

            var pendingCompleted = await sut.CreatePendingWorkSyncInfoAsync(works[0], completedPath);
            await sut.UpdateWorkSyncInfoAsync(new WorkSyncInfoItem
            {
                Id = pendingCompleted.Id,
                MetadataWorkId = works[0].Id,
                SourceId = works[0].SourceId,
                HasSubtitle = false,
                DirSize = 40,
                Status = "COMPLETED",
                FilePath = completedPath,
                FailReason = string.Empty,
                RetryCount = 0,
                UpdatedAt = DateTime.UtcNow,
                FailedAt = null,
            });

            var pendingPath = Path.Combine(tempRoot, "sync-pending");
            Directory.CreateDirectory(pendingPath);
            await File.WriteAllBytesAsync(Path.Combine(pendingPath, "pending.bin"), new byte[5]);
            await sut.CreatePendingWorkSyncInfoAsync(works[1], pendingPath);

            var beforeCleanup = await sut.GetDownloadSnapshotAsync();
            Assert.Equal(1, beforeCleanup.CompletedCount);
            Assert.Equal(1, beforeCleanup.PendingCount);
            Assert.Equal(0, beforeCleanup.RemainingMetadataCount);
            Assert.Equal(40, beforeCleanup.CompletedSizeBytes);

            var removedCount = await sut.CleanupPendingSyncDownloadsAsync();
            var afterCleanup = await sut.GetDownloadSnapshotAsync();

            Assert.Equal(1, removedCount);
            Assert.False(Directory.Exists(pendingPath));
            Assert.Equal(1, afterCleanup.CompletedCount);
            Assert.Equal(0, afterCleanup.PendingCount);
            Assert.Equal(1, afterCleanup.RemainingMetadataCount);
            Assert.Equal(40, afterCleanup.CompletedSizeBytes);
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    [Fact]
    public async Task MetadataSyncStore_ShouldReturnFailedSyncDownloads_ForRetry()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var pathService = new AppPathService(tempRoot);
            var sut = new MetadataSyncStore(pathService);
            var works = new[]
            {
                new MetadataWorkItem { Id = 11, SourceId = "RJ011", Title = "Failed Work", UpdatedAt = DateTime.UtcNow },
                new MetadataWorkItem { Id = 12, SourceId = "RJ012", Title = "Completed Work", UpdatedAt = DateTime.UtcNow },
            };

            await sut.UpsertMetadataWorksAsync(works);

            var failedPending = await sut.CreatePendingWorkSyncInfoAsync(works[0], Path.Combine(tempRoot, "failed-target"));
            await sut.UpdateWorkSyncInfoAsync(new WorkSyncInfoItem
            {
                Id = failedPending.Id,
                MetadataWorkId = works[0].Id,
                SourceId = works[0].SourceId,
                HasSubtitle = false,
                DirSize = 0,
                Status = "FAILED",
                FilePath = Path.Combine(tempRoot, "failed-target"),
                FailReason = "network error",
                RetryCount = 2,
                UpdatedAt = DateTime.UtcNow,
                FailedAt = DateTime.UtcNow,
            });

            var completedPending = await sut.CreatePendingWorkSyncInfoAsync(works[1], Path.Combine(tempRoot, "completed-target"));
            await sut.UpdateWorkSyncInfoAsync(new WorkSyncInfoItem
            {
                Id = completedPending.Id,
                MetadataWorkId = works[1].Id,
                SourceId = works[1].SourceId,
                HasSubtitle = false,
                DirSize = 64,
                Status = "COMPLETED",
                FilePath = Path.Combine(tempRoot, "completed-target"),
                FailReason = string.Empty,
                RetryCount = 0,
                UpdatedAt = DateTime.UtcNow,
                FailedAt = null,
            });

            var failedItems = await sut.GetFailedSyncDownloadsAsync();
            var failedItem = Assert.Single(failedItems);

            Assert.Equal(works[0].Id, failedItem.MetadataWorkId);
            Assert.Equal("FAILED", failedItem.Status);
            Assert.Equal("network error", failedItem.FailReason);
            Assert.Equal(2, failedItem.RetryCount);
            Assert.NotNull(failedItem.FailedAt);
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    [Fact]
    public async Task MetadataSyncStore_ShouldReturnSyncDownloadsByStatus_ForExport()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var pathService = new AppPathService(tempRoot);
            var sut = new MetadataSyncStore(pathService);
            var works = new[]
            {
                new MetadataWorkItem { Id = 21, SourceId = "RJ021", Title = "Completed Work", UpdatedAt = DateTime.UtcNow },
                new MetadataWorkItem { Id = 22, SourceId = "RJ022", Title = "Failed Work", UpdatedAt = DateTime.UtcNow },
            };

            await sut.UpsertMetadataWorksAsync(works);

            var completedPending = await sut.CreatePendingWorkSyncInfoAsync(works[0], Path.Combine(tempRoot, "completed-export"));
            await sut.UpdateWorkSyncInfoAsync(new WorkSyncInfoItem
            {
                Id = completedPending.Id,
                MetadataWorkId = works[0].Id,
                SourceId = works[0].SourceId,
                HasSubtitle = false,
                DirSize = 128,
                Status = "COMPLETED",
                FilePath = Path.Combine(tempRoot, "completed-export"),
                FailReason = string.Empty,
                RetryCount = 0,
                UpdatedAt = DateTime.UtcNow,
                FailedAt = null,
            });

            var failedPending = await sut.CreatePendingWorkSyncInfoAsync(works[1], Path.Combine(tempRoot, "failed-export"));
            await sut.UpdateWorkSyncInfoAsync(new WorkSyncInfoItem
            {
                Id = failedPending.Id,
                MetadataWorkId = works[1].Id,
                SourceId = works[1].SourceId,
                HasSubtitle = true,
                DirSize = 0,
                Status = "FAILED",
                FilePath = Path.Combine(tempRoot, "failed-export"),
                FailReason = "network",
                RetryCount = 1,
                UpdatedAt = DateTime.UtcNow,
                FailedAt = DateTime.UtcNow,
            });

            var completedItems = await sut.GetSyncDownloadsByStatusAsync("COMPLETED");
            var failedItems = await sut.GetSyncDownloadsByStatusAsync("FAILED");

            Assert.Single(completedItems);
            Assert.Single(failedItems);
            Assert.Equal("COMPLETED", completedItems[0].Status);
            Assert.Equal(128, completedItems[0].DirSize);
            Assert.Equal("FAILED", failedItems[0].Status);
            Assert.Equal("network", failedItems[0].FailReason);
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    private static string CreateTempRoot()
    {
        var path = Path.Combine(Path.GetTempPath(), "asmroner-metadata-sync-store-tests", Guid.NewGuid().ToString("N"));
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