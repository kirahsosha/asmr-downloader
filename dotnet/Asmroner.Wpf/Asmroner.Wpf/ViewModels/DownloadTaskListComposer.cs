using Asmroner.Core.Download;

namespace Asmroner.Wpf.ViewModels;

public static class DownloadTaskListComposer
{
    public static IReadOnlyList<DownloadTaskRowViewModel> ComposeRows(
        IReadOnlyList<DownloadTaskItem> activeTasks,
        IReadOnlyList<string> queuedSourceIds,
        IReadOnlyDictionary<string, string> queuedWorkInfoTitles,
        IReadOnlyDictionary<string, DownloadTaskStatus> queuedStatusOverrides,
        IReadOnlyDictionary<string, string> queuedErrorMessages)
    {
        var activeSourceIds = activeTasks
            .Select(static item => item.SourceId)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var queuedPendingRows = queuedSourceIds
            .Where(sourceId => !activeSourceIds.Contains(sourceId))
            .Select(sourceId => DownloadTaskRowViewModel.CreatePending(
                sourceId,
                queuedWorkInfoTitles.TryGetValue(sourceId, out var title) ? title : string.Empty,
                queuedStatusOverrides.TryGetValue(sourceId, out var status) && status != DownloadTaskStatus.Canceled
                    ? status
                    : DownloadTaskStatus.Pending,
                queuedErrorMessages.TryGetValue(sourceId, out var errorMessage) ? errorMessage : null));

        var canceledPendingRows = queuedStatusOverrides
            .Where(item => item.Value == DownloadTaskStatus.Canceled)
            .Where(item => !activeSourceIds.Contains(item.Key))
            .Where(item => !queuedSourceIds.Contains(item.Key, StringComparer.OrdinalIgnoreCase))
            .Select(item => DownloadTaskRowViewModel.CreatePending(
                item.Key,
                queuedWorkInfoTitles.TryGetValue(item.Key, out var title) ? title : string.Empty,
                DownloadTaskStatus.Canceled));

        var activeRows = activeTasks.Select(task =>
        {
            var row = DownloadTaskRowViewModel.From(task);
            if (string.IsNullOrWhiteSpace(row.Title)
                && queuedWorkInfoTitles.TryGetValue(row.SourceId, out var title)
                && !string.IsNullOrWhiteSpace(title))
            {
                return new DownloadTaskRowViewModel
                {
                    TaskId = row.TaskId,
                    SourceId = row.SourceId,
                    Title = title,
                    Status = row.Status,
                    StatusText = row.StatusText,
                    ProgressText = row.ProgressText,
                    ErrorMessage = row.ErrorMessage,
                    TargetDirectory = row.TargetDirectory,
                };
            }

            return row;
        });

        return queuedPendingRows
            .Concat(canceledPendingRows)
            .Concat(activeRows)
            .OrderBy(row => row.Status.GetSortOrder())
            .ThenBy(row => row.SourceId, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
