namespace Asmroner.Core.Download;

public sealed class DownloadStartOptions
{
    public string? TargetRoot { get; init; }

    public IReadOnlyList<string> LookupRoots { get; init; } = Array.Empty<string>();

    public int? WorkId { get; init; }
}