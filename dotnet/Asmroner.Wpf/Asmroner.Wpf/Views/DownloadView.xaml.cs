using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Asmroner.Core.Api;
using Asmroner.Core.Download;
using Asmroner.Core.Interfaces;
using Asmroner.Wpf.ViewModels;
using Microsoft.Extensions.Logging;

namespace Asmroner.Wpf.Views;

public partial class DownloadView : UserControl
{
    private const int RetryAllMaxConcurrency = 2;

    private readonly IAsmrApiClient _asmrApiClient;
    private readonly IDownloadService _downloadService;
    private readonly ISearchStateStore _searchStateStore;
    private readonly IConfigurationService _configurationService;
    private readonly IAppPathService _appPathService;
    private readonly ILogger<DownloadView> _logger;
    private readonly ConcurrentDictionary<string, string> _queuedWorkInfoTitles = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, DownloadTaskStatus> _queuedStatusOverrides = new(StringComparer.OrdinalIgnoreCase);

    public DownloadView()
        : this(null!, null!, null!, null!, null!, null!)
    {
    }

    public DownloadView(
        IAsmrApiClient asmrApiClient,
        IDownloadService downloadService,
        ISearchStateStore searchStateStore,
        IConfigurationService configurationService,
        IAppPathService appPathService,
        ILogger<DownloadView> logger)
    {
        _asmrApiClient = asmrApiClient;
        _downloadService = downloadService;
        _searchStateStore = searchStateStore;
        _configurationService = configurationService;
        _appPathService = appPathService;
        _logger = logger;

        InitializeComponent();
        Loaded += async (_, _) =>
        {
            await LoadFilterFromConfigAsync();
            RefreshView();
            UpdateSelectionActions();
        };
    }

    private async Task LoadFilterFromConfigAsync()
    {
        try
        {
            var config = await _configurationService.LoadAsync();
            if (config?.Downloader.FileFilter is { Length: > 0 } filter)
            {
                FilterTextBox.Text = filter;
            }
        }
        catch
        {
            // 配置加载失败时保持文本框为空
        }
    }

    private async void OnAddSingleClicked(object sender, RoutedEventArgs e)
    {
        var sourceId = DownloadInputNormalizer.NormalizeSingleInputDisplay(SingleSourceIdTextBox.Text);
        ApplyNormalizedText(SingleSourceIdTextBox, sourceId);
        if (string.IsNullOrWhiteSpace(sourceId))
        {
            StatusTextBlock.Text = "请输入 RJID 后再加入下载队列。";
            return;
        }

        await ExecuteGuardedAsync(
            async () =>
            {
                StatusTextBlock.Text = "正在获取作品信息...";

                var workInfos = await ResolveWorkInfoAsync(new[] { sourceId });
                _searchStateStore.EnqueueForDownload(new[] { sourceId });
                _downloadService.UpsertPrefetchedWorkInfo(workInfos);
                _queuedStatusOverrides.TryRemove(sourceId, out _);
                MergeQueuedWorkInfoTitles(DownloadWorkInfoTitlePolicy.BuildNonEmptyTitleMap(workInfos));

                RefreshView();
                StatusTextBlock.Text = $"已加入单个下载：{sourceId}";
            },
            disableRunQueue: true,
            failurePrefix: "加入单个下载失败",
            logMessage: "Failed to enqueue single download with work info.");
    }

    private async void OnAddBatchClicked(object sender, RoutedEventArgs e)
    {
        var sourceIds = DownloadInputNormalizer.ExtractNormalizedSourceIds(BatchSourceIdsTextBox.Text);
        ApplyNormalizedText(BatchSourceIdsTextBox, DownloadInputNormalizer.NormalizeBatchInputForSubmit(BatchSourceIdsTextBox.Text));
        if (sourceIds.Count == 0)
        {
            StatusTextBlock.Text = "请先输入至少一个 RJID。";
            return;
        }

        await ExecuteGuardedAsync(
            async () =>
            {
                StatusTextBlock.Text = $"正在获取 {sourceIds.Count} 个作品信息...";

                var workInfos = await ResolveWorkInfoAsync(sourceIds);
                _searchStateStore.EnqueueForDownload(sourceIds);
                _downloadService.UpsertPrefetchedWorkInfo(workInfos);
                foreach (var sourceIdItem in sourceIds)
                {
                    _queuedStatusOverrides.TryRemove(sourceIdItem, out _);
                }
                MergeQueuedWorkInfoTitles(DownloadWorkInfoTitlePolicy.BuildNonEmptyTitleMap(workInfos));

                RefreshView();

                var resolvedCount = workInfos.Count;
                StatusTextBlock.Text = DownloadOperationStatusTexts.BuildBatchEnqueueResult(sourceIds.Count, resolvedCount);
            },
            disableRunQueue: true,
            failurePrefix: "加入批量下载失败",
            logMessage: "Failed to enqueue batch download with work info.");
    }

