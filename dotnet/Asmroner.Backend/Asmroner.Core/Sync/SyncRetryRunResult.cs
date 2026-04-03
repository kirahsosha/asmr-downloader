namespace Asmroner.Core.Sync;

public sealed class SyncRetryRunResult
{
    public int RetriedCount { get; init; }

    public int RecoveredCount { get; init; }

    public int FailedAgainCount { get; init; }

    public int FailedCountBefore { get; init; }

    public int FailedCountAfter { get; init; }

    public int CompletedCountAfter { get; init; }

    public string Message { get; init; } = string.Empty;
}