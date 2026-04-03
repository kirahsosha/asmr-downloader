using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Sync;
using Asmroner.Wpf.Services;
using Asmroner.Wpf.ViewModels;
using Microsoft.Win32;

namespace Asmroner.Wpf.Views;

public partial class SyncView : UserControl
{
    private readonly IAppPathService _appPathService;
    private readonly ISyncExportService _syncExportService;
    private readonly ISyncService _syncService;

    private bool _isMetadataSyncRunning;
    private bool _isMetadataStopRequested;
    private bool _isDownloadSyncRunning;
    private bool _isDownloadStopRequested;
    private bool _isExclusiveOperationRunning;
    private bool _isRefreshRunning;

    private int _metadataActionInFlight;
    private int _downloadActionInFlight;
    private int _refreshActionInFlight;

    private DateTimeOffset? _lastMetadataStartRequestedAt;
    private DateTimeOffset? _lastDownloadStartRequestedAt;

    public SyncView(ISyncService syncService, ISyncExportService syncExportService, IAppPathService appPathService)
    {
        _appPathService = appPathService;
        _syncExportService = syncExportService;
        _syncService = syncService;

        InitializeComponent();
        RefreshCommandAvailability();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await RefreshSnapshotAsync(updateStatusText: false);
    }

    private async void OnRefreshStatusClicked(object sender, RoutedEventArgs e)
    {
        if (Interlocked.Exchange(ref _refreshActionInFlight, 1) == 1)
        {
            return;
        }

        _isRefreshRunning = true;
        RefreshCommandAvailability();

        try
        {
            await RefreshSnapshotAsync(updateStatusText: true);
            await Dispatcher.InvokeAsync(static () => { }, DispatcherPriority.ApplicationIdle);
        }
        finally
        {
            _isRefreshRunning = false;
            RefreshCommandAvailability();
            Interlocked.Exchange(ref _refreshActionInFlight, 0);
        }
    }

    private async void OnSyncMetadataClicked(object sender, RoutedEventArgs e)
    {
        if (Interlocked.Exchange(ref _metadataActionInFlight, 1) == 1)
        {
            return;
        }

        try
        {
            var now = DateTimeOffset.UtcNow;
            var decision = SyncActionDebouncePolicy.Decide(
                _isMetadataSyncRunning,
                _isMetadataStopRequested,
                _lastMetadataStartRequestedAt,
                now);

            if (decision == SyncActionToggleDecision.Ignore)
            {
                return;
            }

            if (decision == SyncActionToggleDecision.RequestStop)
            {
                await _syncService.RequestStopMetadataSyncAsync();
                _isMetadataStopRequested = true;
                RefreshCommandAvailability();
                StatusTextBlock.Text = "已请求停止元数据同步，将在当前页完成后停止。";
                return;
            }

            _lastMetadataStartRequestedAt = now;
            _isMetadataSyncRunning = true;
            _isMetadataStopRequested = false;
            RefreshCommandAvailability();
            StatusTextBlock.Text = "正在同步元数据，请稍候...";
            _ = RunMetadataSyncAsync();
        }
        finally
        {
            await Dispatcher.InvokeAsync(static () => { }, DispatcherPriority.ApplicationIdle);
            Interlocked.Exchange(ref _metadataActionInFlight, 0);
        }
    }

    private async Task RunMetadataSyncAsync()
    {
        try
        {
            var result = await _syncService.SyncMetadataAsync();
            ApplyResult(result);
            await ApplyReportAsync();
        }
        catch (Exception ex)
        {
            StatusTextBlock.Text = $"同步失败: {ex.Message}";
            DetailsTextBox.Text = ex.ToString();
        }
        finally
        {
            _isMetadataSyncRunning = false;
            _isMetadataStopRequested = false;
            RefreshCommandAvailability();
        }
    }

