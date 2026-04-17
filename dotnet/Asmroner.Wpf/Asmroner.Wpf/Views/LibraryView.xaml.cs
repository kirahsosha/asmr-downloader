using System.Text;
using System.Windows;
using System.Windows.Controls;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Library;
using Asmroner.Core.Playback;

namespace Asmroner.Wpf.Views;

public partial class LibraryView : UserControl
{
    private const int DefaultPageSize = 20;

    private readonly ILibraryQueryService _libraryQueryService;
    private readonly IPlayerService _playerService;

    private int _currentPage = 1;
    private int _pageSize = DefaultPageSize;
    private int _totalPages = 1;
    private int _totalCount;
    private bool _isRefreshing;
    private bool _suppressPageSizeSelectionChanged = true;
    private LibraryWorkItem? _selectedWork;
    private LibraryFileItem? _selectedTreeItem;
    private LibraryFileItem? _selectedFile;
    private LibrarySelectionFeedbackResult _selectionFeedback = new();

    public LibraryView(ILibraryQueryService libraryQueryService, IPlayerService playerService)
    {
        _libraryQueryService = libraryQueryService;
        _playerService = playerService;

        InitializeComponent();
        _pageSize = ReadPageSize();
        UpdatePaginationInfo();
        _suppressPageSizeSelectionChanged = false;
        Loaded += OnLoaded;
        _playerService.ContextChanged += OnPlayerContextChanged;

        UpdatePlaybackContext();
        UpdateCommandAvailability();
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        await RefreshLibraryAsync(resetPage: true);
    }

    private async void OnRefreshLibraryClicked(object sender, RoutedEventArgs e)
    {
        await RefreshLibraryAsync(resetPage: true);
    }

    private async void OnPrevPageClicked(object sender, RoutedEventArgs e)
    {
        if (_currentPage <= 1)
        {
            return;
        }

        _currentPage--;
        await RefreshLibraryAsync(resetPage: false);
    }

    private async void OnNextPageClicked(object sender, RoutedEventArgs e)
    {
        if (_currentPage >= _totalPages)
        {
            return;
        }

        _currentPage++;
        await RefreshLibraryAsync(resetPage: false);
    }

    private async void OnGoPageClicked(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(CurrentPageTextBox.Text.Trim(), out var page) || page <= 0)
        {
            StatusTextBlock.Text = "页码必须为正整数。";
            return;
        }

