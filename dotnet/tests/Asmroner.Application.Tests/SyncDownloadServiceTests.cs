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
            var uiStateStore = new InMemoryUiStateStore();
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
                new InMemorySyncWorkInfoResolver(metadataWorks),
                downloadService,
                new TestAppPathService(tempRoot),
                uiStateStore);

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
            var uiStateStore = new InMemoryUiStateStore();
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
                new InMemorySyncWorkInfoResolver(metadataWorks),
                downloadService,
                new TestAppPathService(tempRoot),
                uiStateStore);

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
    public async Task SyncDownloadAsync_ShouldRescanAllWorks_WhenPreviousRunCompleted()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var uiStateStore = new InMemoryUiStateStore();
            await uiStateStore.SaveSyncDownloadProgressAsync(new SyncDownloadProgressState
            {
                Status = SyncProgressStatuses.Completed,
                LastProcessedSourceId = string.Empty,
                ProcessedCount = 2,
                CompletedCount = 2,
                FailedCount = 0,
                RemainingMetadataCountAfter = 0,
                CompletedSizeBytesBefore = 200,
                CompletedSizeBytesAfter = 200,
                SizeLimitBytes = 1,
                StartedAt = DateTime.UtcNow.AddHours(-2),
                UpdatedAt = DateTime.UtcNow.AddMinutes(-10),
            });

            var metadataWorks = new[]
            {
                CreateMetadataWork(11, "RJ511", "Rescan Title 1"),
                CreateMetadataWork(12, "RJ512", "Rescan Title 2"),
            };
            var store = new InMemorySyncDownloadStore(metadataWorks);
            store.SeedSyncInfo(new WorkSyncInfoItem
            {
                Id = 1,
                MetadataWorkId = metadataWorks[0].Id,
                SourceId = metadataWorks[0].SourceId,
                HasSubtitle = false,
                DirSize = 100,
                Status = "COMPLETED",
                FilePath = Path.Combine(tempRoot, "seed-1"),
                RetryCount = 0,
                UpdatedAt = DateTime.UtcNow.AddDays(-1),
            });
            store.SeedSyncInfo(new WorkSyncInfoItem
            {
                Id = 2,
                MetadataWorkId = metadataWorks[1].Id,
                SourceId = metadataWorks[1].SourceId,
                HasSubtitle = false,
                DirSize = 100,
                Status = "COMPLETED",
                FilePath = Path.Combine(tempRoot, "seed-2"),
                RetryCount = 0,
                UpdatedAt = DateTime.UtcNow.AddDays(-1),
            });

            var downloadService = new ScriptedSyncDownloadRunner(
                tempRoot,
                new Dictionary<string, SyncDownloadPlan>(StringComparer.OrdinalIgnoreCase)
                {
                    ["RJ511"] = new("Rescan Title 1", 70, false),
                    ["RJ512"] = new("Rescan Title 2", 60, false),
                });
            var sut = new SyncDownloadService(
                new TestConfigurationService(tempRoot, syncWantedSize: "1B"),
                store,
                new InMemorySyncWorkInfoResolver(metadataWorks),
                downloadService,
                new TestAppPathService(tempRoot),
                uiStateStore);

            var result = await sut.SyncDownloadAsync();

            Assert.Equal(2, result.ProcessedCount);
            Assert.Equal(2, result.CompletedCount);
            Assert.Equal(0, result.FailedCount);
            Assert.Equal(0, result.RemainingMetadataCountAfter);
            Assert.Contains("校验完成", result.Message, StringComparison.Ordinal);
            Assert.Equal(["RJ511", "RJ512"], downloadService.RequestedSourceIds);
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    [Fact]
    public async Task SyncDownloadAsync_ShouldPassMetadataWorkId_ToDownloadService()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var uiStateStore = new InMemoryUiStateStore();
            var metadataWorks = new[]
            {
                CreateMetadataWork(100000062, "BJ02370869", "BJ Title"),
            };
            var store = new InMemorySyncDownloadStore(metadataWorks);
            var downloadService = new ScriptedSyncDownloadRunner(
                tempRoot,
                new Dictionary<string, SyncDownloadPlan>(StringComparer.OrdinalIgnoreCase)
                {
                    ["BJ02370869"] = new("BJ Title", 32, false),
                });
            var sut = new SyncDownloadService(
                new TestConfigurationService(tempRoot, syncWantedSize: "1KB"),
                store,
                new InMemorySyncWorkInfoResolver(metadataWorks),
                downloadService,
                new TestAppPathService(tempRoot),
                uiStateStore);

            var result = await sut.SyncDownloadAsync();

            Assert.Equal(1, result.ProcessedCount);
            Assert.Equal(1, result.CompletedCount);
            Assert.Equal(["BJ02370869"], downloadService.RequestedSourceIds);
            Assert.Equal([100000062], downloadService.RequestedWorkIds);
            Assert.All(downloadService.RequestedPurposes, static purpose => Assert.Equal(DownloadExecutionPurpose.SyncManaged, purpose));
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    [Fact]
    public async Task SyncDownloadAsync_ShouldUseUnifiedWorkInfoDto_WhenCreatingPendingSyncInfo()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var uiStateStore = new InMemoryUiStateStore();
            var metadataWorks = new[]
            {
                CreateMetadataWork(621, "RJ621", "Stored Title"),
            };
            var store = new InMemorySyncDownloadStore(metadataWorks);
            var workInfoResolver = new InMemorySyncWorkInfoResolver(
            [
                new WorkInfoDto
                {
                    Id = 621,
                    SourceId = "RJ621",
                    Title = "Resolved Title",
                    HasSubtitle = true,
                },
            ]);
            var downloadService = new ScriptedSyncDownloadRunner(
                tempRoot,
                new Dictionary<string, SyncDownloadPlan>(StringComparer.OrdinalIgnoreCase)
                {
                    ["RJ621"] = new("Resolved Title", 24, false),
                });
            var sut = new SyncDownloadService(
                new TestConfigurationService(tempRoot, syncWantedSize: "1KB"),
                store,
                workInfoResolver,
                downloadService,
                new TestAppPathService(tempRoot),
                uiStateStore);

            var result = await sut.SyncDownloadAsync();

            Assert.Equal(1, result.ProcessedCount);
            Assert.NotNull(store.LastCreatedPendingWork);
            Assert.Equal("Resolved Title", store.LastCreatedPendingWork!.Title);
            Assert.True(store.LastCreatedPendingWork.HasSubtitle);
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
            var uiStateStore = new InMemoryUiStateStore();
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
                new InMemorySyncWorkInfoResolver(metadataWorks),
                downloadService,
                new TestAppPathService(tempRoot),
                uiStateStore);

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
            var uiStateStore = new InMemoryUiStateStore();
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
                new InMemorySyncWorkInfoResolver(metadataWorks),
                downloadService,
                new TestAppPathService(tempRoot),
                uiStateStore);

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

    [Fact]
    public async Task SyncDownloadAsync_ShouldStopAfterCurrentWork_WhenStopRequested()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var uiStateStore = new InMemoryUiStateStore();
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
                },
                async (_, callIndex) =>
                {
                    if (callIndex == 1)
                    {
                        await uiStateStore.RequestStopSyncDownloadAsync();
                    }
                });
            var sut = new SyncDownloadService(
                new TestConfigurationService(tempRoot, syncWantedSize: "1KB"),
                store,
                new InMemorySyncWorkInfoResolver(metadataWorks),
                downloadService,
                new TestAppPathService(tempRoot),
                uiStateStore);

            var result = await sut.SyncDownloadAsync();
            var progress = await uiStateStore.LoadSyncDownloadProgressAsync();

            Assert.True(result.WasStopped);
            Assert.Equal(1, result.ProcessedCount);
            Assert.Equal(1, result.CompletedCount);
            Assert.Equal(0, result.FailedCount);
            Assert.Equal(2, result.RemainingMetadataCountAfter);
            Assert.Equal("RJ001", result.LastProcessedSourceId);
            Assert.Equal(SyncProgressStatuses.Stopped, progress.Status);
            Assert.Equal("RJ001", progress.LastProcessedSourceId);
            Assert.Equal(["RJ001"], downloadService.RequestedSourceIds);
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    [Fact]
    public async Task SyncDownloadAsync_ShouldResumeFromSavedProgress_WhenStateIsUnfinished()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var uiStateStore = new InMemoryUiStateStore();
            await uiStateStore.SaveSyncDownloadProgressAsync(new SyncDownloadProgressState
            {
                Status = SyncProgressStatuses.Stopped,
                LastProcessedSourceId = "RJ001",
                ProcessedCount = 1,
                CompletedCount = 1,
                FailedCount = 0,
                RemainingMetadataCountAfter = 2,
                CompletedSizeBytesBefore = 0,
                CompletedSizeBytesAfter = 70,
                SizeLimitBytes = 1024,
                StartedAt = DateTime.UtcNow.AddMinutes(-10),
                UpdatedAt = DateTime.UtcNow.AddMinutes(-1),
            });

            var metadataWorks = new[]
            {
                CreateMetadataWork(1, "RJ001", "Title 1"),
                CreateMetadataWork(2, "RJ002", "Title 2"),
                CreateMetadataWork(3, "RJ003", "Title 3"),
            };
            var store = new InMemorySyncDownloadStore(metadataWorks);
            store.SeedSyncInfo(new WorkSyncInfoItem
            {
                Id = 1,
                MetadataWorkId = 1,
                SourceId = "RJ001",
                HasSubtitle = false,
                DirSize = 70,
                Status = "COMPLETED",
                FilePath = Path.Combine(tempRoot, "existing"),
                FailReason = string.Empty,
                RetryCount = 0,
                UpdatedAt = DateTime.UtcNow.AddMinutes(-8),
            });

            var downloadService = new ScriptedSyncDownloadRunner(
                tempRoot,
                new Dictionary<string, SyncDownloadPlan>(StringComparer.OrdinalIgnoreCase)
                {
                    ["RJ002"] = new("Title 2", 60, false),
                    ["RJ003"] = new("Title 3", 50, false),
                });
            var sut = new SyncDownloadService(
                new TestConfigurationService(tempRoot, syncWantedSize: "1KB"),
                store,
                new InMemorySyncWorkInfoResolver(metadataWorks),
                downloadService,
                new TestAppPathService(tempRoot),
                uiStateStore);

            var result = await sut.SyncDownloadAsync();
            var progress = await uiStateStore.LoadSyncDownloadProgressAsync();

            Assert.True(result.ResumedFromProgress);
            Assert.False(result.WasStopped);
            Assert.Equal(2, result.ProcessedCount);
            Assert.Equal(2, result.CompletedCount);
            Assert.Equal(0, result.FailedCount);
            Assert.Equal(70, result.CompletedSizeBytesBefore);
            Assert.Equal(180, result.CompletedSizeBytesAfter);
            Assert.Equal(0, result.RemainingMetadataCountAfter);
            Assert.Equal(SyncProgressStatuses.Completed, progress.Status);
            Assert.Equal(3, progress.ProcessedCount);
            Assert.Equal(3, progress.CompletedCount);
            Assert.Equal(["RJ002", "RJ003"], downloadService.RequestedSourceIds);
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

    private sealed class InMemorySyncWorkInfoResolver : ISyncWorkInfoResolver
    {
        private readonly IReadOnlyList<WorkInfoDto> _workInfos;

        public InMemorySyncWorkInfoResolver(IEnumerable<MetadataWorkItem> metadataWorks)
            : this(metadataWorks.Select(static work => new WorkInfoDto
            {
                Id = work.Id,
                SourceId = work.SourceId,
                Title = work.Title,
                Release = work.Release,
                HasSubtitle = work.HasSubtitle,
            }))
        {
        }

        public InMemorySyncWorkInfoResolver(IEnumerable<WorkInfoDto> workInfos)
        {
            _workInfos = workInfos
                .OrderBy(static work => work.Id)
                .ToArray();
        }

        public Task<IReadOnlyList<WorkInfoDto>> ResolveAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_workInfos);
        }
    }

    private sealed class ScriptedSyncDownloadRunner : IDownloadService
    {
        private readonly string _targetRoot;
        private readonly IReadOnlyDictionary<string, SyncDownloadPlan> _plans;
        private readonly Func<string, int, Task>? _onStartAsync;
        private int _startCallCount;

        public ScriptedSyncDownloadRunner(
            string targetRoot,
            IReadOnlyDictionary<string, SyncDownloadPlan> plans,
            Func<string, int, Task>? onStartAsync = null)
        {
            _targetRoot = targetRoot;
            _plans = plans;
            _onStartAsync = onStartAsync;
        }

        public List<string> RequestedSourceIds { get; } = [];

        public List<int?> RequestedWorkIds { get; } = [];

        public List<DownloadExecutionPurpose> RequestedPurposes { get; } = [];

        public Task<IReadOnlyList<DownloadTaskItem>> RunQueuedAsync(string? fileFilter = null, bool hdAudioOnly = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<DownloadTaskItem?> StartAsync(string sourceId, string? fileFilter = null, Guid? preferredTaskId = null, bool hdAudioOnly = false, DownloadStartOptions? options = null, CancellationToken cancellationToken = default)
        {
            RequestedSourceIds.Add(sourceId);
            RequestedWorkIds.Add(options?.WorkId);
            RequestedPurposes.Add(options?.Purpose ?? DownloadExecutionPurpose.Standard);
            var startCallCount = Interlocked.Increment(ref _startCallCount);
            var plan = _plans[sourceId];
            var targetDirectory = SyncDownloadPathPolicy.BuildTargetDirectory(options?.TargetRoot ?? _targetRoot, sourceId, plan.Title);
            Directory.CreateDirectory(targetDirectory);

            if (_onStartAsync is not null)
            {
                await _onStartAsync(sourceId, startCallCount);
            }

            if (plan.ShouldFail)
            {
                return new DownloadTaskItem
                {
                    SourceId = sourceId,
                    Title = plan.Title,
                    Status = DownloadTaskStatus.Failed,
                    ErrorMessage = "mock download failed",
                    TargetDirectory = targetDirectory,
                };
            }

            File.WriteAllBytes(Path.Combine(targetDirectory, "payload.bin"), new byte[plan.SizeBytes]);
            return new DownloadTaskItem
            {
                SourceId = sourceId,
                Title = plan.Title,
                Status = DownloadTaskStatus.Completed,
                TargetDirectory = targetDirectory,
            };
        }

        public IReadOnlyList<DownloadTaskItem> GetTasks()
        {
            return Array.Empty<DownloadTaskItem>();
        }

        public Task<bool> CancelAsync(Guid taskId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        public Task<DownloadTaskItem?> RetryFailedAsync(Guid taskId, string? fileFilter = null, bool hdAudioOnly = false, DownloadStartOptions? options = null, CancellationToken cancellationToken = default)
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

        public MetadataWorkItem? LastCreatedPendingWork { get; private set; }

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

        public Task<IReadOnlyDictionary<string, MetadataWorkItem>> GetMetadataWorksBySourceIdsAsync(IReadOnlyCollection<string> sourceIds, CancellationToken cancellationToken = default)
        {
            var normalized = sourceIds
                .Where(static sourceId => !string.IsNullOrWhiteSpace(sourceId))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            var items = _works.Values
                .Where(work => normalized.Contains(work.SourceId))
                .ToDictionary(static work => work.SourceId, StringComparer.OrdinalIgnoreCase);
            return Task.FromResult<IReadOnlyDictionary<string, MetadataWorkItem>>(items);
        }

        public Task<IReadOnlyList<int>> GetExpiredMetadataWorkIdsAsync(DateTime updatedBefore, CancellationToken cancellationToken = default)
        {
            var items = _works.Values
                .Where(work => work.UpdatedAt < updatedBefore)
                .Select(static work => work.Id)
                .OrderBy(static id => id)
                .ToArray();
            return Task.FromResult<IReadOnlyList<int>>(items);
        }

        public Task<IReadOnlyList<MetadataWorkItem>> GetAllMetadataWorksAsync(CancellationToken cancellationToken = default)
        {
            var items = _works.Values
                .OrderBy(static work => work.Id)
                .ToArray();
            return Task.FromResult<IReadOnlyList<MetadataWorkItem>>(items);
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

        public Task<IReadOnlyDictionary<int, WorkSyncInfoItem>> GetWorkSyncInfoMapAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyDictionary<int, WorkSyncInfoItem>>(
                new Dictionary<int, WorkSyncInfoItem>(_syncInfos));
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
            LastCreatedPendingWork = work;
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