    private async void OnSyncDownloadClicked(object sender, RoutedEventArgs e)
    {
        if (Interlocked.Exchange(ref _downloadActionInFlight, 1) == 1)
        {
            return;
        }

        try
        {
            var now = DateTimeOffset.UtcNow;
            var decision = SyncActionDebouncePolicy.Decide(
                _isDownloadSyncRunning,
                _isDownloadStopRequested,
                _lastDownloadStartRequestedAt,
                now);

            if (decision == SyncActionToggleDecision.Ignore)
            {
                return;
            }

            if (decision == SyncActionToggleDecision.RequestStop)
            {
                await _syncService.RequestStopSyncDownloadAsync();
                _isDownloadStopRequested = true;
                RefreshCommandAvailability();
                StatusTextBlock.Text = "已请求停止同步下载，将在当前作品完成后停止。";
                return;
            }

            _lastDownloadStartRequestedAt = now;
            _isDownloadSyncRunning = true;
            _isDownloadStopRequested = false;
            RefreshCommandAvailability();
            StatusTextBlock.Text = "正在执行同步下载，请稍候...";
            _ = RunSyncDownloadAsync();
        }
        finally
        {
            await Dispatcher.InvokeAsync(static () => { }, DispatcherPriority.ApplicationIdle);
            Interlocked.Exchange(ref _downloadActionInFlight, 0);
        }
    }

    private async Task RunSyncDownloadAsync()
    {
        try
        {
            var result = await _syncService.SyncDownloadAsync();
            ApplyResult(result);
            await ApplyReportAsync();
        }
        catch (Exception ex)
        {
            StatusTextBlock.Text = $"同步下载失败: {ex.Message}";
            DetailsTextBox.Text = ex.ToString();
        }
        finally
        {
            _isDownloadSyncRunning = false;
            _isDownloadStopRequested = false;
            RefreshCommandAvailability();
        }
    }

    private async void OnRetryFailedClicked(object sender, RoutedEventArgs e)
    {
        await RunExclusiveOperationAsync(async () =>
        {
            StatusTextBlock.Text = "正在重试失败同步下载，请稍候...";

            try
            {
                var result = await _syncService.RetryFailedAsync();
                ApplyResult(result);
                await ApplyReportAsync();
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"失败重试执行失败: {ex.Message}";
                DetailsTextBox.Text = ex.ToString();
            }
        });
    }

    private async void OnExportFailedClicked(object sender, RoutedEventArgs e)
    {
        await RunExclusiveOperationAsync(async () => await ExportAsync(SyncExportStatus.Failed));
    }

    private async void OnExportCompletedClicked(object sender, RoutedEventArgs e)
    {
        await RunExclusiveOperationAsync(async () => await ExportAsync(SyncExportStatus.Completed));
    }

    private async Task RefreshSnapshotAsync(bool updateStatusText)
    {
        try
        {
            await ApplyReportAsync();
            await ApplyPersistedProgressSummaryAsync(updateStatusText);
        }
        catch (Exception ex)
        {
            StatusTextBlock.Text = $"读取同步统计失败: {ex.Message}";
            DetailsTextBox.Text = ex.ToString();
        }
    }

