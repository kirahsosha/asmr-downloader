using Asmroner.Core.Download;

namespace Asmroner.Wpf.ViewModels;

public static class DownloadTaskSelectionPolicy
{
    public static IReadOnlyList<DownloadTaskRowViewModel> GetCancelable(IReadOnlyList<DownloadTaskRowViewModel> selected)
    {
        return selected
            .Where(static item => item.Status is DownloadTaskStatus.Pending or DownloadTaskStatus.Queued or DownloadTaskStatus.Running)
            .ToArray();
    }

    public static IReadOnlyList<DownloadTaskRowViewModel> GetImmediateStartTargets(IReadOnlyList<DownloadTaskRowViewModel> selected)
    {
        return selected
            .Where(static item => item.Status is DownloadTaskStatus.Pending or DownloadTaskStatus.Failed or DownloadTaskStatus.Canceled)
            .GroupBy(static item => item.SourceId, StringComparer.OrdinalIgnoreCase)
            .Select(static group => group.First())
            .ToArray();
    }
}
