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
            new NoopRateLimiterService());

        var result = await sut.SyncMetadataAsync();
        var snapshot = await sut.GetMetadataSnapshotAsync();

        Assert.Equal(101, result.RemoteTotalCount);
        Assert.Equal(1, result.RemoteSubtitleCount);
        Assert.Equal(101, result.InsertedCount);
        Assert.Equal(101, result.LocalTotalCountAfter);
        Assert.True(result.IsUpToDate);
        Assert.Equal(101, snapshot.LocalTotalCount);
        Assert.Equal(1, snapshot.LocalSubtitleCount);
        Assert.Equal([(1, 1, false), (1, 1, true), (1, 100, false), (2, 100, false)], apiClient.Calls);
    }

    [Fact]
    public async Task SyncMetadataAsync_ShouldSkip_WhenRemoteCountMatchesLocalCount()
    {
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
            new NoopRateLimiterService());

        var result = await sut.SyncMetadataAsync();

        Assert.Equal(0, result.InsertedCount);
        Assert.Equal(0, result.ProcessedPageCount);
        Assert.True(result.IsUpToDate);
        Assert.Contains("无需同步", result.Message, StringComparison.Ordinal);
        Assert.Equal([(1, 1, false), (1, 1, true)], apiClient.Calls);
    }

    [Fact]
    public async Task SyncMetadataAsync_ShouldReport_WhenLocalCountExceedsRemoteCount()
    {
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
            new NoopRateLimiterService());

        var result = await sut.SyncMetadataAsync();

        Assert.Equal(0, result.InsertedCount);
        Assert.False(result.IsUpToDate);
        Assert.Contains("本地元数据数量高于网站", result.Message, StringComparison.Ordinal);
        Assert.Equal([(1, 1, false), (1, 1, true)], apiClient.Calls);
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