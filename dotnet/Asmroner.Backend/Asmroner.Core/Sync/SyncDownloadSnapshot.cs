namespace Asmroner.Core.Sync;

public sealed class SyncDownloadSnapshot
{
    public int PendingCount { get; init; }

    public int CompletedCount { get; init; }

    public int FailedCount { get; init; }

    public int RemainingMetadataCount { get; init; }

    public long CompletedSizeBytes { get; init; }

    public DateTime? LastUpdatedAt { get; init; }
}