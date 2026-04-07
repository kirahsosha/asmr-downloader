using Asmroner.Core.Configuration;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Sync;

namespace Asmroner.Application.Services;

public sealed class MetadataSyncService
{
    private const int SyncPageSize = 100;

    private readonly IAsmrApiClient _apiClient;
    private readonly IConfigurationService _configurationService;
    private readonly IMetadataSyncStore _metadataSyncStore;
    private readonly IRateLimiterService _rateLimiterService;
    private readonly IUiStateStore _uiStateStore;

    public MetadataSyncService(
        IAsmrApiClient apiClient,
        IConfigurationService configurationService,
        IMetadataSyncStore metadataSyncStore,
        IRateLimiterService rateLimiterService,
        IUiStateStore uiStateStore)
    {
        _apiClient = apiClient;
        _configurationService = configurationService;
        _metadataSyncStore = metadataSyncStore;
        _rateLimiterService = rateLimiterService;
        _uiStateStore = uiStateStore;
    }

    public Task<MetadataSyncSnapshot> GetMetadataSnapshotAsync(CancellationToken cancellationToken = default)
    {
        return _metadataSyncStore.GetMetadataSnapshotAsync(cancellationToken);
    }

    public Task<MetadataSyncProgressState> GetProgressAsync(CancellationToken cancellationToken = default)
    {
        return _uiStateStore.LoadMetadataSyncProgressAsync(cancellationToken);
    }

    public Task RequestStopAsync(CancellationToken cancellationToken = default)
    {
        return _uiStateStore.RequestStopMetadataSyncAsync(cancellationToken);
    }

