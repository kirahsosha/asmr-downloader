using Asmroner.Core.Download;
using Asmroner.Core.Api;

namespace Asmroner.Core.Interfaces;

public interface IDownloadService
{
    Task<IReadOnlyList<DownloadTaskItem>> RunQueuedAsync(string? fileFilter = null, CancellationToken cancellationToken = default);

    Task<DownloadTaskItem?> StartAsync(string sourceId, string? fileFilter = null, Guid? preferredTaskId = null, CancellationToken cancellationToken = default);

    IReadOnlyList<DownloadTaskItem> GetTasks();

    Task<bool> CancelAsync(Guid taskId, CancellationToken cancellationToken = default);

    Task<DownloadTaskItem?> RetryFailedAsync(Guid taskId, string? fileFilter = null, CancellationToken cancellationToken = default);

    void UpsertPrefetchedWorkInfo(IReadOnlyDictionary<string, WorkInfoDto> workInfos);

    IReadOnlyDictionary<string, WorkInfoDto> GetPrefetchedWorkInfoSnapshot();
}