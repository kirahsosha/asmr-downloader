using Asmroner.Core.Api;
using Asmroner.Core.Download;

namespace Asmroner.Wpf.ViewModels;

public readonly record struct DownloadQueueCacheSnapshot(
    IReadOnlyDictionary<string, string> Titles,
    IReadOnlyDictionary<string, DownloadTaskStatus> StatusOverrides);

public static class DownloadQueueCachePolicy
{
    public static DownloadQueueCacheSnapshot Reconcile(
        IReadOnlyCollection<string> activeSourceIds,
        IReadOnlyDictionary<string, WorkInfoDto> prefetched,
        IReadOnlyDictionary<string, string> existingTitles,
        IReadOnlyDictionary<string, DownloadTaskStatus> existingStatusOverrides)
    {
        var titles = new Dictionary<string, string>(existingTitles, StringComparer.OrdinalIgnoreCase);
        var statusOverrides = new Dictionary<string, DownloadTaskStatus>(
            existingStatusOverrides,
            StringComparer.OrdinalIgnoreCase);

        var fetchedTitles = DownloadWorkInfoTitlePolicy.BuildNonEmptyTitleMap(prefetched);
        foreach (var (sourceId, title) in fetchedTitles)
        {
            titles[sourceId] = title;
        }

        foreach (var sourceId in activeSourceIds)
        {
            statusOverrides.Remove(sourceId);
        }

        return new DownloadQueueCacheSnapshot(titles, statusOverrides);
    }

    public static DownloadQueueCacheSnapshot ReconcileWithQueuedSourceIds(
        IReadOnlyCollection<string> activeSourceIds,
        IReadOnlyCollection<string> queuedSourceIds,
        IReadOnlyDictionary<string, WorkInfoDto> prefetched,
        IReadOnlyDictionary<string, string> existingTitles,
        IReadOnlyDictionary<string, DownloadTaskStatus> existingStatusOverrides)
    {
        var titles = new Dictionary<string, string>(existingTitles, StringComparer.OrdinalIgnoreCase);
        var statusOverrides = new Dictionary<string, DownloadTaskStatus>(
            existingStatusOverrides,
            StringComparer.OrdinalIgnoreCase);

        var fetchedTitles = DownloadWorkInfoTitlePolicy.BuildNonEmptyTitleMap(prefetched);
        foreach (var (sourceId, title) in fetchedTitles)
        {
            titles[sourceId] = title;
        }

        foreach (var sourceId in activeSourceIds)
        {
            statusOverrides.Remove(sourceId);
        }

        // Remove Canceled overrides for re-queued items
        var queuedSet = new HashSet<string>(queuedSourceIds, StringComparer.OrdinalIgnoreCase);
        foreach (var sourceId in queuedSet)
        {
            if (statusOverrides.TryGetValue(sourceId, out var status) && status == DownloadTaskStatus.Canceled)
            {
                statusOverrides.Remove(sourceId);
            }
        }

        return new DownloadQueueCacheSnapshot(titles, statusOverrides);
    }
}
