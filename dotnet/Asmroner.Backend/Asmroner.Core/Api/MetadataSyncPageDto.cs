using System.Text.Json.Serialization;
using Asmroner.Core.Sync;

namespace Asmroner.Core.Api;

public sealed class MetadataSyncPageDto
{
    public IReadOnlyList<MetadataSyncWorkDto> Works { get; init; } = Array.Empty<MetadataSyncWorkDto>();

    public MetadataSyncPaginationDto Pagination { get; init; } = new();
}

public sealed class MetadataSyncWorkDto
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("circle_id")]
    public int CircleId { get; init; }

    public string Name { get; init; } = string.Empty;

    public bool Nsfw { get; init; }

    public string Release { get; init; } = string.Empty;

    [JsonPropertyName("dl_count")]
    public int DownloadCount { get; init; }

    public int Price { get; init; }

    [JsonPropertyName("review_count")]
    public int ReviewCount { get; init; }

    [JsonPropertyName("rate_count")]
    public int RateCount { get; init; }

    [JsonPropertyName("rate_average_2dp")]
    public double RateAverage { get; init; }

    [JsonPropertyName("has_subtitle")]
    public bool HasSubtitle { get; init; }

    [JsonPropertyName("create_date")]
    public string CreateDate { get; init; } = string.Empty;

    public IReadOnlyList<MetadataSyncVaDto> Vas { get; init; } = Array.Empty<MetadataSyncVaDto>();

    public IReadOnlyList<TagDto> Tags { get; init; } = Array.Empty<TagDto>();

    public int Duration { get; init; }

    [JsonPropertyName("source_type")]
    public string SourceType { get; init; } = string.Empty;

    [JsonPropertyName("source_id")]
    public string SourceId { get; init; } = string.Empty;

    public MetadataWorkItem ToMetadataWorkItem(DateTime updatedAt)
    {
        return new MetadataWorkItem
        {
            Id = Id,
            Title = Title.Trim(),
            CircleId = CircleId,
            CircleName = Name.Trim(),
            Nsfw = Nsfw,
            Release = Release.Trim(),
            DownloadCount = DownloadCount,
            Price = Price,
            ReviewCount = ReviewCount,
            RateCount = RateCount,
            RateAverage = RateAverage,
            HasSubtitle = HasSubtitle,
            CreateDate = CreateDate.Trim(),
            Vas = string.Join(",", Vas
                .Select(static va => va.Name.Trim())
                .Where(static name => !string.IsNullOrWhiteSpace(name))),
            Tags = string.Join(",", Tags
                .OrderBy(static tag => tag.Id)
                .Select(static tag => tag.Name.Trim())
                .Where(static name => !string.IsNullOrWhiteSpace(name))),
            Duration = Duration,
            SourceType = SourceType.Trim(),
            SourceId = SourceId.Trim(),
            UpdatedAt = updatedAt,
        };
    }
}

public sealed class MetadataSyncVaDto
{
    public string Id { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;
}

public sealed class MetadataSyncPaginationDto
{
    [JsonPropertyName("currentPage")]
    public int CurrentPage { get; init; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; init; }

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; init; }
}