namespace Asmroner.Wpf.ViewModels;

public enum SyncActionToggleDecision
{
    Ignore,
    Start,
    RequestStop,
}

public static class SyncActionDebouncePolicy
{
    public static readonly TimeSpan StartDebounceWindow = TimeSpan.FromSeconds(1);

    public static SyncActionToggleDecision Decide(
        bool isSyncRunning,
        bool isStopRequested,
        DateTimeOffset? lastStartedAt,
        DateTimeOffset now)
    {
        if (!isSyncRunning)
        {
            return SyncActionToggleDecision.Start;
        }

        if (isStopRequested)
        {
            return SyncActionToggleDecision.Ignore;
        }

        if (lastStartedAt.HasValue && now - lastStartedAt.Value < StartDebounceWindow)
        {
            return SyncActionToggleDecision.Ignore;
        }

        return SyncActionToggleDecision.RequestStop;
    }
}