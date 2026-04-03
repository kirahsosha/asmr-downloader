namespace Asmroner.Core.Sync;

public sealed class SyncDownloadRunResult
{
    public int ProcessedCount { get; init; }

    public int CompletedCount { get; init; }

    public int FailedCount { get; init; }

    public int RemainingMetadataCountAfter { get; init; }

    public long CompletedSizeBytesBefore { get; init; }

    public long CompletedSizeBytesAfter { get; init; }

    public long SizeLimitBytes { get; init; }

    public bool ReachedSizeLimit { get; init; }

    public string Message { get; init; } = string.Empty;
}