    private async void OnRunQueueClicked(object sender, System.Windows.RoutedEventArgs e)
    {
        await ExecuteGuardedAsync(
            async () =>
            {
                StatusTextBlock.Text = "正在执行下载队列...";
                var context = DownloadOperationContext.Create(FilterTextBox.Text);

                var created = await _downloadService.RunQueuedAsync(
                    context.FileFilter);
                RefreshView();
                StatusTextBlock.Text = DownloadOperationStatusTexts.BuildRunQueueResult(created.Count);
            },
            disableRunQueue: true,
            updateSelectionOnFinally: true,
            failurePrefix: "下载执行失败",
            logMessage: "Failed to run queued downloads.");
    }

    private void OnRefreshClicked(object sender, System.Windows.RoutedEventArgs e)
    {
        RefreshView();
        StatusTextBlock.Text = "任务列表已刷新。";
    }

    private async void OnCancelClicked(object sender, System.Windows.RoutedEventArgs e)
    {
        var precheck = DownloadOperationPrecheckPolicy.CheckCancel(GetSelectedTasks());
        if (!precheck.CanProceed)
        {
            StatusTextBlock.Text = precheck.StatusText;
            return;
        }
        var cancelable = precheck.Cancelable;

        if (!ConfirmWithQuestion(
                DownloadOperationPrompts.BuildCancelConfirmMessage(cancelable.Count),
                "确认取消任务"))
        {
            return;
        }

        await ExecuteGuardedAsync(
            async () =>
            {
                var canceledCount = 0;
                foreach (var task in cancelable)
                {
                    if (task.TaskId == Guid.Empty)
                    {
                        _searchStateStore.RemoveFromQueue(new[] { task.SourceId });
                        _queuedStatusOverrides[task.SourceId] = DownloadTaskStatus.Canceled;
                        canceledCount++;
                        continue;
                    }

                    if (await _downloadService.CancelAsync(task.TaskId))
                    {
                        canceledCount++;
                    }
                }

                RefreshView();
                StatusTextBlock.Text = DownloadOperationStatusTexts.BuildCancelResult(canceledCount, cancelable.Count);
            },
            disableSelectionActions: true,
            updateSelectionOnFinally: true,
            failurePrefix: "取消任务失败",
            logMessage: "Failed to cancel selected download tasks.");
    }

    private async void OnRetryClicked(object sender, System.Windows.RoutedEventArgs e)
    {
        var precheck = DownloadOperationPrecheckPolicy.CheckRetrySingle(GetSelectedTasks());
        if (!precheck.CanProceed)
        {
            StatusTextBlock.Text = precheck.StatusText;
            return;
        }
        var target = precheck.Target!;

        await ExecuteGuardedAsync(
            async () =>
            {
                var context = DownloadOperationContext.Create(FilterTextBox.Text);
                var retried = await _downloadService.RetryFailedAsync(
                    target.TaskId,
                    context.FileFilter);
                RefreshView();
                StatusTextBlock.Text = DownloadOperationStatusTexts.BuildRetryResult(retried is not null, target.SourceId);
            },
            disableSelectionActions: true,
            updateSelectionOnFinally: true,
            failurePrefix: "重试任务失败",
            logMessage: $"Failed to retry download task {target.TaskId}.");
    }

