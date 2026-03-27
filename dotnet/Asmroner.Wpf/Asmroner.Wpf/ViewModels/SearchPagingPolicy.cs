namespace Asmroner.Wpf.ViewModels;

public readonly record struct SearchPageWindow<T>(
    int EffectivePage,
    int TotalPages,
    IReadOnlyList<T> Items);

public static class SearchPagingPolicy
{
    public static SearchPageWindow<T> SlicePage<T>(
        IReadOnlyList<T>? source,
        int requestedPage,
        int pageSize)
    {
        var normalizedSource = source ?? Array.Empty<T>();
        var normalizedPageSize = Math.Max(1, pageSize);
        var totalPages = Math.Max(1, (int)Math.Ceiling(normalizedSource.Count / (double)normalizedPageSize));
        var effectivePage = Math.Clamp(Math.Max(1, requestedPage), 1, totalPages);
        var skip = (effectivePage - 1) * normalizedPageSize;

        var items = normalizedSource
            .Skip(skip)
            .Take(normalizedPageSize)
            .ToArray();

        return new SearchPageWindow<T>(effectivePage, totalPages, items);
    }

    public static bool CanJump(int totalPages)
    {
        return totalPages > 1;
    }
}
