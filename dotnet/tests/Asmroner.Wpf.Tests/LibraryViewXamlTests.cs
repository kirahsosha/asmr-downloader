using System.Xml.Linq;

namespace Asmroner.Wpf.Tests;

public class LibraryViewXamlTests
{
    [Fact]
    public void LibraryViewXaml_ShouldContainCoreLibraryControls_AndStatusBlocks()
    {
        var xamlPath = XamlTestPathLocator.Locate("LibraryView.xaml", "dotnet", "Asmroner.Wpf", "Asmroner.Wpf", "Views");
        var content = File.ReadAllText(xamlPath);

        Assert.Contains("x:Class=\"Asmroner.Wpf.Views.LibraryView\"", content, StringComparison.Ordinal);
        Assert.Contains("Text=\"Library\"", content, StringComparison.Ordinal);
        Assert.Contains("Text=\"本地资源库，文件查看与播放。\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"KeywordTextBox\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"SubtitleOnlyCheckBox\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"AudioOnlyCheckBox\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"RefreshLibraryButton\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"CurrentPageTextBox\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"GoPageButton\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"PageSizeComboBox\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"LibraryWorksDataGrid\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"FileTreeView\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"SelectionFeedbackTextBlock\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"LoadContextButton\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"PlayButton\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"ClearContextButton\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"ContextTextBlock\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"StatusTextBlock\"", content, StringComparison.Ordinal);
        Assert.Contains("ContentTemplate=\"{StaticResource PageStatePresenterTemplate}\"", content, StringComparison.Ordinal);
        Assert.Contains("Content=\"刷新资源库\"", content, StringComparison.Ordinal);
        Assert.Contains("Content=\"载入/切换文件\"", content, StringComparison.Ordinal);
        Assert.Contains("Content=\"播放\"", content, StringComparison.Ordinal);
        Assert.Contains("Content=\"清空上下文\"", content, StringComparison.Ordinal);
        Assert.Contains("Text=\"作品详情\"", content, StringComparison.Ordinal);
        Assert.Contains("Text=\"文件树与播放上下文\"", content, StringComparison.Ordinal);
        Assert.Contains("Text=\"当前选择\"", content, StringComparison.Ordinal);
        Assert.Contains("Text=\"当前已载入上下文\"", content, StringComparison.Ordinal);
        Assert.Contains("Text=\"当前未选择任何作品或文件。请先选择左侧作品，并在文件树中显式选中一个可播放的媒体文件。\"", content, StringComparison.Ordinal);
        Assert.Contains("Text=\"状态：未载入&#10;说明：尚未载入任何可播放媒体文件。\"", content, StringComparison.Ordinal);
        Assert.DoesNotContain("PauseButton", content, StringComparison.Ordinal);
        Assert.DoesNotContain("StopButton", content, StringComparison.Ordinal);
        Assert.DoesNotContain("Content=\"暂停\"", content, StringComparison.Ordinal);
        Assert.DoesNotContain("Content=\"停止\"", content, StringComparison.Ordinal);
        Assert.Contains("Style=\"{StaticResource ResultGridStyle}\"", content, StringComparison.Ordinal);

        var doc = XDocument.Parse(content);
        Assert.NotNull(doc.Root);
    }

    [Fact]
    public void LibraryViewXaml_ShouldContainExpectedGridColumns_AndFileTreeTemplate()
    {
        var xamlPath = XamlTestPathLocator.Locate("LibraryView.xaml", "dotnet", "Asmroner.Wpf", "Asmroner.Wpf", "Views");
        var content = File.ReadAllText(xamlPath);

        Assert.Contains("Header=\"作品ID\"", content, StringComparison.Ordinal);
        Assert.Contains("Header=\"标题\"", content, StringComparison.Ordinal);
        Assert.Contains("Header=\"日期\"", content, StringComparison.Ordinal);
        Assert.Contains("Header=\"字幕\"", content, StringComparison.Ordinal);
        Assert.Contains("Header=\"音频\"", content, StringComparison.Ordinal);
        Assert.Contains("Header=\"文件\"", content, StringComparison.Ordinal);
        Assert.Contains("HierarchicalDataTemplate", content, StringComparison.Ordinal);
        Assert.Contains("Text=\"{Binding DisplayName}\"", content, StringComparison.Ordinal);

        var doc = XDocument.Parse(content);
        XNamespace presentation = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        var dataGridColumns = doc.Descendants(presentation + "DataGridTextColumn").ToArray();

        Assert.True(dataGridColumns.Length >= 5);
    }

    [Fact]
    public void LibraryViewXaml_ShouldUseScrollableLayout_ForPagingAndRightPane()
    {
        var xamlPath = XamlTestPathLocator.Locate("LibraryView.xaml", "dotnet", "Asmroner.Wpf", "Asmroner.Wpf", "Views");
        var content = File.ReadAllText(xamlPath);

        Assert.Contains("Click=\"OnGoPageClicked\"", content, StringComparison.Ordinal);
        Assert.Contains("SelectionChanged=\"OnPageSizeChanged\"", content, StringComparison.Ordinal);
        Assert.Contains("ScrollViewer.HorizontalScrollBarVisibility=\"Auto\"", content, StringComparison.Ordinal);
        Assert.Contains("ScrollViewer.VerticalScrollBarVisibility=\"Auto\"", content, StringComparison.Ordinal);
        Assert.DoesNotContain("Height=\"220\"", content, StringComparison.Ordinal);

        var doc = XDocument.Parse(content);
        XNamespace presentation = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        var fileTree = doc.Descendants(presentation + "TreeView")
            .Single(node => string.Equals((string?)node.Attribute(XName.Get("Name", "http://schemas.microsoft.com/winfx/2006/xaml")), "FileTreeView", StringComparison.Ordinal));

        Assert.Null(fileTree.Attribute("Height"));
        Assert.Equal("2", (string?)fileTree.Attribute("Grid.Row"));
        Assert.True(doc.Descendants(presentation + "ScrollViewer").Any());
    }
}