using System.Globalization;
using Asmroner.Core.Sync;

namespace Asmroner.Wpf.ViewModels;

public static class SyncProgressDetailsBuilder
{
    public static string BuildRetryPendingDetails()
    {
        return "正在重试失败同步下载，完成后这里会显示本次重试结果。";
    }

    public static string BuildPersistedProgressDetails(
        MetadataSyncProgressState metadataProgress,
        SyncDownloadProgressState downloadProgress)
    {
        return string.Join(Environment.NewLine,
        [
            $"元数据进度状态：{FormatProgressStatus(metadataProgress.Status)}",
            $"元数据停止请求：{(metadataProgress.StopRequested ? "是" : "否")}",
            $"元数据已处理分页：{metadataProgress.ProcessedPageCount.ToString(CultureInfo.InvariantCulture)}/{metadataProgress.TotalPageCount.ToString(CultureInfo.InvariantCulture)}",
            $"元数据当前本地总量：{metadataProgress.LocalTotalCount.ToString(CultureInfo.InvariantCulture)}",
            $"元数据当前本地字幕量：{metadataProgress.LocalSubtitleCount.ToString(CultureInfo.InvariantCulture)}",
            $"元数据累计处理：{metadataProgress.ProcessedWorkCount.ToString(CultureInfo.InvariantCulture)}",
            $"元数据累计新增：{metadataProgress.InsertedCount.ToString(CultureInfo.InvariantCulture)}",
            $"元数据最近更新时间：{FormatTimestamp(metadataProgress.UpdatedAt)}",
            string.Empty,
            $"同步下载进度状态：{FormatProgressStatus(downloadProgress.Status)}",
            $"同步下载停止请求：{(downloadProgress.StopRequested ? "是" : "否")}",
            $"同步下载最近处理作品：{FormatSourceId(downloadProgress.LastProcessedSourceId)}",
            $"同步下载累计处理：{downloadProgress.ProcessedCount.ToString(CultureInfo.InvariantCulture)} 项",
            $"同步下载累计成功：{downloadProgress.CompletedCount.ToString(CultureInfo.InvariantCulture)} 项",
            $"同步下载累计失败：{downloadProgress.FailedCount.ToString(CultureInfo.InvariantCulture)} 项",
            $"同步下载剩余待处理：{downloadProgress.RemainingMetadataCountAfter.ToString(CultureInfo.InvariantCulture)} 项",
            $"同步下载累计大小：{SyncSizeText.FormatBytes(downloadProgress.CompletedSizeBytesAfter)} / {SyncSizeText.FormatBytes(downloadProgress.SizeLimitBytes)}",
            $"同步下载最近更新时间：{FormatTimestamp(downloadProgress.UpdatedAt)}",
        ]);
    }

    public static string BuildRetryRefreshDetails(SyncReportSnapshot report)
    {
        return string.Join(Environment.NewLine,
        [
            "当前正在重试失败同步下载。",
            $"当前已完成总数：{report.CompletedCount.ToString(CultureInfo.InvariantCulture)} 项",
            $"当前失败总数：{report.FailedCount.ToString(CultureInfo.InvariantCulture)} 项",
            $"当前待处理总数：{report.RemainingMetadataCount.ToString(CultureInfo.InvariantCulture)} 项",
            $"当前队列中数量：{report.PendingCount.ToString(CultureInfo.InvariantCulture)} 项",
            $"当前已落盘大小：{SyncSizeText.FormatBytes(report.CompletedSizeBytes)}",
            $"最近同步记录更新时间：{FormatTimestamp(report.DownloadUpdatedAt)}",
            "刷新统计已保留当前重试状态，完成后这里会显示本次重试结果。",
        ]);
    }

    private static string FormatProgressStatus(string status)
    {
        return status switch
        {
            SyncProgressStatuses.Running => "进行中",
            SyncProgressStatuses.Stopped => "已停止",
            SyncProgressStatuses.Completed => "已完成",
            _ => "未开始",
        };
    }

    private static string FormatTimestamp(DateTime? value)
    {
        return value.HasValue
            ? value.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
            : "暂无";
    }

    private static string FormatSourceId(string sourceId)
    {
        return string.IsNullOrWhiteSpace(sourceId) ? "暂无" : sourceId;
    }
}