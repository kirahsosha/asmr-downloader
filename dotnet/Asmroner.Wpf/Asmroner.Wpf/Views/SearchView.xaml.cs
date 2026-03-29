using System.IO;
using System.Diagnostics;
using System.Text;
using Asmroner.Core.Api;
using Asmroner.Core.Configuration;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Search;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Views;

public partial class SearchView : UserControl
{
    private static readonly char[] FilterSeparators = [' ', '\t', '\r', '\n', ';', ','];

    private readonly ISearchService _searchService;
    private readonly IAsmrApiClient _asmrApiClient;
    private readonly ISearchExportService _searchExportService;
    private readonly ISearchStateStore _searchStateStore;
    private readonly IDownloadService _downloadService;
    private readonly IUiStateStore _uiStateStore;
    private readonly IConfigurationService _configurationService;
    private readonly IAppPathService _appPathService;
    private readonly ILogger<SearchView> _logger;

    private IReadOnlyList<SearchWorkItem> _results = Array.Empty<SearchWorkItem>();
    private IReadOnlyList<SearchWorkItem> _popularResults = Array.Empty<SearchWorkItem>();
    private int _currentPage = 1;
    private int _pageSize = 20;
    private int _totalCount;
    private bool _isPopularMode;
    private bool _isApplyingSearchUiState;
    private bool _suppressSearchOptionSelectionChanged = true;
    private SearchWorkItem? _contextMenuTargetItem;

    public SearchView()
        : this(
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!)
    {
    }

    public SearchView(
        ISearchService searchService,
        IAsmrApiClient asmrApiClient,
        ISearchExportService searchExportService,
        ISearchStateStore searchStateStore,
        IDownloadService downloadService,
        IUiStateStore uiStateStore,
        IConfigurationService configurationService,
        IAppPathService appPathService,
        ILogger<SearchView> logger)
    {
        _searchService = searchService;
        _asmrApiClient = asmrApiClient;
        _searchExportService = searchExportService;
        _searchStateStore = searchStateStore;
        _downloadService = downloadService;
        _uiStateStore = uiStateStore;
        _configurationService = configurationService;
        _appPathService = appPathService;
        _logger = logger;

        InitializeComponent();
        _pageSize = ReadPageSize();
        UpdatePaginationInfo();
        RegisterSearchUiAutosaveHandlers();

        Loaded += async (_, _) => await LoadSearchUiStateAsync();
        _suppressSearchOptionSelectionChanged = false;
    }

    private async Task LoadSearchUiStateAsync()
    {
        try
        {
            _isApplyingSearchUiState = true;
            var state = await _uiStateStore.LoadSearchUiStateAsync();

            IncludeTranslationCheckBox.IsChecked = state.IncludeTranslationWorks;

            TagTextBox.Text = state.Tag;
            TagExcludeCheckBox.IsChecked = state.TagExclude;

            CircleTextBox.Text = state.Circle;
            CircleExcludeCheckBox.IsChecked = state.CircleExclude;

            VaTextBox.Text = state.Va;
            VaExcludeCheckBox.IsChecked = state.VaExclude;

            DurationTextBox.Text = state.Duration;
            DurationExcludeCheckBox.IsChecked = state.DurationExclude;

            RateTextBox.Text = state.Rate;
            RateExcludeCheckBox.IsChecked = state.RateExclude;

            PriceTextBox.Text = state.Price;
            PriceExcludeCheckBox.IsChecked = state.PriceExclude;

            SellTextBox.Text = state.Sell;
            SellExcludeCheckBox.IsChecked = state.SellExclude;

            AgeTextBox.Text = state.Age;
            AgeExcludeCheckBox.IsChecked = state.AgeExclude;

            LangTextBox.Text = state.Lang;
            LangExcludeCheckBox.IsChecked = state.LangExclude;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to restore search UI state.");
        }
        finally
        {
            _isApplyingSearchUiState = false;
        }
    }

