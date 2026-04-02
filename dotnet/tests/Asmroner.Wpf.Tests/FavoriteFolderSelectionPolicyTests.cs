using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class FavoriteFolderSelectionPolicyTests
{
    [Fact]
    public void NormalizeFolderTitle_ShouldTrimWhitespace()
    {
        Assert.Equal("常用作品", FavoriteFolderSelectionPolicy.NormalizeFolderTitle("  常用作品  "));
    }

    [Fact]
    public void BuildFolderTitles_ShouldDistinctAndSortCaseInsensitive()
    {
        var titles = FavoriteFolderSelectionPolicy.BuildFolderTitles(new[]
        {
            " Beta ",
            "alpha",
            "ALPHA",
            "",
        });

        Assert.Equal(new[] { "alpha", "Beta" }, titles);
    }

    [Fact]
    public void CanConfirm_ShouldAllowNewTitle_WhenCustomInputEnabled()
    {
        var canConfirm = FavoriteFolderSelectionPolicy.CanConfirm(" 新收藏夹 ", allowCustomInput: true, Array.Empty<string>());

        Assert.True(canConfirm);
    }

    [Fact]
    public void CanConfirm_ShouldRejectUnknownTitle_WhenCustomInputDisabled()
    {
        var canConfirm = FavoriteFolderSelectionPolicy.CanConfirm("新收藏夹", allowCustomInput: false, new[] { "已存在收藏夹" });

        Assert.False(canConfirm);
    }
}