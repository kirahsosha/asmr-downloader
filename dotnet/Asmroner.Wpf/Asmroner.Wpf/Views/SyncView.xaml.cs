using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Sync;
using Asmroner.Wpf.Services;
using Microsoft.Win32;

namespace Asmroner.Wpf.Views;

public partial class SyncView : UserControl
{
    private readonly IAppPathService _appPathService;
    private readonly ISyncExportService _syncExportService;
    private readonly ISyncService _syncService;

    public SyncView(ISyncService syncService, ISyncExportService syncExportService, IAppPathService appPathService)
    {
        _appPathService = appPathService;
        _syncExportService = syncExportService;
        _syncService = syncService;

        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await RefreshSnapshotAsync(updateStatusText: false);
    }

    private async void OnRefreshStatusClicked(object sender, RoutedEventArgs e)
    {
        await RefreshSnapshotAsync(updateStatusText: true);
    }

    private async void OnSyncMetadataClicked(object sender, RoutedEventArgs e)
    {
        SetButtonsEnabled(false);
        StatusTextBlock.Text = "正在同步元数据，请稍候...";

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
            SetButtonsEnabled(true);
        }
    }

    private async void OnSyncDownloadClicked(object sender, RoutedEventArgs e)
    {
        SetButtonsEnabled(false);
        StatusTextBlock.Text = "正在执行同步下载，请稍候...";

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
            SetButtonsEnabled(true);
        }
    }

    private async void OnRetryFailedClicked(object sender, RoutedEventArgs e)
    {
        SetButtonsEnabled(false);
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
        finally
        {
            SetButtonsEnabled(true);
        }
    }

    private async void OnExportFailedClicked(object sender, RoutedEventArgs e)
    {
        await ExportAsync(SyncExportStatus.Failed);
    }

    private async void OnExportCompletedClicked(object sender, RoutedEventArgs e)
    {
        await ExportAsync(SyncExportStatus.Completed);
    }

    private async Task RefreshSnapshotAsync(bool updateStatusText)
    {
        SetButtonsEnabled(false);

        try
        {
            await ApplyReportAsync();
            if (updateStatusText)
            {
                StatusTextBlock.Text = "已刷新本地同步统计与报表。";
            }
        }
        catch (Exception ex)
        {
            StatusTextBlock.Text = $"读取同步统计失败: {ex.Message}";
        }
        finally
        {
            SetButtonsEnabled(true);
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
            $"是否追平网站：{(result.IsUpToDate ? "是" : "否")}",
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

    private async Task ExportAsync(SyncExportStatus status)
    {
        SetButtonsEnabled(false);

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
        finally
        {
            SetButtonsEnabled(true);
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

    private void SetButtonsEnabled(bool enabled)
    {
        SyncMetadataButton.IsEnabled = enabled;
        SyncDownloadButton.IsEnabled = enabled;
        RetryFailedButton.IsEnabled = enabled;
        ExportFailedButton.IsEnabled = enabled;
        ExportCompletedButton.IsEnabled = enabled;
        RefreshStatusButton.IsEnabled = enabled;
    }
}