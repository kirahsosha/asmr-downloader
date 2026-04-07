using System.Xml.Linq;

namespace Asmroner.Wpf.Tests;

public class SearchViewXamlTests
{
    [Fact]
    public void SearchViewXaml_ShouldContainUnifiedCardStyles_AndCoreControls()
    {
        var xamlPath = XamlTestPathLocator.Locate("SearchView.xaml", "dotnet", "Asmroner.Wpf", "Asmroner.Wpf", "Views");
        var content = File.ReadAllText(xamlPath);

        Assert.Contains("x:Key=\"CardBorderStyle\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Key=\"ActionButtonStyle\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"QueryTextBox\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"ResultsGrid\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"StatusTextBlock\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"QueueTranslationCheckBox\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"ExportButton\"", content, StringComparison.Ordinal);
        Assert.Contains("Content=\"导出到文件\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"FavoriteButton\"", content, StringComparison.Ordinal);
        Assert.Contains("Content=\"收藏作品\"", content, StringComparison.Ordinal);
        Assert.Contains("Style=\"{StaticResource StatusInfoPanelStyle}\"", content, StringComparison.Ordinal);
        Assert.Contains("Style=\"{StaticResource StatusInfoTextStyle}\"", content, StringComparison.Ordinal);

        var doc = XDocument.Parse(content);
        Assert.NotNull(doc.Root);
    }

    [Fact]
    public void SearchViewXaml_ShouldUseAlignedComboBoxStyles()
    {
        var xamlPath = XamlTestPathLocator.Locate("SearchView.xaml", "dotnet", "Asmroner.Wpf", "Asmroner.Wpf", "Views");
        var content = File.ReadAllText(xamlPath);

        Assert.Contains("x:Key=\"FieldComboBoxStyle\"", content, StringComparison.Ordinal);
        Assert.Contains("VerticalContentAlignment", content, StringComparison.Ordinal);
        Assert.Contains("HorizontalContentAlignment", content, StringComparison.Ordinal);
        Assert.Contains("<Style TargetType=\"ComboBoxItem\">", content, StringComparison.Ordinal);

        var doc = XDocument.Parse(content);
        Assert.NotNull(doc.Root);
    }

    [Fact]
    public void SearchViewXaml_ShouldNotContainStagePrefixText()
    {
        var xamlPath = XamlTestPathLocator.Locate("SearchView.xaml", "dotnet", "Asmroner.Wpf", "Asmroner.Wpf", "Views");
        var content = File.ReadAllText(xamlPath);

        Assert.DoesNotContain("阶段 ", content, StringComparison.Ordinal);
    }

