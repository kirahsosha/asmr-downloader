using Asmroner.Application.Services;
using Asmroner.Core.Api;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Sync;

namespace Asmroner.Application.Tests;

public class MetadataSyncServiceTests
{
    [Fact]
    public async Task SyncMetadataAsync_ShouldInsertAllPages_WhenRemoteHasNewWorks()
    {
        var uiStateStore = new InMemoryUiStateStore();
        var apiClient = new RecordingMetadataApiClient
        {
            RemoteTotalCount = 101,
            RemoteSubtitleCount = 1,
            Pages =
            {
                [1] = BuildPage(startId: 101, count: 100, subtitleIndex: 0),
                [2] =
                [
                    CreateWork(201, "RJ201", "Title 201"),
                ],
            },
        };
        var store = new InMemoryMetadataSyncStore();
        var sut = new MetadataSyncService(
            apiClient,
            new TestConfigurationService(Path.GetTempPath()),
            store,
            new NoopRateLimiterService(),
            uiStateStore);

        var result = await sut.SyncMetadataAsync();
        var snapshot = await sut.GetMetadataSnapshotAsync();
        var progress = await uiStateStore.LoadMetadataSyncProgressAsync();

        Assert.Equal(101, result.RemoteTotalCount);
        Assert.Equal(1, result.RemoteSubtitleCount);
        Assert.Equal(101, result.InsertedCount);
        Assert.Equal(101, result.ProcessedWorkCount);
        Assert.Equal(101, result.LocalTotalCountAfter);
        Assert.True(result.IsUpToDate);
        Assert.Equal(101, snapshot.LocalTotalCount);
        Assert.Equal(1, snapshot.LocalSubtitleCount);
        Assert.Equal(101, progress.LocalTotalCount);
        Assert.Equal(1, progress.LocalSubtitleCount);
        Assert.Equal(101, progress.ProcessedWorkCount);
        Assert.Equal([(1, 1, false), (1, 1, true), (1, 100, false), (2, 100, false)], apiClient.Calls);
    }

    [Fact]
    public async Task SyncMetadataAsync_ShouldTrackProcessedWorks_WhenExistingPagesContainOnlyUpdates()
    {
        var uiStateStore = new InMemoryUiStateStore();
        var apiClient = new RecordingMetadataApiClient
        {
            RemoteTotalCount = 101,
            RemoteSubtitleCount = 1,
            Pages =
            {
                [1] = BuildPage(startId: 101, count: 100, subtitleIndex: 0),
                [2] =
                [
                    CreateWork(201, "RJ201", "Title 201"),
                ],
            },
        };
        var existingWorks = Enumerable.Range(101, 100)
            .Select(id => new MetadataWorkItem
            {
                Id = id,
                SourceId = $"RJ{id}",
                Title = $"Stale Title {id}",
                HasSubtitle = false,
                UpdatedAt = DateTime.UtcNow.AddDays(-7),
            })
            .ToArray();
        var store = new InMemoryMetadataSyncStore(existingWorks);
        var sut = new MetadataSyncService(
            apiClient,
            new TestConfigurationService(Path.GetTempPath()),
            store,
            new NoopRateLimiterService(),
            uiStateStore);

        var result = await sut.SyncMetadataAsync();
        var progress = await uiStateStore.LoadMetadataSyncProgressAsync();
        var refreshed = await store.GetMetadataWorksBySourceIdsAsync(["RJ101", "RJ201"]);

        Assert.Equal(1, result.InsertedCount);
        Assert.Equal(101, result.ProcessedWorkCount);
        Assert.Equal(101, result.LocalTotalCountAfter);
        Assert.Equal(1, progress.InsertedCount);
        Assert.Equal(101, progress.ProcessedWorkCount);
        Assert.Equal("Title 101", refreshed["RJ101"].Title);
        Assert.True(refreshed["RJ101"].HasSubtitle);
        Assert.Equal("Title 201", refreshed["RJ201"].Title);
    }

