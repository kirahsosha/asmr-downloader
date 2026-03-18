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
}