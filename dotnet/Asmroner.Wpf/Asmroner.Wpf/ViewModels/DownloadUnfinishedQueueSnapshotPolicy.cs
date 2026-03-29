using Asmroner.Core.Download;

namespace Asmroner.Wpf.ViewModels;

public static class DownloadUnfinishedQueueSnapshotPolicy
{
    public static IReadOnlyList<string> BuildSnapshot(
        IReadOnlyList<DownloadTaskItem> activeTasks,
        IReadOnlyList<string> queuedSourceIds)
    {
        var pendingSourceIds = activeTasks
            .Where(static item =>
                item.Status is DownloadTaskStatus.Pending
                or DownloadTaskStatus.Queued
                or DownloadTaskStatus.Failed)
            .Select(static item => item.SourceId);

        return pendingSourceIds
            .Concat(queuedSourceIds)
            .Where(static sourceId => !string.IsNullOrWhiteSpace(sourceId))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(static sourceId => sourceId)
            .ToArray();
    }
}