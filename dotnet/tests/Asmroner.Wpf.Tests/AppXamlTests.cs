using System.Xml.Linq;

namespace Asmroner.Wpf.Tests;

public class AppXamlTests
{
    [Fact]
    public void AppXaml_ShouldMergeShellResourcesDictionary()
    {
        var xamlPath = XamlTestPathLocator.Locate("App.xaml", "dotnet", "Asmroner.Wpf", "Asmroner.Wpf");
        var content = File.ReadAllText(xamlPath);

        Assert.Contains("ResourceDictionary.MergedDictionaries", content, StringComparison.Ordinal);
        Assert.Contains("Source=\"Styles/ShellResources.xaml\"", content, StringComparison.Ordinal);

        var doc = XDocument.Parse(content);
        Assert.NotNull(doc.Root);
    }
}