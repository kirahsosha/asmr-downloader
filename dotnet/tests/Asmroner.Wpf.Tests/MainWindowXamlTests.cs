using System.Xml.Linq;

namespace Asmroner.Wpf.Tests;

public class MainWindowXamlTests
{
    [Fact]
    public void MainWindowXaml_ShouldUse1280x720DefaultWindowSize()
    {
        var xamlPath = XamlTestPathLocator.Locate("MainWindow.xaml", "dotnet", "Asmroner.Wpf", "Asmroner.Wpf");
        var content = File.ReadAllText(xamlPath);

        Assert.Contains("Width=\"1280\"", content, StringComparison.Ordinal);
        Assert.Contains("Height=\"720\"", content, StringComparison.Ordinal);
        Assert.Contains("MinWidth=\"1280\"", content, StringComparison.Ordinal);
        Assert.Contains("MinHeight=\"720\"", content, StringComparison.Ordinal);
        Assert.Contains("Title=\"Asmroner\"", content, StringComparison.Ordinal);
        Assert.DoesNotContain("Title=\"Asmroner v", content, StringComparison.Ordinal);
        Assert.Contains("Header=\"Library\"", content, StringComparison.Ordinal);
        Assert.Contains("Header=\"Sync\"", content, StringComparison.Ordinal);
        Assert.Contains("Text=\"{Binding StatusMessage}\"", content, StringComparison.Ordinal);
        Assert.Contains("SelectedIndex=\"{Binding SelectedPageIndex, Mode=TwoWay}\"", content, StringComparison.Ordinal);
        Assert.Contains("IsEnabled=\"{Binding IsSearchEnabled}\"", content, StringComparison.Ordinal);
        Assert.Contains("IsEnabled=\"{Binding IsDownloadEnabled}\"", content, StringComparison.Ordinal);
        Assert.Contains("IsEnabled=\"{Binding IsLibraryEnabled}\"", content, StringComparison.Ordinal);
        Assert.Contains("IsEnabled=\"{Binding IsSyncEnabled}\"", content, StringComparison.Ordinal);

        var doc = XDocument.Parse(content);
        Assert.NotNull(doc.Root);
    }

    [Fact]
    public void MainWindowXaml_ShouldNotContainVersionInWindowTitle()
    {
        var xamlPath = XamlTestPathLocator.Locate("MainWindow.xaml", "dotnet", "Asmroner.Wpf", "Asmroner.Wpf");
        var content = File.ReadAllText(xamlPath);

        Assert.DoesNotContain("Title=\"Asmroner v", content, StringComparison.Ordinal);
    }

    [Fact]
    public void MainWindowXaml_ShouldPlaceLibraryTab_BetweenDownloadAndSync()
    {
        var xamlPath = XamlTestPathLocator.Locate("MainWindow.xaml", "dotnet", "Asmroner.Wpf", "Asmroner.Wpf");
        var content = File.ReadAllText(xamlPath);

        Assert.True(
            content.IndexOf("x:Name=\"DownloadTab\"", StringComparison.Ordinal) < content.IndexOf("x:Name=\"LibraryTab\"", StringComparison.Ordinal),
            "Library 页签应位于 Download 之后。");
        Assert.True(
            content.IndexOf("x:Name=\"LibraryTab\"", StringComparison.Ordinal) < content.IndexOf("x:Name=\"SyncTab\"", StringComparison.Ordinal),
            "Library 页签应位于 Sync 之前。");
        Assert.True(
            content.IndexOf("x:Name=\"LibraryHost\"", StringComparison.Ordinal) > content.IndexOf("x:Name=\"LibraryTab\"", StringComparison.Ordinal),
            "Library 宿主控件应位于 Library 页签内。");
    }
}
