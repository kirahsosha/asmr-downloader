namespace Asmroner.Wpf.ViewModels;

public static class DownloadOperationPrompts
{
    public static string BuildCancelConfirmMessage(int cancelableCount)
    {
        return $"将取消 {cancelableCount} 个任务，是否继续？";
    }

    public static string BuildRetryAllConfirmMessage(int maxConcurrency, IReadOnlyList<string> sourceIds, int previewLimit = 5)
    {
        var preview = string.Join(", ", sourceIds.Take(previewLimit));
        var suffix = sourceIds.Count > previewLimit ? " ..." : string.Empty;
        return $"将按最多 {maxConcurrency} 并发重试 {sourceIds.Count} 个失败任务：{preview}{suffix}\n是否继续？";
    }
}
