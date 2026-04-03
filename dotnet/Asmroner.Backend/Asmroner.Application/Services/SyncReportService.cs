using System.Linq;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Sync;

namespace Asmroner.Application.Services;

public sealed class SyncReportService
{
    private readonly IMetadataSyncStore _metadataSyncStore;

    public SyncReportService(IMetadataSyncStore metadataSyncStore)
    {
        _metadataSyncStore = metadataSyncStore;
    }

    public async Task<SyncReportSnapshot> GetReportAsync(CancellationToken cancellationToken = default)
    {
        var metadataSnapshotTask = _metadataSyncStore.GetMetadataSnapshotAsync(cancellationToken);
        var downloadSnapshotTask = _metadataSyncStore.GetDownloadSnapshotAsync(cancellationToken);
        var completedItemsTask = _metadataSyncStore.GetSyncDownloadsByStatusAsync("COMPLETED", cancellationToken);

        await Task.WhenAll(metadataSnapshotTask, downloadSnapshotTask, completedItemsTask);

        var metadataSnapshot = await metadataSnapshotTask;
        var downloadSnapshot = await downloadSnapshotTask;
        var completedItems = await completedItemsTask;

        var metadataWithoutSubtitleCount = Math.Max(0, metadataSnapshot.LocalTotalCount - metadataSnapshot.LocalSubtitleCount);
        var completedSubtitleCount = completedItems.Count(static item => item.HasSubtitle);
        var completedWithoutSubtitleCount = Math.Max(0, downloadSnapshot.CompletedCount - completedSubtitleCount);

        return new SyncReportSnapshot
        {
            MetadataTotalCount = metadataSnapshot.LocalTotalCount,
            MetadataSubtitleCount = metadataSnapshot.LocalSubtitleCount,
            MetadataWithoutSubtitleCount = metadataWithoutSubtitleCount,
            CompletedCount = downloadSnapshot.CompletedCount,
            CompletedSubtitleCount = completedSubtitleCount,
            CompletedWithoutSubtitleCount = completedWithoutSubtitleCount,
            FailedCount = downloadSnapshot.FailedCount,
            PendingCount = downloadSnapshot.PendingCount,
            RemainingMetadataCount = downloadSnapshot.RemainingMetadataCount,
            CompletedSizeBytes = downloadSnapshot.CompletedSizeBytes,
            OverallProgressPercent = CalculateProgress(downloadSnapshot.CompletedCount, metadataSnapshot.LocalTotalCount),
            SubtitleProgressPercent = CalculateProgress(completedSubtitleCount, metadataSnapshot.LocalSubtitleCount),
            WithoutSubtitleProgressPercent = CalculateProgress(completedWithoutSubtitleCount, metadataWithoutSubtitleCount),
            MetadataUpdatedAt = metadataSnapshot.LastUpdatedAt,
            DownloadUpdatedAt = downloadSnapshot.LastUpdatedAt,
        };
    }

    private static double CalculateProgress(int completedCount, int totalCount)
    {
        if (totalCount <= 0)
        {
            return 0;
        }

        return completedCount * 100d / totalCount;
    }
}