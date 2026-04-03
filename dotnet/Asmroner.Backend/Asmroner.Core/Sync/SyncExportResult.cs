namespace Asmroner.Core.Sync;

public sealed class SyncExportResult
{
    public SyncExportStatus Status { get; init; }

    public int ExportedCount { get; init; }

    public string FilePath { get; init; } = string.Empty;

    public string Format { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;
}