using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Asmroner.Core.Api;
using Asmroner.Core.Configuration;
using Asmroner.Core.Constants;
using Asmroner.Core.Download;
using Asmroner.Core.Interfaces;
using Asmroner.Wpf.Services;
using Asmroner.Wpf.ViewModels;
using NLog;

namespace Asmroner.Wpf.Views;

public partial class DownloadView : UserControl
{
    private const int RetryAllMaxConcurrency = AsmronerConstants.Download.RetryAllMaxConcurrency;
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private readonly IDownloadService _downloadService;
    private readonly IEnqueueWorkInfoResolver _enqueueWorkInfoResolver;
    private readonly IMetadataWorkInfoResolver _metadataWorkInfoResolver;
    private readonly IFavoriteStore _favoriteStore;
    private readonly ISearchStateStore _searchStateStore;
    private readonly IUiStateStore _uiStateStore;
    private readonly IConfigurationService _configurationService;
    private readonly IAppPathService _appPathService;
    private readonly ISearchImportService _importService;
    private readonly IDialogService _dialogService;
    private readonly StartupUnfinishedQueueMetadataRefreshService _startupUnfinishedQueueMetadataRefreshService;
    private readonly ConcurrentDictionary<string, string> _queuedWorkInfoTitles = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, DownloadTaskStatus> _queuedStatusOverrides = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, string> _queuedErrorMessages = new(StringComparer.OrdinalIgnoreCase);
    private bool _isApplyingDownloadUiState;

    public DownloadView()
        : this(null!, null!, null!, null!, null!, null!, null!, null!, null!, null!, null!)
    {
    }

    public DownloadView(
        IDownloadService downloadService,
        IEnqueueWorkInfoResolver enqueueWorkInfoResolver,
        IMetadataWorkInfoResolver metadataWorkInfoResolver,
        IFavoriteStore favoriteStore,
        ISearchStateStore searchStateStore,
        IUiStateStore uiStateStore,
        IConfigurationService configurationService,
        IAppPathService appPathService,
        ISearchImportService importService,
        IDialogService dialogService,
        StartupUnfinishedQueueMetadataRefreshService startupUnfinishedQueueMetadataRefreshService)
    {
        _downloadService = downloadService;
        _enqueueWorkInfoResolver = enqueueWorkInfoResolver;
        _metadataWorkInfoResolver = metadataWorkInfoResolver;
        _favoriteStore = favoriteStore;
        _searchStateStore = searchStateStore;
        _uiStateStore = uiStateStore;
        _configurationService = configurationService;
        _appPathService = appPathService;
        _importService = importService;
        _dialogService = dialogService;
        _startupUnfinishedQueueMetadataRefreshService = startupUnfinishedQueueMetadataRefreshService;

        InitializeComponent();
        Loaded += async (_, _) =>
        {
            await LoadDownloadUiStateAsync();
            await RestoreUnfinishedQueueAsync();
            RefreshView();
            UpdateSelectionActions();
        };

        FilterTextBox.TextChanged += OnDownloadUiStateChanged;
        HdAudioOnlyCheckBox.Checked += OnDownloadUiStateChanged;
        HdAudioOnlyCheckBox.Unchecked += OnDownloadUiStateChanged;
        QueueTranslationCheckBox.Checked += OnDownloadUiStateChanged;
        QueueTranslationCheckBox.Unchecked += OnDownloadUiStateChanged;
    }

    public void StartUnfinishedQueueMetadataRefreshInBackground()
    {
        _ = RefreshUnfinishedQueueMetadataInBackgroundAsync();
    }

    private async Task LoadDownloadUiStateAsync()
    {
        try
        {
            _isApplyingDownloadUiState = true;
            var state = await _uiStateStore.LoadDownloadUiStateAsync();
            FilterTextBox.Text = state.FileFilter;
            HdAudioOnlyCheckBox.IsChecked = state.HdAudioOnly;
            QueueTranslationCheckBox.IsChecked = state.QueueTranslationWorks;
        }
        catch
        {
            // 配置加载失败时保持文本框为空
            HdAudioOnlyCheckBox.IsChecked = true;
            QueueTranslationCheckBox.IsChecked = true;
        }
        finally
        {
            _isApplyingDownloadUiState = false;
        }
    }

