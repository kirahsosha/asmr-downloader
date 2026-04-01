using Asmroner.Core.Download;

namespace Asmroner.Wpf.ViewModels;

public sealed class DownloadCommandAvailability
{
    public bool CanCancel { get; init; }

    public bool CanRetry { get; init; }

    public bool CanRetryAllFailed { get; init; }

    public bool CanStartImmediate { get; init; }

    public static DownloadCommandAvailability Evaluate(
        IEnumerable<DownloadTaskRowViewModel> selectedTasks,
        IEnumerable<DownloadTaskItem> allTasks)
    {
        var selected = selectedTasks.ToArray();
        var all = allTasks.ToArray();

        return new DownloadCommandAvailability
        {
            CanCancel = selected.Any(static item => item.Status is DownloadTaskStatus.Pending or DownloadTaskStatus.Queued or DownloadTaskStatus.Running),
            CanRetry = selected.Length == 1
                && selected[0].Status == DownloadTaskStatus.Failed
                && selected[0].TaskId != Guid.Empty,
            CanRetryAllFailed = all.Any(static item => item.Status == DownloadTaskStatus.Failed),
            CanStartImmediate = selected.Any(static item => item.Status is DownloadTaskStatus.Pending or DownloadTaskStatus.Failed or DownloadTaskStatus.Canceled),
        };
    }
}
