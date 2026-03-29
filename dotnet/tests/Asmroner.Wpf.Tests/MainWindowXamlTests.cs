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
        Assert.Contains("Title=\"Asmroner v0.4.4\"", content, StringComparison.Ordinal);

        var doc = XDocument.Parse(content);
        Assert.NotNull(doc.Root);
    }
}
