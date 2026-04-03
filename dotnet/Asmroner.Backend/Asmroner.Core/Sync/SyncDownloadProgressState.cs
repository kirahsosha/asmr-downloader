namespace Asmroner.Core.Sync;

public sealed class SyncDownloadProgressState
{
    public string Status { get; set; } = SyncProgressStatuses.Idle;

    public bool StopRequested { get; set; }

    public string LastProcessedSourceId { get; set; } = string.Empty;

    public int ProcessedCount { get; set; }

    public int CompletedCount { get; set; }

    public int FailedCount { get; set; }

    public int RemainingMetadataCountAfter { get; set; }

    public long CompletedSizeBytesBefore { get; set; }

    public long CompletedSizeBytesAfter { get; set; }

    public long SizeLimitBytes { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}