    private void ApplyReport(SyncReportSnapshot report)
    {
        CurrentCountTextBlock.Text = $"本地元数据：{report.MetadataTotalCount.ToString(CultureInfo.InvariantCulture)} 条（字幕 {report.MetadataSubtitleCount.ToString(CultureInfo.InvariantCulture)} 条）";
        DownloadCountTextBlock.Text = string.Join(string.Empty,
        [
            $"同步下载：已完成 {report.CompletedCount.ToString(CultureInfo.InvariantCulture)} 项，",
            $"失败 {report.FailedCount.ToString(CultureInfo.InvariantCulture)} 项，",
            $"待处理 {report.RemainingMetadataCount.ToString(CultureInfo.InvariantCulture)} 项，",
            $"队列中 {report.PendingCount.ToString(CultureInfo.InvariantCulture)} 项，",
            $"已落盘 {SyncSizeText.FormatBytes(report.CompletedSizeBytes)}"
        ]);

        MetadataBreakdownTextBlock.Text = $"总量 {report.MetadataTotalCount.ToString(CultureInfo.InvariantCulture)} 条，字幕 {report.MetadataSubtitleCount.ToString(CultureInfo.InvariantCulture)} 条，无字幕 {report.MetadataWithoutSubtitleCount.ToString(CultureInfo.InvariantCulture)} 条";
        CompletedBreakdownTextBlock.Text = $"已完成 {report.CompletedCount.ToString(CultureInfo.InvariantCulture)} 条，字幕 {report.CompletedSubtitleCount.ToString(CultureInfo.InvariantCulture)} 条，无字幕 {report.CompletedWithoutSubtitleCount.ToString(CultureInfo.InvariantCulture)} 条，已落盘 {SyncSizeText.FormatBytes(report.CompletedSizeBytes)}";
        BacklogBreakdownTextBlock.Text = $"失败 {report.FailedCount.ToString(CultureInfo.InvariantCulture)} 条，待处理 {report.PendingCount.ToString(CultureInfo.InvariantCulture)} 条，未建同步记录 {report.RemainingMetadataCount.ToString(CultureInfo.InvariantCulture)} 条";
        ProgressBreakdownTextBlock.Text = $"总进度 {FormatPercent(report.OverallProgressPercent)}，字幕 {FormatPercent(report.SubtitleProgressPercent)}，无字幕 {FormatPercent(report.WithoutSubtitleProgressPercent)}";
        LastSyncTextBlock.Text = $"最近更新：元数据 {FormatTimestamp(report.MetadataUpdatedAt)}；同步记录 {FormatTimestamp(report.DownloadUpdatedAt)}";
    }

    private void ApplyResult(MetadataSyncRunResult result)
    {
        StatusTextBlock.Text = result.Message;
        DetailsTextBox.Text = string.Join(Environment.NewLine,
        [
            $"网站总量：{result.RemoteTotalCount.ToString(CultureInfo.InvariantCulture)}",
            $"网站字幕量：{result.RemoteSubtitleCount.ToString(CultureInfo.InvariantCulture)}",
            $"同步前本地总量：{result.LocalTotalCountBefore.ToString(CultureInfo.InvariantCulture)}",
            $"同步前本地字幕量：{result.LocalSubtitleCountBefore.ToString(CultureInfo.InvariantCulture)}",
            $"同步后本地总量：{result.LocalTotalCountAfter.ToString(CultureInfo.InvariantCulture)}",
            $"同步后本地字幕量：{result.LocalSubtitleCountAfter.ToString(CultureInfo.InvariantCulture)}",
            $"本次新增：{result.InsertedCount.ToString(CultureInfo.InvariantCulture)}",
            $"处理分页：{result.ProcessedPageCount.ToString(CultureInfo.InvariantCulture)}/{result.TotalPageCount.ToString(CultureInfo.InvariantCulture)}",
            $"下次页码：{result.NextPage.ToString(CultureInfo.InvariantCulture)}",
            $"是否追平网站：{(result.IsUpToDate ? "是" : "否")}",
            $"本次是否从断点继续：{(result.ResumedFromProgress ? "是" : "否")}",
            $"本次是否按请求停止：{(result.WasStopped ? "是" : "否")}",
        ]);
    }