        _currentPage = page;
        await RefreshLibraryAsync(resetPage: false);
    }

    private async void OnPageSizeChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_suppressPageSizeSelectionChanged)
        {
            return;
        }

        var newPageSize = ReadPageSize();
        if (newPageSize == _pageSize)
        {
            return;
        }

        _pageSize = newPageSize;
        await RefreshLibraryAsync(resetPage: true);
    }

    private void OnLibrarySelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _selectedWork = LibraryWorksDataGrid.SelectedItem as LibraryWorkItem;
        _selectedTreeItem = null;
        _selectedFile = null;
        _selectionFeedback = LibrarySelectionFeedbackPolicy.Evaluate(_selectedWork, selectedItem: null);

        FileTreeView.ItemsSource = _selectedWork?.Files;
        WorkDetailsTextBlock.Text = BuildWorkDetailsText(_selectedWork);
        UpdatePlaybackContext();
        UpdateCommandAvailability();
    }

    private void OnFileTreeSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        _selectedTreeItem = e.NewValue as LibraryFileItem;
        _selectionFeedback = LibrarySelectionFeedbackPolicy.Evaluate(_selectedWork, _selectedTreeItem);
        _selectedFile = _selectionFeedback.PlayableTarget;
        UpdatePlaybackContext();
        UpdateCommandAvailability();
    }

    private void OnLoadContextClicked(object sender, RoutedEventArgs e)
    {
        if (_selectedWork is null || _selectedFile is null)
        {
            return;
        }

        _playerService.LoadContext(_selectedWork, _selectedFile);
        UpdatePlaybackContext();
        UpdateCommandAvailability();
    }

    private void OnPlayClicked(object sender, RoutedEventArgs e)
    {
        if (_selectedWork is null || _selectedFile is null)
        {
            return;
        }

        var current = _playerService.GetCurrentContext();
        if (ShouldLoadSelectionBeforePlay(current))
        {
            _playerService.LoadContext(_selectedWork, _selectedFile);
            current = _playerService.GetCurrentContext();
            if (current.File is null || current.State == PlaybackState.Failed)
            {
                UpdatePlaybackContext();
                UpdateCommandAvailability();
                return;
            }
        }

        _playerService.Play();
        UpdatePlaybackContext();
        UpdateCommandAvailability();
    }

    private void OnClearContextClicked(object sender, RoutedEventArgs e)
    {
        _playerService.ClearContext();
        UpdatePlaybackContext();
        UpdateCommandAvailability();
    }

    private void OnPlayerContextChanged(object? sender, EventArgs e)
    {
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.Invoke(() => OnPlayerContextChanged(sender, e));
            return;
        }

        UpdatePlaybackContext();
        UpdateCommandAvailability();
    }

    private async Task RefreshLibraryAsync(bool resetPage)
    {
        if (_isRefreshing)
        {
            return;
        }

        if (resetPage)
        {
            _currentPage = 1;
        }

        UpdatePaginationInfo();
        _isRefreshing = true;
        StatusTextBlock.Text = "正在刷新资源库，请稍候...";
        UpdateCommandAvailability();

        try
        {
            var result = await _libraryQueryService.QueryAsync(new LibraryQuery
            {
                Keyword = KeywordTextBox.Text,
                SubtitleOnly = SubtitleOnlyCheckBox.IsChecked == true,
                AudioOnly = AudioOnlyCheckBox.IsChecked == true,
                Page = _currentPage,
                PageSize = _pageSize,
            });

            _currentPage = result.Page;
            _pageSize = result.PageSize;
            _totalPages = result.TotalPages;
            _totalCount = result.TotalCount;

            LibraryWorksDataGrid.ItemsSource = result.Items;
            StatusTextBlock.Text = BuildStatusText(result);

            _selectedWork = null;
            _selectedTreeItem = null;
            _selectedFile = null;
            _selectionFeedback = new LibrarySelectionFeedbackResult();
            FileTreeView.ItemsSource = null;
            WorkDetailsTextBlock.Text = "请选择作品查看详情。";
            UpdatePlaybackContext();
            UpdatePaginationInfo();
        }
        catch (OperationCanceledException)
        {
            StatusTextBlock.Text = "资源库刷新已取消。";
        }
        catch (Exception ex)
        {
            StatusTextBlock.Text = $"刷新资源库失败：{ex.Message}";
        }
        finally
        {
            _isRefreshing = false;
            UpdateCommandAvailability();
        }
    }

    private void UpdatePlaybackContext()
    {
        var context = _playerService.GetCurrentContext();
        var texts = LibraryPlaybackContextTextBuilder.Build(context, _selectionFeedback);

        SelectionFeedbackTextBlock.Text = texts.SelectionText;
        ContextTextBlock.Text = texts.LoadedContextText;
    }

    private void UpdateCommandAvailability()
    {
        var context = _playerService.GetCurrentContext();
        var hasSelectedPlayableTarget = _selectedWork is not null
            && _selectedFile is not null
            && _selectionFeedback.CanLoadContext;
        var canJump = _totalPages > 1;

        RefreshLibraryButton.IsEnabled = !_isRefreshing;
        PrevPageButton.IsEnabled = !_isRefreshing && _currentPage > 1;
        NextPageButton.IsEnabled = !_isRefreshing && _currentPage < _totalPages;
        GoPageButton.IsEnabled = !_isRefreshing && canJump;
        CurrentPageTextBox.IsEnabled = !_isRefreshing && canJump;
        PageSizeComboBox.IsEnabled = !_isRefreshing;
        LoadContextButton.IsEnabled = !_isRefreshing && hasSelectedPlayableTarget;
        PlayButton.IsEnabled = !_isRefreshing && hasSelectedPlayableTarget;
        ClearContextButton.IsEnabled = !_isRefreshing && context.Work is not null;
    }

    private void UpdatePaginationInfo()
    {
        CurrentPageTextBox.Text = _currentPage.ToString();
        PageInfoTextBlock.Text = $"第 {_currentPage}/{_totalPages} 页 · 共 {_totalCount} 个作品";
    }

    private static string BuildWorkDetailsText(LibraryWorkItem? work)
    {
        if (work is null)
        {
            return "请选择左侧作品查看详情。";
        }

        var builder = new StringBuilder();
        builder.AppendLine($"作品ID：{work.SourceId}");
        builder.AppendLine($"标题：{work.Title}");
        builder.AppendLine($"日期：{(string.IsNullOrWhiteSpace(work.Release) ? "-" : work.Release)}");
        builder.AppendLine($"字幕：{(work.HasSubtitle ? "有" : "无")}");
        builder.AppendLine($"标签：{(string.IsNullOrWhiteSpace(work.Tags) ? "-" : work.Tags)}");
        builder.AppendLine($"作品目录：{work.RootDirectory}");
        builder.Append($"文件数：{work.TotalFileCount}，可播放音频数：{work.AudioFileCount}");
        return builder.ToString();
    }

    private static string BuildStatusText(LibraryQueryResult result)
    {
        var builder = new StringBuilder();
        builder.Append($"已扫描 {result.ScannedRootCount} 个资源根目录，识别 {result.ScannedWorkCount} 个作品，当前筛选结果 {result.TotalCount} 个。");

        if (result.SkippedDirectories.Count > 0)
        {
            builder.Append($" 已跳过 {result.SkippedDirectories.Count} 个不兼容目录。");
        }

        if (result.Errors.Count > 0)
        {
            builder.Append($" 遇到 {result.Errors.Count} 个扫描错误；首条：{result.Errors[0]}");
        }

        return builder.ToString();
    }

    private bool ShouldLoadSelectionBeforePlay(PlaybackContext current)
    {
        return LibraryPlaybackSelectionPolicy.ShouldReloadContext(current, _selectedWork, _selectedFile);
    }

    private int ReadPageSize()
    {
        var value = ReadComboValue(PageSizeComboBox, DefaultPageSize.ToString());
        return int.TryParse(value, out var parsed) && parsed > 0 ? parsed : DefaultPageSize;
    }

    private static string ReadComboValue(ComboBox comboBox, string fallback)
    {
        if (comboBox.SelectedItem is ComboBoxItem item)
        {
            if (item.Content is string content)
            {
                return content;
            }
        }

        return fallback;
    }

}