using System.Text.Json.Serialization;

namespace Asmroner.Core.Api;

public sealed class SearchResultDto
{
    public IReadOnlyList<SearchWorkDto> Works { get; init; } = Array.Empty<SearchWorkDto>();

    public SearchPaginationDto Pagination { get; init; } = new();
}

public sealed class SearchWorkDto
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("source_id")]
    public string SourceId { get; init; } = string.Empty;

    public string Release { get; init; } = string.Empty;

    [JsonPropertyName("dl_count")]
    public int DownloadCount { get; init; }

    [JsonPropertyName("rate_average_2dp")]
    public double RateAverage { get; init; }

    [JsonPropertyName("has_subtitle")]
    public bool HasSubtitle { get; init; }
}

public sealed class SearchPaginationDto
{
    [JsonPropertyName("currentPage")]
    public int CurrentPage { get; init; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; init; }

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; init; }
}