    private void ApplyResult(SyncDownloadRunResult result)
    {
        StatusTextBlock.Text = result.Message;
        DetailsTextBox.Text = string.Join(Environment.NewLine,
        [
            $"本次处理：{result.ProcessedCount.ToString(CultureInfo.InvariantCulture)} 项",
            $"成功：{result.CompletedCount.ToString(CultureInfo.InvariantCulture)} 项",
            $"失败：{result.FailedCount.ToString(CultureInfo.InvariantCulture)} 项",
            $"同步前已落盘：{SyncSizeText.FormatBytes(result.CompletedSizeBytesBefore)}",
            $"同步后已落盘：{SyncSizeText.FormatBytes(result.CompletedSizeBytesAfter)}",
            $"容量上限：{SyncSizeText.FormatBytes(result.SizeLimitBytes)}",
            $"达到容量上限：{(result.ReachedSizeLimit ? "是" : "否")}",
            $"剩余待同步：{result.RemainingMetadataCountAfter.ToString(CultureInfo.InvariantCulture)} 项",
            $"最近处理作品：{FormatSourceId(result.LastProcessedSourceId)}",
            $"本次是否从断点继续：{(result.ResumedFromProgress ? "是" : "否")}",
            $"本次是否按请求停止：{(result.WasStopped ? "是" : "否")}",
        ]);
    }

    private void ApplyResult(SyncRetryRunResult result)
    {
        StatusTextBlock.Text = result.Message;
        DetailsTextBox.Text = string.Join(Environment.NewLine,
        [
            $"本次重试：{result.RetriedCount.ToString(CultureInfo.InvariantCulture)} 项",
            $"恢复成功：{result.RecoveredCount.ToString(CultureInfo.InvariantCulture)} 项",
            $"重试后仍失败：{result.FailedAgainCount.ToString(CultureInfo.InvariantCulture)} 项",
            $"重试前失败总数：{result.FailedCountBefore.ToString(CultureInfo.InvariantCulture)} 项",
            $"重试后失败总数：{result.FailedCountAfter.ToString(CultureInfo.InvariantCulture)} 项",
            $"当前已完成总数：{result.CompletedCountAfter.ToString(CultureInfo.InvariantCulture)} 项",
        ]);
    }

    private void ApplyResult(SyncExportResult result)
    {
        StatusTextBlock.Text = result.Message;
        DetailsTextBox.Text = string.Join(Environment.NewLine,
        [
            $"导出状态：{GetStatusLabel(result.Status)}",
            $"导出数量：{result.ExportedCount.ToString(CultureInfo.InvariantCulture)} 条",
            $"导出格式：{result.Format.ToUpperInvariant()}",
            $"导出路径：{result.FilePath}",
        ]);
    }

    private async Task ApplyReportAsync()
    {
        ApplyReport(await _syncService.GetReportAsync());
    }

    private async Task ApplyPersistedProgressSummaryAsync(bool updateStatusText)
    {
        var metadataProgress = await _syncService.GetMetadataSyncProgressAsync();
        var downloadProgress = await _syncService.GetSyncDownloadProgressAsync();

        DetailsTextBox.Text = BuildPersistedProgressDetails(metadataProgress, downloadProgress);

        if (updateStatusText)
        {
            StatusTextBlock.Text = BuildPersistedProgressStatusText(metadataProgress, downloadProgress);
        }
    }

    private async Task ExportAsync(SyncExportStatus status)
    {
        try
        {
            Directory.CreateDirectory(_appPathService.MetadataDirectory);
            var exportTarget = ShowSaveFileDialog(BuildDefaultFileName(status));
            if (exportTarget is null)
            {
                StatusTextBlock.Text = "已取消导出。";
                return;
            }

            var result = await _syncExportService.ExportAsync(status, exportTarget.Value.FullPath);
            ApplyResult(result);

            if (result.ExportedCount > 0)
            {
                ExplorerHelper.SelectInExplorer(result.FilePath);
            }
        }
        catch (Exception ex)
        {
            StatusTextBlock.Text = $"导出失败：{ex.Message}";
            DetailsTextBox.Text = ex.ToString();
        }
    }

