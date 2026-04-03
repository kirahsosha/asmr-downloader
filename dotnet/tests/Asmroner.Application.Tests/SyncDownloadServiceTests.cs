using Asmroner.Application.Services;
using Asmroner.Core.Api;
using Asmroner.Core.Download;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Sync;

namespace Asmroner.Application.Tests;

public class SyncDownloadServiceTests
{
    [Fact]
    public async Task SyncDownloadAsync_ShouldStopAfterReachingConfiguredSizeLimit()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var metadataWorks = new[]
            {
                CreateMetadataWork(1, "RJ001", "Title 1"),
                CreateMetadataWork(2, "RJ002", "Title 2"),
                CreateMetadataWork(3, "RJ003", "Title 3"),
            };
            var store = new InMemorySyncDownloadStore(metadataWorks);
            var downloadService = new ScriptedSyncDownloadRunner(
                tempRoot,
                new Dictionary<string, SyncDownloadPlan>(StringComparer.OrdinalIgnoreCase)
                {
                    ["RJ001"] = new("Title 1", 70, false),
                    ["RJ002"] = new("Title 2", 60, false),
                    ["RJ003"] = new("Title 3", 50, false),
                });
            var sut = new SyncDownloadService(
                new TestConfigurationService(tempRoot, syncWantedSize: "120B"),
                store,
                downloadService,
                new TestAppPathService(tempRoot));

            var result = await sut.SyncDownloadAsync();
            var snapshot = await sut.GetSnapshotAsync();

            Assert.Equal(2, result.ProcessedCount);
            Assert.Equal(2, result.CompletedCount);
            Assert.Equal(0, result.FailedCount);
            Assert.True(result.ReachedSizeLimit);
            Assert.Equal(130, result.CompletedSizeBytesAfter);
            Assert.Equal(1, result.RemainingMetadataCountAfter);
            Assert.Equal(2, snapshot.CompletedCount);
            Assert.Equal(1, snapshot.RemainingMetadataCount);
            Assert.Equal(["RJ001", "RJ002"], downloadService.RequestedSourceIds);
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    [Fact]
    public async Task SyncDownloadAsync_ShouldMarkFailedItems_AndContinueToNextCandidate()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var metadataWorks = new[]
            {
                CreateMetadataWork(1, "RJ101", "Failed Work"),
                CreateMetadataWork(2, "RJ102", "Completed Work"),
            };
            var store = new InMemorySyncDownloadStore(metadataWorks);
            var downloadService = new ScriptedSyncDownloadRunner(
                tempRoot,
                new Dictionary<string, SyncDownloadPlan>(StringComparer.OrdinalIgnoreCase)
                {
                    ["RJ101"] = new("Failed Work", 0, true),
                    ["RJ102"] = new("Completed Work", 80, false),
                });
            var sut = new SyncDownloadService(
                new TestConfigurationService(tempRoot, syncWantedSize: "1KB"),
                store,
                downloadService,
                new TestAppPathService(tempRoot));

            var result = await sut.SyncDownloadAsync();
            var snapshot = await sut.GetSnapshotAsync();

            Assert.Equal(2, result.ProcessedCount);
            Assert.Equal(1, result.CompletedCount);
            Assert.Equal(1, result.FailedCount);
            Assert.False(result.ReachedSizeLimit);
            Assert.Equal(80, result.CompletedSizeBytesAfter);
            Assert.Equal(0, result.RemainingMetadataCountAfter);
            Assert.Equal(1, snapshot.CompletedCount);
            Assert.Equal(1, snapshot.FailedCount);
            Assert.Contains("剩余待同步 0 项", result.Message, StringComparison.Ordinal);
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    [Fact]
    public async Task RetryFailedAsync_ShouldReDownloadFailedItems_AndIncrementRetryCount()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var metadataWorks = new[]
            {
                CreateMetadataWork(1, "RJ201", "Recovered Work"),
            };
            var store = new InMemorySyncDownloadStore(metadataWorks);
            var stalePath = Path.Combine(tempRoot, "stale-failed");
            Directory.CreateDirectory(stalePath);
            await File.WriteAllBytesAsync(Path.Combine(stalePath, "stale.bin"), new byte[8]);
            store.SeedSyncInfo(new WorkSyncInfoItem
            {
                Id = 1,
                MetadataWorkId = metadataWorks[0].Id,
                SourceId = metadataWorks[0].SourceId,
                HasSubtitle = false,
                DirSize = 0,
                Status = "FAILED",
                FilePath = stalePath,
                FailReason = "initial failure",
                RetryCount = 1,
                UpdatedAt = DateTime.UtcNow.AddMinutes(-5),
                FailedAt = DateTime.UtcNow.AddMinutes(-5),
            });

