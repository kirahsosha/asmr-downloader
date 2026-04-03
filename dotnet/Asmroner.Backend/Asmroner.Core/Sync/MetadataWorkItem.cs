namespace Asmroner.Core.Sync;

public sealed class MetadataWorkItem
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public int CircleId { get; init; }

    public string CircleName { get; init; } = string.Empty;

    public bool Nsfw { get; init; }

    public string Release { get; init; } = string.Empty;

    public int DownloadCount { get; init; }

    public int Price { get; init; }

    public int ReviewCount { get; init; }

    public int RateCount { get; init; }

    public double RateAverage { get; init; }

    public bool HasSubtitle { get; init; }

    public string CreateDate { get; init; } = string.Empty;

    public string Vas { get; init; } = string.Empty;

    public string Tags { get; init; } = string.Empty;

    public int Duration { get; init; }

    public string SourceType { get; init; } = string.Empty;

    public string SourceId { get; init; } = string.Empty;

    public DateTime UpdatedAt { get; init; }
}