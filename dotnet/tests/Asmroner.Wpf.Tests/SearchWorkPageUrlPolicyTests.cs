using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class SearchWorkPageUrlPolicyTests
{
    [Fact]
    public void TryBuild_ShouldReplacePlaceholder_WithNormalizedSourceId()
    {
        var ok = SearchWorkPageUrlPolicy.TryBuild(
            "https://www.asmr.one/work/{RJID}",
            "https://api.asmr.one/api/work/rj778899",
            778899,
            out var url,
            out var errorMessage);

        Assert.True(ok);
        Assert.Equal("https://www.asmr.one/work/RJ778899", url);
        Assert.Equal(string.Empty, errorMessage);
    }

    [Fact]
    public void TryBuild_ShouldAppendSourceId_WhenPlaceholderMissing()
    {
        var ok = SearchWorkPageUrlPolicy.TryBuild(
            "https://www.asmr.one/work",
            "rj1001",
            1001,
            out var url,
            out var errorMessage);

        Assert.True(ok);
        Assert.Equal("https://www.asmr.one/work/RJ1001", url);
        Assert.Equal(string.Empty, errorMessage);
    }

    [Fact]
    public void TryBuild_ShouldFallbackToDefaultTemplate_WhenTemplateEmpty()
    {
        var ok = SearchWorkPageUrlPolicy.TryBuild(
            "   ",
            "1234",
            1234,
            out var url,
            out var errorMessage);

        Assert.True(ok);
        Assert.Equal("https://www.asmr.one/work/RJ1234", url);
        Assert.Equal(string.Empty, errorMessage);
    }

    [Fact]
    public void TryBuild_ShouldFail_WhenSourceIdInvalid()
    {
        var ok = SearchWorkPageUrlPolicy.TryBuild(
            "https://www.asmr.one/work/{RJID}",
            " ",
            0,
            out var url,
            out var errorMessage);

        Assert.False(ok);
        Assert.Equal(string.Empty, url);
        Assert.Contains("作品编号", errorMessage, StringComparison.Ordinal);
    }

    [Fact]
    public void TryBuild_ShouldFail_WhenTemplateInvalid()
    {
        var ok = SearchWorkPageUrlPolicy.TryBuild(
            "not-a-url-{RJID}",
            "RJ1001",
            1001,
            out var url,
            out var errorMessage);

        Assert.False(ok);
        Assert.Equal(string.Empty, url);
        Assert.Contains("workPageUrlTemplate", errorMessage, StringComparison.Ordinal);
    }

    [Fact]
    public void TryBuild_ShouldPreferWorkId_ForNonRjSourceId()
    {
        var ok = SearchWorkPageUrlPolicy.TryBuild(
            "https://www.asmr.one/work/{RJID}",
            "BJ02370869",
            100000062,
            out var url,
            out var errorMessage);

        Assert.True(ok);
        Assert.Equal("https://www.asmr.one/work/100000062", url);
        Assert.Equal(string.Empty, errorMessage);
    }
}
