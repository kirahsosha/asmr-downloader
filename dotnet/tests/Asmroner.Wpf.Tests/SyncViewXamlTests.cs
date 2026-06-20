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
        Assert.Contains("Text=\"Sync\"", content, StringComparison.Ordinal);
        Assert.Contains("Text=\"元数据同步、下载同步、失败重试与统计报表。\"", content, StringComparison.Ordinal);
        Assert.Contains("Background=\"{StaticResource ShellBackgroundBrush}\"", content, StringComparison.Ordinal);
        Assert.Contains("Style=\"{StaticResource ActionButtonStyle}\"", content, StringComparison.Ordinal);
        Assert.Contains("Style=\"{StaticResource SecondaryActionButtonStyle}\"", content, StringComparison.Ordinal);
        Assert.Contains("Style=\"{StaticResource StatsCardBorderStyle}\"", content, StringComparison.Ordinal);
        Assert.DoesNotContain("Text=\"同步能力（阶段 5 增强）\"", content, StringComparison.Ordinal);
        Assert.DoesNotContain("Background=\"#F3F4F6\"", content, StringComparison.Ordinal);
        Assert.Contains("Text=\"同步状态：点击同步、重试、导出或刷新统计后，这里会显示元数据同步状态。\"", content, StringComparison.Ordinal);
        Assert.Contains("Text=\"下载状态：点击同步、重试、导出或刷新统计后，这里会显示同步下载状态。\"", content, StringComparison.Ordinal);
        Assert.Contains("Style=\"{StaticResource StatusInfoPanelStyle}\"", content, StringComparison.Ordinal);
        Assert.Contains("Style=\"{StaticResource StatusInfoTextStyle}\"", content, StringComparison.Ordinal);
        Assert.Contains("Style=\"{StaticResource StatusInfoMutedTextStyle}\"", content, StringComparison.Ordinal);
        Assert.Contains("Style=\"{StaticResource MutedLabelStyle}\"", content, StringComparison.Ordinal);

        var doc = XDocument.Parse(content);
        Assert.NotNull(doc.Root);
    }

    [Fact]
    public void SyncViewXaml_ShouldContainFilterCheckboxes()
    {
        var xamlPath = XamlTestPathLocator.Locate("SyncView.xaml", "dotnet", "Asmroner.Wpf", "Asmroner.Wpf", "Views");
        var content = File.ReadAllText(xamlPath);

        Assert.Contains("x:Name=\"UseSearchFilterCheckBox\"", content, StringComparison.Ordinal);
        Assert.Contains("x:Name=\"UseDownloadFilterCheckBox\"", content, StringComparison.Ordinal);
        Assert.Contains("Content=\"同步下载使用Search页面高级筛选条件\"", content, StringComparison.Ordinal);
        Assert.Contains("Content=\"同步下载使用Download页面文件筛选条件\"", content, StringComparison.Ordinal);
        Assert.Contains("Checked=\"OnSyncFilterChanged\"", content, StringComparison.Ordinal);
        Assert.Contains("Unchecked=\"OnSyncFilterChanged\"", content, StringComparison.Ordinal);
    }
}