            var downloadService = new ScriptedSyncDownloadRunner(
                tempRoot,
                new Dictionary<string, SyncDownloadPlan>(StringComparer.OrdinalIgnoreCase)
                {
                    ["RJ201"] = new("Recovered Work", 32, false),
                });
            var sut = new SyncDownloadService(
                new TestConfigurationService(tempRoot, syncWantedSize: "1KB"),
                store,
                downloadService,
                new TestAppPathService(tempRoot));

            var result = await sut.RetryFailedAsync();
            var snapshot = await sut.GetSnapshotAsync();
            var updated = store.GetSyncInfo(metadataWorks[0].Id);

            Assert.NotNull(updated);
            Assert.Equal(1, result.RetriedCount);
            Assert.Equal(1, result.RecoveredCount);
            Assert.Equal(0, result.FailedAgainCount);
            Assert.Equal(1, result.FailedCountBefore);
            Assert.Equal(0, result.FailedCountAfter);
            Assert.Equal(1, snapshot.CompletedCount);
            Assert.Equal(0, snapshot.FailedCount);
            Assert.Equal("COMPLETED", updated!.Status);
            Assert.Equal(2, updated.RetryCount);
            Assert.Equal(string.Empty, updated.FailReason);
            Assert.Null(updated.FailedAt);
            Assert.NotEqual(stalePath, updated.FilePath);
            Assert.False(Directory.Exists(stalePath));
            Assert.Equal(["RJ201"], downloadService.RequestedSourceIds);
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    [Fact]
    public async Task RetryFailedAsync_ShouldKeepFailedStatus_WhenRetryFailsAgain()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var metadataWorks = new[]
            {
                CreateMetadataWork(1, "RJ301", "Still Failed"),
            };
            var store = new InMemorySyncDownloadStore(metadataWorks);
            var stalePath = Path.Combine(tempRoot, "stale-failed-again");
            Directory.CreateDirectory(stalePath);
            await File.WriteAllBytesAsync(Path.Combine(stalePath, "stale.bin"), new byte[4]);
            store.SeedSyncInfo(new WorkSyncInfoItem
            {
                Id = 1,
                MetadataWorkId = metadataWorks[0].Id,
                SourceId = metadataWorks[0].SourceId,
                HasSubtitle = false,
                DirSize = 0,
                Status = "FAILED",
                FilePath = stalePath,
                FailReason = "previous failure",
                RetryCount = 2,
                UpdatedAt = DateTime.UtcNow.AddMinutes(-3),
                FailedAt = DateTime.UtcNow.AddMinutes(-3),
            });

            var downloadService = new ScriptedSyncDownloadRunner(
                tempRoot,
                new Dictionary<string, SyncDownloadPlan>(StringComparer.OrdinalIgnoreCase)
                {
                    ["RJ301"] = new("Still Failed", 0, true),
                });
            var sut = new SyncDownloadService(
                new TestConfigurationService(tempRoot, syncWantedSize: "1KB"),
                store,
                downloadService,
                new TestAppPathService(tempRoot));

            var result = await sut.RetryFailedAsync();
            var snapshot = await sut.GetSnapshotAsync();
            var updated = store.GetSyncInfo(metadataWorks[0].Id);

            Assert.NotNull(updated);
            Assert.Equal(1, result.RetriedCount);
            Assert.Equal(0, result.RecoveredCount);
            Assert.Equal(1, result.FailedAgainCount);
            Assert.Equal(1, result.FailedCountBefore);
            Assert.Equal(1, result.FailedCountAfter);
            Assert.Equal(0, snapshot.CompletedCount);
            Assert.Equal(1, snapshot.FailedCount);
            Assert.Equal("FAILED", updated!.Status);
            Assert.Equal(3, updated.RetryCount);
            Assert.Contains("mock download failed", updated.FailReason, StringComparison.Ordinal);
            Assert.NotNull(updated.FailedAt);
            Assert.NotEqual(stalePath, updated.FilePath);
            Assert.False(Directory.Exists(stalePath));
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    private static MetadataWorkItem CreateMetadataWork(int id, string sourceId, string title)
    {
        return new MetadataWorkItem
        {
            Id = id,
            SourceId = sourceId,
            Title = title,
            UpdatedAt = DateTime.UtcNow,
        };
    }