    [Fact]
    public async Task SyncMetadataAsync_ShouldSkip_WhenRemoteCountMatchesLocalCount()
    {
        var uiStateStore = new InMemoryUiStateStore();
        var apiClient = new RecordingMetadataApiClient
        {
            RemoteTotalCount = 2,
            RemoteSubtitleCount = 1,
        };
        var store = new InMemoryMetadataSyncStore(
        [
            new MetadataWorkItem
            {
                Id = 201,
                SourceId = "RJ201",
                Title = "Local 201",
                HasSubtitle = true,
                UpdatedAt = DateTime.UtcNow,
            },
            new MetadataWorkItem
            {
                Id = 202,
                SourceId = "RJ202",
                Title = "Local 202",
                UpdatedAt = DateTime.UtcNow,
            },
        ]);
        var sut = new MetadataSyncService(
            apiClient,
            new TestConfigurationService(Path.GetTempPath()),
            store,
            new NoopRateLimiterService(),
            uiStateStore);

        var result = await sut.SyncMetadataAsync();

        Assert.Equal(0, result.InsertedCount);
        Assert.Equal(0, result.ProcessedWorkCount);
        Assert.Equal(0, result.ProcessedPageCount);
        Assert.True(result.IsUpToDate);
        Assert.Contains("无需同步", result.Message, StringComparison.Ordinal);
        Assert.Equal([(1, 1, false), (1, 1, true)], apiClient.Calls);
    }

    [Fact]
    public async Task SyncMetadataAsync_ShouldRefreshExpiredMetadata_WhenPreviousRunCompleted()
    {
        var uiStateStore = new InMemoryUiStateStore();
        await uiStateStore.SaveMetadataSyncProgressAsync(new MetadataSyncProgressState
        {
            Status = SyncProgressStatuses.Completed,
            NextPage = 1,
            ProcessedPageCount = 1,
            TotalPageCount = 1,
            RemoteTotalCount = 1,
            RemoteSubtitleCount = 0,
            InsertedCount = 1,
            StartedAt = DateTime.UtcNow.AddDays(-2),
            UpdatedAt = DateTime.UtcNow.AddDays(-1),
        });

        var apiClient = new RecordingMetadataApiClient
        {
            RemoteTotalCount = 1,
            RemoteSubtitleCount = 0,
            Pages =
            {
                [1] =
                [
                    CreateWork(401, "RJ401", "Updated Title 401"),
                ],
            },
        };
        var store = new InMemoryMetadataSyncStore(
        [
            new MetadataWorkItem
            {
                Id = 401,
                SourceId = "RJ401",
                Title = "Stale Title 401",
                UpdatedAt = DateTime.UtcNow.AddDays(-45),
            },
        ]);
        var sut = new MetadataSyncService(
            apiClient,
            new TestConfigurationService(Path.GetTempPath(), metadataValidityDays: 30),
            store,
            new NoopRateLimiterService(),
            uiStateStore);

        var result = await sut.SyncMetadataAsync();
        var refreshed = await store.GetMetadataWorksBySourceIdsAsync(new[] { "RJ401" });

        Assert.Equal(0, result.InsertedCount);
        Assert.Equal(1, result.ProcessedWorkCount);
        Assert.Contains("过期刷新完成", result.Message, StringComparison.Ordinal);
        Assert.Equal([(1, 1, false), (1, 1, true), (1, 100, false)], apiClient.Calls);
        Assert.Equal("Updated Title 401", refreshed["RJ401"].Title);
    }

    [Fact]
    public async Task SyncMetadataAsync_ShouldReport_WhenLocalCountExceedsRemoteCount()
    {
        var uiStateStore = new InMemoryUiStateStore();
        var apiClient = new RecordingMetadataApiClient
        {
            RemoteTotalCount = 1,
            RemoteSubtitleCount = 0,
        };
        var store = new InMemoryMetadataSyncStore(
        [
            new MetadataWorkItem
            {
                Id = 301,
                SourceId = "RJ301",
                Title = "Local 301",
                UpdatedAt = DateTime.UtcNow,
            },
            new MetadataWorkItem
            {
                Id = 302,
                SourceId = "RJ302",
                Title = "Local 302",
                UpdatedAt = DateTime.UtcNow,
            },
        ]);
        var sut = new MetadataSyncService(
            apiClient,
            new TestConfigurationService(Path.GetTempPath()),
            store,
            new NoopRateLimiterService(),
            uiStateStore);

        var result = await sut.SyncMetadataAsync();

        Assert.Equal(0, result.InsertedCount);
        Assert.Equal(0, result.ProcessedWorkCount);
        Assert.False(result.IsUpToDate);
        Assert.Contains("本地元数据数量高于网站", result.Message, StringComparison.Ordinal);
        Assert.Equal([(1, 1, false), (1, 1, true)], apiClient.Calls);
    }

