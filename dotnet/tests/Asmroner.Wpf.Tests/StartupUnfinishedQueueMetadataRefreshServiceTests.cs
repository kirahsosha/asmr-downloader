using System.Diagnostics;
using Asmroner.Core.Api;
using Asmroner.Core.Configuration;
using Asmroner.Core.Download;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Sync;
using Asmroner.Wpf.Services;

namespace Asmroner.Wpf.Tests;

public class StartupUnfinishedQueueMetadataRefreshServiceTests
{
    [Fact]
    public async Task StartInBackgroundAsync_ShouldReturnImmediately_WhenRefreshIsSlow()
    {
        var uiStateStore = new StubUiStateStore(new[] { "RJ1001" });
        var downloadService = new RecordingDownloadService();
        var apiClient = new SlowAsmrApiClient();
        var warmupService = CreateWarmupService();
        var sut = new StartupUnfinishedQueueMetadataRefreshService(
            uiStateStore,
            downloadService,
            apiClient,
            warmupService,
            TimeSpan.FromSeconds(5));

        var stopwatch = Stopwatch.StartNew();
        var refreshTask = sut.StartInBackgroundAsync();
        stopwatch.Stop();

        await apiClient.Started.Task.WaitAsync(TimeSpan.FromSeconds(1));

        Assert.True(stopwatch.ElapsedMilliseconds < 300, $"Refresh call should be non-blocking, actual={stopwatch.ElapsedMilliseconds}ms");
        Assert.False(refreshTask.IsCompleted);

        apiClient.Complete("RJ1001", "示例作品");

        var result = await refreshTask;
        Assert.Equal(1, result.UpdatedWorkInfos.Count);
        Assert.Empty(result.Failures);
        Assert.Equal("示例作品", downloadService.GetPrefetchedWorkInfoSnapshot()["RJ1001"].Title);
    }

    [Fact]
    public async Task RefreshAsync_ShouldReturnZero_WhenQueueEmpty()
    {
        var uiStateStore = new StubUiStateStore(Array.Empty<string>());
        var downloadService = new RecordingDownloadService();
        var apiClient = new RecordingAsmrApiClient();
        var warmupService = CreateWarmupService();
        var sut = new StartupUnfinishedQueueMetadataRefreshService(uiStateStore, downloadService, apiClient, warmupService);

        var result = await sut.RefreshAsync();

        Assert.Empty(result.UpdatedWorkInfos);
        Assert.Empty(result.Failures);
        Assert.Empty(apiClient.RequestedSourceIds);
        Assert.Equal(0, downloadService.UpsertCallCount);
    }

    [Fact]
    public async Task RefreshAsync_ShouldOnlyFetchMissingOrUntitledWorkInfo_AndUpsertFetchedItems()
    {
        var uiStateStore = new StubUiStateStore(new[] { "RJ1001", "rj1002", "RJ1003", "RJ1003" });
        var downloadService = new RecordingDownloadService(new Dictionary<string, WorkInfoDto>(StringComparer.OrdinalIgnoreCase)
        {
            ["RJ1001"] = new WorkInfoDto { SourceId = "RJ1001", Title = "已缓存标题" },
            ["RJ1002"] = new WorkInfoDto { SourceId = "RJ1002", Title = string.Empty },
        });
        var apiClient = new RecordingAsmrApiClient();
        apiClient.SetResponse("RJ1002", "补拉标题-2");
        apiClient.SetResponse("RJ1003", "补拉标题-3");
        var warmupService = CreateWarmupService();
        var sut = new StartupUnfinishedQueueMetadataRefreshService(uiStateStore, downloadService, apiClient, warmupService);

        var result = await sut.RefreshAsync();

        Assert.Equal(2, result.UpdatedWorkInfos.Count);
        Assert.Empty(result.Failures);
        Assert.Equal(
            new[] { "RJ1002", "RJ1003" },
            apiClient.RequestedSourceIds
                .Select(static item => item.ToUpperInvariant())
                .OrderBy(static item => item));
        Assert.Equal(1, downloadService.UpsertCallCount);
        Assert.Equal("已缓存标题", downloadService.GetPrefetchedWorkInfoSnapshot()["RJ1001"].Title);
        Assert.Equal("补拉标题-2", downloadService.GetPrefetchedWorkInfoSnapshot()["RJ1002"].Title);
        Assert.Equal("补拉标题-3", downloadService.GetPrefetchedWorkInfoSnapshot()["RJ1003"].Title);
    }