    private void RegisterSearchUiAutosaveHandlers()
    {
        IncludeTranslationCheckBox.Checked += OnSearchUiStateChanged;
        IncludeTranslationCheckBox.Unchecked += OnSearchUiStateChanged;

        TagTextBox.TextChanged += OnSearchUiStateChanged;
        TagExcludeCheckBox.Checked += OnSearchUiStateChanged;
        TagExcludeCheckBox.Unchecked += OnSearchUiStateChanged;

        CircleTextBox.TextChanged += OnSearchUiStateChanged;
        CircleExcludeCheckBox.Checked += OnSearchUiStateChanged;
        CircleExcludeCheckBox.Unchecked += OnSearchUiStateChanged;

        VaTextBox.TextChanged += OnSearchUiStateChanged;
        VaExcludeCheckBox.Checked += OnSearchUiStateChanged;
        VaExcludeCheckBox.Unchecked += OnSearchUiStateChanged;

        DurationTextBox.TextChanged += OnSearchUiStateChanged;
        DurationExcludeCheckBox.Checked += OnSearchUiStateChanged;
        DurationExcludeCheckBox.Unchecked += OnSearchUiStateChanged;

        RateTextBox.TextChanged += OnSearchUiStateChanged;
        RateExcludeCheckBox.Checked += OnSearchUiStateChanged;
        RateExcludeCheckBox.Unchecked += OnSearchUiStateChanged;

        PriceTextBox.TextChanged += OnSearchUiStateChanged;
        PriceExcludeCheckBox.Checked += OnSearchUiStateChanged;
        PriceExcludeCheckBox.Unchecked += OnSearchUiStateChanged;

        SellTextBox.TextChanged += OnSearchUiStateChanged;
        SellExcludeCheckBox.Checked += OnSearchUiStateChanged;
        SellExcludeCheckBox.Unchecked += OnSearchUiStateChanged;

        AgeTextBox.TextChanged += OnSearchUiStateChanged;
        AgeExcludeCheckBox.Checked += OnSearchUiStateChanged;
        AgeExcludeCheckBox.Unchecked += OnSearchUiStateChanged;

        LangTextBox.TextChanged += OnSearchUiStateChanged;
        LangExcludeCheckBox.Checked += OnSearchUiStateChanged;
        LangExcludeCheckBox.Unchecked += OnSearchUiStateChanged;
    }

    private void OnSearchUiStateChanged(object? sender, EventArgs e)
    {
        if (_isApplyingSearchUiState)
        {
            return;
        }

        _ = SaveSearchUiStateSafeAsync();
    }

    private async Task SaveSearchUiStateSafeAsync()
    {
        try
        {
            await _uiStateStore.SaveSearchUiStateAsync(BuildSearchUiState());
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to persist search UI state.");
        }
    }

    private SearchUiState BuildSearchUiState()
    {
        return new SearchUiState
        {
            IncludeTranslationWorks = IncludeTranslationCheckBox.IsChecked == true,
            Tag = TagTextBox.Text,
            TagExclude = TagExcludeCheckBox.IsChecked == true,
            Circle = CircleTextBox.Text,
            CircleExclude = CircleExcludeCheckBox.IsChecked == true,
            Va = VaTextBox.Text,
            VaExclude = VaExcludeCheckBox.IsChecked == true,
            Duration = DurationTextBox.Text,
            DurationExclude = DurationExcludeCheckBox.IsChecked == true,
            Rate = RateTextBox.Text,
            RateExclude = RateExcludeCheckBox.IsChecked == true,
            Price = PriceTextBox.Text,
            PriceExclude = PriceExcludeCheckBox.IsChecked == true,
            Sell = SellTextBox.Text,
            SellExclude = SellExcludeCheckBox.IsChecked == true,
            Age = AgeTextBox.Text,
            AgeExclude = AgeExcludeCheckBox.IsChecked == true,
            Lang = LangTextBox.Text,
            LangExclude = LangExcludeCheckBox.IsChecked == true,
        };
    }

    private void OnAdvancedFilterExpanded(object sender, System.Windows.RoutedEventArgs e)
    {
        try
        {
            if (sender is not Expander expander)
            {
                return;
            }

            // Get parent window and expander
            var window = System.Windows.Window.GetWindow(this);
            if (window == null)
            {
                return;
            }

            // Measure expander content to get desired height
            expander.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
            var expanderDesiredHeight = expander.DesiredSize.Height;

            // Calculate required window height with safety margin
            const double safetyMargin = 200;
            var requiredHeight = window.ActualHeight + expanderDesiredHeight + safetyMargin;

            // Calculate 90% of available screen height
            var screenHeight = System.Windows.SystemParameters.WorkArea.Height;
            var maxAllowedHeight = screenHeight * 0.9;

            // Apply height increase with ceiling protection
            var newHeight = Math.Min(requiredHeight, maxAllowedHeight);
            window.Height = Math.Max(window.Height, newHeight);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to auto-resize window on advanced filter expanded.");
        }
    }

