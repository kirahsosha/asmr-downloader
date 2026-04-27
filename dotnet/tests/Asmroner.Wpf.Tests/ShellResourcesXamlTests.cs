using System.Xml.Linq;

namespace Asmroner.Wpf.Tests;

public class ShellResourcesXamlTests
{
    [Fact]
    public void ShellResourcesXaml_ShouldContainSharedShellResourceKeys()
    {
        var xamlPath = XamlTestPathLocator.Locate("ShellResources.xaml", "dotnet", "Asmroner.Wpf", "Asmroner.Wpf", "Styles");
        var content = File.ReadAllText(xamlPath);

        Assert.Contains("x:Key=\"ShellBackgroundBrush\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Key=\"ShellCardBorderBaseStyle\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Key=\"ShellSectionTitleStyle\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Key=\"ShellFieldTextBoxBaseStyle\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Key=\"ShellActionButtonBaseStyle\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Key=\"ShellLightActionButtonBaseStyle\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Key=\"PageStateBusyPanelStyle\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Key=\"PageStateEmptyPanelStyle\"", content, StringComparison.Ordinal);

        var doc = XDocument.Parse(content);
        Assert.NotNull(doc.Root);
    }
}