    [Fact]
    public async Task SyncMetadataAsync_ShouldResumeFromSavedProgress_WhenStateIsUnfinished()
    {
        var uiStateStore = new InMemoryUiStateStore();
        await uiStateStore.SaveMetadataSyncProgressAsync(new MetadataSyncProgressState
        {
            Status = SyncProgressStatuses.Stopped,
            NextPage = 2,
            ProcessedPageCount = 1,
            TotalPageCount = 2,
            RemoteTotalCount = 101,
            RemoteSubtitleCount = 1,
            InsertedCount = 100,
            ProcessedWorkCount = 100,
            StartedAt = DateTime.UtcNow.AddMinutes(-10),
            UpdatedAt = DateTime.UtcNow.AddMinutes(-1),
        });

        var apiClient = new RecordingMetadataApiClient
        {
            RemoteTotalCount = 101,
            RemoteSubtitleCount = 1,
            Pages =
            {
                [1] = BuildPage(startId: 101, count: 100, subtitleIndex: 0),
                [2] =
                [
                    CreateWork(201, "RJ201", "Title 201"),
                ],
            },
        };
        var existingWorks = Enumerable.Range(101, 100)
            .Select((id, index) => new MetadataWorkItem
            {
                Id = id,
                SourceId = $"RJ{id}",
                Title = $"Title {id}",
                HasSubtitle = index == 0,
                UpdatedAt = DateTime.UtcNow.AddMinutes(-20),
            })
            .ToArray();
        var store = new InMemoryMetadataSyncStore(existingWorks);
        var sut = new MetadataSyncService(
            apiClient,
            new TestConfigurationService(Path.GetTempPath()),
            store,
            new NoopRateLimiterService(),
            uiStateStore);

        var result = await sut.SyncMetadataAsync();
        var progress = await uiStateStore.LoadMetadataSyncProgressAsync();

        Assert.True(result.ResumedFromProgress);
        Assert.False(result.WasStopped);
        Assert.Equal(1, result.InsertedCount);
        Assert.Equal(1, result.ProcessedWorkCount);
        Assert.Equal(2, result.ProcessedPageCount);
        Assert.Equal(1, result.NextPage);
        Assert.Equal(SyncProgressStatuses.Completed, progress.Status);
        Assert.Equal(101, progress.InsertedCount);
        Assert.Equal(101, progress.LocalTotalCount);
        Assert.Equal(1, progress.LocalSubtitleCount);
        Assert.Equal(101, progress.ProcessedWorkCount);
        Assert.Equal([(1, 1, false), (1, 1, true), (2, 100, false)], apiClient.Calls);
    }

    [Fact]
    public async Task SyncMetadataAsync_ShouldStopAfterCurrentPage_WhenStopRequested()
    {
        var uiStateStore = new InMemoryUiStateStore();
        var apiClient = new RecordingMetadataApiClient
        {
            RemoteTotalCount = 101,
            RemoteSubtitleCount = 1,
            Pages =
            {
                [1] = BuildPage(startId: 101, count: 100, subtitleIndex: 0),
                [2] =
                [
                    CreateWork(201, "RJ201", "Title 201"),
                ],
            },
        };
        var store = new InMemoryMetadataSyncStore();
        store.OnUpsertAsync = async _ => await uiStateStore.RequestStopMetadataSyncAsync();
        var sut = new MetadataSyncService(
            apiClient,
            new TestConfigurationService(Path.GetTempPath()),
            store,
            new NoopRateLimiterService(),
            uiStateStore);

        var result = await sut.SyncMetadataAsync();
        var progress = await uiStateStore.LoadMetadataSyncProgressAsync();

        Assert.True(result.WasStopped);
        Assert.False(result.IsUpToDate);
        Assert.Equal(100, result.InsertedCount);
        Assert.Equal(100, result.ProcessedWorkCount);
        Assert.Equal(1, result.ProcessedPageCount);
        Assert.Equal(2, result.NextPage);
        Assert.Equal(SyncProgressStatuses.Stopped, progress.Status);
        Assert.Equal(2, progress.NextPage);
        Assert.Equal(100, progress.LocalTotalCount);
        Assert.Equal(1, progress.LocalSubtitleCount);
        Assert.Equal(100, progress.ProcessedWorkCount);
        Assert.Equal([(1, 1, false), (1, 1, true), (1, 100, false)], apiClient.Calls);
    }

    private static MetadataSyncWorkDto CreateWork(int id, string sourceId, string title, bool hasSubtitle = false)
    {
        return new MetadataSyncWorkDto
        {
            Id = id,
            SourceId = sourceId,
            Title = title,
            CircleId = 1,
            Name = "Circle",
            Release = "2026-04-02",
            HasSubtitle = hasSubtitle,
            Vas = [new MetadataSyncVaDto { Id = "1", Name = "VA" }],
            Tags = [new TagDto { Id = 1, Name = "tag" }],
            SourceType = "DLSITE",
        };
    }

