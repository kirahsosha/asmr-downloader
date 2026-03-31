using System.Collections.Concurrent;
using Asmroner.Core.Api;
using Asmroner.Core.Constants;
using Asmroner.Core.Interfaces;
using NLog;

namespace Asmroner.Wpf.Services;

public sealed class StartupUnfinishedQueueMetadataRefreshService
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private readonly IUiStateStore _uiStateStore;
    private readonly IDownloadService _downloadService;
    private readonly IAsmrApiClient _asmrApiClient;
    private readonly StartupEndpointWarmupService _startupEndpointWarmupService;
    private readonly TimeSpan _timeout;

    private int _isRunning;

    public StartupUnfinishedQueueMetadataRefreshService(
        IUiStateStore uiStateStore,
        IDownloadService downloadService,
        IAsmrApiClient asmrApiClient,
        StartupEndpointWarmupService startupEndpointWarmupService,
        TimeSpan? timeout = null)
    {
        _uiStateStore = uiStateStore;
        _downloadService = downloadService;
        _asmrApiClient = asmrApiClient;
        _startupEndpointWarmupService = startupEndpointWarmupService;

        var effectiveTimeout = timeout ?? AsmronerConstants.Download.StartupMetadataRefreshTimeout;
        _timeout = effectiveTimeout > TimeSpan.Zero
            ? effectiveTimeout
            : AsmronerConstants.Download.StartupMetadataRefreshTimeout;
    }

    public Task<int> StartInBackgroundAsync(CancellationToken cancellationToken = default)
    {
        if (Interlocked.Exchange(ref _isRunning, 1) == 1)
        {
            return Task.FromResult(0);
        }

        return Task.Run(async () =>
        {
            try
            {
                return await RefreshAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                Interlocked.Exchange(ref _isRunning, 0);
            }
        }, CancellationToken.None);
    }

    public async Task<int> RefreshAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var unfinishedSourceIds = await _uiStateStore.LoadUnfinishedQueueAsync(cancellationToken).ConfigureAwait(false);
            if (unfinishedSourceIds.Count == 0)
            {
                return 0;
            }

            var prefetched = _downloadService.GetPrefetchedWorkInfoSnapshot();
            var missingSourceIds = unfinishedSourceIds
                .Where(sourceId =>
                    !prefetched.TryGetValue(sourceId, out var workInfo) ||
                    string.IsNullOrWhiteSpace(workInfo.Title))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (missingSourceIds.Length == 0)
            {
                return 0;
            }

            await _startupEndpointWarmupService.StartInBackgroundAsync(cancellationToken).ConfigureAwait(false);

            using var timeoutCancellation = new CancellationTokenSource(_timeout);
            using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCancellation.Token);

            var refreshedWorkInfos = await FetchWorkInfoMapAsync(missingSourceIds, linkedCancellation.Token).ConfigureAwait(false);
            if (refreshedWorkInfos.Count == 0)
            {
                return 0;
            }

            _downloadService.UpsertPrefetchedWorkInfo(refreshedWorkInfos);
            _logger.Info("Startup unfinished queue metadata refresh updated {Count} items.", refreshedWorkInfos.Count);
            return refreshedWorkInfos.Count;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.Warn("Startup unfinished queue metadata refresh timed out after {TimeoutMs} ms.", (long)_timeout.TotalMilliseconds);
            return 0;
        }
        catch (Exception ex)
        {
            _logger.Warn(ex, "Startup unfinished queue metadata refresh failed.");
            return 0;
        }
    }

    private async Task<IReadOnlyDictionary<string, WorkInfoDto>> FetchWorkInfoMapAsync(
        IReadOnlyList<string> sourceIds,
        CancellationToken cancellationToken)
    {
        var result = new ConcurrentDictionary<string, WorkInfoDto>(StringComparer.OrdinalIgnoreCase);
        using var limiter = new SemaphoreSlim(AsmronerConstants.Download.WorkInfoFetchMaxConcurrency);

        var jobs = sourceIds.Select(async sourceId =>
        {
            await limiter.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                var workInfo = await _asmrApiClient.GetWorkInfoAsync(sourceId, cancellationToken).ConfigureAwait(false);
                result[sourceId] = workInfo;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Startup unfinished queue metadata refresh failed for {SourceId}.", sourceId);
            }
            finally
            {
                limiter.Release();
            }
        });

        await Task.WhenAll(jobs).ConfigureAwait(false);
        return result;
    }
}