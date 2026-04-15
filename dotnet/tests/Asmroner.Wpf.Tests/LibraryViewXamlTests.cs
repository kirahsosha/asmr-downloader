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
        Assert.Contains("Text=\"阶段 6 首批：本地资源扫描、列表浏览与播放上下文装载。\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"KeywordTextBox\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"SubtitleOnlyCheckBox\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"AudioOnlyCheckBox\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"RefreshLibraryButton\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"LibraryWorksDataGrid\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"FileTreeView\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"LoadContextButton\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"ClearContextButton\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"ContextTextBlock\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"StatusTextBlock\"", content, StringComparison.Ordinal);
        Assert.Contains("Content=\"刷新资源库\"", content, StringComparison.Ordinal);
        Assert.Contains("Content=\"载入选中文件\"", content, StringComparison.Ordinal);
        Assert.Contains("Content=\"清空上下文\"", content, StringComparison.Ordinal);
        Assert.Contains("Style=\"{StaticResource ResultGridStyle}\"", content, StringComparison.Ordinal);

        var doc = XDocument.Parse(content);
        Assert.NotNull(doc.Root);
    }

    [Fact]
    public void LibraryViewXaml_ShouldContainExpectedGridColumns_AndFileTreeTemplate()
    {
        var xamlPath = XamlTestPathLocator.Locate("LibraryView.xaml", "dotnet", "Asmroner.Wpf", "Asmroner.Wpf", "Views");
        var content = File.ReadAllText(xamlPath);

        Assert.Contains("Header=\"SourceId\"", content, StringComparison.Ordinal);
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
}