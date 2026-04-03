using System.Xml.Linq;

namespace Asmroner.Wpf.Tests;

public class SyncViewXamlTests
{
    [Fact]
    public void SyncViewXaml_ShouldContainPrimaryActions_AndStatusFields()
    {
        var xamlPath = XamlTestPathLocator.Locate("SyncView.xaml", "dotnet", "Asmroner.Wpf", "Asmroner.Wpf", "Views");
        var content = File.ReadAllText(xamlPath);

        Assert.Contains("x:Name=\"SyncMetadataButton\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"SyncDownloadButton\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"RetryFailedButton\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"ExportFailedButton\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"ExportCompletedButton\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"RefreshStatusButton\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"CurrentCountTextBlock\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"DownloadCountTextBlock\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"MetadataBreakdownTextBlock\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"CompletedBreakdownTextBlock\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"BacklogBreakdownTextBlock\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"ProgressBreakdownTextBlock\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"LastSyncTextBlock\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"StatusTextBlock\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"DetailsTextBox\"", content, StringComparison.Ordinal);
        Assert.Contains("Text=\"同步能力（阶段 5 前五批）\"", content, StringComparison.Ordinal);

        var doc = XDocument.Parse(content);
        Assert.NotNull(doc.Root);
    }
}