    private (string FullPath, string Extension)? ShowSaveFileDialog(string defaultFileName)
    {
        var dialog = new SaveFileDialog
        {
            Title = "导出同步记录",
            InitialDirectory = _appPathService.MetadataDirectory,
            FileName = defaultFileName,
            DefaultExt = ".csv",
            AddExtension = true,
            OverwritePrompt = true,
            Filter = "CSV 文件 (*.csv)|*.csv|JSON 文件 (*.json)|*.json|所有文件 (*.*)|*.*",
            FilterIndex = 1,
        };

        if (dialog.ShowDialog() != true)
        {
            return null;
        }

        var extension = ResolveExportExtension(dialog.FileName, dialog.FilterIndex);
        var fullPath = EnsureExportFileExtension(dialog.FileName, extension);
        return (fullPath, extension);
    }

    private static string ResolveExportExtension(string filePath, int filterIndex)
    {
        var extension = Path.GetExtension(filePath);
        if (extension.Equals(".csv", StringComparison.OrdinalIgnoreCase))
        {
            return "csv";
        }

        if (extension.Equals(".json", StringComparison.OrdinalIgnoreCase))
        {
            return "json";
        }

        return filterIndex == 2 ? "json" : "csv";
    }

    private static string EnsureExportFileExtension(string filePath, string extension)
    {
        var normalizedExtension = "." + extension;
        var currentExtension = Path.GetExtension(filePath);
        if (currentExtension.Equals(normalizedExtension, StringComparison.OrdinalIgnoreCase))
        {
            return filePath;
        }

        if (string.IsNullOrWhiteSpace(currentExtension))
        {
            return filePath + normalizedExtension;
        }

        return Path.ChangeExtension(filePath, extension);
    }

    private static string BuildDefaultFileName(SyncExportStatus status)
    {
        var prefix = status == SyncExportStatus.Failed ? "sync-failed-export" : "sync-completed-export";
        return $"{prefix}-{DateTime.Now:yyyyMMdd-HHmmss}";
    }

    private static string GetStatusLabel(SyncExportStatus status)
    {
        return status == SyncExportStatus.Failed ? "失败" : "成功";
    }

    private static string FormatPercent(double value)
    {
        return value.ToString("0.00", CultureInfo.InvariantCulture) + "%";
    }

    private static string FormatTimestamp(DateTime? value)
    {
        return value.HasValue
            ? value.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
            : "暂无";
    }

    private void RefreshCommandAvailability()
    {
        var availability = SyncCommandAvailability.Evaluate(
            _isMetadataSyncRunning,
            _isMetadataStopRequested,
            _isDownloadSyncRunning,
            _isDownloadStopRequested,
            _isExclusiveOperationRunning,
            _isRefreshRunning);

        SyncMetadataButton.IsEnabled = availability.CanMetadataAction;
        SyncMetadataButton.Content = availability.MetadataActionText;
        SyncDownloadButton.IsEnabled = availability.CanDownloadAction;
        SyncDownloadButton.Content = availability.DownloadActionText;
        RetryFailedButton.IsEnabled = availability.CanRetryFailed;
        ExportFailedButton.IsEnabled = availability.CanExportFailed;
        ExportCompletedButton.IsEnabled = availability.CanExportCompleted;
        RefreshStatusButton.IsEnabled = availability.CanRefresh;
    }

    private async Task RunExclusiveOperationAsync(Func<Task> action)
    {
        _isExclusiveOperationRunning = true;
        RefreshCommandAvailability();

        try
        {
            await action();
        }
        finally
        {
            _isExclusiveOperationRunning = false;
            RefreshCommandAvailability();
        }
    }