    private async void OnRetryAllFailedClicked(object sender, RoutedEventArgs e)
    {
        var failedTasks = DownloadTaskSnapshotPolicy.GetFailedTasks(_downloadService.GetTasks());

        var precheck = DownloadOperationPrecheckPolicy.CheckRetryAllFailed(failedTasks);
        if (!precheck.CanProceed)
        {
            StatusTextBlock.Text = precheck.StatusText;
            return;
        }

        failedTasks = precheck.FailedTasks.ToArray();
        var failedSourceIds = failedTasks.Select(static item => item.SourceId).ToArray();
        if (!ConfirmWithQuestion(
                DownloadOperationPrompts.BuildRetryAllConfirmMessage(RetryAllMaxConcurrency, failedSourceIds),
                "确认批量重试"))
        {
            return;
        }

        await ExecuteGuardedAsync(
            async () =>
            {
                var semaphore = new SemaphoreSlim(RetryAllMaxConcurrency);
                var context = DownloadOperationContext.Create(FilterTextBox.Text);
                var jobs = failedTasks.Select(async item =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        var retried = await _downloadService.RetryFailedAsync(item.TaskId, context.FileFilter);
                        return retried is not null;
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                var results = await Task.WhenAll(jobs);
                var retriedCount = results.Count(static value => value);

                RefreshView();
                StatusTextBlock.Text = DownloadOperationStatusTexts.BuildRetryAllResult(retriedCount, failedTasks.Count);
            },
            disableRunQueue: true,
            disableSelectionActions: true,
            updateSelectionOnFinally: true,
            failurePrefix: "批量重试失败",
            logMessage: "Failed to retry all failed download tasks.");
    }

    private void OnTaskSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        UpdateSelectionActions();
    }

    private async void OnStartSelectedClicked(object sender, RoutedEventArgs e)
    {
        var precheck = DownloadOperationPrecheckPolicy.CheckStartImmediate(GetSelectedTasks());
        if (!precheck.CanProceed)
        {
            StatusTextBlock.Text = precheck.StatusText;
            return;
        }
        var selected = precheck.Targets;

        await ExecuteGuardedAsync(
            async () =>
            {
                var context = DownloadOperationContext.Create(FilterTextBox.Text);
                var startedCount = 0;

                foreach (var task in selected)
                {
                    _searchStateStore.RemoveFromQueue(new[] { task.SourceId });
                    _queuedStatusOverrides.TryRemove(task.SourceId, out _);

                    Guid? preferredTaskId = task.TaskId == Guid.Empty ? null : task.TaskId;
                    var started = await _downloadService.StartAsync(task.SourceId, context.FileFilter, preferredTaskId);
                    if (started is not null)
                    {
                        startedCount++;
                    }
                }

                RefreshView();
                StatusTextBlock.Text = DownloadOperationStatusTexts.BuildStartSelectedResult(startedCount, selected.Count);
            },
            disableRunQueue: true,
            disableSelectionActions: true,
            updateSelectionOnFinally: true,
            failurePrefix: "立即下载失败",
            logMessage: "Failed to start selected download tasks immediately.");
    }

    private void OnSingleSourceIdTextChanged(object sender, TextChangedEventArgs e)
    {
        // 单项输入改为“提交时规范化”，避免实时改写打断输入。
    }

    private void OnBatchSourceIdsTextChanged(object sender, TextChangedEventArgs e)
    {
        // 批量输入框不做实时改写，避免影响空格/逗号/分号输入与多次粘贴体验。
    }

    private async void OnOpenDownloadDirectoryClicked(object sender, RoutedEventArgs e)
    {
        try
        {
            var config = await _configurationService.LoadAsync();
            var path = DownloadDirectoryPathPolicy.Resolve(
                config?.Downloader.SyncDataFolder,
                _appPathService.DefaultSyncDataDirectory);

            Directory.CreateDirectory(path);

            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true,
            });

