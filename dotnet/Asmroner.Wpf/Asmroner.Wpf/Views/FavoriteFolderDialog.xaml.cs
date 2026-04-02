using System.Windows;
using System.Windows.Controls;
using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Views;

public partial class FavoriteFolderDialog : Window
{
    private readonly bool _allowCustomInput;
    private readonly IReadOnlyList<string> _folderTitles;

    public FavoriteFolderDialog(
        IEnumerable<string> folderTitles,
        bool allowCustomInput,
        string title,
        string confirmButtonText)
    {
        _allowCustomInput = allowCustomInput;
        _folderTitles = FavoriteFolderSelectionPolicy.BuildFolderTitles(folderTitles);

        InitializeComponent();

        Title = title;
        ConfirmButton.Content = confirmButtonText;
        FolderTitleComboBox.IsEditable = allowCustomInput;
        FolderTitleComboBox.IsTextSearchEnabled = !allowCustomInput;
        FolderTitleComboBox.StaysOpenOnEdit = allowCustomInput;
        FolderTitleComboBox.ItemsSource = _folderTitles;

        PromptTextBlock.Text = allowCustomInput
            ? "请选择现有收藏夹或输入新的收藏夹标题"
            : "请选择要导出的收藏夹";

        HintTextBlock.Text = allowCustomInput
            ? "保存后会按当前版本选择结果写入本地 SQLite 收藏夹。"
            : "导出时会把选中收藏夹中的作品加入当前下载队列。";

        if (_folderTitles.Count > 0)
        {
            FolderTitleComboBox.SelectedIndex = 0;
        }

        FolderTitleComboBox.SelectionChanged += OnFolderTitleChanged;
        FolderTitleComboBox.AddHandler(TextBox.TextChangedEvent, new TextChangedEventHandler(OnFolderTitleTextChanged));

        UpdateConfirmButtonState();
    }

    public string SelectedFolderTitle { get; private set; } = string.Empty;

    private void OnFolderTitleChanged(object sender, SelectionChangedEventArgs e)
    {
        UpdateConfirmButtonState();
    }

    private void OnFolderTitleTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateConfirmButtonState();
    }

    private void OnConfirmClicked(object sender, RoutedEventArgs e)
    {
        var normalizedFolderTitle = FavoriteFolderSelectionPolicy.NormalizeFolderTitle(FolderTitleComboBox.Text);
        if (!FavoriteFolderSelectionPolicy.CanConfirm(normalizedFolderTitle, _allowCustomInput, _folderTitles))
        {
            return;
        }

        SelectedFolderTitle = normalizedFolderTitle;
        DialogResult = true;
    }

    private void UpdateConfirmButtonState()
    {
        ConfirmButton.IsEnabled = FavoriteFolderSelectionPolicy.CanConfirm(
            FolderTitleComboBox.Text,
            _allowCustomInput,
            _folderTitles);
    }
}