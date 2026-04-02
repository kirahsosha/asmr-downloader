using Asmroner.Core.Download;

namespace Asmroner.Wpf.ViewModels;

public sealed class DownloadCommandAvailability
{
    public bool CanCancel { get; init; }

    public bool CanRetry { get; init; }

    public bool CanStartImmediate { get; init; }

    public static DownloadCommandAvailability Evaluate(
        IEnumerable<DownloadTaskRowViewModel> selectedTasks,
        IEnumerable<DownloadTaskItem> allTasks)
    {
        var selected = selectedTasks.ToArray();
        var all = allTasks.ToArray();
        var hasSelection = selected.Length > 0;

        return new DownloadCommandAvailability
        {
            CanCancel = selected.Any(static item => item.Status is DownloadTaskStatus.Pending or DownloadTaskStatus.Queued or DownloadTaskStatus.Running),
            CanRetry = hasSelection
                ? selected.Any(static item => item.Status == DownloadTaskStatus.Failed && item.TaskId != Guid.Empty)
                : all.Any(static item => item.Status == DownloadTaskStatus.Failed && item.TaskId != Guid.Empty),
            CanStartImmediate = selected.Any(static item => item.Status is DownloadTaskStatus.Pending or DownloadTaskStatus.Failed or DownloadTaskStatus.Canceled),
        };
    }
}