    public async Task<MetadataSyncRunResult> SyncMetadataAsync(CancellationToken cancellationToken = default)
    {
        var progress = await _uiStateStore.LoadMetadataSyncProgressAsync(cancellationToken);
        var resumedFromProgress = ShouldResume(progress);
        var config = await _configurationService.LoadAsync(cancellationToken) ?? new AppConfig();
        var before = await _metadataSyncStore.GetMetadataSnapshotAsync(cancellationToken);
        var expiredMetadataCount = await CountExpiredMetadataAsync(progress, resumedFromProgress, config, cancellationToken);
        var shouldRefreshExpiredMetadata = expiredMetadataCount > 0;

        var remoteFirstPage = await GetMetadataWorksAsync(page: 1, pageSize: 1, subtitleOnly: false, config, cancellationToken);
        var remoteSubtitlePage = await GetMetadataWorksAsync(page: 1, pageSize: 1, subtitleOnly: true, config, cancellationToken);

        var remoteTotalCount = remoteFirstPage.Pagination.TotalCount;
        var remoteSubtitleCount = remoteSubtitlePage.Pagination.TotalCount;
        var totalPages = Math.Max(1, (int)Math.Ceiling(remoteTotalCount / (double)SyncPageSize));
        var startPage = resumedFromProgress
            ? NormalizeNextPage(progress.NextPage, totalPages)
            : 1;
        var processedPageCountOverall = resumedFromProgress
            ? Math.Max(0, startPage - 1)
            : 0;
        var cumulativeInsertedCount = resumedFromProgress
            ? Math.Max(0, progress.InsertedCount)
            : 0;
        var startedAt = resumedFromProgress
            ? progress.StartedAt ?? DateTime.UtcNow
            : DateTime.UtcNow;

        if (!resumedFromProgress && !shouldRefreshExpiredMetadata && remoteTotalCount == before.LocalTotalCount)
        {
            await _uiStateStore.SaveMetadataSyncProgressAsync(
                BuildProgressState(
                    SyncProgressStatuses.Completed,
                    nextPage: 1,
                    processedPageCount: 0,
                    totalPageCount: totalPages,
                    remoteTotalCount,
                    remoteSubtitleCount,
                    insertedCount: 0,
                    startedAt,
                    updatedAt: DateTime.UtcNow),
                cancellationToken);

            return BuildNoOpResult(
                remoteTotalCount,
                remoteSubtitleCount,
                before,
                message: "网站元数据与本地一致，无需同步。",
                isUpToDate: true,
                resumedFromProgress: false);
        }

        if (!resumedFromProgress && !shouldRefreshExpiredMetadata && remoteTotalCount < before.LocalTotalCount)
        {
            await _uiStateStore.SaveMetadataSyncProgressAsync(
                BuildProgressState(
                    SyncProgressStatuses.Completed,
                    nextPage: 1,
                    processedPageCount: 0,
                    totalPageCount: totalPages,
                    remoteTotalCount,
                    remoteSubtitleCount,
                    insertedCount: 0,
                    startedAt,
                    updatedAt: DateTime.UtcNow),
                cancellationToken);

            return BuildNoOpResult(
                remoteTotalCount,
                remoteSubtitleCount,
                before,
                message: "本地元数据数量高于网站，未执行同步。",
                isUpToDate: false,
                resumedFromProgress: false);
        }

        await _uiStateStore.SaveMetadataSyncProgressAsync(
            BuildProgressState(
                SyncProgressStatuses.Running,
                nextPage: startPage,
                processedPageCount: processedPageCountOverall,
                totalPageCount: totalPages,
                remoteTotalCount,
                remoteSubtitleCount,
                insertedCount: cumulativeInsertedCount,
                startedAt,
                updatedAt: DateTime.UtcNow),
            cancellationToken);

        var insertedCount = 0;

        for (var page = startPage; page <= totalPages; page++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var pageResult = await GetMetadataWorksAsync(page, SyncPageSize, subtitleOnly: false, config, cancellationToken);
            var updatedAt = DateTime.UtcNow;
            var works = pageResult.Works
                .Select(work => work.ToMetadataWorkItem(updatedAt))
                .Where(static work => work.Id > 0 && !string.IsNullOrWhiteSpace(work.SourceId))
                .ToArray();

            var insertedThisPage = await _metadataSyncStore.UpsertMetadataWorksAsync(works, cancellationToken);
            insertedCount += insertedThisPage;
            cumulativeInsertedCount += insertedThisPage;
            processedPageCountOverall = page;

            if (page < totalPages)
            {
                await _uiStateStore.SaveMetadataSyncProgressAsync(
                    BuildProgressState(
                        SyncProgressStatuses.Running,
                        nextPage: page + 1,
                        processedPageCount: processedPageCountOverall,
                        totalPageCount: totalPages,
                        remoteTotalCount,
                        remoteSubtitleCount,
                        insertedCount: cumulativeInsertedCount,
                        startedAt,
                        updatedAt: DateTime.UtcNow),
                    cancellationToken);

                var currentProgress = await _uiStateStore.LoadMetadataSyncProgressAsync(cancellationToken);
                if (currentProgress.StopRequested)
                {
                    await _uiStateStore.SaveMetadataSyncProgressAsync(
                        BuildProgressState(
                            SyncProgressStatuses.Stopped,
                            nextPage: page + 1,
                            processedPageCount: processedPageCountOverall,
                            totalPageCount: totalPages,
                            remoteTotalCount,
                            remoteSubtitleCount,
                            insertedCount: cumulativeInsertedCount,
                            startedAt,
                            updatedAt: DateTime.UtcNow),
                        cancellationToken);

                    var afterStopped = await _metadataSyncStore.GetMetadataSnapshotAsync(cancellationToken);
                    return new MetadataSyncRunResult
                    {
                        RemoteTotalCount = remoteTotalCount,
                        RemoteSubtitleCount = remoteSubtitleCount,
                        LocalTotalCountBefore = before.LocalTotalCount,
                        LocalSubtitleCountBefore = before.LocalSubtitleCount,
                        LocalTotalCountAfter = afterStopped.LocalTotalCount,
                        LocalSubtitleCountAfter = afterStopped.LocalSubtitleCount,
                        InsertedCount = insertedCount,
                        ProcessedPageCount = processedPageCountOverall,
                        TotalPageCount = totalPages,
                        NextPage = page + 1,
                        Message = shouldRefreshExpiredMetadata
                            ? $"元数据过期刷新已按请求停止：已处理 {processedPageCountOverall}/{totalPages} 页，本地现有 {afterStopped.LocalTotalCount} 条。"
                            : $"元数据同步已按请求停止：已处理 {processedPageCountOverall}/{totalPages} 页，本地现有 {afterStopped.LocalTotalCount} 条。",
                        IsUpToDate = afterStopped.LocalTotalCount == remoteTotalCount,
                        WasStopped = true,
                        ResumedFromProgress = resumedFromProgress,
                    };
                }
            }
        }

        var after = await _metadataSyncStore.GetMetadataSnapshotAsync(cancellationToken);
        var isUpToDate = after.LocalTotalCount == remoteTotalCount;

        await _uiStateStore.SaveMetadataSyncProgressAsync(
            BuildProgressState(
                SyncProgressStatuses.Completed,
                nextPage: 1,
                processedPageCount: processedPageCountOverall,
                totalPageCount: totalPages,
                remoteTotalCount,
                remoteSubtitleCount,
                insertedCount: cumulativeInsertedCount,
                startedAt,
                updatedAt: DateTime.UtcNow),
            cancellationToken);

        return new MetadataSyncRunResult
        {
            RemoteTotalCount = remoteTotalCount,
            RemoteSubtitleCount = remoteSubtitleCount,
            LocalTotalCountBefore = before.LocalTotalCount,
            LocalSubtitleCountBefore = before.LocalSubtitleCount,
            LocalTotalCountAfter = after.LocalTotalCount,
            LocalSubtitleCountAfter = after.LocalSubtitleCount,
            InsertedCount = insertedCount,
            ProcessedPageCount = processedPageCountOverall,
            TotalPageCount = totalPages,
            NextPage = 1,
            Message = shouldRefreshExpiredMetadata
                ? BuildExpiredRefreshCompletionMessage(expiredMetadataCount, after.LocalTotalCount, remoteTotalCount, isUpToDate)
                : isUpToDate
                    ? $"元数据同步完成：新增 {insertedCount} 条，本地现有 {after.LocalTotalCount} 条。"
                    : $"元数据同步完成，但本地数量仍为 {after.LocalTotalCount}，未完全追平网站的 {remoteTotalCount} 条。",
            IsUpToDate = isUpToDate,
            WasStopped = false,
            ResumedFromProgress = resumedFromProgress,
        };
    }

