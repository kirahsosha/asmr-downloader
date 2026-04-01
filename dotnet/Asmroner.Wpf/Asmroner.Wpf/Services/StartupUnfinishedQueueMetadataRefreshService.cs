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

    public Task<StartupMetadataRefreshResult> StartInBackgroundAsync(CancellationToken cancellationToken = default)
    {
        if (Interlocked.Exchange(ref _isRunning, 1) == 1)
        {
            return Task.FromResult(new StartupMetadataRefreshResult());
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

    public async Task<StartupMetadataRefreshResult> RefreshAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<string> missingSourceIds = Array.Empty<string>();

        try
        {
            var unfinishedSourceIds = await _uiStateStore.LoadUnfinishedQueueAsync(cancellationToken).ConfigureAwait(false);
            if (unfinishedSourceIds.Count == 0)
            {
                return new StartupMetadataRefreshResult();
            }

            var prefetched = _downloadService.GetPrefetchedWorkInfoSnapshot();
            missingSourceIds = unfinishedSourceIds
                .Where(sourceId =>
                    !prefetched.TryGetValue(sourceId, out var workInfo) ||
                    string.IsNullOrWhiteSpace(workInfo.Title))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (missingSourceIds.Count == 0)
            {
                return new StartupMetadataRefreshResult();
            }

            await _startupEndpointWarmupService.StartInBackgroundAsync(cancellationToken).ConfigureAwait(false);

            using var timeoutCancellation = new CancellationTokenSource(_timeout);
            using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCancellation.Token);

            var refreshResult = await FetchWorkInfoMapAsync(missingSourceIds, linkedCancellation.Token).ConfigureAwait(false);
            if (refreshResult.UpdatedWorkInfos.Count == 0 && refreshResult.Failures.Count == 0)
            {
                return refreshResult;
            }

            if (refreshResult.UpdatedWorkInfos.Count > 0)
            {
                _downloadService.UpsertPrefetchedWorkInfo(refreshResult.UpdatedWorkInfos, WorkInfoCacheEntryLevel.Full);
                _logger.Info("Startup unfinished queue metadata refresh updated {Count} items.", refreshResult.UpdatedWorkInfos.Count);
            }

            return refreshResult;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.Warn("Startup unfinished queue metadata refresh timed out after {TimeoutMs} ms.", (long)_timeout.TotalMilliseconds);
            return BuildGlobalFailureResult(missingSourceIds, $"后台更新作品信息超时（{(long)_timeout.TotalMilliseconds} ms）。");
        }
        catch (Exception ex)
        {
            _logger.Warn(ex, "Startup unfinished queue metadata refresh failed.");
            return BuildGlobalFailureResult(missingSourceIds, BuildFailureMessage(ex));
        }
    }

    private async Task<StartupMetadataRefreshResult> FetchWorkInfoMapAsync(
        IReadOnlyList<string> sourceIds,
        CancellationToken cancellationToken)
    {
        var result = new ConcurrentDictionary<string, WorkInfoDto>(StringComparer.OrdinalIgnoreCase);
        var failures = new ConcurrentBag<StartupMetadataRefreshFailure>();
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
                failures.Add(new StartupMetadataRefreshFailure
                {
                    SourceId = sourceId,
                    ErrorMessage = BuildFailureMessage(ex),
                });
            }
            finally
            {
                limiter.Release();
            }
        });

        await Task.WhenAll(jobs).ConfigureAwait(false);
        return new StartupMetadataRefreshResult
        {
            UpdatedWorkInfos = new Dictionary<string, WorkInfoDto>(result, StringComparer.OrdinalIgnoreCase),
            Failures = failures
                .OrderBy(static item => item.SourceId, StringComparer.OrdinalIgnoreCase)
                .ToArray(),
        };
    }

    private static StartupMetadataRefreshResult BuildGlobalFailureResult(
        IReadOnlyList<string> sourceIds,
        string errorMessage)
    {
        if (sourceIds.Count == 0)
        {
            return new StartupMetadataRefreshResult();
        }

        return new StartupMetadataRefreshResult
        {
            Failures = sourceIds
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(static sourceId => sourceId, StringComparer.OrdinalIgnoreCase)
                .Select(sourceId => new StartupMetadataRefreshFailure
                {
                    SourceId = sourceId,
                    ErrorMessage = errorMessage,
                })
                .ToArray(),
        };
    }

    private static string BuildFailureMessage(Exception ex)
    {
        if (ex is AsmrApiException apiException && !string.IsNullOrWhiteSpace(apiException.Error.Message))
        {
            return apiException.Error.Message;
        }

        return string.IsNullOrWhiteSpace(ex.Message)
            ? "后台更新作品信息失败。"
            : ex.Message;
    }
}

public sealed class StartupMetadataRefreshResult
{
    public IReadOnlyDictionary<string, WorkInfoDto> UpdatedWorkInfos { get; init; } = new Dictionary<string, WorkInfoDto>(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<StartupMetadataRefreshFailure> Failures { get; init; } = Array.Empty<StartupMetadataRefreshFailure>();
}

public sealed class StartupMetadataRefreshFailure
{
    public string SourceId { get; init; } = string.Empty;

    public string ErrorMessage { get; init; } = string.Empty;
}