using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Sync;
using Asmroner.Wpf.Services;
using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Views;

public partial class SyncView : UserControl
{
    private readonly IAppPathService _appPathService;
    private readonly IDialogService _dialogService;
    private readonly IUiMessageService _uiMessageService;
    private readonly ISyncExportService _syncExportService;
    private readonly ISyncService _syncService;

    private bool _isMetadataSyncRunning;
    private bool _isMetadataStopRequested;
    private bool _isDownloadSyncRunning;
    private bool _isDownloadStopRequested;
    private bool _isRetryFailedRunning;
    private bool _isExportRunning;
    private bool _isRefreshRunning;

    private int _metadataActionInFlight;
    private int _downloadActionInFlight;
    private int _refreshActionInFlight;

    private DateTimeOffset? _lastMetadataStartRequestedAt;
    private DateTimeOffset? _lastDownloadStartRequestedAt;

    public PageLoadState PageState { get; }

    public SyncView(
        ISyncService syncService,
        ISyncExportService syncExportService,
        IAppPathService appPathService,
        IDialogService dialogService,
        IPageLoadStateService pageLoadStateService,
        IUiMessageService uiMessageService)
    {
        _appPathService = appPathService;
        _dialogService = dialogService;
        _uiMessageService = uiMessageService;
        _syncExportService = syncExportService;
        _syncService = syncService;
        PageState = pageLoadStateService.Create("Sync");

        InitializeComponent();
        ShellStatusTextSynchronizer.Attach(StatusTextBlock, _uiMessageService, ShouldPublishMetadataShellStatus);
        ShellStatusTextSynchronizer.Attach(DownloadStatusTextBlock, _uiMessageService, ShouldPublishDownloadShellStatus);
        PageState.ShowEmpty("暂无同步记录", "执行元数据同步、同步下载或刷新统计后，这里会显示汇总信息与执行摘要。");
        RefreshCommandAvailability();
        Loaded += OnLoaded;
    }

