using Asmroner.Core.Download;

namespace Asmroner.Wpf.ViewModels;

public readonly record struct CancelPrecheckResult(
    bool CanProceed,
    IReadOnlyList<DownloadTaskRowViewModel> Cancelable,
    string? StatusText);

public readonly record struct RetryPrecheckResult(
    bool CanProceed,
    IReadOnlyList<RetryTaskTarget> Targets,
    string? StatusText,
    bool UsesSelection);

public readonly record struct RetryTaskTarget(
    Guid TaskId,
    string SourceId);

public readonly record struct StartPrecheckResult(
    bool CanProceed,
    IReadOnlyList<DownloadTaskRowViewModel> Targets,
    string? StatusText);

public static class DownloadOperationPrecheckPolicy
{
    public static CancelPrecheckResult CheckCancel(IReadOnlyList<DownloadTaskRowViewModel> selected)
    {
        if (selected.Count == 0)
        {
            return new CancelPrecheckResult(false, Array.Empty<DownloadTaskRowViewModel>(), "请先选择要取消的任务。");
        }

        var cancelable = DownloadTaskSelectionPolicy.GetCancelable(selected);
        if (cancelable.Count == 0)
        {
            return new CancelPrecheckResult(false, cancelable, "选中任务均不可取消（可能已结束）。");
        }

        return new CancelPrecheckResult(true, cancelable, null);
    }

    public static RetryPrecheckResult CheckRetry(IReadOnlyList<DownloadTaskRowViewModel> selected, IReadOnlyList<DownloadTaskItem> allTasks)
    {
        if (selected.Count > 0)
        {
            var targets = selected
                .Where(static item => item.Status == DownloadTaskStatus.Failed && item.TaskId != Guid.Empty)
                .GroupBy(static item => item.TaskId)
                .Select(static group => new RetryTaskTarget(group.Key, group.First().SourceId))
                .ToArray();

            return targets.Length == 0
                ? new RetryPrecheckResult(false, Array.Empty<RetryTaskTarget>(), "选中项中没有可重试的失败任务。", true)
                : new RetryPrecheckResult(true, targets, null, true);
        }

        var failedTasks = DownloadTaskSnapshotPolicy.GetFailedTasks(allTasks)
            .Where(static item => item.TaskId != Guid.Empty)
            .GroupBy(static item => item.TaskId)
            .Select(static group => new RetryTaskTarget(group.Key, group.First().SourceId))
            .ToArray();
        if (failedTasks.Length == 0)
        {
            return new RetryPrecheckResult(false, Array.Empty<RetryTaskTarget>(), "当前没有失败任务可重试。", false);
        }

        return new RetryPrecheckResult(true, failedTasks, null, false);
    }

    public static StartPrecheckResult CheckStartImmediate(IReadOnlyList<DownloadTaskRowViewModel> selected)
    {
        var targets = DownloadTaskSelectionPolicy.GetImmediateStartTargets(selected);
        if (targets.Count == 0)
        {
            return new StartPrecheckResult(false, targets, "请选择状态为未下载、失败或已取消的任务。");
        }

        return new StartPrecheckResult(true, targets, null);
    }
}
