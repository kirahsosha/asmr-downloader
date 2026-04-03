using Asmroner.Core.Interfaces;
using Asmroner.Core.Sync;

namespace Asmroner.Application.Services;

public sealed class SyncService : ISyncService
{
    private readonly MetadataSyncService _metadataSyncService;
    private readonly SyncDownloadService _syncDownloadService;
    private readonly SyncReportService _syncReportService;

    public SyncService(
        MetadataSyncService metadataSyncService,
        SyncDownloadService syncDownloadService,
        SyncReportService syncReportService)
    {
        _metadataSyncService = metadataSyncService;
        _syncDownloadService = syncDownloadService;
        _syncReportService = syncReportService;
    }

    public Task<SyncReportSnapshot> GetReportAsync(CancellationToken cancellationToken = default)
    {
        return _syncReportService.GetReportAsync(cancellationToken);
    }

    public Task<MetadataSyncSnapshot> GetMetadataSnapshotAsync(CancellationToken cancellationToken = default)
    {
        return _metadataSyncService.GetMetadataSnapshotAsync(cancellationToken);
    }

    public Task<MetadataSyncRunResult> SyncMetadataAsync(CancellationToken cancellationToken = default)
    {
        return _metadataSyncService.SyncMetadataAsync(cancellationToken);
    }

    public Task<SyncDownloadSnapshot> GetDownloadSnapshotAsync(CancellationToken cancellationToken = default)
    {
        return _syncDownloadService.GetSnapshotAsync(cancellationToken);
    }

    public Task<SyncDownloadRunResult> SyncDownloadAsync(CancellationToken cancellationToken = default)
    {
        return _syncDownloadService.SyncDownloadAsync(cancellationToken);
    }

    public Task<SyncRetryRunResult> RetryFailedAsync(CancellationToken cancellationToken = default)
    {
        return _syncDownloadService.RetryFailedAsync(cancellationToken);
    }
}