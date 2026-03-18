using Asmroner.Core.Api;
using Asmroner.Core.Download;
using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class DownloadQueueCachePolicyTests
{
    [Fact]
    public void Reconcile_ShouldMergePrefetchedTitles_AndKeepActiveTitles()
    {
        var activeSourceIds = new[] { "RJ1002", "RJ1003" };
        var prefetched = new Dictionary<string, WorkInfoDto>(StringComparer.OrdinalIgnoreCase)
        {
            ["RJ1001"] = CreateWorkInfo("new-title"),
            ["RJ1003"] = CreateWorkInfo("queued-title"),
            ["RJ1004"] = CreateWorkInfo("   "),
        };
        var existingTitles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["RJ1001"] = "old-title",
            ["RJ1002"] = "active-title",
        };
        var existingStatusOverrides = new Dictionary<string, DownloadTaskStatus>(StringComparer.OrdinalIgnoreCase)
        {
            ["RJ1002"] = DownloadTaskStatus.Canceled,
            ["RJ1005"] = DownloadTaskStatus.Pending,
        };

        var result = DownloadQueueCachePolicy.Reconcile(
            activeSourceIds,
            prefetched,
            existingTitles,
            existingStatusOverrides);

        Assert.Equal(3, result.Titles.Count);
        Assert.Equal("new-title", result.Titles["RJ1001"]);
        Assert.Equal("active-title", result.Titles["RJ1002"]);
        Assert.Equal("queued-title", result.Titles["RJ1003"]);
        Assert.DoesNotContain("RJ1004", result.Titles.Keys);

        Assert.Single(result.StatusOverrides);
        Assert.Equal(DownloadTaskStatus.Pending, result.StatusOverrides["RJ1005"]);
    }

    [Fact]
    public void Reconcile_ShouldKeepCaseInsensitiveLookup()
    {
        var activeSourceIds = Array.Empty<string>();
        var prefetched = new Dictionary<string, WorkInfoDto>(StringComparer.OrdinalIgnoreCase)
        {
            ["rj2001"] = CreateWorkInfo("title-2001"),
        };

        var result = DownloadQueueCachePolicy.Reconcile(
            activeSourceIds,
            prefetched,
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
            new Dictionary<string, DownloadTaskStatus>(StringComparer.OrdinalIgnoreCase));

        Assert.Equal("title-2001", result.Titles["RJ2001"]);
    }

    private static WorkInfoDto CreateWorkInfo(string title)
    {
        return new WorkInfoDto
        {
            SourceId = "RJ0001",
            Title = title,
        };
    }
}
