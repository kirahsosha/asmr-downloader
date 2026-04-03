using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class SyncActionDebouncePolicyTests
{
    [Fact]
    public void Decide_ShouldReturnStart_WhenSyncIsIdle()
    {
        var now = DateTimeOffset.UtcNow;

        var actual = SyncActionDebouncePolicy.Decide(
            isSyncRunning: false,
            isStopRequested: false,
            lastStartedAt: now.AddSeconds(-10),
            now: now);

        Assert.Equal(SyncActionToggleDecision.Start, actual);
    }

    [Fact]
    public void Decide_ShouldIgnoreMetadataDoubleClick_WhenSecondClickIsWithinOneSecond()
    {
        var now = DateTimeOffset.UtcNow;

        var actual = SyncActionDebouncePolicy.Decide(
            isSyncRunning: true,
            isStopRequested: false,
            lastStartedAt: now.AddMilliseconds(-999),
            now: now);

        Assert.Equal(SyncActionToggleDecision.Ignore, actual);
    }

    [Fact]
    public void Decide_ShouldRequestMetadataStop_WhenSecondClickIsAfterOneSecond()
    {
        var now = DateTimeOffset.UtcNow;

        var actual = SyncActionDebouncePolicy.Decide(
            isSyncRunning: true,
            isStopRequested: false,
            lastStartedAt: now - SyncActionDebouncePolicy.StartDebounceWindow,
            now: now);

        Assert.Equal(SyncActionToggleDecision.RequestStop, actual);
    }

    [Fact]
    public void Decide_ShouldIgnoreDownloadDoubleClick_WhenSecondClickIsWithinOneSecond()
    {
        var now = DateTimeOffset.UtcNow;

        var actual = SyncActionDebouncePolicy.Decide(
            isSyncRunning: true,
            isStopRequested: false,
            lastStartedAt: now.AddMilliseconds(-250),
            now: now);

        Assert.Equal(SyncActionToggleDecision.Ignore, actual);
    }

    [Fact]
    public void Decide_ShouldIgnore_WhenStopAlreadyRequested()
    {
        var now = DateTimeOffset.UtcNow;

        var actual = SyncActionDebouncePolicy.Decide(
            isSyncRunning: true,
            isStopRequested: true,
            lastStartedAt: now.AddSeconds(-2),
            now: now);

        Assert.Equal(SyncActionToggleDecision.Ignore, actual);
    }
}