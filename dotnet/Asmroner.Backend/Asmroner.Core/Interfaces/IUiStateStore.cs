using Asmroner.Core.Configuration;
using Asmroner.Core.Sync;

namespace Asmroner.Core.Interfaces;

public interface IUiStateStore
{
    Task<SearchUiState> LoadSearchUiStateAsync(CancellationToken cancellationToken = default);

    Task SaveSearchUiStateAsync(SearchUiState state, CancellationToken cancellationToken = default);

    Task<DownloadUiState> LoadDownloadUiStateAsync(CancellationToken cancellationToken = default);

    Task SaveDownloadUiStateAsync(DownloadUiState state, CancellationToken cancellationToken = default);

    Task<MetadataSyncProgressState> LoadMetadataSyncProgressAsync(CancellationToken cancellationToken = default);

    Task SaveMetadataSyncProgressAsync(MetadataSyncProgressState state, CancellationToken cancellationToken = default);

    Task RequestStopMetadataSyncAsync(CancellationToken cancellationToken = default);

    Task<SyncDownloadProgressState> LoadSyncDownloadProgressAsync(CancellationToken cancellationToken = default);

    Task SaveSyncDownloadProgressAsync(SyncDownloadProgressState state, CancellationToken cancellationToken = default);

    Task RequestStopSyncDownloadAsync(CancellationToken cancellationToken = default);

    Task<SyncUiState> LoadSyncUiStateAsync(CancellationToken cancellationToken = default);

    Task SaveSyncUiStateAsync(SyncUiState state, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> LoadUnfinishedQueueAsync(CancellationToken cancellationToken = default);

    Task SaveUnfinishedQueueAsync(IReadOnlyCollection<string> sourceIds, CancellationToken cancellationToken = default);

    Task ClearUnfinishedQueueAsync(CancellationToken cancellationToken = default);
}
