using Asmroner.Core.Search;
using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class SearchExportScopePolicyTests
{
    [Fact]
    public void Build_ShouldReturnAllResults_WhenScopeIsAll()
    {
        var allResults = new[]
        {
            new SearchWorkItem { SourceId = "RJ1001" },
            new SearchWorkItem { SourceId = "RJ1002" },
        };
        var selected = new[]
        {
            new SearchWorkItem { SourceId = "RJ1002" },
        };

        var plan = SearchExportScopePolicy.Build(allResults, selected, SearchExportScope.All);

        Assert.Equal(allResults, plan.Items);
        Assert.False(plan.FallbackToAll);
    }

    [Fact]
    public void Build_ShouldReturnSelectedResults_WhenScopeIsSelectedAndSelectionExists()
    {
        var allResults = new[]
        {
            new SearchWorkItem { SourceId = "RJ1001" },
            new SearchWorkItem { SourceId = "RJ1002" },
        };
        var selected = new[]
        {
            new SearchWorkItem { SourceId = "RJ1002" },
        };

        var plan = SearchExportScopePolicy.Build(allResults, selected, SearchExportScope.Selected);

        Assert.Equal(selected, plan.Items);
        Assert.False(plan.FallbackToAll);
    }

    [Fact]
    public void Build_ShouldFallbackToAll_WhenScopeIsSelectedAndSelectionEmpty()
    {
        var allResults = new[]
        {
            new SearchWorkItem { SourceId = "RJ1001" },
        };

        var plan = SearchExportScopePolicy.Build(allResults, Array.Empty<SearchWorkItem>(), SearchExportScope.Selected);

        Assert.Equal(allResults, plan.Items);
        Assert.True(plan.FallbackToAll);
    }

    [Fact]
    public void Build_ShouldReturnEmpty_WhenNoResultsAndSelectionEmpty()
    {
        var plan = SearchExportScopePolicy.Build(
            Array.Empty<SearchWorkItem>(),
            Array.Empty<SearchWorkItem>(),
            SearchExportScope.Selected);

        Assert.Empty(plan.Items);
        Assert.True(plan.FallbackToAll);
    }
}
