using System.Xml.Linq;

namespace Asmroner.Wpf.Tests;

public class FavoriteFolderDialogXamlTests
{
    [Fact]
    public void FavoriteFolderDialogXaml_ShouldContainEditableComboBoxAndConfirmButtons()
    {
        var xamlPath = XamlTestPathLocator.Locate("FavoriteFolderDialog.xaml", "dotnet", "Asmroner.Wpf", "Asmroner.Wpf", "Views");
        var content = File.ReadAllText(xamlPath);

        Assert.Contains("x:Class=\"Asmroner.Wpf.Views.FavoriteFolderDialog\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"FolderTitleComboBox\"", content, StringComparison.Ordinal);
        Assert.Contains("IsEditable=\"True\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"ConfirmButton\"", content, StringComparison.Ordinal);
        Assert.Contains("Content=\"保存\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"CancelButton\"", content, StringComparison.Ordinal);

        var doc = XDocument.Parse(content);
        Assert.NotNull(doc.Root);
    }
}