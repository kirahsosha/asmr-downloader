using Asmroner.Core.Download;

namespace Asmroner.Wpf.ViewModels;

public readonly record struct CancelPrecheckResult(
    bool CanProceed,
    IReadOnlyList<DownloadTaskRowViewModel> Cancelable,
    string? StatusText);

public readonly record struct RetryPrecheckResult(
    bool CanProceed,
    DownloadTaskRowViewModel? Target,
    string? StatusText);

public readonly record struct RetryAllPrecheckResult(
    bool CanProceed,
    IReadOnlyList<DownloadTaskItem> FailedTasks,
    string? StatusText);

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

    public static RetryPrecheckResult CheckRetrySingle(IReadOnlyList<DownloadTaskRowViewModel> selected)
    {
        if (selected.Count == 0)
        {
            return new RetryPrecheckResult(false, null, "请先选择要重试的失败任务。");
        }

        if (selected.Count > 1)
        {
            return new RetryPrecheckResult(false, null, "重试仅支持单个失败任务，请只选择一条记录。");
        }

        var target = selected[0];
        if (target.TaskId == Guid.Empty)
        {
            return new RetryPrecheckResult(false, null, "该任务尚未开始执行，无需重试。");
        }

        return new RetryPrecheckResult(true, target, null);
    }

    public static RetryAllPrecheckResult CheckRetryAllFailed(IReadOnlyList<DownloadTaskItem> failedTasks)
    {
        if (failedTasks.Count == 0)
        {
            return new RetryAllPrecheckResult(false, Array.Empty<DownloadTaskItem>(), "当前没有失败任务可重试。");
        }

        return new RetryAllPrecheckResult(true, failedTasks, null);
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
