using System.Windows;

namespace Asmroner.Wpf.ViewModels;

public readonly record struct DownloadConfirmationDecision(bool ShouldContinue, string? StatusText);

public static class DownloadConfirmationPolicy
{
    public static DownloadConfirmationDecision Evaluate(MessageBoxResult result)
    {
        return result == MessageBoxResult.Yes
            ? new DownloadConfirmationDecision(true, null)
            : new DownloadConfirmationDecision(false, "已取消本次操作。");
    }
}
