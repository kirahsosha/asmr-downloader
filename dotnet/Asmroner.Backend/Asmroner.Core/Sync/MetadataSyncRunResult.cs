namespace Asmroner.Core.Sync;

public sealed class MetadataSyncRunResult
{
    public int RemoteTotalCount { get; init; }

    public int RemoteSubtitleCount { get; init; }

    public int LocalTotalCountBefore { get; init; }

    public int LocalSubtitleCountBefore { get; init; }

    public int LocalTotalCountAfter { get; init; }

    public int LocalSubtitleCountAfter { get; init; }

    public int InsertedCount { get; init; }

    public int ProcessedPageCount { get; init; }

    public int TotalPageCount { get; init; }

    public int NextPage { get; init; } = 1;

    public string Message { get; init; } = string.Empty;

    public bool IsUpToDate { get; init; }

    public bool WasStopped { get; init; }

    public bool ResumedFromProgress { get; init; }
}