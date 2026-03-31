using Asmroner.Core.Download;

namespace Asmroner.Wpf.ViewModels;

/// <summary>
/// Filters out sourceIds that already exist in the download task list (regardless of status).
/// Prevents duplicate RJID entries from being added to the queue.
/// </summary>
public static class DownloadEnqueueDuplicatePolicy
{
    /// <summary>
    /// Returns only those sourceIds from <paramref name="incoming"/> that are NOT already
    /// present in <paramref name="existingTasks"/> (case-insensitive comparison).
    /// </summary>
    public static IReadOnlyList<string> FilterAlreadyPresent(
        IReadOnlyList<DownloadTaskItem> existingTasks,
        IEnumerable<string> incoming)
    {
        var existingIds = new HashSet<string>(
            existingTasks.Select(static t => t.SourceId),
            StringComparer.OrdinalIgnoreCase);

        return incoming
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(id => !existingIds.Contains(id))
            .ToArray();
    }
}
