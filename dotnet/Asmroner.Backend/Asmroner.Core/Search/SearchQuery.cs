namespace Asmroner.Core.Search;

public sealed class SearchQuery
{
    public string RawQuery { get; init; } = string.Empty;

    public IReadOnlyList<string> PlainTexts { get; init; } = Array.Empty<string>();

    public SearchFilter Filter { get; init; } = new();

    public SearchPageOptions PageOptions { get; init; } = SearchPageOptions.CreateDefault();
}

public sealed class SearchFilter
{
    public string Tag { get; init; } = string.Empty;

    public string Circle { get; init; } = string.Empty;

    public string Va { get; init; } = string.Empty;

    public string Duration { get; init; } = string.Empty;

    public string Rate { get; init; } = string.Empty;

    public string Price { get; init; } = string.Empty;

    public string Sell { get; init; } = string.Empty;

    public string Age { get; init; } = string.Empty;

    public string Lang { get; init; } = string.Empty;
}

public sealed class SearchPageOptions
{
    public string Order { get; init; } = "release";

    public string Sort { get; init; } = "desc";

    public string Subtitle { get; init; } = "0";

    public bool IncludeTranslationWorks { get; init; } = true;

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;

    public static SearchPageOptions CreateDefault()
    {
        return new SearchPageOptions
        {
            Order = "release",
            Sort = "desc",
            Subtitle = "0",
            IncludeTranslationWorks = true,
            Page = 1,
            PageSize = 20,
        };
    }
}

public sealed class SearchWorkItem
{
    public string SourceId { get; init; } = string.Empty;

    public string Release { get; init; } = string.Empty;

    public bool HasSubtitle { get; init; }

    public string Tags { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;
}

public sealed class SearchExecutionResult
{
    public IReadOnlyList<SearchWorkItem> Items { get; init; } = Array.Empty<SearchWorkItem>();

    public int TotalCount { get; init; }

    public int ReturnedCount { get; init; }

    public string ResolvedQuery { get; init; } = string.Empty;
}