            StatusTextBlock.Text = $"已打开下载目录：{path}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to open download directory.");
            StatusTextBlock.Text = $"打开下载目录失败：{ex.Message}";
        }
    }

    private void RefreshView()
    {
        var activeTasks = _downloadService.GetTasks();
        var activeSourceIds = DownloadTaskSnapshotPolicy.GetActiveSourceIds(activeTasks);
        var queuedSourceIds = _searchStateStore.GetQueuedSourceIds();

        var prefetched = _downloadService.GetPrefetchedWorkInfoSnapshot();
        var cacheSnapshot = DownloadQueueCachePolicy.ReconcileWithQueuedSourceIds(
            activeSourceIds,
            queuedSourceIds,
            prefetched,
            _queuedWorkInfoTitles,
            _queuedStatusOverrides);
        ReplaceQueuedCache(cacheSnapshot);

        var combined = DownloadTaskListComposer.ComposeRows(
            activeTasks,
            queuedSourceIds,
            _queuedWorkInfoTitles,
            _queuedStatusOverrides);

        TaskGrid.ItemsSource = combined;
        QueueCountTextBlock.Text = $"待下载队列：{queuedSourceIds.Count}";
        UpdateSelectionActions();
    }

    private void ReplaceQueuedCache(DownloadQueueCacheSnapshot cacheSnapshot)
    {
        _queuedWorkInfoTitles.Clear();
        foreach (var (sourceId, title) in cacheSnapshot.Titles)
        {
            _queuedWorkInfoTitles[sourceId] = title;
        }

        _queuedStatusOverrides.Clear();
        foreach (var (sourceId, status) in cacheSnapshot.StatusOverrides)
        {
            _queuedStatusOverrides[sourceId] = status;
        }
    }

    private void ToggleRunQueue(bool isEnabled)
    {
        RunQueueButton.IsEnabled = isEnabled;
        AddSingleButton.IsEnabled = isEnabled;
        AddBatchButton.IsEnabled = isEnabled;
        OpenDownloadDirectoryButton.IsEnabled = isEnabled;
    }

    private void ToggleSelectionActions(bool isEnabled)
    {
        CancelButton.IsEnabled = isEnabled;
        RetryButton.IsEnabled = isEnabled;
        RetryAllFailedButton.IsEnabled = isEnabled;
        StartSelectedButton.IsEnabled = isEnabled;
    }

    private void UpdateSelectionActions()
    {
        var selected = GetSelectedTasks();
        // 允许未下载占位项（TaskId=Empty）参与取消按钮可用性判断。
        var availability = DownloadCommandAvailability.Evaluate(
            selected.Select(static item => item.Status),
            _downloadService.GetTasks());

        CancelButton.IsEnabled = availability.CanCancel;
        RetryButton.IsEnabled = availability.CanRetry;
        RetryAllFailedButton.IsEnabled = availability.CanRetryAllFailed;
        StartSelectedButton.IsEnabled = availability.CanStartImmediate;
    }

    private List<DownloadTaskRowViewModel> GetSelectedTasks()
    {
        return TaskGrid.SelectedItems
            .OfType<DownloadTaskRowViewModel>()
            .ToList();
    }

    private void ApplyNormalizedText(TextBox textBox, string normalized)
    {
        if (textBox.Text == normalized)
        {
            return;
        }

        textBox.Text = normalized;
        textBox.SelectionStart = textBox.Text.Length;
    }

    private bool ConfirmWithQuestion(string message, string title)
    {
        var result = MessageBox.Show(
            message,
            title,
            MessageBoxButton.YesNo,
            MessageBoxImage.Question,
            MessageBoxResult.No);

        var decision = DownloadConfirmationPolicy.Evaluate(result);
        if (!decision.ShouldContinue)
        {
            StatusTextBlock.Text = decision.StatusText;
            return false;
        }

        return true;
    }

    private async Task<IReadOnlyDictionary<string, WorkInfoDto>> ResolveWorkInfoAsync(IReadOnlyList<string> sourceIds)
    {
        var result = new ConcurrentDictionary<string, WorkInfoDto>(StringComparer.OrdinalIgnoreCase);
        using var limiter = new SemaphoreSlim(4);

        var jobs = sourceIds.Select(async sourceId =>
        {
            await limiter.WaitAsync();
            try
            {
                var workInfo = await _asmrApiClient.GetWorkInfoAsync(sourceId);
                result[sourceId] = workInfo;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Fetch work info failed for {SourceId} before enqueue.", sourceId);
            }
            finally
            {
                limiter.Release();
            }
        });

        await Task.WhenAll(jobs);
        return result;
    }

    private void MergeQueuedWorkInfoTitles(IReadOnlyDictionary<string, string> titles)
    {
        foreach (var (sourceId, title) in titles)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                continue;
            }

            _queuedWorkInfoTitles[sourceId] = title;
        }
    }

    private async Task ExecuteGuardedAsync(
        Func<Task> action,
        string failurePrefix,
        string logMessage,
        bool disableRunQueue = false,
        bool disableSelectionActions = false,
        bool updateSelectionOnFinally = false)
    {
        if (disableRunQueue)
        {
            ToggleRunQueue(false);
        }

        if (disableSelectionActions)
        {
            ToggleSelectionActions(false);
        }

        try
        {
            await action();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, logMessage);
            StatusTextBlock.Text = $"{failurePrefix}：{ex.Message}";
        }
        finally
        {
            if (disableRunQueue)
            {
                ToggleRunQueue(true);
            }

            if (updateSelectionOnFinally || disableSelectionActions)
            {
                UpdateSelectionActions();
            }
        }
    }

}