    private async void OnSearchClicked(object sender, System.Windows.RoutedEventArgs e)
    {
        _isPopularMode = false;
        _popularResults = Array.Empty<SearchWorkItem>();
        _currentPage = 1;
        await ExecuteSearchAsync();
    }

    private async Task ExecuteSearchAsync(bool allowOptionOnlyQuery = false)
    {
        var rawQuery = BuildEffectiveQuery(allowOptionOnlyQuery);
        if (string.IsNullOrWhiteSpace(rawQuery))
        {
            StatusTextBlock.Text = "请输入关键词，或填写至少一个高级筛选条件。";
            return;
        }

        ToggleActionButtons(false);
        StatusTextBlock.Text = "正在搜索...";

        try
        {
            var result = await _searchService.SearchAsync(rawQuery, _pageSize);
            _results = result.Items;
            ResultsGrid.ItemsSource = _results;
            _totalCount = result.TotalCount;
            UpdatePaginationInfo();
            StatusTextBlock.Text = $"搜索完成：返回 {result.ReturnedCount} 条 / 总计 {result.TotalCount} 条。";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Search failed for query: {Query}", rawQuery);
            StatusTextBlock.Text = $"搜索失败：{ex.Message}";
        }
        finally
        {
            ToggleActionButtons(true);
        }
    }

    private async void OnExportCsvClicked(object sender, System.Windows.RoutedEventArgs e)
    {
        await ExportAsync("csv", SearchExportScope.All);
    }

    private async void OnExportJsonClicked(object sender, System.Windows.RoutedEventArgs e)
    {
        await ExportAsync("json", SearchExportScope.All);
    }

    private async void OnQueueClicked(object sender, System.Windows.RoutedEventArgs e)
    {
        var selected = ResultsGrid.SelectedItems
            .OfType<SearchWorkItem>()
            .Select(static item => item.SourceId)
            .ToArray();

        var sourceIds = selected.Length > 0
            ? selected
            : _results.Select(static item => item.SourceId).ToArray();

        if (sourceIds.Length == 0)
        {
            StatusTextBlock.Text = "当前没有可入队的搜索结果。";
            return;
        }

        var queuePlan = SearchQueueCountPolicy.Build(
            sourceIds,
            _downloadService.GetTasks(),
            _searchStateStore.GetQueuedSourceIds());

        if (queuePlan.ToEnqueue.Count == 0)
        {
            StatusTextBlock.Text = $"所选作品均已在下载列表或队列中，跳过 {queuePlan.SkippedCount} 项。";
            return;
        }

        _searchStateStore.EnqueueForDownload(queuePlan.ToEnqueue);
        _downloadService.UpsertPrefetchedWorkInfo(BuildPrefetchedWorkInfoMap(queuePlan.ToEnqueue));

        var queuedSourceIds = _searchStateStore.GetQueuedSourceIds();
        await PersistUnfinishedQueueSnapshotSafeAsync(queuedSourceIds);

        var queueCount = queuedSourceIds.Count;
        StatusTextBlock.Text = queuePlan.SkippedCount > 0
            ? $"已加入下载队列 {queuePlan.ToEnqueue.Count} 项，跳过 {queuePlan.SkippedCount} 项（已存在或重复），当前队列总数 {queueCount}。"
            : $"已加入下载队列 {queuePlan.ToEnqueue.Count} 项，当前队列总数 {queueCount}。";
    }

    private void OnContextMenuQueueClicked(object sender, RoutedEventArgs e)
    {
        OnQueueClicked(sender, e);
    }

    private void OnContextMenuExportCsvClicked(object sender, RoutedEventArgs e)
    {
        OnExportCsvClicked(sender, e);
    }

    private void OnContextMenuExportJsonClicked(object sender, RoutedEventArgs e)
    {
        OnExportJsonClicked(sender, e);
    }

    private async void OnContextMenuExportSelectedCsvClicked(object sender, RoutedEventArgs e)
    {
        await ExportAsync("csv", SearchExportScope.Selected);
    }

    private async void OnContextMenuExportSelectedJsonClicked(object sender, RoutedEventArgs e)
    {
        await ExportAsync("json", SearchExportScope.Selected);
    }

