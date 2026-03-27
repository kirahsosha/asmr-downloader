using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class SearchPagingPolicyTests
{
    [Fact]
    public void SlicePage_ShouldClampPage_AndReturnExpectedItems()
    {
        var source = new[] { 1, 2, 3, 4, 5 };

        var window = SearchPagingPolicy.SlicePage(source, requestedPage: 4, pageSize: 2);

        Assert.Equal(3, window.EffectivePage);
        Assert.Equal(3, window.TotalPages);
        Assert.Equal(new[] { 5 }, window.Items);
    }

    [Fact]
    public void CanJump_ShouldReturnFalse_WhenOnlySinglePage()
    {
        Assert.False(SearchPagingPolicy.CanJump(1));
    }

    [Fact]
    public void CanJump_ShouldReturnTrue_WhenMultiplePages()
    {
        Assert.True(SearchPagingPolicy.CanJump(2));
    }
}
