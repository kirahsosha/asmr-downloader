using System.IO;
using System.Text;
using Asmroner.Core.Api;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Search;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System.Windows.Controls;
using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Views;

public partial class DashboardView : UserControl
{
    private static readonly char[] FilterSeparators = [' ', '\t', '\r', '\n', ';', ','];

    private readonly ISearchService _searchService;
    private readonly IAsmrApiClient _asmrApiClient;
    private readonly ISearchExportService _searchExportService;
    private readonly ISearchStateStore _searchStateStore;
    private readonly IDownloadService _downloadService;
    private readonly IConfigurationService _configurationService;
    private readonly IAppPathService _appPathService;
    private readonly ILogger<DashboardView> _logger;

    private IReadOnlyList<SearchWorkItem> _results = Array.Empty<SearchWorkItem>();
    private IReadOnlyList<SearchWorkItem> _popularResults = Array.Empty<SearchWorkItem>();
    private int _currentPage = 1;
    private int _pageSize = 20;
    private int _totalCount;
    private bool _isPopularMode;
    private bool _globalSearchRuleLoadedAtStartup;

    public DashboardView()
        : this(
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

    public DashboardView(
        ISearchService searchService,
        IAsmrApiClient asmrApiClient,
        ISearchExportService searchExportService,
        ISearchStateStore searchStateStore,
        IDownloadService downloadService,
        IConfigurationService configurationService,
        IAppPathService appPathService,
        ILogger<DashboardView> logger)
    {
        _searchService = searchService;
        _asmrApiClient = asmrApiClient;
        _searchExportService = searchExportService;
        _searchStateStore = searchStateStore;
        _downloadService = downloadService;
        _configurationService = configurationService;
        _appPathService = appPathService;
        _logger = logger;

        InitializeComponent();
        _pageSize = ReadPageSize();
        UpdatePaginationInfo();

        Loaded += async (_, _) => await LoadGlobalSearchRuleAsync();
    }

    private async Task LoadGlobalSearchRuleAsync()
    {
        // Only load global search rules once at startup
        if (_globalSearchRuleLoadedAtStartup)
        {
            return;
        }

        try
        {
            var config = await _configurationService.LoadAsync();
            if (string.IsNullOrWhiteSpace(config?.Downloader.GlobalSearchRule))
            {
                return;
            }

            ApplyGlobalSearchRule(config.Downloader.GlobalSearchRule);
            _globalSearchRuleLoadedAtStartup = true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load global search rule from configuration.");
        }
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

    private async Task ExecuteSearchAsync()
    {
        var rawQuery = BuildEffectiveQuery();
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
        await ExportAsync("csv");
    }

    private async void OnExportJsonClicked(object sender, System.Windows.RoutedEventArgs e)
    {
        await ExportAsync("json");
    }

    private void OnQueueClicked(object sender, System.Windows.RoutedEventArgs e)
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
        var queueCount = _searchStateStore.GetQueuedSourceIds().Count;
        StatusTextBlock.Text = queuePlan.SkippedCount > 0
            ? $"已加入下载队列 {queuePlan.ToEnqueue.Count} 项，跳过 {queuePlan.SkippedCount} 项（已存在或重复），当前队列总数 {queueCount}。"
            : $"已加入下载队列 {queuePlan.ToEnqueue.Count} 项，当前队列总数 {queueCount}。";
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

        OrderComboBox.SelectedIndex = 0;
        SortComboBox.SelectedIndex = 0;
        SubtitleComboBox.SelectedIndex = 0;
        PageSizeComboBox.SelectedIndex = 0;

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

        await ExecuteSearchAsync();
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

        await ExecuteSearchAsync();
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

        await ExecuteSearchAsync();
    }

    private async void OnPageSizeChanged(object sender, SelectionChangedEventArgs e)
    {
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

        if (!string.IsNullOrWhiteSpace(QueryTextBox.Text) || HasAdvancedFilter())
        {
            await ExecuteSearchAsync();
            return;
        }

        UpdatePaginationInfo();
    }

    private async Task ExportAsync(string extension)
    {
        if (_results.Count == 0)
        {
            StatusTextBlock.Text = "当前没有可导出的搜索结果。";
            return;
        }

        ToggleActionButtons(false);

        try
        {
            Directory.CreateDirectory(_appPathService.MetadataDirectory);
            var fileName = $"search-export-{DateTime.Now:yyyyMMdd-HHmmss}.{extension}";
            var fullPath = ShowSaveFileDialog(extension, fileName);
            if (string.IsNullOrWhiteSpace(fullPath))
            {
                StatusTextBlock.Text = "已取消导出。";
                return;
            }

            if (extension.Equals("csv", StringComparison.OrdinalIgnoreCase))
            {
                await _searchExportService.ExportCsvAsync(_results, fullPath);
            }
            else
            {
                await _searchExportService.ExportJsonAsync(_results, fullPath);
            }

            StatusTextBlock.Text = $"导出成功：{fullPath}";
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

    private string BuildEffectiveQuery()
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

        if (builder.Length == 0)
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

    private void ApplyGlobalSearchRule(string globalSearchRule)
    {
        var tokens = globalSearchRule
            .Split(FilterSeparators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var token in tokens)
        {
            var normalizedToken = token.Trim();
            var colonIndex = normalizedToken.IndexOf(':');
            if (colonIndex <= 0 || colonIndex == normalizedToken.Length - 1)
            {
                continue;
            }

            var isExclude = normalizedToken.StartsWith("-", StringComparison.Ordinal);
            var keyStart = isExclude ? 1 : 0;
            var key = normalizedToken[keyStart..colonIndex].Trim().ToLowerInvariant();
            var value = normalizedToken[(colonIndex + 1)..].Trim();
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            switch (key)
            {
                case "tag":
                    ApplyFilterValue(TagTextBox, TagExcludeCheckBox, value, isExclude);
                    break;
                case "circle":
                    ApplyFilterValue(CircleTextBox, CircleExcludeCheckBox, value, isExclude);
                    break;
                case "va":
                    ApplyFilterValue(VaTextBox, VaExcludeCheckBox, value, isExclude);
                    break;
                case "duration":
                    ApplyFilterValue(DurationTextBox, DurationExcludeCheckBox, value, isExclude);
                    break;
                case "rate":
                    ApplyFilterValue(RateTextBox, RateExcludeCheckBox, value, isExclude);
                    break;
                case "price":
                    ApplyFilterValue(PriceTextBox, PriceExcludeCheckBox, value, isExclude);
                    break;
                case "sell":
                    ApplyFilterValue(SellTextBox, SellExcludeCheckBox, value, isExclude);
                    break;
                case "age":
                    ApplyFilterValue(AgeTextBox, AgeExcludeCheckBox, value, isExclude);
                    break;
                case "lang":
                    ApplyFilterValue(LangTextBox, LangExcludeCheckBox, value, isExclude);
                    break;
            }
        }
    }

    private static void ApplyFilterValue(TextBox textBox, CheckBox excludeCheckBox, string value, bool isExclude)
    {
        textBox.Text = SearchFilterValuePolicy.MergeDistinct(textBox.Text, value);

        if (isExclude)
        {
            excludeCheckBox.IsChecked = true;
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