    private async void OnContextMenuOpenWorkPageClicked(object sender, RoutedEventArgs e)
    {
        var target = ResolveContextMenuTargetItem();
        if (target is null)
        {
            StatusTextBlock.Text = "请先右键选择一条搜索结果。";
            return;
        }

        try
        {
            var config = await _configurationService.LoadAsync();
            var template = config?.Downloader.WorkPageUrlTemplate;

            if (!SearchWorkPageUrlPolicy.TryBuild(template, target.SourceId, out var url, out var errorMessage))
            {
                StatusTextBlock.Text = errorMessage;
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true,
            });

            StatusTextBlock.Text = $"已在浏览器打开：{url}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to open search work page for {SourceId}.", target.SourceId);
            StatusTextBlock.Text = $"打开浏览器失败：{ex.Message}";
        }
    }

    private SearchWorkItem? ResolveContextMenuTargetItem()
    {
        if (_contextMenuTargetItem is not null)
        {
            return _contextMenuTargetItem;
        }

        if (ResultsGrid.SelectedItem is SearchWorkItem selectedItem)
        {
            return selectedItem;
        }

        return ResultsGrid.SelectedItems.OfType<SearchWorkItem>().FirstOrDefault();
    }

    private void OnResultsGridPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        var row = FindParent<DataGridRow>(e.OriginalSource as DependencyObject);
        if (row?.Item is not SearchWorkItem clickedItem)
        {
            _contextMenuTargetItem = null;
            return;
        }

        _contextMenuTargetItem = clickedItem;

        if (ResultsGrid.SelectedItems.Count == 0)
        {
            ResultsGrid.SelectedItem = clickedItem;
        }

        row.Focus();
    }

    private static T? FindParent<T>(DependencyObject? child)
        where T : DependencyObject
    {
        while (child is not null)
        {
            if (child is T typed)
            {
                return typed;
            }

            child = VisualTreeHelper.GetParent(child);
        }

        return null;
    }

    private async Task PersistUnfinishedQueueSnapshotSafeAsync(IReadOnlyList<string> queuedSourceIds)
    {
        try
        {
            var snapshot = DownloadUnfinishedQueueSnapshotPolicy.BuildSnapshot(
                _downloadService.GetTasks(),
                queuedSourceIds);
            await _uiStateStore.SaveUnfinishedQueueAsync(snapshot);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to persist unfinished queue snapshot from search view.");
        }
    }

    private async void OnQueryHotClicked(object sender, System.Windows.RoutedEventArgs e)
    {
        ToggleActionButtons(false);
        StatusTextBlock.Text = "正在查询热门作品...";

        try
        {
            var popular = await _asmrApiClient.GetPopularAsync();
            _popularResults = popular
                .Where(static item => !string.IsNullOrWhiteSpace(item.SourceId))
                .Select(static item => new SearchWorkItem
                {
                    SourceId = item.SourceId,
                    Title = item.Title,
                    DownloadCount = item.DownloadCount,
                    Release = string.Empty,
                    HasSubtitle = false,
                    RateAverage = 0,
                })
                .ToArray();

            _isPopularMode = true;
            _totalCount = _popularResults.Count;
            _currentPage = 1;
            RenderPopularPage();
            StatusTextBlock.Text = BuildPopularPageStatusText();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Query popular works failed.");
            StatusTextBlock.Text = $"查询热门作品失败：{ex.Message}";
        }
        finally
        {
            ToggleActionButtons(true);
        }
    }

    private void OnClearClicked(object sender, System.Windows.RoutedEventArgs e)
    {
        QueryTextBox.Text = string.Empty;

        _suppressSearchOptionSelectionChanged = true;
        try
        {
            OrderComboBox.SelectedIndex = 0;
            SortComboBox.SelectedIndex = 0;
            SubtitleComboBox.SelectedIndex = 0;
            PageSizeComboBox.SelectedIndex = 0;
        }
        finally
        {
            _suppressSearchOptionSelectionChanged = false;
        }

        _isPopularMode = false;
        _popularResults = Array.Empty<SearchWorkItem>();
        _currentPage = 1;
        _pageSize = 20;
        _totalCount = 0;
        _results = Array.Empty<SearchWorkItem>();
        ResultsGrid.ItemsSource = _results;
        UpdatePaginationInfo();
        StatusTextBlock.Text = "已清空关键词、排序、分页与当前结果。";
    }

    private async void OnPrevPageClicked(object sender, System.Windows.RoutedEventArgs e)
    {
        if (_currentPage <= 1)
        {
            return;
        }

        _currentPage--;

        if (_isPopularMode)
        {
            RenderPopularPage();
            StatusTextBlock.Text = BuildPopularPageStatusText();
            return;
        }

        await ExecuteSearchAsync(ShouldAllowOptionOnlyQuery());
    }

    private async void OnNextPageClicked(object sender, System.Windows.RoutedEventArgs e)
    {
        var totalPages = GetTotalPages();
        if (_currentPage >= totalPages)
        {
            return;
        }

        _currentPage++;

        if (_isPopularMode)
        {
            RenderPopularPage();
            StatusTextBlock.Text = BuildPopularPageStatusText();
            return;
        }

        await ExecuteSearchAsync(ShouldAllowOptionOnlyQuery());
    }

    private async void OnGoPageClicked(object sender, System.Windows.RoutedEventArgs e)
    {
        if (!int.TryParse(CurrentPageTextBox.Text.Trim(), out var page) || page <= 0)
        {
            StatusTextBlock.Text = "页码必须为正整数。";
            return;
        }

        _currentPage = page;

        if (_isPopularMode)
        {
            RenderPopularPage();
            StatusTextBlock.Text = BuildPopularPageStatusText();
            return;
        }

        await ExecuteSearchAsync(ShouldAllowOptionOnlyQuery());
    }

    private async void OnOrderChanged(object sender, SelectionChangedEventArgs e)
    {
        await OnSearchOptionChangedAsync();
    }

    private async void OnSortChanged(object sender, SelectionChangedEventArgs e)
    {
        await OnSearchOptionChangedAsync();
    }

    private async void OnSubtitleChanged(object sender, SelectionChangedEventArgs e)
    {
        await OnSearchOptionChangedAsync();
    }

    private async Task OnSearchOptionChangedAsync()
    {
        if (_suppressSearchOptionSelectionChanged)
        {
            return;
        }

        _currentPage = 1;

        if (_isPopularMode)
        {
            RenderPopularPage();
            StatusTextBlock.Text = BuildPopularPageStatusText();
            return;
        }

        await ExecuteSearchAsync(allowOptionOnlyQuery: true);
    }

    private async void OnPageSizeChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_suppressSearchOptionSelectionChanged)
        {
            return;
        }

        var newPageSize = ReadPageSize();
        if (newPageSize == _pageSize)
        {
            return;
        }

        _pageSize = newPageSize;
        _currentPage = 1;

        if (_isPopularMode)
        {
            RenderPopularPage();
            StatusTextBlock.Text = BuildPopularPageStatusText();
            return;
        }

        await ExecuteSearchAsync(allowOptionOnlyQuery: true);
    }

    private async Task ExportAsync(string extension, SearchExportScope scope)
    {
        var selectedItems = ResultsGrid.SelectedItems.OfType<SearchWorkItem>().ToArray();
        var plan = SearchExportScopePolicy.Build(_results, selectedItems, scope);
        var exportItems = plan.Items;
        var isSelectedScope = scope == SearchExportScope.Selected;
        var fallbackToAll = plan.FallbackToAll;

        if (exportItems.Count == 0)
        {
            StatusTextBlock.Text = "当前没有可导出的搜索结果。";
            return;
        }

        ToggleActionButtons(false);

        try
        {
            Directory.CreateDirectory(_appPathService.MetadataDirectory);
            var filePrefix = isSelectedScope && !fallbackToAll
                ? "search-selected-export"
                : "search-export";
            var fileName = $"{filePrefix}-{DateTime.Now:yyyyMMdd-HHmmss}.{extension}";
            var fullPath = ShowSaveFileDialog(extension, fileName);
            if (string.IsNullOrWhiteSpace(fullPath))
            {
                StatusTextBlock.Text = "已取消导出。";
                return;
            }

            if (extension.Equals("csv", StringComparison.OrdinalIgnoreCase))
            {
                await _searchExportService.ExportCsvAsync(exportItems, fullPath);
            }
            else
            {
                await _searchExportService.ExportJsonAsync(exportItems, fullPath);
            }

            if (isSelectedScope && fallbackToAll)
            {
                StatusTextBlock.Text = $"未选中任何结果，已回退导出全部：{fullPath}";
            }
            else if (isSelectedScope)
            {
                StatusTextBlock.Text = $"导出选中任务成功：{fullPath}";
            }
            else
            {
                StatusTextBlock.Text = $"导出成功：{fullPath}";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Export failed.");
            StatusTextBlock.Text = $"导出失败：{ex.Message}";
        }
        finally
        {
            ToggleActionButtons(true);
        }
    }

    private string? ShowSaveFileDialog(string extension, string defaultFileName)
    {
        var dialog = new SaveFileDialog
        {
            Title = "导出搜索结果",
            InitialDirectory = _appPathService.MetadataDirectory,
            FileName = defaultFileName,
            DefaultExt = "." + extension,
            AddExtension = true,
            OverwritePrompt = true,
            Filter = extension.Equals("csv", StringComparison.OrdinalIgnoreCase)
                ? "CSV 文件 (*.csv)|*.csv|所有文件 (*.*)|*.*"
                : "JSON 文件 (*.json)|*.json|所有文件 (*.*)|*.*",
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    private void ToggleActionButtons(bool isEnabled)
    {
        var canJump = SearchPagingPolicy.CanJump(GetTotalPages());

        SearchButton.IsEnabled = isEnabled;
        QueryHotButton.IsEnabled = isEnabled;
        ExportCsvButton.IsEnabled = isEnabled;
        ExportJsonButton.IsEnabled = isEnabled;
        QueueButton.IsEnabled = isEnabled;
        ClearButton.IsEnabled = isEnabled;
        PrevPageButton.IsEnabled = isEnabled && _currentPage > 1;
        NextPageButton.IsEnabled = isEnabled && _currentPage < GetTotalPages();
        GoPageButton.IsEnabled = isEnabled && canJump;
        CurrentPageTextBox.IsEnabled = isEnabled && canJump;
        PageSizeComboBox.IsEnabled = isEnabled;
    }

    private void RenderPopularPage()
    {
        _totalCount = _popularResults.Count;
        var window = SearchPagingPolicy.SlicePage(_popularResults, _currentPage, _pageSize);
        _currentPage = window.EffectivePage;
        _results = window.Items;

        ResultsGrid.ItemsSource = _results;
        UpdatePaginationInfo();
    }

    private string BuildPopularPageStatusText()
    {
        return $"热门查询：第 {_currentPage}/{GetTotalPages()} 页，当前 {_results.Count} 条 / 总计 {_totalCount} 条。";
    }

    private string BuildEffectiveQuery(bool allowOptionOnlyQuery = false)
    {
        var plain = QueryTextBox.Text.Trim();
        var filters = BuildAdvancedFilterTokens();
        var order = ReadComboValue(OrderComboBox, "release");
        var sort = ReadComboValue(SortComboBox, "desc");
        var subtitle = ReadComboValue(SubtitleComboBox, "0");
        var includeTranslation = IncludeTranslationCheckBox.IsChecked == true;

        var builder = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(plain))
        {
            builder.Append(plain);
        }

        if (filters.Count > 0)
        {
            if (builder.Length > 0)
            {
                builder.Append('@');
            }

            builder.Append(string.Join(',', filters));
        }

        if (builder.Length == 0 && !allowOptionOnlyQuery)
        {
            return string.Empty;
        }

        builder.Append($"?order={order}&sort={sort}&page={_currentPage}&pageSize={_pageSize}&subtitle={subtitle}&includeTranslationWorks={includeTranslation.ToString().ToLowerInvariant()}");
        return builder.ToString();
    }

    private List<string> BuildAdvancedFilterTokens()
    {
        var tokens = new List<string>();

        AddFilterToken(tokens, "tag", TagTextBox.Text, TagExcludeCheckBox.IsChecked == true);
        AddFilterToken(tokens, "circle", CircleTextBox.Text, CircleExcludeCheckBox.IsChecked == true);
        AddFilterToken(tokens, "va", VaTextBox.Text, VaExcludeCheckBox.IsChecked == true);
        AddFilterToken(tokens, "duration", DurationTextBox.Text, DurationExcludeCheckBox.IsChecked == true);
        AddFilterToken(tokens, "rate", RateTextBox.Text, RateExcludeCheckBox.IsChecked == true);
        AddFilterToken(tokens, "price", PriceTextBox.Text, PriceExcludeCheckBox.IsChecked == true);
        AddFilterToken(tokens, "sell", SellTextBox.Text, SellExcludeCheckBox.IsChecked == true);
        AddFilterToken(tokens, "age", AgeTextBox.Text, AgeExcludeCheckBox.IsChecked == true);
        AddFilterToken(tokens, "lang", LangTextBox.Text, LangExcludeCheckBox.IsChecked == true);

        return tokens;
    }

    private static void AddFilterToken(List<string> tokens, string key, string rawValue, bool isExclude)
    {
        var values = rawValue
            .Split(FilterSeparators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var candidate in values)
        {
            var value = candidate.Trim();
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            if (value.StartsWith($"{key}:", StringComparison.OrdinalIgnoreCase)
                || value.StartsWith($"-{key}:", StringComparison.OrdinalIgnoreCase))
            {
                tokens.Add(value);
                continue;
            }

            var isValueExclude = value.StartsWith("-", StringComparison.Ordinal);
            var normalizedValue = isValueExclude ? value[1..].Trim() : value;
            if (string.IsNullOrWhiteSpace(normalizedValue))
            {
                continue;
            }

            var useExcludePrefix = isExclude ^ isValueExclude;
            tokens.Add(useExcludePrefix ? $"-{key}:{normalizedValue}" : $"{key}:{normalizedValue}");
        }
    }

    private static string ReadComboValue(ComboBox comboBox, string fallback)
    {
        if (comboBox.SelectedItem is ComboBoxItem item)
        {
            if (item.Tag is string tag && !string.IsNullOrWhiteSpace(tag))
            {
                return tag;
            }

            if (item.Content is string content)
            {
                return content;
            }
        }

        return fallback;
    }

    private IReadOnlyDictionary<string, WorkInfoDto> BuildPrefetchedWorkInfoMap(IEnumerable<string> sourceIds)
    {
        return _results
            .Where(item => sourceIds.Contains(item.SourceId, StringComparer.OrdinalIgnoreCase))
            .Where(static item => !string.IsNullOrWhiteSpace(item.SourceId))
            .GroupBy(static item => item.SourceId, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                static group => group.Key,
                static group =>
                {
                    var item = group.First();
                    return new WorkInfoDto
                    {
                        SourceId = item.SourceId,
                        Title = item.Title,
                        Release = item.Release,
                        HasSubtitle = item.HasSubtitle,
                    };
                },
                StringComparer.OrdinalIgnoreCase);
    }

    private int ReadPageSize()
    {
        var value = ReadComboValue(PageSizeComboBox, "20");
        return int.TryParse(value, out var parsed) && parsed > 0 ? parsed : 20;
    }

    private bool HasAdvancedFilter()
    {
        return !string.IsNullOrWhiteSpace(TagTextBox.Text)
            || !string.IsNullOrWhiteSpace(CircleTextBox.Text)
            || !string.IsNullOrWhiteSpace(VaTextBox.Text)
            || !string.IsNullOrWhiteSpace(DurationTextBox.Text)
            || !string.IsNullOrWhiteSpace(RateTextBox.Text)
            || !string.IsNullOrWhiteSpace(PriceTextBox.Text)
            || !string.IsNullOrWhiteSpace(SellTextBox.Text)
            || !string.IsNullOrWhiteSpace(AgeTextBox.Text)
            || !string.IsNullOrWhiteSpace(LangTextBox.Text);
    }

    private bool ShouldAllowOptionOnlyQuery()
    {
        return !string.IsNullOrWhiteSpace(QueryTextBox.Text)
            || HasAdvancedFilter()
            || _totalCount > 0
            || _results.Count > 0;
    }

    private int GetTotalPages()
    {
        return Math.Max(1, (int)Math.Ceiling(_totalCount / (double)Math.Max(1, _pageSize)));
    }

    private void UpdatePaginationInfo()
    {
        var totalPages = GetTotalPages();
        if (_currentPage > totalPages)
        {
            _currentPage = totalPages;
        }

        var canJump = SearchPagingPolicy.CanJump(totalPages);

        CurrentPageTextBox.Text = _currentPage.ToString();
        PageInfoTextBlock.Text = $"第 {_currentPage}/{totalPages} 页，总计 {_totalCount} 条";
        PrevPageButton.IsEnabled = _currentPage > 1;
        NextPageButton.IsEnabled = _currentPage < totalPages;
        GoPageButton.IsEnabled = canJump;
        CurrentPageTextBox.IsEnabled = canJump;
    }
}