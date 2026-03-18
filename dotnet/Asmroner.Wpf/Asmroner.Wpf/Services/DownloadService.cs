using Asmroner.Core.Download;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Api;

namespace Asmroner.Wpf.Services;

public sealed class DownloadService : IDownloadService
{
    public Task<IReadOnlyList<DownloadTaskItem>> RunQueuedAsync(string? fileFilter = null, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("请使用应用层 DownloadService。当前占位实现不参与运行。");
    }

    public Task<DownloadTaskItem?> StartAsync(string sourceId, string? fileFilter = null, Guid? preferredTaskId = null, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("请使用应用层 DownloadService。当前占位实现不参与运行。");
    }

    public IReadOnlyList<DownloadTaskItem> GetTasks()
    {
        return Array.Empty<DownloadTaskItem>();
    }

    public Task<bool> CancelAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("请使用应用层 DownloadService。当前占位实现不参与运行。");
    }

    public Task<DownloadTaskItem?> RetryFailedAsync(Guid taskId, string? fileFilter = null, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("请使用应用层 DownloadService。当前占位实现不参与运行。");
    }

    public void UpsertPrefetchedWorkInfo(IReadOnlyDictionary<string, WorkInfoDto> workInfos)
    {
        throw new NotSupportedException("请使用应用层 DownloadService。当前占位实现不参与运行。");
    }

    public IReadOnlyDictionary<string, WorkInfoDto> GetPrefetchedWorkInfoSnapshot()
    {
        throw new NotSupportedException("请使用应用层 DownloadService。当前占位实现不参与运行。");
    }
}