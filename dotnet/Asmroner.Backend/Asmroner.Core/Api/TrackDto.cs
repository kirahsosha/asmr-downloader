using System.Text.Json.Serialization;

namespace Asmroner.Core.Api;

public sealed class TrackDto
{
    public string Type { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public IReadOnlyList<TrackDto> Children { get; init; } = Array.Empty<TrackDto>();

    [JsonPropertyName("mediaStreamUrl")]
    public string MediaStreamUrl { get; init; } = string.Empty;

    [JsonPropertyName("mediaDownloadUrl")]
    public string MediaDownloadUrl { get; init; } = string.Empty;
}