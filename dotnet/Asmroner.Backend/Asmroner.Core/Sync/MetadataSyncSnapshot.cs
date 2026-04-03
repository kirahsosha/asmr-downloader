namespace Asmroner.Core.Sync;

public sealed class MetadataSyncSnapshot
{
    public int LocalTotalCount { get; init; }

    public int LocalSubtitleCount { get; init; }

    public DateTime? LastUpdatedAt { get; init; }
}