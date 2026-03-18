using System.Xml.Linq;

namespace Asmroner.Wpf.Tests;

public class DashboardViewXamlTests
{
    [Fact]
    public void DashboardViewXaml_ShouldContainUnifiedCardStyles_AndCoreControls()
    {
        var xamlPath = XamlTestPathLocator.Locate("DashboardView.xaml", "dotnet", "Asmroner.Wpf", "Asmroner.Wpf", "Views");
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
    public void DashboardViewXaml_ShouldUseAlignedComboBoxStyles()
    {
        var xamlPath = XamlTestPathLocator.Locate("DashboardView.xaml", "dotnet", "Asmroner.Wpf", "Asmroner.Wpf", "Views");
        var content = File.ReadAllText(xamlPath);

        Assert.Contains("x:Key=\"FieldComboBoxStyle\"", content, StringComparison.Ordinal);
        Assert.Contains("VerticalContentAlignment", content, StringComparison.Ordinal);
        Assert.Contains("HorizontalContentAlignment", content, StringComparison.Ordinal);
        Assert.Contains("<Style TargetType=\"ComboBoxItem\">", content, StringComparison.Ordinal);

        var doc = XDocument.Parse(content);
        Assert.NotNull(doc.Root);
    }

    [Fact]
    public void DashboardViewXaml_ShouldNotContainStagePrefixText()
    {
        var xamlPath = XamlTestPathLocator.Locate("DashboardView.xaml", "dotnet", "Asmroner.Wpf", "Asmroner.Wpf", "Views");
        var content = File.ReadAllText(xamlPath);

        Assert.DoesNotContain("阶段 ", content, StringComparison.Ordinal);
    }
}
