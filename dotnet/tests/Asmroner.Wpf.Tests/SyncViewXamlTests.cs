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
        Assert.DoesNotContain("x:Name=\"StopMetadataSyncButton\"", content, StringComparison.Ordinal);
        Assert.DoesNotContain("x:Name=\"StopSyncDownloadButton\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"CurrentCountTextBlock\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"DownloadCountTextBlock\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"MetadataBreakdownTextBlock\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"CompletedBreakdownTextBlock\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"BacklogBreakdownTextBlock\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"ProgressBreakdownTextBlock\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"LastSyncTextBlock\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"StatusTextBlock\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"DownloadStatusTextBlock\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"DetailsTextBox\"", content, StringComparison.Ordinal);
        Assert.Contains("ContentTemplate=\"{StaticResource PageStatePresenterTemplate}\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"Root\"", content, StringComparison.Ordinal);
        Assert.Contains("Text=\"同步能力（阶段 5 增强）\"", content, StringComparison.Ordinal);
        Assert.Contains("Text=\"同步状态：点击同步、重试、导出或刷新统计后，这里会显示元数据同步状态。\"", content, StringComparison.Ordinal);
        Assert.Contains("Text=\"下载状态：点击同步、重试、导出或刷新统计后，这里会显示同步下载状态。\"", content, StringComparison.Ordinal);
        Assert.Contains("Style=\"{StaticResource StatusInfoPanelStyle}\"", content, StringComparison.Ordinal);
        Assert.Contains("Style=\"{StaticResource StatusInfoTextStyle}\"", content, StringComparison.Ordinal);
        Assert.Contains("Style=\"{StaticResource StatusInfoMutedTextStyle}\"", content, StringComparison.Ordinal);

        var doc = XDocument.Parse(content);
        Assert.NotNull(doc.Root);
    }
}