    private async Task<int> CountExpiredMetadataAsync(
        MetadataSyncProgressState progress,
        bool resumedFromProgress,
        AppConfig config,
        CancellationToken cancellationToken)
    {
        if (resumedFromProgress || !string.Equals(progress.Status, SyncProgressStatuses.Completed, StringComparison.Ordinal))
        {
            return 0;
        }

        var validDays = Math.Max(1, config.Downloader.MetadataValidityDays);
        var updatedBefore = DateTime.UtcNow.AddDays(-validDays);
        var expiredIds = await _metadataSyncStore.GetExpiredMetadataWorkIdsAsync(updatedBefore, cancellationToken);
        return expiredIds.Count;
    }

    private static string BuildExpiredRefreshCompletionMessage(int expiredMetadataCount, int localTotalCount, int remoteTotalCount, bool isUpToDate)
    {
        return isUpToDate
            ? $"元数据过期刷新完成：已检查 {expiredMetadataCount} 条过期记录，本地现有 {localTotalCount} 条。"
            : $"元数据过期刷新完成，但本地数量仍为 {localTotalCount}，未完全追平网站的 {remoteTotalCount} 条。";
    }

    private async Task<Core.Api.MetadataSyncPageDto> GetMetadataWorksAsync(
        int page,
        int pageSize,
        bool subtitleOnly,
        AppConfig config,
        CancellationToken cancellationToken)
    {
        await _rateLimiterService.WaitAsync(
            config.Limit.SyncQps,
            config.Limit.SyncJitterMin,
            config.Limit.SyncJitterMax,
            cancellationToken);

        return await _apiClient.GetMetadataWorksAsync(page, pageSize, subtitleOnly, cancellationToken);
    }

    private static MetadataSyncRunResult BuildNoOpResult(
        int remoteTotalCount,
        int remoteSubtitleCount,
        MetadataSyncSnapshot snapshot,
        string message,
        bool isUpToDate,
        bool resumedFromProgress)
    {
        return new MetadataSyncRunResult
        {
            RemoteTotalCount = remoteTotalCount,
            RemoteSubtitleCount = remoteSubtitleCount,
            LocalTotalCountBefore = snapshot.LocalTotalCount,
            LocalSubtitleCountBefore = snapshot.LocalSubtitleCount,
            LocalTotalCountAfter = snapshot.LocalTotalCount,
            LocalSubtitleCountAfter = snapshot.LocalSubtitleCount,
            InsertedCount = 0,
            ProcessedPageCount = 0,
            TotalPageCount = 0,
            NextPage = 1,
            Message = message,
            IsUpToDate = isUpToDate,
            WasStopped = false,
            ResumedFromProgress = resumedFromProgress,
        };
    }

    private static bool ShouldResume(MetadataSyncProgressState state)
    {
        return !string.Equals(state.Status, SyncProgressStatuses.Completed, StringComparison.Ordinal)
            && !string.Equals(state.Status, SyncProgressStatuses.Idle, StringComparison.Ordinal)
            && state.NextPage > 1;
    }

    private static int NormalizeNextPage(int nextPage, int totalPages)
    {
        return Math.Min(Math.Max(nextPage, 1), Math.Max(totalPages, 1));
    }

    private static MetadataSyncProgressState BuildProgressState(
        string status,
        int nextPage,
        int processedPageCount,
        int totalPageCount,
        int remoteTotalCount,
        int remoteSubtitleCount,
        int insertedCount,
        DateTime startedAt,
        DateTime updatedAt)
    {
        return new MetadataSyncProgressState
        {
            Status = status,
            NextPage = nextPage,
            ProcessedPageCount = processedPageCount,
            TotalPageCount = totalPageCount,
            RemoteTotalCount = remoteTotalCount,
            RemoteSubtitleCount = remoteSubtitleCount,
            InsertedCount = insertedCount,
            StartedAt = startedAt,
            UpdatedAt = updatedAt,
        };
    }
}