    [Fact]
    public async Task RefreshAsync_ShouldContinue_WhenSingleFetchFails()
    {
        var uiStateStore = new StubUiStateStore(new[] { "RJ2001", "RJ2002" });
        var downloadService = new RecordingDownloadService();
        var apiClient = new RecordingAsmrApiClient();
        apiClient.SetResponse("RJ2001", "成功标题");
        apiClient.SetFailure("RJ2002", new InvalidOperationException("mock api failure"));
        var warmupService = CreateWarmupService();
        var sut = new StartupUnfinishedQueueMetadataRefreshService(uiStateStore, downloadService, apiClient, warmupService);

        var result = await sut.RefreshAsync();

        Assert.Equal(1, result.UpdatedWorkInfos.Count);
        var failure = Assert.Single(result.Failures);
        Assert.Equal("RJ2002", failure.SourceId);
        Assert.Equal("mock api failure", failure.ErrorMessage);
        var snapshot = downloadService.GetPrefetchedWorkInfoSnapshot();
        Assert.True(snapshot.ContainsKey("RJ2001"));
        Assert.False(snapshot.ContainsKey("RJ2002"));
    }

    [Fact]
    public async Task RefreshAsync_ShouldWaitForWarmupBeforeFetchingWorkInfo()
    {
        var uiStateStore = new StubUiStateStore(new[] { "RJ3001" });
        var downloadService = new RecordingDownloadService();
        var apiClient = new RecordingAsmrApiClient();
        apiClient.SetResponse("RJ3001", "等待预热后补拉成功");
        var endpointService = new BlockingApiEndpointUrlService();
        var warmupService = CreateWarmupService(endpointService, TimeSpan.FromSeconds(5));
        var sut = new StartupUnfinishedQueueMetadataRefreshService(
            uiStateStore,
            downloadService,
            apiClient,
            warmupService,
            TimeSpan.FromSeconds(5));

        var refreshTask = sut.RefreshAsync();

        await endpointService.Started.Task.WaitAsync(TimeSpan.FromSeconds(1));
        Assert.Empty(apiClient.RequestedSourceIds);

        endpointService.Complete();

        var result = await refreshTask;
        Assert.Equal(1, result.UpdatedWorkInfos.Count);
        Assert.Empty(result.Failures);
        Assert.Equal(new[] { "RJ3001" }, apiClient.RequestedSourceIds.Select(static item => item.ToUpperInvariant()));
    }

    [Fact]
    public async Task RefreshAsync_ShouldContinue_WhenWarmupFails()
    {
        var uiStateStore = new StubUiStateStore(new[] { "RJ4001" });
        var downloadService = new RecordingDownloadService();
        var apiClient = new RecordingAsmrApiClient();
        apiClient.SetResponse("RJ4001", "预热失败后仍补拉成功");
        var warmupService = CreateWarmupService(new ThrowingApiEndpointUrlService(), TimeSpan.FromSeconds(1));
        var sut = new StartupUnfinishedQueueMetadataRefreshService(
            uiStateStore,
            downloadService,
            apiClient,
            warmupService,
            TimeSpan.FromSeconds(5));

        var result = await sut.RefreshAsync();

        Assert.Equal(1, result.UpdatedWorkInfos.Count);
        Assert.Empty(result.Failures);
        Assert.Equal(new[] { "RJ4001" }, apiClient.RequestedSourceIds.Select(static item => item.ToUpperInvariant()));
        Assert.Equal("预热失败后仍补拉成功", downloadService.GetPrefetchedWorkInfoSnapshot()["RJ4001"].Title);
    }

    [Fact]
    public async Task RefreshAsync_ShouldReturnGlobalFailures_WhenTimeoutOccurs()
    {
        var uiStateStore = new StubUiStateStore(new[] { "RJ5001" });
        var downloadService = new RecordingDownloadService();
        var apiClient = new SlowAsmrApiClient();
        var warmupService = CreateWarmupService();
        var sut = new StartupUnfinishedQueueMetadataRefreshService(
            uiStateStore,
            downloadService,
            apiClient,
            warmupService,
            TimeSpan.FromMilliseconds(10));

        var result = await sut.RefreshAsync();

        Assert.Empty(result.UpdatedWorkInfos);
        var failure = Assert.Single(result.Failures);
        Assert.Equal("RJ5001", failure.SourceId);
        Assert.Contains("超时", failure.ErrorMessage, StringComparison.Ordinal);
    }

