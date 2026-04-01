namespace Asmroner.Wpf.ViewModels;

public static class DownloadOperationStatusTexts
{
    public static string BuildBatchEnqueueResult(int totalCount, int resolvedCount)
    {
        return resolvedCount == totalCount
            ? $"已加入批量下载：{totalCount} 个任务"
            : $"已加入批量下载：{totalCount} 个任务（{resolvedCount} 个已更新作品信息）";
    }

    public static string BuildRunQueueResult(int createdCount)
    {
        return createdCount == 0
            ? "下载队列为空，无需执行。"
            : $"已触发 {createdCount} 个下载任务。";
    }

    public static string BuildCancelResult(int canceledCount, int totalCount)
    {
        return canceledCount == 0
            ? "当前任务不可取消（可能已结束）。"
            : $"已取消 {canceledCount}/{totalCount} 个任务。";
    }

    public static string BuildRetryResult(bool retried, string sourceId)
    {
        return retried
            ? $"重试任务已执行：{sourceId}"
            : "仅失败状态任务支持重试。";
    }

    public static string BuildRetryAllResult(int retriedCount, int totalCount)
    {
        return $"批量重试完成：成功触发 {retriedCount}/{totalCount}。";
    }

    public static string BuildStartSelectedResult(int startedCount, int totalCount)
    {
        return startedCount == 0
            ? "没有可立即下载的任务。"
            : $"已立即启动 {startedCount}/{totalCount} 个任务。";
    }

    public static string AppendTranslationSwitchClause(string message, int switchedCount)
    {
        return switchedCount > 0
            ? $"{message}；其中 {switchedCount} 项已切换为翻译作品"
            : message;
    }

    public static string BuildWorkInfoRefreshResult(int updatedCount, int failedCount)
    {
        if (updatedCount > 0 && failedCount > 0)
        {
            return $"作品信息更新完成：成功更新 {updatedCount} 项，失败 {failedCount} 项。";
        }

        if (updatedCount > 0)
        {
            return $"作品信息更新完成：成功更新 {updatedCount} 项。";
        }

        return $"作品信息更新失败：共有 {failedCount} 项未能更新。";
    }
}