    private async Task RestoreUnfinishedQueueAsync()
    {
        try
        {
            var unfinishedSourceIds = await _uiStateStore.LoadUnfinishedQueueAsync();
            if (unfinishedSourceIds.Count == 0)
            {
                return;
            }

            _searchStateStore.EnqueueForDownload(unfinishedSourceIds);
        }
        catch (Exception ex)
        {
            _logger.Warn(ex, "Failed to restore unfinished queue.");
        }
    }

    private async Task RefreshUnfinishedQueueMetadataInBackgroundAsync()
    {
        try
        {
            var refreshResult = await _startupUnfinishedQueueMetadataRefreshService.StartInBackgroundAsync();
            if (refreshResult.UpdatedWorkInfos.Count == 0 && refreshResult.Failures.Count == 0)
            {
                return;
            }

            await Dispatcher.InvokeAsync(() =>
            {
                ApplyStartupMetadataRefreshResult(refreshResult);
                RefreshView();
                StatusTextBlock.Text = DownloadOperationStatusTexts.BuildWorkInfoRefreshResult(
                    refreshResult.UpdatedWorkInfos.Count,
                    refreshResult.Failures.Count);
            });
        }
        catch (Exception ex)
        {
            _logger.Warn(ex, "Failed to refresh unfinished queue metadata in background.");
        }
    }

    /// <summary>
    /// 处理 Download 界面控件变更并自动保存界面状态。
    /// </summary>
    private void OnDownloadUiStateChanged(object? sender, EventArgs e)
    {
        if (_isApplyingDownloadUiState)
        {
            return;
        }

        _ = SaveDownloadUiStateSafeAsync();
    }

    private async Task SaveDownloadUiStateSafeAsync()
    {
        try
        {
            var state = new DownloadUiState
            {
                FileFilter = FilterTextBox.Text,
                HdAudioOnly = HdAudioOnlyCheckBox.IsChecked == true,
                QueueTranslationWorks = QueueTranslationCheckBox.IsChecked == true,
            };

            await _uiStateStore.SaveDownloadUiStateAsync(state);
        }
        catch (Exception ex)
        {
            _logger.Warn(ex, "Failed to persist download UI state.");
        }
    }

    /// <summary>
    /// 处理“加入单个下载”按钮点击。
    /// </summary>
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
                IReadOnlyDictionary<string, WorkInfoDto> workInfos;
                IReadOnlyList<string> candidateSourceIds;
                var failedCount = 0;
                var switchedCount = 0;

                if (QueueTranslationCheckBox.IsChecked == true)
                {
                    StatusTextBlock.Text = "正在分析作品语言并获取作品信息...";
                    var resolution = await _enqueueWorkInfoResolver.ResolvePreferTranslatedAsync(BuildEnqueueRequests(new[] { sourceId }));
                    workInfos = resolution.WorkInfos;
                    candidateSourceIds = resolution.WorkInfos.Keys.ToArray();
                    failedCount = resolution.FailedSourceIds.Count;
                    switchedCount = resolution.SwitchedSourceCount;
                }
                else
                {
                    StatusTextBlock.Text = "正在获取作品信息...";
                    var resolution = await ResolveWorkInfoAsync(BuildWorkInfoRequests(new[] { sourceId }));
                    workInfos = resolution.WorkInfos;
                    candidateSourceIds = resolution.WorkInfos.Keys.ToArray();
                    failedCount = resolution.FailedSourceIds.Count;
                }

                if (candidateSourceIds.Count == 0)
                {
                    StatusTextBlock.Text = failedCount > 0
                        ? $"获取作品信息失败，跳过：{sourceId}"
                        : "未找到可加入的作品信息。";
                    return;
                }

                var queuePlan = BuildQueuePlan(candidateSourceIds);
                if (queuePlan.ToEnqueue.Count == 0)
                {
                    StatusTextBlock.Text = DownloadOperationStatusTexts.AppendTranslationSwitchClause(
                        $"该作品已在下载列表或待下载队列中，跳过：{candidateSourceIds[0]}",
                        switchedCount) + "。";
                    return;
                }

                var enqueuedSourceId = queuePlan.ToEnqueue[0];
                var enqueuedWorkInfos = FilterWorkInfoMap(workInfos, queuePlan.ToEnqueue);

                _searchStateStore.EnqueueForDownload(new[] { enqueuedSourceId });
                _downloadService.UpsertPrefetchedWorkInfo(enqueuedWorkInfos, WorkInfoCacheEntryLevel.Full);
                ClearQueuedFailureState(new[] { enqueuedSourceId });
                MergeQueuedWorkInfoTitles(DownloadWorkInfoTitlePolicy.BuildNonEmptyTitleMap(enqueuedWorkInfos));

