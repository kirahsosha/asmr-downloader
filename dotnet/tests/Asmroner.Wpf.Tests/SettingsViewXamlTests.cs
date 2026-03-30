using System.Xml.Linq;

namespace Asmroner.Wpf.Tests;

public class SettingsViewXamlTests
{
    [Fact]
    public void SettingsViewXaml_ShouldContainCardSections_AndActionButtons()
    {
        var xamlPath = XamlTestPathLocator.Locate("SettingsView.xaml", "dotnet", "Asmroner.Wpf", "Asmroner.Wpf", "Views");
        var content = File.ReadAllText(xamlPath);

        Assert.Contains("x:Key=\"CardBorderStyle\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Key=\"FieldTextBoxStyle\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"SaveButton\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"TestConnectionButton\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"StatusTextBlock\"", content, StringComparison.Ordinal);
        Assert.DoesNotContain("FileFilterTextBox", content, StringComparison.Ordinal);
        Assert.DoesNotContain("GlobalSearchRuleTextBox", content, StringComparison.Ordinal);
        Assert.Contains("Text=\"版本：v0.4.5\"", content, StringComparison.Ordinal);

        var doc = XDocument.Parse(content);
        Assert.NotNull(doc.Root);
    }

    [Fact]
    public void SettingsViewXaml_ShouldNotContainStagePrefixText()
    {
        var xamlPath = XamlTestPathLocator.Locate("SettingsView.xaml", "dotnet", "Asmroner.Wpf", "Asmroner.Wpf", "Views");
        var content = File.ReadAllText(xamlPath);

        Assert.DoesNotContain("阶段 ", content, StringComparison.Ordinal);
    }
}