    [Fact]
    public void SearchViewXaml_ShouldUseSearchViewClassName()
    {
        var xamlPath = XamlTestPathLocator.Locate("SearchView.xaml", "dotnet", "Asmroner.Wpf", "Asmroner.Wpf", "Views");
        var content = File.ReadAllText(xamlPath);

        Assert.Contains("x:Class=\"Asmroner.Wpf.Views.SearchView\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public void SearchViewXaml_ShouldWireSelectionChangedHandlersForQueryOptions()
    {
        var xamlPath = XamlTestPathLocator.Locate("SearchView.xaml", "dotnet", "Asmroner.Wpf", "Asmroner.Wpf", "Views");
        var content = File.ReadAllText(xamlPath);

        Assert.Contains("SelectionChanged=\"OnOrderChanged\"", content, StringComparison.Ordinal);
        Assert.Contains("SelectionChanged=\"OnSortChanged\"", content, StringComparison.Ordinal);
        Assert.Contains("SelectionChanged=\"OnSubtitleChanged\"", content, StringComparison.Ordinal);
        Assert.Contains("SelectionChanged=\"OnPageSizeChanged\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public void SearchViewXaml_ShouldContainResultsGridContextMenuItems()
    {
        var xamlPath = XamlTestPathLocator.Locate("SearchView.xaml", "dotnet", "Asmroner.Wpf", "Asmroner.Wpf", "Views");
        var content = File.ReadAllText(xamlPath);

        Assert.Contains("PreviewMouseRightButtonDown=\"OnResultsGridPreviewMouseRightButtonDown\"", content, StringComparison.Ordinal);
        Assert.Contains("<DataGrid.ContextMenu>", content, StringComparison.Ordinal);
        Assert.Contains("Header=\"加入下载队列\"", content, StringComparison.Ordinal);
        Assert.Contains("Header=\"导出全部任务到文件\"", content, StringComparison.Ordinal);
        Assert.Contains("Header=\"导出选中任务到文件\"", content, StringComparison.Ordinal);
        Assert.Contains("Header=\"在浏览器打开\"", content, StringComparison.Ordinal);
        Assert.Contains("Click=\"OnContextMenuQueueClicked\"", content, StringComparison.Ordinal);
        Assert.Contains("Click=\"OnContextMenuExportAllClicked\"", content, StringComparison.Ordinal);
        Assert.Contains("Click=\"OnContextMenuExportSelectedClicked\"", content, StringComparison.Ordinal);
        Assert.Contains("Click=\"OnContextMenuOpenWorkPageClicked\"", content, StringComparison.Ordinal);
    }

    [Fact]
    public void SearchViewXaml_ShouldContainSeparateQueueTranslationCheckbox()
    {
        var xamlPath = XamlTestPathLocator.Locate("SearchView.xaml", "dotnet", "Asmroner.Wpf", "Asmroner.Wpf", "Views");
        var content = File.ReadAllText(xamlPath);

        Assert.Contains("x:Name=\"IncludeTranslationCheckBox\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"QueueTranslationCheckBox\"", content, StringComparison.Ordinal);
        Assert.Contains("Content=\"加入翻译作品\"", content, StringComparison.Ordinal);
        Assert.Contains("IsChecked=\"True\"", content, StringComparison.Ordinal);
        Assert.True(
            content.IndexOf("x:Name=\"IncludeTranslationCheckBox\"", StringComparison.Ordinal) < content.IndexOf("x:Name=\"QueueTranslationCheckBox\"", StringComparison.Ordinal),
            "加入翻译作品复选框应位于包含翻译作品右侧。");
    }

    [Fact]
    public void SearchViewXaml_ShouldUseHeaderBorders_AndLockSubtitleAndDateColumnWidths()
    {
        var xamlPath = XamlTestPathLocator.Locate("SearchView.xaml", "dotnet", "Asmroner.Wpf", "Asmroner.Wpf", "Views");
        var content = File.ReadAllText(xamlPath);

        Assert.Contains("x:Key=\"ResultGridHeaderStyle\"", content, StringComparison.Ordinal);
        Assert.Contains("Value=\"1,1,1,1\"", content, StringComparison.Ordinal);
        Assert.Contains("Property=\"HorizontalScrollBarVisibility\"", content, StringComparison.Ordinal);
        Assert.Contains("CanUserReorderColumns=\"True\"", content, StringComparison.Ordinal);

        var doc = XDocument.Parse(content);
        XNamespace presentation = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        var subtitleColumn = doc.Descendants(presentation + "DataGridCheckBoxColumn")
            .First(column => string.Equals((string?)column.Attribute("Header"), "字幕", StringComparison.Ordinal));
        var releaseColumn = doc.Descendants(presentation + "DataGridTextColumn")
            .First(column => string.Equals((string?)column.Attribute("Header"), "日期", StringComparison.Ordinal));

        Assert.Equal("42", (string?)subtitleColumn.Attribute("Width"));
        Assert.Equal("False", (string?)subtitleColumn.Attribute("CanUserResize"));
        Assert.Equal("75", (string?)releaseColumn.Attribute("Width"));
        Assert.Equal("False", (string?)releaseColumn.Attribute("CanUserResize"));
    }
}
