namespace Asmroner.Core.Sync;

public sealed class SyncReportSnapshot
{
    public int MetadataTotalCount { get; init; }

    public int MetadataSubtitleCount { get; init; }

    public int MetadataWithoutSubtitleCount { get; init; }

    public int CompletedCount { get; init; }

    public int CompletedSubtitleCount { get; init; }

    public int CompletedWithoutSubtitleCount { get; init; }

    public int FailedCount { get; init; }

    public int PendingCount { get; init; }

    public int RemainingMetadataCount { get; init; }

    public long CompletedSizeBytes { get; init; }

    public double OverallProgressPercent { get; init; }

    public double SubtitleProgressPercent { get; init; }

    public double WithoutSubtitleProgressPercent { get; init; }

    public DateTime? MetadataUpdatedAt { get; init; }

    public DateTime? DownloadUpdatedAt { get; init; }
}