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
}

