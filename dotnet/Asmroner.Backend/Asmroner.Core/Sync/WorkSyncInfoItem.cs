namespace Asmroner.Core.Sync;

public sealed class WorkSyncInfoItem
{
    public int Id { get; init; }

    public int MetadataWorkId { get; init; }

    public string SourceId { get; init; } = string.Empty;

    public bool HasSubtitle { get; init; }

    public long DirSize { get; init; }

    public string Status { get; init; } = "PENDING";

    public string FilePath { get; init; } = string.Empty;

    public string FailReason { get; init; } = string.Empty;

    public int RetryCount { get; init; }

    public DateTime UpdatedAt { get; init; }

    public DateTime? FailedAt { get; init; }
}