using System.Text.Json.Serialization;

namespace Asmroner.Core.Api;

public sealed class WorkInfoDto
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Release { get; init; } = string.Empty;

    [JsonPropertyName("has_subtitle")]
    public bool HasSubtitle { get; init; }

    [JsonPropertyName("source_id")]
    public string SourceId { get; init; } = string.Empty;

    [JsonPropertyName("mainCoverUrl")]
    public string MainCoverUrl { get; init; } = string.Empty;

    [JsonPropertyName("work_attributes")]
    public string WorkAttributes { get; init; } = string.Empty;

    [JsonPropertyName("language_editions")]
    public IReadOnlyList<WorkLanguageEditionDto> LanguageEditions { get; init; } = Array.Empty<WorkLanguageEditionDto>();

    [JsonPropertyName("other_language_editions_in_db")]
    public IReadOnlyList<WorkOtherLanguageEditionDto> OtherLanguageEditionsInDb { get; init; } = Array.Empty<WorkOtherLanguageEditionDto>();

    [JsonPropertyName("translation_info")]
    public WorkTranslationInfoDto TranslationInfo { get; init; } = new();
}

public sealed class WorkLanguageEditionDto
{
    public string Lang { get; init; } = string.Empty;

    public string Label { get; init; } = string.Empty;

    [JsonPropertyName("workno")]
    public string WorkNo { get; init; } = string.Empty;

    [JsonPropertyName("display_order")]
    public int DisplayOrder { get; init; }
}

public sealed class WorkOtherLanguageEditionDto
{
    public int Id { get; init; }

    public string Lang { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("source_id")]
    public string SourceId { get; init; } = string.Empty;

    [JsonPropertyName("is_original")]
    public bool IsOriginal { get; init; }

    [JsonPropertyName("source_type")]
    public string SourceType { get; init; } = string.Empty;
}

public sealed class WorkTranslationInfoDto
{
    public string? Lang { get; init; }

    [JsonPropertyName("is_original")]
    public bool IsOriginal { get; init; }
}