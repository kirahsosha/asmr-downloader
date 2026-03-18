using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class SearchFilterValuePolicyTests
{
    [Fact]
    public void MergeDistinct_ShouldAppendValue_WhenMissing()
    {
        var merged = SearchFilterValuePolicy.MergeDistinct("tag1", "tag2");

        Assert.Equal("tag1,tag2", merged);
    }

    [Fact]
    public void MergeDistinct_ShouldNotDuplicateExistingValue()
    {
        var merged = SearchFilterValuePolicy.MergeDistinct("tag1;tag2", "tag2");

        Assert.Equal("tag1;tag2", merged);
    }
}
