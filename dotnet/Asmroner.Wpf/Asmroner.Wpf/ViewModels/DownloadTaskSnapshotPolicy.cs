using Asmroner.Core.Download;

namespace Asmroner.Wpf.ViewModels;

public static class DownloadTaskSnapshotPolicy
{
    public static IReadOnlyList<DownloadTaskItem> GetFailedTasks(IReadOnlyList<DownloadTaskItem> tasks)
    {
        return tasks.Where(static item => item.Status == DownloadTaskStatus.Failed).ToArray();
    }

    public static HashSet<string> GetActiveSourceIds(IReadOnlyList<DownloadTaskItem> tasks)
    {
        return tasks
            .Select(static item => item.SourceId)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}