    private static string BuildPersistedProgressStatusText(
        MetadataSyncProgressState metadataProgress,
        SyncDownloadProgressState downloadProgress)
    {
        var messages = new List<string>();

        if (string.Equals(metadataProgress.Status, SyncProgressStatuses.Running, StringComparison.Ordinal))
        {
            messages.Add(metadataProgress.StopRequested
                ? $"元数据同步正在停止：已处理 {metadataProgress.ProcessedPageCount.ToString(CultureInfo.InvariantCulture)}/{metadataProgress.TotalPageCount.ToString(CultureInfo.InvariantCulture)} 页，等待当前页完成。"
                : $"元数据同步进行中：已处理 {metadataProgress.ProcessedPageCount.ToString(CultureInfo.InvariantCulture)}/{metadataProgress.TotalPageCount.ToString(CultureInfo.InvariantCulture)} 页，下次页码 {metadataProgress.NextPage.ToString(CultureInfo.InvariantCulture)}。"
            );
        }
        else if (string.Equals(metadataProgress.Status, SyncProgressStatuses.Stopped, StringComparison.Ordinal))
        {
            messages.Add($"检测到未完成元数据同步进度，下次将从第 {metadataProgress.NextPage.ToString(CultureInfo.InvariantCulture)} 页继续。");
        }
        else if (string.Equals(metadataProgress.Status, SyncProgressStatuses.Completed, StringComparison.Ordinal)
            && metadataProgress.TotalPageCount > 0)
        {
            messages.Add($"元数据同步已完成：共处理 {metadataProgress.ProcessedPageCount.ToString(CultureInfo.InvariantCulture)}/{metadataProgress.TotalPageCount.ToString(CultureInfo.InvariantCulture)} 页。");
        }

        if (string.Equals(downloadProgress.Status, SyncProgressStatuses.Running, StringComparison.Ordinal))
        {
            messages.Add(downloadProgress.StopRequested
                ? $"同步下载正在停止：已处理 {downloadProgress.ProcessedCount.ToString(CultureInfo.InvariantCulture)} 项，等待当前作品完成。"
                : $"同步下载进行中：已处理 {downloadProgress.ProcessedCount.ToString(CultureInfo.InvariantCulture)} 项，成功 {downloadProgress.CompletedCount.ToString(CultureInfo.InvariantCulture)} 项，失败 {downloadProgress.FailedCount.ToString(CultureInfo.InvariantCulture)} 项。"
            );
        }
        else if (string.Equals(downloadProgress.Status, SyncProgressStatuses.Stopped, StringComparison.Ordinal))
        {
            messages.Add(string.IsNullOrWhiteSpace(downloadProgress.LastProcessedSourceId)
                ? "检测到未完成同步下载进度，下次将继续当前任务。"
                : $"检测到未完成同步下载进度，下次将从 {downloadProgress.LastProcessedSourceId} 之后继续。"
            );
        }
        else if (string.Equals(downloadProgress.Status, SyncProgressStatuses.Completed, StringComparison.Ordinal)
            && downloadProgress.ProcessedCount > 0)
        {
            messages.Add($"同步下载已完成：已处理 {downloadProgress.ProcessedCount.ToString(CultureInfo.InvariantCulture)} 项，成功 {downloadProgress.CompletedCount.ToString(CultureInfo.InvariantCulture)} 项，失败 {downloadProgress.FailedCount.ToString(CultureInfo.InvariantCulture)} 项。");
        }

        return messages.Count == 0
            ? "已刷新本地同步统计与进度。"
            : string.Join("；", messages);
    }

    private static string BuildPersistedProgressDetails(
        MetadataSyncProgressState metadataProgress,
        SyncDownloadProgressState downloadProgress)
    {
        return string.Join(Environment.NewLine,
        [
            $"元数据进度状态：{FormatProgressStatus(metadataProgress.Status)}",
            $"元数据停止请求：{(metadataProgress.StopRequested ? "是" : "否")}",
            $"元数据下次页码：{metadataProgress.NextPage.ToString(CultureInfo.InvariantCulture)}",
            $"元数据已处理分页：{metadataProgress.ProcessedPageCount.ToString(CultureInfo.InvariantCulture)}/{metadataProgress.TotalPageCount.ToString(CultureInfo.InvariantCulture)}",
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

    private static string FormatSourceId(string sourceId)
    {
        return string.IsNullOrWhiteSpace(sourceId) ? "暂无" : sourceId;
    }
}