                RefreshView();
                var statusText = failedCount > 0
                    ? $"已加入单个下载：{enqueuedSourceId}；另有 {failedCount} 项解析失败。"
                    : $"已加入单个下载：{enqueuedSourceId}";
                StatusTextBlock.Text = DownloadOperationStatusTexts.AppendTranslationSwitchClause(statusText.TrimEnd('。'), switchedCount);
            },
            disableRunQueue: true,
            failurePrefix: "加入单个下载失败",
            logMessage: "Failed to enqueue single download with work info.");
    }

    /// <summary>
    /// 处理“加入批量下载”按钮点击。
    /// </summary>
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
                IReadOnlyDictionary<string, WorkInfoDto> workInfos;
                SearchQueueCountPlan queuePlan;
                var failedCount = 0;
                var switchedCount = 0;

                if (QueueTranslationCheckBox.IsChecked == true)
                {
                    StatusTextBlock.Text = $"正在分析 {sourceIds.Count} 个作品的语言并获取作品信息...";
                    var resolution = await _enqueueWorkInfoResolver.ResolvePreferTranslatedAsync(BuildEnqueueRequests(sourceIds));
                    workInfos = resolution.WorkInfos;
                    queuePlan = BuildQueuePlan(resolution.WorkInfos.Keys.ToArray());
                    failedCount = resolution.FailedSourceIds.Count;
                    switchedCount = resolution.SwitchedSourceCount;
                }
                else
                {
                    StatusTextBlock.Text = $"正在获取 {sourceIds.Count} 个作品信息...";
                    queuePlan = BuildQueuePlan(sourceIds);
                    var resolution = await ResolveWorkInfoAsync(BuildWorkInfoRequests(queuePlan.ToEnqueue));
                    workInfos = resolution.WorkInfos;
                    failedCount = resolution.FailedSourceIds.Count;
                }

                if (queuePlan.ToEnqueue.Count == 0)
                {
                    var statusText = failedCount > 0
                        ? $"已跳过全部 {queuePlan.SkippedCount} 项；另有 {failedCount} 项解析失败。"
                        : $"已跳过全部 {queuePlan.SkippedCount} 项。";
                    StatusTextBlock.Text = DownloadOperationStatusTexts.AppendTranslationSwitchClause(statusText.TrimEnd('。'), switchedCount) + "。";
                    return;
                }

                var enqueuedWorkInfos = FilterWorkInfoMap(workInfos, queuePlan.ToEnqueue);
                _searchStateStore.EnqueueForDownload(queuePlan.ToEnqueue);
                _downloadService.UpsertPrefetchedWorkInfo(enqueuedWorkInfos, WorkInfoCacheEntryLevel.Full);
                foreach (var sourceIdItem in queuePlan.ToEnqueue)
                {
                    ClearQueuedFailureState(new[] { sourceIdItem });
                }
                MergeQueuedWorkInfoTitles(DownloadWorkInfoTitlePolicy.BuildNonEmptyTitleMap(enqueuedWorkInfos));

                RefreshView();

                var resolvedCount = enqueuedWorkInfos.Count;
                var batchStatusMsg = DownloadOperationStatusTexts.AppendTranslationSwitchClause(
                    DownloadOperationStatusTexts.BuildBatchEnqueueResult(queuePlan.ToEnqueue.Count, resolvedCount),
                    switchedCount);
                StatusTextBlock.Text = queuePlan.SkippedCount > 0
                    ? $"{batchStatusMsg}；已跳过 {queuePlan.SkippedCount} 项{BuildFailedSuffix(failedCount)}。"
                    : $"{batchStatusMsg}{BuildFailedSuffix(failedCount)}";
            },
            disableRunQueue: true,
            failurePrefix: "加入批量下载失败",
            logMessage: "Failed to enqueue batch download with work info.");
    }

    /// <summary>
    /// 处理“执行下载队列”按钮点击。
    /// </summary>
    private async void OnRunQueueClicked(object sender, System.Windows.RoutedEventArgs e)
    {
        await ExecuteGuardedAsync(
            async () =>
            {
                StatusTextBlock.Text = "正在执行下载队列...";
                var context = DownloadOperationContext.Create(FilterTextBox.Text, HdAudioOnlyCheckBox.IsChecked == true);

                var created = await _downloadService.RunQueuedAsync(
                    context.FileFilter, context.HdAudioOnly);
                RefreshView();
                StatusTextBlock.Text = DownloadOperationStatusTexts.BuildRunQueueResult(created.Count);
            },
            disableRunQueue: true,
            updateSelectionOnFinally: true,
            failurePrefix: "下载执行失败",
            logMessage: "Failed to run queued downloads.");
    }

    /// <summary>
    /// 处理“刷新任务列表”按钮点击。
    /// </summary>
    private void OnRefreshClicked(object sender, System.Windows.RoutedEventArgs e)
    {
        RefreshView();
        StatusTextBlock.Text = "任务列表已刷新。";
    }

    /// <summary>
    /// 处理“清空任务列表”按钮点击。
    /// </summary>
    private async void OnClearTaskListClicked(object sender, RoutedEventArgs e)
    {
        if (!ConfirmWithQuestion("将停止正在下载的任务并清空任务列表，是否继续？", "确认清空任务列表"))
        {
            return;
        }

        await ExecuteGuardedAsync(
            async () =>
            {
                await _downloadService.ClearAllTasksAsync();
                await _uiStateStore.ClearUnfinishedQueueAsync();
                _queuedStatusOverrides.Clear();
                _queuedErrorMessages.Clear();
                _queuedWorkInfoTitles.Clear();

                RefreshView();
                StatusTextBlock.Text = "任务列表已清空。";
            },
            disableRunQueue: true,
            disableSelectionActions: true,
            updateSelectionOnFinally: true,
            failurePrefix: "清空任务列表失败",
            logMessage: "Failed to clear all download tasks.");
    }

    /// <summary>
    /// 处理“取消选中任务”按钮点击。
    /// </summary>
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
                        _queuedErrorMessages.TryRemove(task.SourceId, out _);
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

    /// <summary>
    /// 处理“重试失败任务”按钮点击。
    /// </summary>
    private async void OnRetryClicked(object sender, System.Windows.RoutedEventArgs e)
    {
        var precheck = DownloadOperationPrecheckPolicy.CheckRetry(GetSelectedTasks(), _downloadService.GetTasks());
        if (!precheck.CanProceed)
        {
            StatusTextBlock.Text = precheck.StatusText;
            return;
        }
        var targets = precheck.Targets;

        if (targets.Count > 1
            && !ConfirmWithQuestion(
                DownloadOperationPrompts.BuildRetryConfirmMessage(
                    RetryAllMaxConcurrency,
                    targets.Select(static item => item.SourceId).ToArray(),
                    precheck.UsesSelection),
                "确认批量重试"))
        {
            return;
        }

        await ExecuteGuardedAsync(
            async () =>
            {
                var context = DownloadOperationContext.Create(FilterTextBox.Text, HdAudioOnlyCheckBox.IsChecked == true);
                var retriedCount = await RetryFailedTargetsAsync(targets, context);
                RefreshView();
                StatusTextBlock.Text = targets.Count == 1
                    ? DownloadOperationStatusTexts.BuildRetryResult(retriedCount > 0, targets[0].SourceId)
                    : DownloadOperationStatusTexts.BuildRetryAllResult(retriedCount, targets.Count);
            },
            disableRunQueue: targets.Count > 1,
            disableSelectionActions: true,
            updateSelectionOnFinally: true,
            failurePrefix: "重试任务失败",
            logMessage: targets.Count == 1
                ? $"Failed to retry download task {targets[0].TaskId}."
                : "Failed to retry selected failed download tasks.");
    }

    /// <summary>
    /// 处理下载任务列表选中项变更。
    /// </summary>
    private void OnTaskSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        UpdateSelectionActions();
    }

    /// <summary>
    /// 处理“立即下载选中任务”按钮点击。
    /// </summary>
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
                var context = DownloadOperationContext.Create(FilterTextBox.Text, HdAudioOnlyCheckBox.IsChecked == true);
                var startedCount = 0;

                foreach (var task in selected)
                {
                    _searchStateStore.RemoveFromQueue(new[] { task.SourceId });
                    ClearQueuedFailureState(new[] { task.SourceId });

                    Guid? preferredTaskId = task.TaskId == Guid.Empty ? null : task.TaskId;
                    var started = await _downloadService.StartAsync(task.SourceId, context.FileFilter, preferredTaskId, context.HdAudioOnly);
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

    /// <summary>
    /// 处理单个 RJID 输入框文本变更。
    /// </summary>
    private void OnSingleSourceIdTextChanged(object sender, TextChangedEventArgs e)
    {
        // 单项输入改为“提交时规范化”，避免实时改写打断输入。
    }

    /// <summary>
    /// 处理批量 RJID 输入框文本变更。
    /// </summary>
    private void OnBatchSourceIdsTextChanged(object sender, TextChangedEventArgs e)
    {
        // 批量输入框不做实时改写，避免影响空格/逗号/分号输入与多次粘贴体验。
    }

    /// <summary>
    /// 处理“从文件导入”按钮点击。
    /// </summary>
    private async void OnImportFileClicked(object sender, RoutedEventArgs e)
    {
        var filePath = _dialogService.ShowOpenFileDialog(new OpenFileDialogOptions
        {
            Title = "选择要导入的文件",
            InitialDirectory = _appPathService.MetadataDirectory,
            Filter = "支持的文件 (*.csv;*.json)|*.csv;*.json|CSV 文件 (*.csv)|*.csv|JSON 文件 (*.json)|*.json|所有文件 (*.*)|*.*",
            DefaultExt = ".csv",
        });

        if (string.IsNullOrWhiteSpace(filePath))
        {
            return;
        }

        var format = ResolveImportFileFormat(filePath);
        if (format is null)
        {
            StatusTextBlock.Text = "仅支持导入 CSV 或 JSON 文件。";
            return;
        }

        await ImportFileAsync(filePath, format);
    }

    private async Task ImportFileAsync(string filePath, string format)
    {
        var formatLabel = format.Equals("csv", StringComparison.OrdinalIgnoreCase) ? "CSV" : "JSON";

        await ExecuteGuardedAsync(
            async () =>
            {
                var items = format.Equals("csv", StringComparison.OrdinalIgnoreCase)
                    ? await _importService.ParseCsvAsync(filePath)
                    : await _importService.ParseJsonAsync(filePath);
                var sourceIds = items.Select(static x => x.SourceId).ToArray();
                SearchQueueCountPlan queuePlan;
                IReadOnlyDictionary<string, WorkInfoDto> workInfos;
                var failedCount = 0;
                var switchedCount = 0;

                if (QueueTranslationCheckBox.IsChecked == true)
                {
                    StatusTextBlock.Text = $"正在分析 {formatLabel} 作品语言并准备加入下载队列...";
                    var resolution = await _enqueueWorkInfoResolver.ResolvePreferTranslatedAsync(BuildEnqueueRequests(sourceIds));
                    workInfos = resolution.WorkInfos;
                    queuePlan = BuildQueuePlan(resolution.WorkInfos.Keys.ToArray());
                    failedCount = resolution.FailedSourceIds.Count;
                    switchedCount = resolution.SwitchedSourceCount;
                }
                else
                {
                    StatusTextBlock.Text = $"正在导入 {formatLabel} 文件...";
                    queuePlan = BuildQueuePlan(sourceIds);
                    var resolution = await ResolveWorkInfoAsync(items
                        .Where(x => queuePlan.ToEnqueue.Contains(x.SourceId, StringComparer.OrdinalIgnoreCase))
                        .Select(static x => new WorkInfoResolutionRequest
                        {
                            SourceId = x.SourceId,
                        })
                        .ToArray());
                    workInfos = resolution.WorkInfos;
                    failedCount = resolution.FailedSourceIds.Count;
                }

                if (queuePlan.ToEnqueue.Count == 0)
                {
                    var skippedStatusText = failedCount > 0
                        ? $"{formatLabel} 中全部 {queuePlan.SkippedCount} 项已存在、重复或解析失败，已跳过。"
                        : $"{formatLabel} 中全部 {queuePlan.SkippedCount} 项已存在或重复，已跳过。";
                    StatusTextBlock.Text = DownloadOperationStatusTexts.AppendTranslationSwitchClause(skippedStatusText.TrimEnd('。'), switchedCount) + "。";
                    return;
                }

                var enqueuedWorkInfos = FilterWorkInfoMap(workInfos, queuePlan.ToEnqueue);
                _searchStateStore.EnqueueForDownload(queuePlan.ToEnqueue);
                var cacheLevel = QueueTranslationCheckBox.IsChecked == true
                    ? WorkInfoCacheEntryLevel.Full
                    : WorkInfoCacheEntryLevel.Summary;
                _downloadService.UpsertPrefetchedWorkInfo(enqueuedWorkInfos, cacheLevel);

                foreach (var sourceId in queuePlan.ToEnqueue)
                {
                    ClearQueuedFailureState(new[] { sourceId });
                }

                RefreshView();
                var importStatusText = queuePlan.SkippedCount > 0
                    ? $"已从 {formatLabel} 导入 {queuePlan.ToEnqueue.Count} 项；已跳过 {queuePlan.SkippedCount} 项{BuildFailedSuffix(failedCount)}"
                    : $"已从 {formatLabel} 导入 {queuePlan.ToEnqueue.Count} 项{BuildFailedSuffix(failedCount)}";
                StatusTextBlock.Text = DownloadOperationStatusTexts.AppendTranslationSwitchClause(importStatusText, switchedCount) + "。";
            },
            disableRunQueue: true,
            failurePrefix: $"导入 {formatLabel} 失败",
            logMessage: $"Failed to import {formatLabel} file.");
    }

    /// <summary>
    /// 处理“从收藏夹导入”按钮点击。
    /// </summary>
    private async void OnImportFavoritesClicked(object sender, RoutedEventArgs e)
    {
        IReadOnlyList<string> folderTitles;
        try
        {
            folderTitles = await _favoriteStore.LoadFavoriteFolderTitlesAsync();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to load favorite folder titles for import.");
            StatusTextBlock.Text = $"读取收藏夹失败：{ex.Message}";
            return;
        }

        if (folderTitles.Count == 0)
        {
            StatusTextBlock.Text = "当前没有可导入的收藏夹。";
            return;
        }

        var dialog = new FavoriteFolderDialog(folderTitles, allowCustomInput: false, title: "从收藏夹导入", confirmButtonText: "导入")
        {
            Owner = Window.GetWindow(this),
        };

        if (dialog.ShowDialog() != true)
        {
            StatusTextBlock.Text = "已取消从收藏夹导入。";
            return;
        }

        var folderTitle = dialog.SelectedFolderTitle;
        if (string.IsNullOrWhiteSpace(folderTitle))
        {
            StatusTextBlock.Text = "请先选择收藏夹。";
            return;
        }

        await ExecuteGuardedAsync(
            async () =>
            {
                var items = await _favoriteStore.LoadFavoriteFolderItemsAsync(folderTitle);
                if (items.Count == 0)
                {
                    StatusTextBlock.Text = $"收藏夹“{folderTitle}”中没有可导入的作品。";
                    return;
                }

                var queuePlan = BuildQueuePlan(items.Select(static item => item.SourceId).ToArray());
                if (queuePlan.ToEnqueue.Count == 0)
                {
                    StatusTextBlock.Text = $"收藏夹“{folderTitle}”中的作品均已在下载列表或队列中，跳过 {queuePlan.SkippedCount} 项。";
                    return;
                }

                var resolution = await ResolveWorkInfoAsync(items
                    .Where(item => queuePlan.ToEnqueue.Contains(item.SourceId, StringComparer.OrdinalIgnoreCase))
                    .Select(static item => new WorkInfoResolutionRequest
                    {
                        SourceId = item.SourceId,
                        WorkId = item.WorkId,
                    })
                    .ToArray());
                var workInfos = resolution.WorkInfos;

                if (workInfos.Count == 0)
                {
                    StatusTextBlock.Text = resolution.FailedSourceIds.Count > 0
                        ? $"收藏夹“{folderTitle}”中的作品信息获取失败 {resolution.FailedSourceIds.Count} 项。"
                        : $"收藏夹“{folderTitle}”中没有可导入的作品信息。";
                    return;
                }

                _searchStateStore.EnqueueForDownload(queuePlan.ToEnqueue);
                _downloadService.UpsertPrefetchedWorkInfo(workInfos, WorkInfoCacheEntryLevel.Summary);
                ClearQueuedFailureState(queuePlan.ToEnqueue);
                MergeQueuedWorkInfoTitles(DownloadWorkInfoTitlePolicy.BuildNonEmptyTitleMap(workInfos));

                RefreshView();

                var queueCount = _searchStateStore.GetQueuedSourceIds().Count;
                var failedSuffix = resolution.FailedSourceIds.Count > 0
                    ? $"；另有 {resolution.FailedSourceIds.Count} 项解析失败"
                    : string.Empty;
                StatusTextBlock.Text = queuePlan.SkippedCount > 0
            ? $"已从收藏夹“{folderTitle}”导入 {queuePlan.ToEnqueue.Count} 项；已跳过 {queuePlan.SkippedCount} 项，当前队列总数 {queueCount}{failedSuffix}。"
            : $"已从收藏夹“{folderTitle}”导入 {queuePlan.ToEnqueue.Count} 项，当前队列总数 {queueCount}{failedSuffix}。";
            },
            disableRunQueue: true,
        failurePrefix: "从收藏夹导入失败",
        logMessage: "Failed to import favorite folder items into download queue.");
    }

    /// <summary>
    /// 处理“打开下载目录”按钮点击。
    /// </summary>
    private async void OnOpenDownloadDirectoryClicked(object sender, RoutedEventArgs e)
    {
        try
        {
            var config = await _configurationService.LoadAsync();
            var path = DownloadDirectoryPathPolicy.Resolve(
                config?.Downloader.DownloadDataFolder,
                _appPathService.DefaultDownloadDataDirectory);

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
            _logger.Error(ex, "Failed to open download directory.");
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
        PruneQueuedFailureOverrides(queuedSourceIds);
        PruneQueuedErrorMessages(activeSourceIds, queuedSourceIds);

        var combined = DownloadTaskListComposer.ComposeRows(
            activeTasks,
            queuedSourceIds,
            _queuedWorkInfoTitles,
            _queuedStatusOverrides,
            _queuedErrorMessages);

        TaskGrid.ItemsSource = combined;
        QueueCountTextBlock.Text = $"待下载队列：{queuedSourceIds.Count}";
        UpdateSelectionActions();

        _ = PersistUnfinishedQueueSnapshotSafeAsync(activeTasks, queuedSourceIds);
    }

    private async Task PersistUnfinishedQueueSnapshotSafeAsync(
        IReadOnlyList<DownloadTaskItem> activeTasks,
        IReadOnlyList<string> queuedSourceIds)
    {
        try
        {
            var snapshot = DownloadUnfinishedQueueSnapshotPolicy.BuildSnapshot(activeTasks, queuedSourceIds);
            await _uiStateStore.SaveUnfinishedQueueAsync(snapshot);
        }
        catch (Exception ex)
        {
            _logger.Warn(ex, "Failed to persist unfinished queue snapshot.");
        }
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

    private void SetQueueMutationRunning(bool isRunning)
    {
        var availability = DownloadToolbarAvailability.Evaluate(isRunning);
        RunQueueButton.IsEnabled = availability.CanRunQueue;
        AddSingleButton.IsEnabled = availability.CanAddSingle;
        AddBatchButton.IsEnabled = availability.CanAddBatch;
        ImportFileButton.IsEnabled = availability.CanImportFile;
        ImportFavoritesButton.IsEnabled = availability.CanImportFavorites;
        OpenDownloadDirectoryButton.IsEnabled = availability.CanOpenDownloadDirectory;
        ClearTaskListButton.IsEnabled = availability.CanClearTaskList;
    }

    private void ToggleSelectionActions(bool isEnabled)
    {
        CancelButton.IsEnabled = isEnabled;
        RetryButton.IsEnabled = isEnabled;
        StartSelectedButton.IsEnabled = isEnabled;
    }

    private void UpdateSelectionActions()
    {
        var selected = GetSelectedTasks();
        // 允许未下载占位项（TaskId=Empty）参与取消按钮可用性判断。
        var availability = DownloadCommandAvailability.Evaluate(
            selected,
            _downloadService.GetTasks());

        CancelButton.IsEnabled = availability.CanCancel;
        RetryButton.IsEnabled = availability.CanRetry;
        StartSelectedButton.IsEnabled = availability.CanStartImmediate;
    }

    private static string? ResolveImportFileFormat(string filePath)
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

        return null;
    }

    private async Task<int> RetryFailedTargetsAsync(IReadOnlyList<RetryTaskTarget> targets, DownloadOperationContext context)
    {
        if (targets.Count == 1)
        {
            var retried = await _downloadService.RetryFailedAsync(targets[0].TaskId, context.FileFilter, context.HdAudioOnly);
            return retried is null ? 0 : 1;
        }

        var semaphore = new SemaphoreSlim(RetryAllMaxConcurrency);
        var jobs = targets.Select(async target =>
        {
            await semaphore.WaitAsync();
            try
            {
                var retried = await _downloadService.RetryFailedAsync(target.TaskId, context.FileFilter, context.HdAudioOnly);
                return retried is not null;
            }
            finally
            {
                semaphore.Release();
            }
        });

        var results = await Task.WhenAll(jobs);
        return results.Count(static value => value);
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

    private SearchQueueCountPlan BuildQueuePlan(IEnumerable<string> sourceIds)
    {
        return SearchQueueCountPolicy.Build(
            sourceIds,
            _downloadService.GetTasks(),
            _searchStateStore.GetQueuedSourceIds());
    }

    private bool ConfirmWithQuestion(string message, string title)
    {
        var shouldContinue = _dialogService.ConfirmQuestion(message, title);
        var decision = DownloadConfirmationPolicy.Evaluate(shouldContinue ? MessageBoxResult.Yes : MessageBoxResult.No);
        if (!decision.ShouldContinue)
        {
            StatusTextBlock.Text = decision.StatusText;
            return false;
        }

        return true;
    }

    private Task<MetadataWorkInfoResolutionResult> ResolveWorkInfoAsync(IReadOnlyList<WorkInfoResolutionRequest> requests)
    {
        return _metadataWorkInfoResolver.ResolveAsync(requests);
    }

    private static IReadOnlyDictionary<string, WorkInfoDto> FilterWorkInfoMap(
        IReadOnlyDictionary<string, WorkInfoDto> workInfos,
        IReadOnlyCollection<string> sourceIds)
    {
        var targetSourceIds = sourceIds.ToHashSet(StringComparer.OrdinalIgnoreCase);
        return workInfos
            .Where(item => targetSourceIds.Contains(item.Key))
            .ToDictionary(static item => item.Key, static item => item.Value, StringComparer.OrdinalIgnoreCase);
    }

    private static string BuildFailedSuffix(int failedCount)
    {
        return failedCount > 0 ? $"；另有 {failedCount} 项解析失败" : string.Empty;
    }

    private static IReadOnlyList<EnqueueWorkInfoRequest> BuildEnqueueRequests(IEnumerable<string> sourceIds)
    {
        return sourceIds
            .Select(static sourceId => new EnqueueWorkInfoRequest
            {
                SourceId = sourceId,
            })
            .ToArray();
    }

    private static IReadOnlyList<WorkInfoResolutionRequest> BuildWorkInfoRequests(IEnumerable<string> sourceIds)
    {
        return sourceIds
            .Select(static sourceId => new WorkInfoResolutionRequest
            {
                SourceId = sourceId,
            })
            .ToArray();
    }

    private void ClearQueuedFailureState(IEnumerable<string> sourceIds)
    {
        foreach (var sourceId in sourceIds)
        {
            _queuedStatusOverrides.TryRemove(sourceId, out _);
            _queuedErrorMessages.TryRemove(sourceId, out _);
        }
    }

    private void ApplyStartupMetadataRefreshResult(StartupMetadataRefreshResult refreshResult)
    {
        foreach (var sourceId in refreshResult.UpdatedWorkInfos.Keys)
        {
            ClearQueuedFailureState(new[] { sourceId });
        }

        foreach (var failure in refreshResult.Failures)
        {
            _queuedStatusOverrides[failure.SourceId] = DownloadTaskStatus.Failed;
            _queuedErrorMessages[failure.SourceId] = failure.ErrorMessage;
        }
    }

    private void PruneQueuedErrorMessages(
        IReadOnlyCollection<string> activeSourceIds,
        IReadOnlyCollection<string> queuedSourceIds)
    {
        var retained = activeSourceIds
            .Concat(queuedSourceIds)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var sourceId in _queuedErrorMessages.Keys.ToArray())
        {
            if (!retained.Contains(sourceId))
            {
                _queuedErrorMessages.TryRemove(sourceId, out _);
            }
        }
    }

    private void PruneQueuedFailureOverrides(IReadOnlyCollection<string> queuedSourceIds)
    {
        var retained = queuedSourceIds.ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var item in _queuedStatusOverrides.ToArray())
        {
            if (item.Value == DownloadTaskStatus.Failed && !retained.Contains(item.Key))
            {
                _queuedStatusOverrides.TryRemove(item.Key, out _);
            }
        }
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
            SetQueueMutationRunning(true);
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
            _logger.Error(ex, logMessage);
            StatusTextBlock.Text = $"{failurePrefix}：{ex.Message}";
        }
        finally
        {
            if (disableRunQueue)
            {
                SetQueueMutationRunning(false);
            }

            if (updateSelectionOnFinally || disableSelectionActions)
            {
                UpdateSelectionActions();
            }
        }
    }

}
