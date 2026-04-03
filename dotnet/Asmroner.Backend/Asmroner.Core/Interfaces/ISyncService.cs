using Asmroner.Core.Sync;

namespace Asmroner.Core.Interfaces;

public interface ISyncService
{
    Task<SyncReportSnapshot> GetReportAsync(CancellationToken cancellationToken = default);

    Task<MetadataSyncSnapshot> GetMetadataSnapshotAsync(CancellationToken cancellationToken = default);

    Task<MetadataSyncRunResult> SyncMetadataAsync(CancellationToken cancellationToken = default);

    Task<SyncDownloadSnapshot> GetDownloadSnapshotAsync(CancellationToken cancellationToken = default);

    Task<SyncDownloadRunResult> SyncDownloadAsync(CancellationToken cancellationToken = default);

    Task<SyncRetryRunResult> RetryFailedAsync(CancellationToken cancellationToken = default);
}