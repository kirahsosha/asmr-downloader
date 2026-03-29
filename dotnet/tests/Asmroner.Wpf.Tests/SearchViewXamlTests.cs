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
        Assert.Contains("Header=\"导出全部任务到 CSV\"", content, StringComparison.Ordinal);
        Assert.Contains("Header=\"导出全部任务到 JSON\"", content, StringComparison.Ordinal);
        Assert.Contains("Header=\"导出选中任务到 CSV\"", content, StringComparison.Ordinal);
        Assert.Contains("Header=\"导出选中任务到 JSON\"", content, StringComparison.Ordinal);
        Assert.Contains("Header=\"在浏览器打开\"", content, StringComparison.Ordinal);
        Assert.Contains("Click=\"OnContextMenuQueueClicked\"", content, StringComparison.Ordinal);
        Assert.Contains("Click=\"OnContextMenuExportCsvClicked\"", content, StringComparison.Ordinal);
        Assert.Contains("Click=\"OnContextMenuExportJsonClicked\"", content, StringComparison.Ordinal);
        Assert.Contains("Click=\"OnContextMenuExportSelectedCsvClicked\"", content, StringComparison.Ordinal);
        Assert.Contains("Click=\"OnContextMenuExportSelectedJsonClicked\"", content, StringComparison.Ordinal);
        Assert.Contains("Click=\"OnContextMenuOpenWorkPageClicked\"", content, StringComparison.Ordinal);
    }
}
