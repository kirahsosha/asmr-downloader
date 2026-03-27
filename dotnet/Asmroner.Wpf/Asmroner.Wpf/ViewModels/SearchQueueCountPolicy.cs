using Asmroner.Core.Download;
using Asmroner.Core.Utils;

namespace Asmroner.Wpf.ViewModels;

public sealed record SearchQueueCountPlan(
    IReadOnlyList<string> ToEnqueue,
    int SkippedCount);

public static class SearchQueueCountPolicy
{
    public static SearchQueueCountPlan Build(
        IEnumerable<string> sourceIds,
        IReadOnlyList<DownloadTaskItem> existingTasks,
        IReadOnlyCollection<string> queuedSourceIds)
    {
        var normalizedSourceIds = sourceIds
            .Select(SourceIdNormalizer.Normalize)
            .Where(static sourceId => !string.IsNullOrWhiteSpace(sourceId))
            .ToArray();

        if (normalizedSourceIds.Length == 0)
        {
            return new SearchQueueCountPlan(Array.Empty<string>(), 0);
        }

        var distinctSourceIds = normalizedSourceIds
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var queuedSourceSet = new HashSet<string>(queuedSourceIds, StringComparer.OrdinalIgnoreCase);
        var candidates = distinctSourceIds
            .Where(sourceId => !queuedSourceSet.Contains(sourceId))
            .ToArray();

        var toEnqueue = DownloadEnqueueDuplicatePolicy.FilterAlreadyPresent(existingTasks, candidates);

        var skippedByExistingTasks = candidates.Length - toEnqueue.Count;
        var skippedByQueued = distinctSourceIds.Length - candidates.Length;
        var skippedByInputDup = normalizedSourceIds.Length - distinctSourceIds.Length;
        var skippedCount = skippedByExistingTasks + skippedByQueued + skippedByInputDup;

        return new SearchQueueCountPlan(toEnqueue, skippedCount);
    }
}