    private static string CreateTempRoot()
    {
        var path = Path.Combine(Path.GetTempPath(), "asmroner-sync-download-tests", Guid.NewGuid().ToString("N"));
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

    private sealed record SyncDownloadPlan(string Title, int SizeBytes, bool ShouldFail);

    private sealed class ScriptedSyncDownloadRunner : IDownloadService
    {
        private readonly string _targetRoot;
        private readonly IReadOnlyDictionary<string, SyncDownloadPlan> _plans;

        public ScriptedSyncDownloadRunner(string targetRoot, IReadOnlyDictionary<string, SyncDownloadPlan> plans)
        {
            _targetRoot = targetRoot;
            _plans = plans;
        }

        public List<string> RequestedSourceIds { get; } = [];

        public Task<IReadOnlyList<DownloadTaskItem>> RunQueuedAsync(string? fileFilter = null, bool hdAudioOnly = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<DownloadTaskItem?> StartAsync(string sourceId, string? fileFilter = null, Guid? preferredTaskId = null, bool hdAudioOnly = false, CancellationToken cancellationToken = default)
        {
            RequestedSourceIds.Add(sourceId);
            var plan = _plans[sourceId];
            var targetDirectory = SyncDownloadPathPolicy.BuildTargetDirectory(_targetRoot, sourceId, plan.Title);
            Directory.CreateDirectory(targetDirectory);

            if (plan.ShouldFail)
            {
                return Task.FromResult<DownloadTaskItem?>(new DownloadTaskItem
                {
                    SourceId = sourceId,
                    Title = plan.Title,
                    Status = DownloadTaskStatus.Failed,
                    ErrorMessage = "mock download failed",
                    TargetDirectory = targetDirectory,
                });
            }

            File.WriteAllBytes(Path.Combine(targetDirectory, "payload.bin"), new byte[plan.SizeBytes]);
            return Task.FromResult<DownloadTaskItem?>(new DownloadTaskItem
            {
                SourceId = sourceId,
                Title = plan.Title,
                Status = DownloadTaskStatus.Completed,
                TargetDirectory = targetDirectory,
            });
        }

        public IReadOnlyList<DownloadTaskItem> GetTasks()
        {
            return Array.Empty<DownloadTaskItem>();
        }

        public Task<bool> CancelAsync(Guid taskId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        public Task<DownloadTaskItem?> RetryFailedAsync(Guid taskId, string? fileFilter = null, bool hdAudioOnly = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task ClearAllTasksAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void UpsertPrefetchedWorkInfo(IReadOnlyDictionary<string, WorkInfoDto> workInfos, WorkInfoCacheEntryLevel cacheLevel = WorkInfoCacheEntryLevel.Summary)
        {
        }

        public IReadOnlyDictionary<string, WorkInfoDto> GetPrefetchedWorkInfoSnapshot()
        {
            return new Dictionary<string, WorkInfoDto>(StringComparer.OrdinalIgnoreCase);
        }
    }

    private sealed class InMemorySyncDownloadStore : IMetadataSyncStore
    {
        private readonly Dictionary<int, MetadataWorkItem> _works;
        private readonly Dictionary<int, WorkSyncInfoItem> _syncInfos = [];
        private int _nextId = 1;

        public InMemorySyncDownloadStore(IEnumerable<MetadataWorkItem> works)
        {
            _works = works.ToDictionary(static item => item.Id);
        }

        public Task<MetadataSyncSnapshot> GetMetadataSnapshotAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new MetadataSyncSnapshot
            {
                LocalTotalCount = _works.Count,
                LocalSubtitleCount = _works.Values.Count(static item => item.HasSubtitle),
                LastUpdatedAt = _works.Count == 0 ? null : _works.Values.Max(static item => item.UpdatedAt),
            });
        }

        public Task<int> UpsertMetadataWorksAsync(IReadOnlyCollection<MetadataWorkItem> works, CancellationToken cancellationToken = default)
        {
            var insertedCount = 0;
            foreach (var work in works)
            {
                if (!_works.ContainsKey(work.Id))
                {
                    insertedCount++;
                }

                _works[work.Id] = work;
            }

            return Task.FromResult(insertedCount);
        }

        public Task<SyncDownloadSnapshot> GetDownloadSnapshotAsync(CancellationToken cancellationToken = default)
        {
            var items = _syncInfos.Values.ToArray();
            return Task.FromResult(new SyncDownloadSnapshot
            {
                PendingCount = items.Count(static item => item.Status == "PENDING"),
                CompletedCount = items.Count(static item => item.Status == "COMPLETED"),
                FailedCount = items.Count(static item => item.Status == "FAILED"),
                RemainingMetadataCount = _works.Keys.Count(id => !_syncInfos.ContainsKey(id)),
                CompletedSizeBytes = items.Where(static item => item.Status == "COMPLETED").Sum(static item => item.DirSize),
                LastUpdatedAt = items.Length == 0 ? null : items.Max(static item => item.UpdatedAt),
            });
        }

        public Task<int> CleanupPendingSyncDownloadsAsync(CancellationToken cancellationToken = default)
        {
            var removedIds = _syncInfos
                .Where(static pair => pair.Value.Status == "PENDING")
                .Select(static pair => pair.Key)
                .ToArray();
            foreach (var key in removedIds)
            {
                _syncInfos.Remove(key);
            }

            return Task.FromResult(removedIds.Length);
        }

        public Task<IReadOnlyList<MetadataWorkItem>> GetSyncDownloadCandidatesAsync(int count, CancellationToken cancellationToken = default)
        {
            var items = _works.Values
                .Where(work => !_syncInfos.ContainsKey(work.Id))
                .OrderBy(static work => work.Id)
                .Take(count)
                .ToArray();
            return Task.FromResult<IReadOnlyList<MetadataWorkItem>>(items);
        }

        public Task<IReadOnlyList<WorkSyncInfoItem>> GetFailedSyncDownloadsAsync(CancellationToken cancellationToken = default)
        {
            var items = _syncInfos.Values
                .Where(static item => item.Status == "FAILED")
                .OrderBy(static item => item.FailedAt ?? item.UpdatedAt)
                .ThenBy(static item => item.Id)
                .ToArray();
            return Task.FromResult<IReadOnlyList<WorkSyncInfoItem>>(items);
        }

        public Task<IReadOnlyList<WorkSyncInfoItem>> GetSyncDownloadsByStatusAsync(string status, CancellationToken cancellationToken = default)
        {
            var items = _syncInfos.Values
                .Where(item => string.Equals(item.Status, status, StringComparison.OrdinalIgnoreCase))
                .OrderBy(static item => item.FailedAt ?? item.UpdatedAt)
                .ThenBy(static item => item.Id)
                .ToArray();
            return Task.FromResult<IReadOnlyList<WorkSyncInfoItem>>(items);
        }

        public Task<WorkSyncInfoItem> CreatePendingWorkSyncInfoAsync(MetadataWorkItem work, string filePath, CancellationToken cancellationToken = default)
        {
            var item = new WorkSyncInfoItem
            {
                Id = _nextId++,
                MetadataWorkId = work.Id,
                SourceId = work.SourceId,
                HasSubtitle = work.HasSubtitle,
                Status = "PENDING",
                FilePath = filePath,
                UpdatedAt = DateTime.UtcNow,
            };
            _syncInfos[work.Id] = item;
            return Task.FromResult(item);
        }

        public Task UpdateWorkSyncInfoAsync(WorkSyncInfoItem item, CancellationToken cancellationToken = default)
        {
            _syncInfos[item.MetadataWorkId] = item;
            return Task.CompletedTask;
        }

        public void SeedSyncInfo(WorkSyncInfoItem item)
        {
            _syncInfos[item.MetadataWorkId] = item;
            _nextId = Math.Max(_nextId, item.Id + 1);
        }

        public WorkSyncInfoItem? GetSyncInfo(int metadataWorkId)
        {
            return _syncInfos.TryGetValue(metadataWorkId, out var item) ? item : null;
        }
    }
}