    private static MetadataSyncWorkDto[] BuildPage(int startId, int count, int subtitleIndex)
    {
        return Enumerable.Range(startId, count)
            .Select((id, index) => CreateWork(
                id,
                $"RJ{id}",
                $"Title {id}",
                hasSubtitle: index == subtitleIndex))
            .ToArray();
    }

    private sealed class RecordingMetadataApiClient : IAsmrApiClient
    {
        public int RemoteTotalCount { get; init; }

        public int RemoteSubtitleCount { get; init; }

        public Dictionary<int, MetadataSyncWorkDto[]> Pages { get; } = new();

        public List<(int Page, int PageSize, bool SubtitleOnly)> Calls { get; } = [];

        public Task<MetadataSyncPageDto> GetMetadataWorksAsync(int page, int pageSize, bool subtitleOnly = false, CancellationToken cancellationToken = default)
        {
            Calls.Add((page, pageSize, subtitleOnly));

            if (subtitleOnly)
            {
                return Task.FromResult(new MetadataSyncPageDto
                {
                    Pagination = new MetadataSyncPaginationDto
                    {
                        CurrentPage = page,
                        PageSize = pageSize,
                        TotalCount = RemoteSubtitleCount,
                    },
                });
            }

            Pages.TryGetValue(page, out var works);
            works ??= Array.Empty<MetadataSyncWorkDto>();

            return Task.FromResult(new MetadataSyncPageDto
            {
                Works = pageSize == 1 ? works.Take(1).ToArray() : works,
                Pagination = new MetadataSyncPaginationDto
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = RemoteTotalCount,
                },
            });
        }

        public Task<WorkInfoDto> GetWorkInfoAsync(string id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<TrackDto>> GetTracksAsync(string id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DownloadFileAsync(string url, string destinationPath, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<SearchResultDto> SearchAsync(string query, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<SearchWorkDto>> GetPopularAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    private sealed class InMemoryMetadataSyncStore : IMetadataSyncStore
    {
        private readonly Dictionary<int, MetadataWorkItem> _works;
        private readonly Dictionary<int, WorkSyncInfoItem> _syncInfos = [];
        private int _nextSyncInfoId = 1;

        public Func<int, Task>? OnUpsertAsync { get; set; }

        public InMemoryMetadataSyncStore(IEnumerable<MetadataWorkItem>? works = null)
        {
            _works = (works ?? Array.Empty<MetadataWorkItem>())
                .ToDictionary(static work => work.Id);
        }

        public Task<MetadataSyncSnapshot> GetMetadataSnapshotAsync(CancellationToken cancellationToken = default)
        {
            var values = _works.Values.ToArray();
            return Task.FromResult(new MetadataSyncSnapshot
            {
                LocalTotalCount = values.Length,
                LocalSubtitleCount = values.Count(static work => work.HasSubtitle),
                LastUpdatedAt = values.Length == 0 ? null : values.Max(static work => work.UpdatedAt),
            });
        }

        public async Task<int> UpsertMetadataWorksAsync(IReadOnlyCollection<MetadataWorkItem> works, CancellationToken cancellationToken = default)
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

            if (OnUpsertAsync is not null)
            {
                await OnUpsertAsync(insertedCount);
            }

            return insertedCount;
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
            var syncInfos = _syncInfos.Values.ToArray();
            return Task.FromResult(new SyncDownloadSnapshot
            {
                PendingCount = syncInfos.Count(static item => item.Status == "PENDING"),
                CompletedCount = syncInfos.Count(static item => item.Status == "COMPLETED"),
                FailedCount = syncInfos.Count(static item => item.Status == "FAILED"),
                RemainingMetadataCount = _works.Keys.Count(id => !_syncInfos.ContainsKey(id)),
                CompletedSizeBytes = syncInfos
                    .Where(static item => item.Status == "COMPLETED")
                    .Sum(static item => item.DirSize),
                LastUpdatedAt = syncInfos.Length == 0 ? null : syncInfos.Max(static item => item.UpdatedAt),
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

            foreach (var metadataWorkId in removedIds)
            {
                _syncInfos.Remove(metadataWorkId);
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
                Id = _nextSyncInfoId++,
                MetadataWorkId = work.Id,
                SourceId = work.SourceId,
                HasSubtitle = work.HasSubtitle,
                DirSize = 0,
                Status = "PENDING",
                FilePath = filePath,
                FailReason = string.Empty,
                RetryCount = 0,
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
    }
}