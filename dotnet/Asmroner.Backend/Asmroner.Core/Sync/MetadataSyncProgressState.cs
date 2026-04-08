namespace Asmroner.Core.Sync;

public sealed class MetadataSyncProgressState
{
    public string Status { get; set; } = SyncProgressStatuses.Idle;

    public bool StopRequested { get; set; }

    public int NextPage { get; set; } = 1;

    public int ProcessedPageCount { get; set; }

    public int TotalPageCount { get; set; }

    public int RemoteTotalCount { get; set; }

    public int RemoteSubtitleCount { get; set; }

    public int LocalTotalCount { get; set; }

    public int LocalSubtitleCount { get; set; }

    public int InsertedCount { get; set; }

    public int ProcessedWorkCount { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}