    /// <summary>
    /// 处理 Sync 页面加载并刷新同步摘要。
    /// </summary>
    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await RefreshSnapshotAsync(updateStatusText: false);
    }

    /// <summary>
    /// 处理“刷新统计”按钮点击。
    /// </summary>
    private async void OnRefreshStatusClicked(object sender, RoutedEventArgs e)
    {
        if (Interlocked.Exchange(ref _refreshActionInFlight, 1) == 1)
        {
            return;
        }

        _isRefreshRunning = true;
        PageState.ShowBusy("正在刷新同步统计，请稍候...");
        RefreshCommandAvailability();

        try
        {
            await RefreshSnapshotAsync(updateStatusText: true);
            await Dispatcher.InvokeAsync(static () => { }, DispatcherPriority.ApplicationIdle);
        }
        finally
        {
            _isRefreshRunning = false;
            PageState.HideBusy();
            RefreshCommandAvailability();
            Interlocked.Exchange(ref _refreshActionInFlight, 0);
        }
    }

    /// <summary>
    /// 处理“开始同步元数据 / 停止同步元数据”按钮点击。
    /// </summary>
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
                SetMetadataStatus(SyncStatusTextBuilder.BuildMetadataActionStatus("已请求停止元数据同步，将在当前页完成后停止。"));
                return;
            }

            _lastMetadataStartRequestedAt = now;
            _isMetadataSyncRunning = true;
            _isMetadataStopRequested = false;
            PageState.ShowBusy("正在同步元数据，请稍候...");
            RefreshCommandAvailability();
            SetMetadataStatus(SyncStatusTextBuilder.BuildMetadataActionStatus("正在同步元数据，请稍候..."));
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
            SetMetadataStatus(SyncStatusTextBuilder.BuildMetadataActionStatus($"同步失败：{ex.Message}"));
            DetailsTextBox.Text = ex.ToString();
        }
        finally
        {
            _isMetadataSyncRunning = false;
            _isMetadataStopRequested = false;
            PageState.HideBusy();
            RefreshCommandAvailability();
        }
    }

    /// <summary>
    /// 处理“开始同步下载 / 停止同步下载”按钮点击。
    /// </summary>
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
                SetDownloadStatus(SyncStatusTextBuilder.BuildDownloadActionStatus("已请求停止同步下载，将在当前作品完成后停止。"));
                return;
            }

            _lastDownloadStartRequestedAt = now;
            _isDownloadSyncRunning = true;
            _isDownloadStopRequested = false;
            PageState.ShowBusy("正在执行同步下载，请稍候...");
            RefreshCommandAvailability();
            SetDownloadStatus(SyncStatusTextBuilder.BuildDownloadActionStatus("正在执行同步下载，请稍候..."));
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
            SetDownloadStatus(SyncStatusTextBuilder.BuildDownloadActionStatus($"同步下载失败：{ex.Message}"));
            DetailsTextBox.Text = ex.ToString();
        }
        finally
        {
            _isDownloadSyncRunning = false;
            _isDownloadStopRequested = false;
            PageState.HideBusy();
            RefreshCommandAvailability();
        }
    }

    /// <summary>
    /// 处理“重试失败任务”按钮点击。
    /// </summary>
    private async void OnRetryFailedClicked(object sender, RoutedEventArgs e)
    {
        await RunRetryOperationAsync(async () =>
        {
            SetDownloadStatus(SyncStatusTextBuilder.BuildDownloadActionStatus("正在重试失败同步下载，请稍候..."));
            DetailsTextBox.Text = SyncProgressDetailsBuilder.BuildRetryPendingDetails();

            try
            {
                var result = await _syncService.RetryFailedAsync();
                ApplyResult(result);
                await ApplyReportAsync();
            }
            catch (Exception ex)
            {
                SetDownloadStatus(SyncStatusTextBuilder.BuildDownloadActionStatus($"失败重试执行失败：{ex.Message}"));
                DetailsTextBox.Text = ex.ToString();
            }
        });
    }

    /// <summary>
    /// 处理“导出失败记录”按钮点击。
    /// </summary>
    private async void OnExportFailedClicked(object sender, RoutedEventArgs e)
    {
        await RunExportOperationAsync(async () => await ExportAsync(SyncExportStatus.Failed));
    }

    /// <summary>
    /// 处理“导出已完成记录”按钮点击。
    /// </summary>
    private async void OnExportCompletedClicked(object sender, RoutedEventArgs e)
    {
        await RunExportOperationAsync(async () => await ExportAsync(SyncExportStatus.Completed));
    }

    private async Task RefreshSnapshotAsync(bool updateStatusText)
    {
        try
        {
            var report = await _syncService.GetReportAsync();
            ApplyReport(report);
            await ApplyPersistedProgressSummaryAsync(updateStatusText, report);
        }
        catch (Exception ex)
        {
            SetMetadataStatus(SyncStatusTextBuilder.BuildMetadataActionStatus($"读取同步统计失败：{ex.Message}"));
            if (!_isRetryFailedRunning)
            {
                SetDownloadStatus(SyncStatusTextBuilder.BuildDownloadActionStatus($"读取同步统计失败：{ex.Message}"));
            }

            DetailsTextBox.Text = ex.ToString();
        }
    }

    private void ApplyReport(SyncReportSnapshot report)
    {
        UpdateSyncPageState(report);
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
        SetMetadataStatus(SyncStatusTextBuilder.BuildMetadataActionStatus(result.Message));
        DetailsTextBox.Text = string.Join(Environment.NewLine,
        [
            $"网站总量：{result.RemoteTotalCount.ToString(CultureInfo.InvariantCulture)}",
            $"网站字幕量：{result.RemoteSubtitleCount.ToString(CultureInfo.InvariantCulture)}",
            $"同步前本地总量：{result.LocalTotalCountBefore.ToString(CultureInfo.InvariantCulture)}",
            $"同步前本地字幕量：{result.LocalSubtitleCountBefore.ToString(CultureInfo.InvariantCulture)}",
            $"同步后本地总量：{result.LocalTotalCountAfter.ToString(CultureInfo.InvariantCulture)}",
            $"同步后本地字幕量：{result.LocalSubtitleCountAfter.ToString(CultureInfo.InvariantCulture)}",
            $"本次处理：{result.ProcessedWorkCount.ToString(CultureInfo.InvariantCulture)}",
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
        SetDownloadStatus(SyncStatusTextBuilder.BuildDownloadActionStatus(result.Message));
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
        SetDownloadStatus(SyncStatusTextBuilder.BuildDownloadActionStatus(result.Message));
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
        SetDownloadStatus(SyncStatusTextBuilder.BuildDownloadActionStatus(result.Message));
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

    private async Task ApplyPersistedProgressSummaryAsync(bool updateStatusText, SyncReportSnapshot report)
    {
        var metadataProgress = await _syncService.GetMetadataSyncProgressAsync();

        if (_isRetryFailedRunning)
        {
            DetailsTextBox.Text = SyncProgressDetailsBuilder.BuildRetryRefreshDetails(report);

            if (updateStatusText)
            {
                SetMetadataStatus(SyncStatusTextBuilder.BuildMetadataStatus(metadataProgress));
            }

            return;
        }

        var downloadProgress = await _syncService.GetSyncDownloadProgressAsync();

        DetailsTextBox.Text = SyncProgressDetailsBuilder.BuildPersistedProgressDetails(metadataProgress, downloadProgress);

        if (updateStatusText)
        {
            SetMetadataStatus(SyncStatusTextBuilder.BuildMetadataStatus(metadataProgress));
            SetDownloadStatus(SyncStatusTextBuilder.BuildDownloadStatus(downloadProgress));
        }
    }

    private async Task ExportAsync(SyncExportStatus status)
    {
        try
        {
            Directory.CreateDirectory(_appPathService.MetadataDirectory);
            var exportTarget = _dialogService.ShowSaveFileDialog(new SaveFileDialogOptions
            {
                Title = "导出同步记录",
                InitialDirectory = _appPathService.MetadataDirectory,
                FileName = BuildDefaultFileName(status),
            });
            if (exportTarget is null)
            {
                SetDownloadStatus(SyncStatusTextBuilder.BuildDownloadActionStatus("已取消导出。"));
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
            SetDownloadStatus(SyncStatusTextBuilder.BuildDownloadActionStatus($"导出失败：{ex.Message}"));
            DetailsTextBox.Text = ex.ToString();
        }
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
            _isRetryFailedRunning,
            _isExportRunning,
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

    private async Task RunRetryOperationAsync(Func<Task> action)
    {
        _isRetryFailedRunning = true;
        PageState.ShowBusy("正在重试失败同步下载，请稍候...");
        RefreshCommandAvailability();

        try
        {
            await action();
        }
        finally
        {
            _isRetryFailedRunning = false;
            PageState.HideBusy();
            RefreshCommandAvailability();
        }
    }

    private async Task RunExportOperationAsync(Func<Task> action)
    {
        _isExportRunning = true;
        PageState.ShowBusy("正在导出同步记录，请稍候...");
        RefreshCommandAvailability();

        try
        {
            await action();
        }
        finally
        {
            _isExportRunning = false;
            PageState.HideBusy();
            RefreshCommandAvailability();
        }
    }

    private void SetMetadataStatus(string text)
    {
        StatusTextBlock.Text = text;
    }

    private void SetDownloadStatus(string text)
    {
        DownloadStatusTextBlock.Text = text;
    }

    private bool ShouldPublishMetadataShellStatus()
    {
        return SyncShellStatusRelayPolicy.ShouldPublishMetadata(
            isViewVisible: IsLoaded && IsVisible,
            isMetadataSyncRunning: _isMetadataSyncRunning,
            isMetadataStopRequested: _isMetadataStopRequested,
            isDownloadSyncRunning: _isDownloadSyncRunning,
            isDownloadStopRequested: _isDownloadStopRequested,
            isRetryFailedRunning: _isRetryFailedRunning,
            isExportRunning: _isExportRunning,
            isRefreshRunning: _isRefreshRunning);
    }

    private bool ShouldPublishDownloadShellStatus()
    {
        return SyncShellStatusRelayPolicy.ShouldPublishDownload(
            isViewVisible: IsLoaded && IsVisible,
            isDownloadSyncRunning: _isDownloadSyncRunning,
            isDownloadStopRequested: _isDownloadStopRequested,
            isRetryFailedRunning: _isRetryFailedRunning,
            isExportRunning: _isExportRunning);
    }

    private static string FormatSourceId(string sourceId)
    {
        return string.IsNullOrWhiteSpace(sourceId) ? "暂无" : sourceId;
    }

    private void UpdateSyncPageState(SyncReportSnapshot report)
    {
        var hasData = report.MetadataTotalCount > 0
            || report.CompletedCount > 0
            || report.FailedCount > 0
            || report.PendingCount > 0
            || report.RemainingMetadataCount > 0;

        if (hasData)
        {
            PageState.ClearEmpty();
            return;
        }

        PageState.ShowEmpty("暂无同步记录", "执行元数据同步、同步下载或刷新统计后，这里会显示汇总信息与执行摘要。");
    }
}