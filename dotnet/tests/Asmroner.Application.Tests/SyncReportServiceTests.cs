using Asmroner.Application.Services;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Sync;

namespace Asmroner.Application.Tests;

public class SyncReportServiceTests
{
    [Fact]
    public async Task GetReportAsync_ShouldBuildBreakdownAndProgress_FromSnapshots()
    {
        var metadataUpdatedAt = new DateTime(2026, 4, 3, 2, 0, 0, DateTimeKind.Utc);
        var downloadUpdatedAt = new DateTime(2026, 4, 3, 2, 5, 0, DateTimeKind.Utc);
        var sut = new SyncReportService(new StubMetadataSyncStore(
            new MetadataSyncSnapshot
            {
                LocalTotalCount = 10,
                LocalSubtitleCount = 4,
                LastUpdatedAt = metadataUpdatedAt,
            },
            new SyncDownloadSnapshot
            {
                CompletedCount = 3,
                FailedCount = 2,
                PendingCount = 1,
                RemainingMetadataCount = 7,
                CompletedSizeBytes = 512,
                LastUpdatedAt = downloadUpdatedAt,
            },
            [
                new WorkSyncInfoItem { Status = "COMPLETED", HasSubtitle = true },
                new WorkSyncInfoItem { Status = "COMPLETED", HasSubtitle = true },
                new WorkSyncInfoItem { Status = "COMPLETED", HasSubtitle = false },
            ]));

        var report = await sut.GetReportAsync();

        Assert.Equal(10, report.MetadataTotalCount);
        Assert.Equal(4, report.MetadataSubtitleCount);
        Assert.Equal(6, report.MetadataWithoutSubtitleCount);
        Assert.Equal(3, report.CompletedCount);
        Assert.Equal(2, report.CompletedSubtitleCount);
        Assert.Equal(1, report.CompletedWithoutSubtitleCount);
        Assert.Equal(2, report.FailedCount);
        Assert.Equal(1, report.PendingCount);
        Assert.Equal(7, report.RemainingMetadataCount);
        Assert.Equal(512, report.CompletedSizeBytes);
        Assert.Equal(30d, report.OverallProgressPercent);
        Assert.Equal(50d, report.SubtitleProgressPercent);
        Assert.Equal(16.67d, Math.Round(report.WithoutSubtitleProgressPercent, 2));
        Assert.Equal(metadataUpdatedAt, report.MetadataUpdatedAt);
        Assert.Equal(downloadUpdatedAt, report.DownloadUpdatedAt);
    }

    [Fact]
    public async Task GetReportAsync_ShouldReturnZeroProgress_WhenMetadataIsEmpty()
    {
        var sut = new SyncReportService(new StubMetadataSyncStore(
            new MetadataSyncSnapshot(),
            new SyncDownloadSnapshot
            {
                CompletedCount = 0,
                FailedCount = 1,
                PendingCount = 0,
                RemainingMetadataCount = 0,
            },
            Array.Empty<WorkSyncInfoItem>()));

        var report = await sut.GetReportAsync();

        Assert.Equal(0, report.MetadataTotalCount);
        Assert.Equal(0, report.MetadataSubtitleCount);
        Assert.Equal(0, report.MetadataWithoutSubtitleCount);
        Assert.Equal(0d, report.OverallProgressPercent);
        Assert.Equal(0d, report.SubtitleProgressPercent);
        Assert.Equal(0d, report.WithoutSubtitleProgressPercent);
    }

    private sealed class StubMetadataSyncStore : IMetadataSyncStore
    {
        private readonly MetadataSyncSnapshot _metadataSnapshot;
        private readonly SyncDownloadSnapshot _downloadSnapshot;
        private readonly IReadOnlyList<WorkSyncInfoItem> _completedItems;

        public StubMetadataSyncStore(
            MetadataSyncSnapshot metadataSnapshot,
            SyncDownloadSnapshot downloadSnapshot,
            IReadOnlyList<WorkSyncInfoItem> completedItems)
        {
            _metadataSnapshot = metadataSnapshot;
            _downloadSnapshot = downloadSnapshot;
            _completedItems = completedItems;
        }

        public Task<MetadataSyncSnapshot> GetMetadataSnapshotAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_metadataSnapshot);
        }

        public Task<int> UpsertMetadataWorksAsync(IReadOnlyCollection<MetadataWorkItem> works, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyDictionary<string, MetadataWorkItem>> GetMetadataWorksBySourceIdsAsync(IReadOnlyCollection<string> sourceIds, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyDictionary<string, MetadataWorkItem>>(
                new Dictionary<string, MetadataWorkItem>(StringComparer.OrdinalIgnoreCase));
        }

        public Task<IReadOnlyList<int>> GetExpiredMetadataWorkIdsAsync(DateTime updatedBefore, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<int>>(Array.Empty<int>());
        }

        public Task<IReadOnlyList<MetadataWorkItem>> GetAllMetadataWorksAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<MetadataWorkItem>>(Array.Empty<MetadataWorkItem>());
        }

        public Task<SyncDownloadSnapshot> GetDownloadSnapshotAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_downloadSnapshot);
        }

        public Task<IReadOnlyDictionary<int, WorkSyncInfoItem>> GetWorkSyncInfoMapAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyDictionary<int, WorkSyncInfoItem>>(
                new Dictionary<int, WorkSyncInfoItem>());
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
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<WorkSyncInfoItem>> GetSyncDownloadsByStatusAsync(string status, CancellationToken cancellationToken = default)
        {
            var items = string.Equals(status, "COMPLETED", StringComparison.OrdinalIgnoreCase)
                ? _completedItems
                : Array.Empty<WorkSyncInfoItem>();
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