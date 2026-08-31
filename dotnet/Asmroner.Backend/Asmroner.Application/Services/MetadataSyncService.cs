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
        var context = await ResolveInitializationContextAsync(cancellationToken);
        var decision = DetermineSyncBranch(context);

        if (decision == MetadataSyncBranch.RefreshExpired)
        {
            return await RefreshExpiredMetadataAsync(
                context.Before,
                context.Config,
                context.RemoteTotalCount,
                context.RemoteSubtitleCount,
                context.ExpiredMetadataCount,
                context.StartedAt,
                cancellationToken);
        }

        if (decision == MetadataSyncBranch.SkipUpToDate)
        {
            await SaveProgressAsync(
                SyncProgressStatuses.Completed,
                nextPage: 1,
                processedPageCount: 0,
                totalPageCount: context.TotalPages,
                context.RemoteTotalCount,
                context.RemoteSubtitleCount,
                localTotalCount: context.Before.LocalTotalCount,
                localSubtitleCount: context.Before.LocalSubtitleCount,
                insertedCount: 0,
                processedWorkCount: 0,
                context.StartedAt,
                cancellationToken);

            return BuildNoOpResult(
                context.RemoteTotalCount,
                context.RemoteSubtitleCount,
                context.Before,
                message: "网站元数据与本地一致，无需同步。",
                isUpToDate: true,
                resumedFromProgress: false);
        }

        if (decision == MetadataSyncBranch.SkipLocalAhead)
        {
            await SaveProgressAsync(
                SyncProgressStatuses.Completed,
                nextPage: 1,
                processedPageCount: 0,
                totalPageCount: context.TotalPages,
                context.RemoteTotalCount,
                context.RemoteSubtitleCount,
                localTotalCount: context.Before.LocalTotalCount,
                localSubtitleCount: context.Before.LocalSubtitleCount,
                insertedCount: 0,
                processedWorkCount: 0,
                context.StartedAt,
                cancellationToken);

            return BuildNoOpResult(
                context.RemoteTotalCount,
                context.RemoteSubtitleCount,
                context.Before,
                message: "本地元数据数量高于网站，未执行同步。",
                isUpToDate: false,
                resumedFromProgress: false);
        }

        var pageSyncState = await ExecutePagedSyncAsync(context, cancellationToken);
        return await BuildFinalResultAsync(context, pageSyncState, cancellationToken);
    }

    private async Task<MetadataSyncInitializationContext> ResolveInitializationContextAsync(CancellationToken cancellationToken)
    {
        var progress = await _uiStateStore.LoadMetadataSyncProgressAsync(cancellationToken);
        var resumedFromProgress = ShouldResume(progress);
        var config = await _configurationService.LoadAsync(cancellationToken) ?? new AppConfig();
        var before = await _metadataSyncStore.GetMetadataSnapshotAsync(cancellationToken);
        var expiredMetadataCount = await CountExpiredMetadataAsync(progress, resumedFromProgress, config, cancellationToken);

        var remoteFirstPage = await GetMetadataWorksAsync(page: 1, pageSize: 1, subtitleOnly: false, config, cancellationToken);
        var remoteSubtitlePage = await GetMetadataWorksAsync(page: 1, pageSize: 1, subtitleOnly: true, config, cancellationToken);

        var remoteTotalCount = remoteFirstPage.Pagination.TotalCount;
        var remoteSubtitleCount = remoteSubtitlePage.Pagination.TotalCount;
        var totalPages = Math.Max(1, (int)Math.Ceiling(remoteTotalCount / (double)SyncPageSize));
        var startPage = resumedFromProgress
            ? NormalizeNextPage(progress.NextPage, totalPages)
            : 1;

        return new MetadataSyncInitializationContext(
            progress,
            resumedFromProgress,
            config,
            before,
            expiredMetadataCount,
            remoteTotalCount,
            remoteSubtitleCount,
            totalPages,
            startPage,
            resumedFromProgress ? Math.Max(0, startPage - 1) : 0,
            resumedFromProgress ? Math.Max(0, progress.InsertedCount) : 0,
            resumedFromProgress ? Math.Max(0, progress.ProcessedWorkCount > 0 ? progress.ProcessedWorkCount : progress.InsertedCount) : 0,
            resumedFromProgress ? progress.StartedAt ?? DateTime.UtcNow : DateTime.UtcNow);
    }

    private static MetadataSyncBranch DetermineSyncBranch(MetadataSyncInitializationContext context)
    {
        if (context.ExpiredMetadataCount > 0)
        {
            return MetadataSyncBranch.RefreshExpired;
        }

        if (!context.ResumedFromProgress && context.RemoteTotalCount == context.Before.LocalTotalCount)
        {
            return MetadataSyncBranch.SkipUpToDate;
        }

        if (!context.ResumedFromProgress && context.RemoteTotalCount < context.Before.LocalTotalCount)
        {
            return MetadataSyncBranch.SkipLocalAhead;
        }

        return MetadataSyncBranch.Proceed;
    }

    private async Task<MetadataSyncPagedResult> ExecutePagedSyncAsync(
        MetadataSyncInitializationContext context,
        CancellationToken cancellationToken)
    {
        var processedPageCountOverall = context.InitialProcessedPageCount;
        var cumulativeInsertedCount = context.InitialCumulativeInsertedCount;
        var cumulativeProcessedWorkCount = context.InitialCumulativeProcessedWorkCount;
        var insertedCount = 0;
        var processedWorkCount = 0;
        var currentSnapshot = context.Before;

        await SaveProgressAsync(
            SyncProgressStatuses.Running,
            nextPage: context.StartPage,
            processedPageCount: processedPageCountOverall,
            totalPageCount: context.TotalPages,
            context.RemoteTotalCount,
            context.RemoteSubtitleCount,
            localTotalCount: context.Before.LocalTotalCount,
            localSubtitleCount: context.Before.LocalSubtitleCount,
            insertedCount: cumulativeInsertedCount,
            processedWorkCount: cumulativeProcessedWorkCount,
            context.StartedAt,
            cancellationToken);

        for (var page = context.StartPage; page <= context.TotalPages; page++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var pageResult = await GetMetadataWorksAsync(page, SyncPageSize, subtitleOnly: false, context.Config, cancellationToken);
            var updatedAt = DateTime.UtcNow;
            var works = pageResult.Works
                .Select(work => work.ToMetadataWorkItem(updatedAt))
                .Where(static work => work.Id > 0 && !string.IsNullOrWhiteSpace(work.SourceId))
                .ToArray();

            var processedThisPage = works.Length;
            var insertedThisPage = await _metadataSyncStore.UpsertMetadataWorksAsync(works, cancellationToken);

            insertedCount += insertedThisPage;
            cumulativeInsertedCount += insertedThisPage;
            processedWorkCount += processedThisPage;
            cumulativeProcessedWorkCount += processedThisPage;
            processedPageCountOverall = page;
            currentSnapshot = await _metadataSyncStore.GetMetadataSnapshotAsync(cancellationToken);

            if (page >= context.TotalPages)
            {
                continue;
            }

            await SaveProgressAsync(
                SyncProgressStatuses.Running,
                nextPage: page + 1,
                processedPageCount: processedPageCountOverall,
                totalPageCount: context.TotalPages,
                context.RemoteTotalCount,
                context.RemoteSubtitleCount,
                localTotalCount: currentSnapshot.LocalTotalCount,
                localSubtitleCount: currentSnapshot.LocalSubtitleCount,
                insertedCount: cumulativeInsertedCount,
                processedWorkCount: cumulativeProcessedWorkCount,
                context.StartedAt,
                cancellationToken);

            var currentProgress = await _uiStateStore.LoadMetadataSyncProgressAsync(cancellationToken);
            if (!currentProgress.StopRequested)
            {
                continue;
            }

            await SaveProgressAsync(
                SyncProgressStatuses.Stopped,
                nextPage: page + 1,
                processedPageCount: processedPageCountOverall,
                totalPageCount: context.TotalPages,
                context.RemoteTotalCount,
                context.RemoteSubtitleCount,
                localTotalCount: currentSnapshot.LocalTotalCount,
                localSubtitleCount: currentSnapshot.LocalSubtitleCount,
                insertedCount: cumulativeInsertedCount,
                processedWorkCount: cumulativeProcessedWorkCount,
                context.StartedAt,
                cancellationToken);

            return new MetadataSyncPagedResult(
                insertedCount,
                processedWorkCount,
                processedPageCountOverall,
                page + 1,
                currentSnapshot,
                currentSnapshot.LocalTotalCount == context.RemoteTotalCount,
                WasStopped: true,
                cumulativeInsertedCount,
                cumulativeProcessedWorkCount);
        }

        return new MetadataSyncPagedResult(
            insertedCount,
            processedWorkCount,
            processedPageCountOverall,
            NextPage: 1,
            currentSnapshot,
            currentSnapshot.LocalTotalCount == context.RemoteTotalCount,
            WasStopped: false,
            cumulativeInsertedCount,
            cumulativeProcessedWorkCount);
    }

    private async Task<MetadataSyncRunResult> BuildFinalResultAsync(
        MetadataSyncInitializationContext context,
        MetadataSyncPagedResult paged,
        CancellationToken cancellationToken)
    {
        if (paged.WasStopped)
        {
            return new MetadataSyncRunResult
            {
                RemoteTotalCount = context.RemoteTotalCount,
                RemoteSubtitleCount = context.RemoteSubtitleCount,
                LocalTotalCountBefore = context.Before.LocalTotalCount,
                LocalSubtitleCountBefore = context.Before.LocalSubtitleCount,
                LocalTotalCountAfter = paged.Snapshot.LocalTotalCount,
                LocalSubtitleCountAfter = paged.Snapshot.LocalSubtitleCount,
                InsertedCount = paged.InsertedCount,
                ProcessedWorkCount = paged.ProcessedWorkCount,
                ProcessedPageCount = paged.ProcessedPageCount,
                TotalPageCount = context.TotalPages,
                NextPage = paged.NextPage,
                Message = $"元数据同步已按请求停止：已处理 {paged.ProcessedPageCount}/{context.TotalPages} 页，共处理 {paged.ProcessedWorkCount} 条，本地现有 {paged.Snapshot.LocalTotalCount} 条。",
                IsUpToDate = paged.IsUpToDate,
                WasStopped = true,
                ResumedFromProgress = context.ResumedFromProgress,
            };
        }

        await SaveProgressAsync(
            SyncProgressStatuses.Completed,
            nextPage: 1,
            processedPageCount: paged.ProcessedPageCount,
            totalPageCount: context.TotalPages,
            context.RemoteTotalCount,
            context.RemoteSubtitleCount,
            localTotalCount: paged.Snapshot.LocalTotalCount,
            localSubtitleCount: paged.Snapshot.LocalSubtitleCount,
            insertedCount: paged.CumulativeInsertedCount,
            processedWorkCount: paged.CumulativeProcessedWorkCount,
            context.StartedAt,
            cancellationToken);

        return new MetadataSyncRunResult
        {
            RemoteTotalCount = context.RemoteTotalCount,
            RemoteSubtitleCount = context.RemoteSubtitleCount,
            LocalTotalCountBefore = context.Before.LocalTotalCount,
            LocalSubtitleCountBefore = context.Before.LocalSubtitleCount,
            LocalTotalCountAfter = paged.Snapshot.LocalTotalCount,
            LocalSubtitleCountAfter = paged.Snapshot.LocalSubtitleCount,
            InsertedCount = paged.InsertedCount,
            ProcessedWorkCount = paged.ProcessedWorkCount,
            ProcessedPageCount = paged.ProcessedPageCount,
            TotalPageCount = context.TotalPages,
            NextPage = 1,
            Message = paged.IsUpToDate
                ? $"元数据同步完成：本次处理 {paged.ProcessedWorkCount} 条，新增 {paged.InsertedCount} 条，本地现有 {paged.Snapshot.LocalTotalCount} 条。"
                : $"元数据同步完成：本次处理 {paged.ProcessedWorkCount} 条，新增 {paged.InsertedCount} 条，但本地数量仍为 {paged.Snapshot.LocalTotalCount}，未完全追平网站的 {context.RemoteTotalCount} 条。",
            IsUpToDate = paged.IsUpToDate,
            WasStopped = false,
            ResumedFromProgress = context.ResumedFromProgress,
        };
    }

    private async Task SaveProgressAsync(
        string status,
        int nextPage,
        int processedPageCount,
        int totalPageCount,
        int remoteTotalCount,
        int remoteSubtitleCount,
        int localTotalCount,
        int localSubtitleCount,
        int insertedCount,
        int processedWorkCount,
        DateTime startedAt,
        CancellationToken cancellationToken)
    {
        await _uiStateStore.SaveMetadataSyncProgressAsync(
            BuildProgressState(
                status,
                nextPage,
                processedPageCount,
                totalPageCount,
                remoteTotalCount,
                remoteSubtitleCount,
                localTotalCount,
                localSubtitleCount,
                insertedCount,
                processedWorkCount,
                startedAt,
                DateTime.UtcNow),
            cancellationToken);
    }

    private async Task<MetadataSyncRunResult> RefreshExpiredMetadataAsync(
        MetadataSyncSnapshot before,
        AppConfig config,
        int remoteTotalCount,
        int remoteSubtitleCount,
        int expiredMetadataCount,
        DateTime startedAt,
        CancellationToken cancellationToken)
    {
        var allWorks = await _metadataSyncStore.GetAllMetadataWorksAsync(cancellationToken);
        var validDays = Math.Max(1, config.Downloader.MetadataValidityDays);
        var updatedBefore = DateTime.UtcNow.AddDays(-validDays);
        var expiredSourceIds = allWorks
            .Where(work => work.UpdatedAt < updatedBefore)
            .Select(work => work.SourceId)
            .Where(sourceId => !string.IsNullOrWhiteSpace(sourceId))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        await _uiStateStore.SaveMetadataSyncProgressAsync(
            BuildProgressState(
                SyncProgressStatuses.Running,
                nextPage: 1,
                processedPageCount: 0,
                totalPageCount: 1,
                remoteTotalCount,
                remoteSubtitleCount,
                localTotalCount: before.LocalTotalCount,
                localSubtitleCount: before.LocalSubtitleCount,
                insertedCount: 0,
                processedWorkCount: 0,
                startedAt,
                updatedAt: DateTime.UtcNow),
            cancellationToken);

        if (expiredSourceIds.Count == 0)
        {
            await _uiStateStore.SaveMetadataSyncProgressAsync(
                BuildProgressState(
                    SyncProgressStatuses.Completed,
                    nextPage: 1,
                    processedPageCount: 1,
                    totalPageCount: 1,
                    remoteTotalCount,
                    remoteSubtitleCount,
                    localTotalCount: before.LocalTotalCount,
                    localSubtitleCount: before.LocalSubtitleCount,
                    insertedCount: 0,
                    processedWorkCount: 0,
                    startedAt,
                    updatedAt: DateTime.UtcNow),
                cancellationToken);

            return BuildNoOpResult(
                remoteTotalCount,
                remoteSubtitleCount,
                before,
                message: "元数据过期刷新完成：没有检测到需要刷新的过期记录。",
                isUpToDate: before.LocalTotalCount == remoteTotalCount,
                resumedFromProgress: false);
        }

        var totalPages = Math.Max(1, (int)Math.Ceiling(remoteTotalCount / (double)SyncPageSize));
        var refreshedWorks = new List<MetadataWorkItem>(expiredSourceIds.Count);
        var scannedPageCount = 0;

        for (var page = 1; page <= totalPages; page++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var pageResult = await GetMetadataWorksAsync(page, SyncPageSize, subtitleOnly: false, config, cancellationToken);
            scannedPageCount = page;
            var updatedAt = DateTime.UtcNow;
            foreach (var candidate in pageResult.Works)
            {
                if (!expiredSourceIds.Remove(candidate.SourceId))
                {
                    continue;
                }

                refreshedWorks.Add(candidate.ToMetadataWorkItem(updatedAt));
            }

            if (expiredSourceIds.Count == 0)
            {
                break;
            }
        }

        var insertedCount = 0;
        if (refreshedWorks.Count > 0)
        {
            insertedCount = await _metadataSyncStore.UpsertMetadataWorksAsync(refreshedWorks, cancellationToken);
        }

        var after = await _metadataSyncStore.GetMetadataSnapshotAsync(cancellationToken);
        var isUpToDate = after.LocalTotalCount == remoteTotalCount;
        var processedWorkCount = refreshedWorks.Count;

        await _uiStateStore.SaveMetadataSyncProgressAsync(
            BuildProgressState(
                SyncProgressStatuses.Completed,
                nextPage: 1,
                processedPageCount: scannedPageCount,
                totalPageCount: totalPages,
                remoteTotalCount,
                remoteSubtitleCount,
                localTotalCount: after.LocalTotalCount,
                localSubtitleCount: after.LocalSubtitleCount,
                insertedCount,
                processedWorkCount,
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
            ProcessedWorkCount = processedWorkCount,
            ProcessedPageCount = scannedPageCount,
            TotalPageCount = totalPages,
            NextPage = 1,
            Message = BuildExpiredRefreshCompletionMessage(processedWorkCount, insertedCount, expiredMetadataCount, after.LocalTotalCount, remoteTotalCount, isUpToDate),
            IsUpToDate = isUpToDate,
            WasStopped = false,
            ResumedFromProgress = false,
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

    private static string BuildExpiredRefreshCompletionMessage(int processedWorkCount, int insertedCount, int expiredMetadataCount, int localTotalCount, int remoteTotalCount, bool isUpToDate)
    {
        return isUpToDate
            ? $"元数据过期刷新完成：本次处理 {processedWorkCount} 条（目标过期记录 {expiredMetadataCount} 条），新增 {insertedCount} 条，本地现有 {localTotalCount} 条。"
            : $"元数据过期刷新完成：本次处理 {processedWorkCount} 条，新增 {insertedCount} 条，但本地数量仍为 {localTotalCount}，未完全追平网站的 {remoteTotalCount} 条。";
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
            ProcessedWorkCount = 0,
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
        int localTotalCount,
        int localSubtitleCount,
        int insertedCount,
        int processedWorkCount,
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
            LocalTotalCount = localTotalCount,
            LocalSubtitleCount = localSubtitleCount,
            InsertedCount = insertedCount,
            ProcessedWorkCount = processedWorkCount,
            StartedAt = startedAt,
            UpdatedAt = updatedAt,
        };
    }

    private sealed record MetadataSyncInitializationContext(
        MetadataSyncProgressState Progress,
        bool ResumedFromProgress,
        AppConfig Config,
        MetadataSyncSnapshot Before,
        int ExpiredMetadataCount,
        int RemoteTotalCount,
        int RemoteSubtitleCount,
        int TotalPages,
        int StartPage,
        int InitialProcessedPageCount,
        int InitialCumulativeInsertedCount,
        int InitialCumulativeProcessedWorkCount,
        DateTime StartedAt);

    private sealed record MetadataSyncPagedResult(
        int InsertedCount,
        int ProcessedWorkCount,
        int ProcessedPageCount,
        int NextPage,
        MetadataSyncSnapshot Snapshot,
        bool IsUpToDate,
        bool WasStopped,
        int CumulativeInsertedCount,
        int CumulativeProcessedWorkCount);

    private enum MetadataSyncBranch
    {
        RefreshExpired,
        SkipUpToDate,
        SkipLocalAhead,
        Proceed,
    }
}