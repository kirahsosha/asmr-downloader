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

    public MetadataSyncService(
        IAsmrApiClient apiClient,
        IConfigurationService configurationService,
        IMetadataSyncStore metadataSyncStore,
        IRateLimiterService rateLimiterService)
    {
        _apiClient = apiClient;
        _configurationService = configurationService;
        _metadataSyncStore = metadataSyncStore;
        _rateLimiterService = rateLimiterService;
    }

    public Task<MetadataSyncSnapshot> GetMetadataSnapshotAsync(CancellationToken cancellationToken = default)
    {
        return _metadataSyncStore.GetMetadataSnapshotAsync(cancellationToken);
    }

    public async Task<MetadataSyncRunResult> SyncMetadataAsync(CancellationToken cancellationToken = default)
    {
        var config = await _configurationService.LoadAsync(cancellationToken) ?? new AppConfig();
        var before = await _metadataSyncStore.GetMetadataSnapshotAsync(cancellationToken);

        var remoteFirstPage = await GetMetadataWorksAsync(page: 1, pageSize: 1, subtitleOnly: false, config, cancellationToken);
        var remoteSubtitlePage = await GetMetadataWorksAsync(page: 1, pageSize: 1, subtitleOnly: true, config, cancellationToken);

        var remoteTotalCount = remoteFirstPage.Pagination.TotalCount;
        var remoteSubtitleCount = remoteSubtitlePage.Pagination.TotalCount;

        if (remoteTotalCount == before.LocalTotalCount)
        {
            return BuildNoOpResult(
                remoteTotalCount,
                remoteSubtitleCount,
                before,
                message: "网站元数据与本地一致，无需同步。",
                isUpToDate: true);
        }

        if (remoteTotalCount < before.LocalTotalCount)
        {
            return BuildNoOpResult(
                remoteTotalCount,
                remoteSubtitleCount,
                before,
                message: "本地元数据数量高于网站，未执行同步。",
                isUpToDate: false);
        }

        var totalPages = Math.Max(1, (int)Math.Ceiling(remoteTotalCount / (double)SyncPageSize));
        var insertedCount = 0;

        for (var page = 1; page <= totalPages; page++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var pageResult = await GetMetadataWorksAsync(page, SyncPageSize, subtitleOnly: false, config, cancellationToken);
            var updatedAt = DateTime.UtcNow;
            var works = pageResult.Works
                .Select(work => work.ToMetadataWorkItem(updatedAt))
                .Where(static work => work.Id > 0 && !string.IsNullOrWhiteSpace(work.SourceId))
                .ToArray();

            insertedCount += await _metadataSyncStore.UpsertMetadataWorksAsync(works, cancellationToken);
        }

        var after = await _metadataSyncStore.GetMetadataSnapshotAsync(cancellationToken);
        var isUpToDate = after.LocalTotalCount == remoteTotalCount;

        return new MetadataSyncRunResult
        {
            RemoteTotalCount = remoteTotalCount,
            RemoteSubtitleCount = remoteSubtitleCount,
            LocalTotalCountBefore = before.LocalTotalCount,
            LocalSubtitleCountBefore = before.LocalSubtitleCount,
            LocalTotalCountAfter = after.LocalTotalCount,
            LocalSubtitleCountAfter = after.LocalSubtitleCount,
            InsertedCount = insertedCount,
            ProcessedPageCount = totalPages,
            TotalPageCount = totalPages,
            Message = isUpToDate
                ? $"元数据同步完成：新增 {insertedCount} 条，本地现有 {after.LocalTotalCount} 条。"
                : $"元数据同步完成，但本地数量仍为 {after.LocalTotalCount}，未完全追平网站的 {remoteTotalCount} 条。",
            IsUpToDate = isUpToDate,
        };
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
        bool isUpToDate)
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
            Message = message,
            IsUpToDate = isUpToDate,
        };
    }
}