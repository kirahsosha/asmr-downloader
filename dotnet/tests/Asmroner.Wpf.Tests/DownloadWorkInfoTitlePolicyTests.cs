using Asmroner.Core.Api;
using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class DownloadWorkInfoTitlePolicyTests
{
    [Fact]
    public void BuildNonEmptyTitleMap_ShouldFilterWhitespaceTitles()
    {
        var workInfos = new Dictionary<string, WorkInfoDto>(StringComparer.OrdinalIgnoreCase)
        {
            ["RJ3001"] = CreateWorkInfo("title-1"),
            ["RJ3002"] = CreateWorkInfo("   "),
            ["RJ3003"] = CreateWorkInfo(string.Empty),
        };

        var result = DownloadWorkInfoTitlePolicy.BuildNonEmptyTitleMap(workInfos);

        Assert.Single(result);
        Assert.Equal("title-1", result["RJ3001"]);
        Assert.DoesNotContain("RJ3002", result.Keys);
        Assert.DoesNotContain("RJ3003", result.Keys);
    }

    [Fact]
    public void BuildNonEmptyTitleMap_ShouldUseCaseInsensitiveKeys_AndLastWriteWins()
    {
        var workInfos = new Dictionary<string, WorkInfoDto>(StringComparer.OrdinalIgnoreCase)
        {
            ["rj3010"] = CreateWorkInfo("old"),
            ["RJ3010"] = CreateWorkInfo("new"),
        };

        var result = DownloadWorkInfoTitlePolicy.BuildNonEmptyTitleMap(workInfos);

        Assert.Single(result);
        Assert.Equal("new", result["RJ3010"]);
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
