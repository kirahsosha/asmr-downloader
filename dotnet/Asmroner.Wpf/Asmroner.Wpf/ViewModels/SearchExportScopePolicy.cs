using Asmroner.Core.Search;

namespace Asmroner.Wpf.ViewModels;

public enum SearchExportScope
{
    All,
    Selected,
}

public sealed record SearchExportScopePlan(
    IReadOnlyList<SearchWorkItem> Items,
    bool FallbackToAll);

public static class SearchExportScopePolicy
{
    public static SearchExportScopePlan Build(
        IReadOnlyList<SearchWorkItem>? allResults,
        IReadOnlyList<SearchWorkItem>? selectedResults,
        SearchExportScope scope)
    {
        var normalizedAll = allResults ?? Array.Empty<SearchWorkItem>();
        var normalizedSelected = selectedResults ?? Array.Empty<SearchWorkItem>();

        if (scope == SearchExportScope.Selected && normalizedSelected.Count > 0)
        {
            return new SearchExportScopePlan(normalizedSelected, false);
        }

        return new SearchExportScopePlan(normalizedAll, scope == SearchExportScope.Selected);
    }
}