    private static StartupEndpointWarmupService CreateWarmupService(
        IApiEndpointUrlService? endpointService = null,
        TimeSpan? timeout = null)
    {
        return new StartupEndpointWarmupService(
            endpointService ?? new RecordingApiEndpointUrlService(),
            timeout ?? TimeSpan.FromSeconds(1));
    }

    private sealed class StubUiStateStore : IUiStateStore
    {
        private readonly IReadOnlyList<string> _unfinishedQueue;

        public StubUiStateStore(IReadOnlyList<string> unfinishedQueue)
        {
            _unfinishedQueue = unfinishedQueue;
        }

        public Task<SearchUiState> LoadSearchUiStateAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(new SearchUiState());

        public Task SaveSearchUiStateAsync(SearchUiState state, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<DownloadUiState> LoadDownloadUiStateAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(new DownloadUiState());

        public Task SaveDownloadUiStateAsync(DownloadUiState state, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<SyncUiState> LoadSyncUiStateAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(new SyncUiState());

        public Task SaveSyncUiStateAsync(SyncUiState state, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<MetadataSyncProgressState> LoadMetadataSyncProgressAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(new MetadataSyncProgressState());

        public Task SaveMetadataSyncProgressAsync(MetadataSyncProgressState state, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task RequestStopMetadataSyncAsync(CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<SyncDownloadProgressState> LoadSyncDownloadProgressAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(new SyncDownloadProgressState());

        public Task SaveSyncDownloadProgressAsync(SyncDownloadProgressState state, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task RequestStopSyncDownloadAsync(CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<IReadOnlyList<string>> LoadUnfinishedQueueAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(_unfinishedQueue);

        public Task SaveUnfinishedQueueAsync(IReadOnlyCollection<string> sourceIds, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task ClearUnfinishedQueueAsync(CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    private sealed class RecordingDownloadService : IDownloadService
    {
        private readonly Dictionary<string, WorkInfoDto> _prefetchedWorkInfos;

        public RecordingDownloadService(IReadOnlyDictionary<string, WorkInfoDto>? initialSnapshot = null)
        {
            _prefetchedWorkInfos = initialSnapshot is null
                ? new Dictionary<string, WorkInfoDto>(StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, WorkInfoDto>(initialSnapshot, StringComparer.OrdinalIgnoreCase);
        }

        public int UpsertCallCount { get; private set; }

        public Task<IReadOnlyList<DownloadTaskItem>> RunQueuedAsync(string? fileFilter = null, bool hdAudioOnly = false, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<DownloadTaskItem>>(Array.Empty<DownloadTaskItem>());

        public Task<DownloadTaskItem?> StartAsync(string sourceId, string? fileFilter = null, Guid? preferredTaskId = null, bool hdAudioOnly = false, DownloadStartOptions? options = null, CancellationToken cancellationToken = default)
            => Task.FromResult<DownloadTaskItem?>(null);

        public IReadOnlyList<DownloadTaskItem> GetTasks()
            => Array.Empty<DownloadTaskItem>();

        public Task<bool> CancelAsync(Guid taskId, CancellationToken cancellationToken = default)
            => Task.FromResult(false);

        public Task<DownloadTaskItem?> RetryFailedAsync(Guid taskId, string? fileFilter = null, bool hdAudioOnly = false, DownloadStartOptions? options = null, CancellationToken cancellationToken = default)
            => Task.FromResult<DownloadTaskItem?>(null);

        public Task ClearAllTasksAsync(CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public void UpsertPrefetchedWorkInfo(IReadOnlyDictionary<string, WorkInfoDto> workInfos, WorkInfoCacheEntryLevel cacheLevel = WorkInfoCacheEntryLevel.Summary)
        {
            UpsertCallCount++;
            foreach (var (sourceId, workInfo) in workInfos)
            {
                _prefetchedWorkInfos[sourceId] = workInfo;
            }
        }

        public IReadOnlyDictionary<string, WorkInfoDto> GetPrefetchedWorkInfoSnapshot()
            => new Dictionary<string, WorkInfoDto>(_prefetchedWorkInfos, StringComparer.OrdinalIgnoreCase);
    }

    private sealed class RecordingAsmrApiClient : IAsmrApiClient
    {
        private readonly Dictionary<string, WorkInfoDto> _responses = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Exception> _failures = new(StringComparer.OrdinalIgnoreCase);

        public List<string> RequestedSourceIds { get; } = new();

        public Task<MetadataSyncPageDto> GetMetadataWorksAsync(int page, int pageSize, bool subtitleOnly = false, CancellationToken cancellationToken = default)
            => Task.FromResult(new MetadataSyncPageDto());

        public void SetResponse(string sourceId, string title)
        {
            _responses[sourceId] = new WorkInfoDto
            {
                SourceId = sourceId,
                Title = title,
            };
        }

        public void SetFailure(string sourceId, Exception exception)
        {
            _failures[sourceId] = exception;
        }

        public Task<WorkInfoDto> GetWorkInfoAsync(string id, CancellationToken cancellationToken = default)
        {
            RequestedSourceIds.Add(id);

            if (_failures.TryGetValue(id, out var exception))
            {
                throw exception;
            }

            if (_responses.TryGetValue(id, out var response))
            {
                return Task.FromResult(response);
            }

            return Task.FromResult(new WorkInfoDto
            {
                SourceId = id,
                Title = $"Title-{id}",
            });
        }

        public Task<IReadOnlyList<TrackDto>> GetTracksAsync(string id, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<TrackDto>>(Array.Empty<TrackDto>());

        public Task DownloadFileAsync(string url, string destinationPath, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<SearchResultDto> SearchAsync(string query, CancellationToken cancellationToken = default)
            => Task.FromResult(new SearchResultDto());

        public Task<IReadOnlyList<SearchWorkDto>> GetPopularAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<SearchWorkDto>>(Array.Empty<SearchWorkDto>());
    }

    private sealed class SlowAsmrApiClient : IAsmrApiClient
    {
        private readonly TaskCompletionSource<WorkInfoDto> _completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource<bool> Started { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task<MetadataSyncPageDto> GetMetadataWorksAsync(int page, int pageSize, bool subtitleOnly = false, CancellationToken cancellationToken = default)
            => Task.FromResult(new MetadataSyncPageDto());

        public Task<WorkInfoDto> GetWorkInfoAsync(string id, CancellationToken cancellationToken = default)
        {
            Started.TrySetResult(true);
            var registration = cancellationToken.Register(() => _completion.TrySetCanceled(cancellationToken));
            _ = _completion.Task.ContinueWith(
                _ => registration.Dispose(),
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
            return _completion.Task;
        }

        public void Complete(string sourceId, string title)
        {
            _completion.TrySetResult(new WorkInfoDto
            {
                SourceId = sourceId,
                Title = title,
            });
        }

        public Task<IReadOnlyList<TrackDto>> GetTracksAsync(string id, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<TrackDto>>(Array.Empty<TrackDto>());

        public Task DownloadFileAsync(string url, string destinationPath, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<SearchResultDto> SearchAsync(string query, CancellationToken cancellationToken = default)
            => Task.FromResult(new SearchResultDto());

        public Task<IReadOnlyList<SearchWorkDto>> GetPopularAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<SearchWorkDto>>(Array.Empty<SearchWorkDto>());
    }

    private sealed class RecordingApiEndpointUrlService : IApiEndpointUrlService
    {
        public Task<string> GetCurrentBaseUrlAsync(CancellationToken cancellationToken = default)
            => Task.FromResult("https://api.example.com");

        public Task<EndpointDiscoveryResult> DiscoverAndPersistAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new EndpointDiscoveryResult
            {
                BaseUrl = "https://api.example.com",
                LatencyMs = 10,
                Candidates = new[] { "https://api.example.com" },
            });
        }
    }

    private sealed class BlockingApiEndpointUrlService : IApiEndpointUrlService
    {
        private readonly TaskCompletionSource<bool> _completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource<bool> Started { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task<string> GetCurrentBaseUrlAsync(CancellationToken cancellationToken = default)
            => Task.FromResult("https://api.example.com");

        public async Task<EndpointDiscoveryResult> DiscoverAndPersistAsync(CancellationToken cancellationToken = default)
        {
            Started.TrySetResult(true);
            using var registration = cancellationToken.Register(() => _completion.TrySetCanceled(cancellationToken));
            await _completion.Task;

            return new EndpointDiscoveryResult
            {
                BaseUrl = "https://api.example.com",
                LatencyMs = 10,
                Candidates = new[] { "https://api.example.com" },
            };
        }

        public void Complete()
        {
            _completion.TrySetResult(true);
        }
    }

    private sealed class ThrowingApiEndpointUrlService : IApiEndpointUrlService
    {
        public Task<string> GetCurrentBaseUrlAsync(CancellationToken cancellationToken = default)
            => Task.FromResult("https://api.example.com");

        public Task<EndpointDiscoveryResult> DiscoverAndPersistAsync(CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("forced warmup failure");
        }
    }
}