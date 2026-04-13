using System.Globalization;
using Asmroner.Core.Sync;

namespace Asmroner.Wpf.ViewModels;

public static class SyncStatusTextBuilder
{
    private const string MetadataPrefix = "同步状态：";
    private const string DownloadPrefix = "下载状态：";

    public static string BuildMetadataActionStatus(string message)
    {
        return MetadataPrefix + message;
    }

    public static string BuildDownloadActionStatus(string message)
    {
        return DownloadPrefix + message;
    }

    public static string BuildMetadataStatus(MetadataSyncProgressState metadataProgress)
    {
        if (string.Equals(metadataProgress.Status, SyncProgressStatuses.Running, StringComparison.Ordinal))
        {
            return BuildMetadataActionStatus(metadataProgress.StopRequested
                ? $"元数据同步正在停止：已处理 {metadataProgress.ProcessedPageCount.ToString(CultureInfo.InvariantCulture)}/{metadataProgress.TotalPageCount.ToString(CultureInfo.InvariantCulture)} 页，累计处理 {metadataProgress.ProcessedWorkCount.ToString(CultureInfo.InvariantCulture)} 条，本地现有 {metadataProgress.LocalTotalCount.ToString(CultureInfo.InvariantCulture)} 条，等待当前页完成。"
                : $"元数据同步进行中：已处理 {metadataProgress.ProcessedPageCount.ToString(CultureInfo.InvariantCulture)}/{metadataProgress.TotalPageCount.ToString(CultureInfo.InvariantCulture)} 页，累计处理 {metadataProgress.ProcessedWorkCount.ToString(CultureInfo.InvariantCulture)} 条，本地现有 {metadataProgress.LocalTotalCount.ToString(CultureInfo.InvariantCulture)} 条，下次页码 {metadataProgress.NextPage.ToString(CultureInfo.InvariantCulture)}。"
            );
        }

        if (string.Equals(metadataProgress.Status, SyncProgressStatuses.Stopped, StringComparison.Ordinal))
        {
            return BuildMetadataActionStatus($"检测到未完成元数据同步进度，累计处理 {metadataProgress.ProcessedWorkCount.ToString(CultureInfo.InvariantCulture)} 条，本地现有 {metadataProgress.LocalTotalCount.ToString(CultureInfo.InvariantCulture)} 条，下次将从第 {metadataProgress.NextPage.ToString(CultureInfo.InvariantCulture)} 页继续。");
        }

        if (string.Equals(metadataProgress.Status, SyncProgressStatuses.Completed, StringComparison.Ordinal)
            && metadataProgress.TotalPageCount > 0)
        {
            return BuildMetadataActionStatus($"元数据同步已完成：共处理 {metadataProgress.ProcessedPageCount.ToString(CultureInfo.InvariantCulture)}/{metadataProgress.TotalPageCount.ToString(CultureInfo.InvariantCulture)} 页，累计处理 {metadataProgress.ProcessedWorkCount.ToString(CultureInfo.InvariantCulture)} 条，本地现有 {metadataProgress.LocalTotalCount.ToString(CultureInfo.InvariantCulture)} 条。");
        }

        return BuildMetadataActionStatus("已刷新本地元数据统计与进度。");
    }

    public static string BuildDownloadStatus(SyncDownloadProgressState downloadProgress)
    {
        if (string.Equals(downloadProgress.Status, SyncProgressStatuses.Running, StringComparison.Ordinal))
        {
            return BuildDownloadActionStatus(downloadProgress.StopRequested
                ? $"同步下载正在停止：已处理 {downloadProgress.ProcessedCount.ToString(CultureInfo.InvariantCulture)} 项，等待当前作品完成。"
                : $"同步下载进行中：已处理 {downloadProgress.ProcessedCount.ToString(CultureInfo.InvariantCulture)} 项，成功 {downloadProgress.CompletedCount.ToString(CultureInfo.InvariantCulture)} 项，失败 {downloadProgress.FailedCount.ToString(CultureInfo.InvariantCulture)} 项。"
            );
        }

        if (string.Equals(downloadProgress.Status, SyncProgressStatuses.Stopped, StringComparison.Ordinal))
        {
            return BuildDownloadActionStatus(string.IsNullOrWhiteSpace(downloadProgress.LastProcessedSourceId)
                ? "检测到未完成同步下载进度，下次将继续当前任务。"
                : $"检测到未完成同步下载进度，下次将从 {downloadProgress.LastProcessedSourceId} 之后继续。"
            );
        }

        if (string.Equals(downloadProgress.Status, SyncProgressStatuses.Completed, StringComparison.Ordinal)
            && downloadProgress.ProcessedCount > 0)
        {
            return BuildDownloadActionStatus($"同步下载已完成：已处理 {downloadProgress.ProcessedCount.ToString(CultureInfo.InvariantCulture)} 项，成功 {downloadProgress.CompletedCount.ToString(CultureInfo.InvariantCulture)} 项，失败 {downloadProgress.FailedCount.ToString(CultureInfo.InvariantCulture)} 项。");
        }

        return BuildDownloadActionStatus("已刷新本地下载统计与进度。");
    }
}