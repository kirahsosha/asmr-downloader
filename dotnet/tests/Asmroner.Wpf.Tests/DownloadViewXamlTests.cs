using System.Xml.Linq;

namespace Asmroner.Wpf.Tests;

public class DownloadViewXamlTests
{
    [Fact]
    public void DownloadViewXaml_ShouldContainBeautifiedStyleResources_AndCoreControls()
    {
        var xamlPath = XamlTestPathLocator.Locate("DownloadView.xaml", "dotnet", "Asmroner.Wpf", "Asmroner.Wpf", "Views");
        var content = File.ReadAllText(xamlPath);

        Assert.Contains("x:Key=\"PageBackgroundBrush\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Key=\"CardBorderStyle\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Key=\"ActionButtonStyle\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"TaskGrid\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"StartSelectedButton\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"ClearTaskListButton\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"HdAudioOnlyCheckBox\"", content, StringComparison.Ordinal);
        Assert.Contains("IsChecked=\"True\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"QueueTranslationCheckBox\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"StatusTextBlock\"", content, StringComparison.Ordinal);

        var doc = XDocument.Parse(content);
        Assert.NotNull(doc.Root);
    }

    [Fact]
    public void DownloadViewXaml_ShouldNotContainStagePrefixText()
    {
        var xamlPath = XamlTestPathLocator.Locate("DownloadView.xaml", "dotnet", "Asmroner.Wpf", "Asmroner.Wpf", "Views");
        var content = File.ReadAllText(xamlPath);

        Assert.DoesNotContain("阶段 ", content, StringComparison.Ordinal);
    }

    [Fact]
    public void DownloadViewXaml_ShouldUseHeaderBorders_AndLockStatusWidthWhileLeavingProgressResizable()
    {
        var xamlPath = XamlTestPathLocator.Locate("DownloadView.xaml", "dotnet", "Asmroner.Wpf", "Asmroner.Wpf", "Views");
        var content = File.ReadAllText(xamlPath);

        Assert.Contains("x:Key=\"TaskGridHeaderStyle\"", content, StringComparison.Ordinal);
        Assert.Contains("Value=\"1,1,1,1\"", content, StringComparison.Ordinal);
        Assert.Contains("HorizontalScrollBarVisibility=\"Auto\"", content, StringComparison.Ordinal);
        Assert.Contains("CanUserReorderColumns=\"True\"", content, StringComparison.Ordinal);

        var doc = XDocument.Parse(content);
        XNamespace presentation = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        var statusColumn = doc.Descendants(presentation + "DataGridTextColumn")
            .First(column => string.Equals((string?)column.Attribute("Header"), "状态", StringComparison.Ordinal));
        var progressColumn = doc.Descendants(presentation + "DataGridTextColumn")
            .First(column => string.Equals((string?)column.Attribute("Header"), "进度", StringComparison.Ordinal));

        Assert.Equal("50", (string?)statusColumn.Attribute("Width"));
        Assert.Equal("False", (string?)statusColumn.Attribute("CanUserResize"));
        Assert.Equal("96", (string?)progressColumn.Attribute("Width"));
        Assert.Null((string?)progressColumn.Attribute("CanUserResize"));
    }

    [Fact]
    public void DownloadViewXaml_ShouldContainQueueTranslationCheckbox()
    {
        var xamlPath = XamlTestPathLocator.Locate("DownloadView.xaml", "dotnet", "Asmroner.Wpf", "Asmroner.Wpf", "Views");
        var content = File.ReadAllText(xamlPath);

        Assert.Contains("x:Name=\"QueueTranslationCheckBox\"", content, StringComparison.Ordinal);
        Assert.Contains("Content=\"加入翻译作品\"", content, StringComparison.Ordinal);
        Assert.Contains("IsChecked=\"True\"", content, StringComparison.Ordinal);
        Assert.True(
            content.IndexOf("x:Name=\"HdAudioOnlyCheckBox\"", StringComparison.Ordinal) < content.IndexOf("x:Name=\"QueueTranslationCheckBox\"", StringComparison.Ordinal),
            "加入翻译作品复选框应位于只下载高清音频右侧。");
    }
}

