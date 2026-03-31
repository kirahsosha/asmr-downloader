using System.Diagnostics;
using Asmroner.Core.Api;
using Asmroner.Core.Interfaces;
using Asmroner.Wpf.Services;

namespace Asmroner.Wpf.Tests;

public class StartupEndpointWarmupServiceTests
{
    [Fact]
    public async Task StartInBackgroundAsync_ShouldReturnImmediately_WhenDiscoveryIsSlow()
    {
        var endpointService = new SlowApiEndpointUrlService();
        var sut = new StartupEndpointWarmupService(
            endpointService,
            TimeSpan.FromSeconds(5));

        var stopwatch = Stopwatch.StartNew();
        var warmupTask = sut.StartInBackgroundAsync();
        stopwatch.Stop();

        await endpointService.Started.Task.WaitAsync(TimeSpan.FromSeconds(1));

        Assert.True(stopwatch.ElapsedMilliseconds < 300, $"Warmup call should be non-blocking, actual={stopwatch.ElapsedMilliseconds}ms");
        Assert.False(warmupTask.IsCompleted);

        endpointService.Complete();
        await warmupTask;
        Assert.Equal(1, endpointService.DiscoverCallCount);
    }

    [Fact]
    public async Task StartInBackgroundAsync_ShouldInvokeDiscoverAndPersistAsync()
    {
        var endpointService = new RecordingApiEndpointUrlService();
        var sut = new StartupEndpointWarmupService(
            endpointService,
            TimeSpan.FromSeconds(1));

        await sut.StartInBackgroundAsync();

        Assert.Equal(1, endpointService.DiscoverCallCount);
    }

    [Fact]
    public async Task StartInBackgroundAsync_ShouldReuseInFlightWarmupTask_AndDiscoverOnce()
    {
        var endpointService = new SlowApiEndpointUrlService();
        var sut = new StartupEndpointWarmupService(
            endpointService,
            TimeSpan.FromSeconds(5));

        var firstWarmupTask = sut.StartInBackgroundAsync();
        await endpointService.Started.Task.WaitAsync(TimeSpan.FromSeconds(1));

        var secondWarmupTask = sut.StartInBackgroundAsync();

        Assert.Same(firstWarmupTask, secondWarmupTask);

        endpointService.Complete();
        await Task.WhenAll(firstWarmupTask, secondWarmupTask);

        Assert.Equal(1, endpointService.DiscoverCallCount);
    }

    [Fact]
    public async Task StartInBackgroundAsync_ShouldNotThrow_WhenDiscoveryFails()
    {
        var endpointService = new ThrowingApiEndpointUrlService();
        var sut = new StartupEndpointWarmupService(
            endpointService,
            TimeSpan.FromSeconds(1));

        var exception = await Record.ExceptionAsync(() => sut.StartInBackgroundAsync());

        Assert.Null(exception);
        Assert.Equal(1, endpointService.DiscoverCallCount);
    }

    [Fact]
    public async Task StartInBackgroundAsync_ShouldRespectTimeout_AndContinue()
    {
        var endpointService = new TimeoutAwareApiEndpointUrlService();
        var sut = new StartupEndpointWarmupService(
            endpointService,
            TimeSpan.FromMilliseconds(50));

        var stopwatch = Stopwatch.StartNew();
        await sut.StartInBackgroundAsync();
        stopwatch.Stop();

        Assert.Equal(1, endpointService.DiscoverCallCount);
        Assert.True(endpointService.CancellationObserved);
        Assert.True(stopwatch.ElapsedMilliseconds < 2000, $"Warmup timeout should complete quickly, actual={stopwatch.ElapsedMilliseconds}ms");
    }

    private sealed class RecordingApiEndpointUrlService : IApiEndpointUrlService
    {
        public int DiscoverCallCount { get; private set; }

        public Task<string> GetCurrentBaseUrlAsync(CancellationToken cancellationToken = default)
            => Task.FromResult("https://api.example.com");

        public Task<EndpointDiscoveryResult> DiscoverAndPersistAsync(CancellationToken cancellationToken = default)
        {
            DiscoverCallCount++;
            return Task.FromResult(new EndpointDiscoveryResult
            {
                BaseUrl = "https://api.example.com",
                LatencyMs = 10,
                Candidates = new[] { "https://api.example.com" },
            });
        }
    }

    private sealed class SlowApiEndpointUrlService : IApiEndpointUrlService
    {
        private readonly TaskCompletionSource<bool> _completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource<bool> Started { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public int DiscoverCallCount { get; private set; }

        public Task<string> GetCurrentBaseUrlAsync(CancellationToken cancellationToken = default)
            => Task.FromResult("https://api.example.com");

        public async Task<EndpointDiscoveryResult> DiscoverAndPersistAsync(CancellationToken cancellationToken = default)
        {
            DiscoverCallCount++;
            Started.TrySetResult(true);

            using var registration = cancellationToken.Register(() => _completion.TrySetCanceled(cancellationToken));
            await _completion.Task;

            return new EndpointDiscoveryResult
            {
                BaseUrl = "https://api.example.com",
                LatencyMs = 12,
                Candidates = new[] { "https://api.example.com" },
            };
        }

        public void Complete() => _completion.TrySetResult(true);
    }

    private sealed class ThrowingApiEndpointUrlService : IApiEndpointUrlService
    {
        public int DiscoverCallCount { get; private set; }

        public Task<string> GetCurrentBaseUrlAsync(CancellationToken cancellationToken = default)
            => Task.FromResult("https://api.example.com");

        public Task<EndpointDiscoveryResult> DiscoverAndPersistAsync(CancellationToken cancellationToken = default)
        {
            DiscoverCallCount++;
            throw new InvalidOperationException("forced discovery failure");
        }
    }

    private sealed class TimeoutAwareApiEndpointUrlService : IApiEndpointUrlService
    {
        public int DiscoverCallCount { get; private set; }

        public bool CancellationObserved { get; private set; }

        public Task<string> GetCurrentBaseUrlAsync(CancellationToken cancellationToken = default)
            => Task.FromResult("https://api.example.com");

        public async Task<EndpointDiscoveryResult> DiscoverAndPersistAsync(CancellationToken cancellationToken = default)
        {
            DiscoverCallCount++;
            try
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                CancellationObserved = true;
                throw;
            }

            return new EndpointDiscoveryResult
            {
                BaseUrl = "https://api.example.com",
                LatencyMs = 0,
                Candidates = new[] { "https://api.example.com" },
            };
        }
    }
}