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
    private int _totalPages = 1;
    private bool _isRefreshing;
    private LibraryWorkItem? _selectedWork;
    private LibraryFileItem? _selectedFile;

    public LibraryView(ILibraryQueryService libraryQueryService, IPlayerService playerService)
    {
        _libraryQueryService = libraryQueryService;
        _playerService = playerService;

        InitializeComponent();
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

    private void OnLibrarySelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _selectedWork = LibraryWorksDataGrid.SelectedItem as LibraryWorkItem;
        _selectedFile = null;

        FileTreeView.ItemsSource = _selectedWork?.Files;
        WorkDetailsTextBlock.Text = BuildWorkDetailsText(_selectedWork);
        UpdateCommandAvailability();
    }

    private void OnFileTreeSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        var selectedItem = e.NewValue as LibraryFileItem;
        _selectedFile = selectedItem is { IsDirectory: false, IsPlayable: true } ? selectedItem : null;
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
                PageSize = DefaultPageSize,
            });

            _currentPage = result.Page;
            _totalPages = result.TotalPages;

            LibraryWorksDataGrid.ItemsSource = result.Items;
            PageInfoTextBlock.Text = $"第 {result.Page}/{result.TotalPages} 页 · 共 {result.TotalCount} 个作品";
            StatusTextBlock.Text = BuildStatusText(result);

            _selectedWork = null;
            _selectedFile = null;
            FileTreeView.ItemsSource = null;
            WorkDetailsTextBlock.Text = "请选择左侧作品查看详情。";
            UpdatePlaybackContext();
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
        if (context.Work is null || context.File is null)
        {
            ContextTextBlock.Text = context.Message;
            return;
        }

        ContextTextBlock.Text = $"状态：{BuildPlaybackStateText(context.State)}\n作品：{context.Work.SourceId} {context.Work.Title}\n文件：{context.File.RelativePath}\n说明：{context.Message}";
    }

    private void UpdateCommandAvailability()
    {
        var context = _playerService.GetCurrentContext();
        var hasSelectedPlayableTarget = _selectedWork is not null && _selectedFile is not null;

        RefreshLibraryButton.IsEnabled = !_isRefreshing;
        PrevPageButton.IsEnabled = !_isRefreshing && _currentPage > 1;
        NextPageButton.IsEnabled = !_isRefreshing && _currentPage < _totalPages;
        LoadContextButton.IsEnabled = !_isRefreshing && hasSelectedPlayableTarget;
        PlayButton.IsEnabled = !_isRefreshing && hasSelectedPlayableTarget;
        ClearContextButton.IsEnabled = !_isRefreshing && context.Work is not null;
    }

    private static string BuildWorkDetailsText(LibraryWorkItem? work)
    {
        if (work is null)
        {
            return "请选择左侧作品查看详情。";
        }

        var builder = new StringBuilder();
        builder.AppendLine($"SourceId：{work.SourceId}");
        builder.AppendLine($"标题：{work.Title}");
        builder.AppendLine($"日期：{(string.IsNullOrWhiteSpace(work.Release) ? "-" : work.Release)}");
        builder.AppendLine($"字幕：{(work.HasSubtitle ? "有" : "无")}");
        builder.AppendLine($"标签：{(string.IsNullOrWhiteSpace(work.Tags) ? "-" : work.Tags)}");
        builder.AppendLine($"目录格式：{work.DirectoryScheme}");
        builder.AppendLine($"来源根目录：{work.SourceRoot}");
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

    private static string BuildPlaybackStateText(PlaybackState state)
    {
        return state switch
        {
            PlaybackState.None => "未载入",
            PlaybackState.Ready => "已载入",
            PlaybackState.Launched => "已调用系统打开",
            PlaybackState.Failed => "打开失败",
            _ => state.ToString(),
        };
    }
}