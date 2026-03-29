using Asmroner.Core.Configuration;

namespace Asmroner.Core.Interfaces;

public interface IUiStateStore
{
    Task<SearchUiState> LoadSearchUiStateAsync(CancellationToken cancellationToken = default);

    Task SaveSearchUiStateAsync(SearchUiState state, CancellationToken cancellationToken = default);

    Task<DownloadUiState> LoadDownloadUiStateAsync(CancellationToken cancellationToken = default);

    Task SaveDownloadUiStateAsync(DownloadUiState state, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> LoadUnfinishedQueueAsync(CancellationToken cancellationToken = default);

    Task SaveUnfinishedQueueAsync(IReadOnlyCollection<string> sourceIds, CancellationToken cancellationToken = default);

    Task ClearUnfinishedQueueAsync(CancellationToken cancellationToken = default);
}
