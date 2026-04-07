using Asmroner.Core.Sync;

namespace Asmroner.Core.Interfaces;

public interface IMetadataSyncStore
{
    Task<MetadataSyncSnapshot> GetMetadataSnapshotAsync(CancellationToken cancellationToken = default);

    Task<int> UpsertMetadataWorksAsync(IReadOnlyCollection<MetadataWorkItem> works, CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<string, MetadataWorkItem>> GetMetadataWorksBySourceIdsAsync(IReadOnlyCollection<string> sourceIds, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<int>> GetExpiredMetadataWorkIdsAsync(DateTime updatedBefore, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MetadataWorkItem>> GetAllMetadataWorksAsync(CancellationToken cancellationToken = default);

    Task<SyncDownloadSnapshot> GetDownloadSnapshotAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<int, WorkSyncInfoItem>> GetWorkSyncInfoMapAsync(CancellationToken cancellationToken = default);

    Task<int> CleanupPendingSyncDownloadsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MetadataWorkItem>> GetSyncDownloadCandidatesAsync(int count, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WorkSyncInfoItem>> GetFailedSyncDownloadsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WorkSyncInfoItem>> GetSyncDownloadsByStatusAsync(string status, CancellationToken cancellationToken = default);

    Task<WorkSyncInfoItem> CreatePendingWorkSyncInfoAsync(MetadataWorkItem work, string filePath, CancellationToken cancellationToken = default);

    Task UpdateWorkSyncInfoAsync(WorkSyncInfoItem item, CancellationToken cancellationToken = default);
}