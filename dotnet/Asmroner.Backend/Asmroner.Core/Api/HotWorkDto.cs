using System.Text.Json.Serialization;

namespace Asmroner.Core.Api;

public sealed class HotWorkDto
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("source_id")]
    public string SourceId { get; init; } = string.Empty;

    [JsonPropertyName("dl_count")]
    public int DownloadCount { get; init; }
}