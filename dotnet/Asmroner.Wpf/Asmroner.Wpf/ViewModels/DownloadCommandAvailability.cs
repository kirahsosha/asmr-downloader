using Asmroner.Core.Download;

namespace Asmroner.Wpf.ViewModels;

public sealed class DownloadCommandAvailability
{
    public bool CanCancel { get; init; }

    public bool CanRetry { get; init; }

    public bool CanRetryAllFailed { get; init; }

    public bool CanStartImmediate { get; init; }

    public static DownloadCommandAvailability Evaluate(
        IEnumerable<DownloadTaskStatus> selectedStatuses,
        IEnumerable<DownloadTaskItem> allTasks)
    {
        var selected = selectedStatuses.ToArray();
        var all = allTasks.ToArray();

        return new DownloadCommandAvailability
        {
            CanCancel = selected.Any(static status => status is DownloadTaskStatus.Pending or DownloadTaskStatus.Queued or DownloadTaskStatus.Running),
            CanRetry = selected.Length == 1 && selected[0] == DownloadTaskStatus.Failed,
            CanRetryAllFailed = all.Any(static item => item.Status == DownloadTaskStatus.Failed),
            CanStartImmediate = selected.Any(static status => status is DownloadTaskStatus.Pending or DownloadTaskStatus.Failed or DownloadTaskStatus.Canceled),
        };
    }
}
