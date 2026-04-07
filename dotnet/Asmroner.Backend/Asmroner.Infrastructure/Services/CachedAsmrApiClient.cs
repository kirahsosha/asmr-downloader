using System.Globalization;
using Asmroner.Core.Api;
using Asmroner.Core.Interfaces;

namespace Asmroner.Infrastructure.Services;

public sealed class CachedAsmrApiClient : IAsmrApiClient
{
    private readonly IAsmrApiClient _inner;
    private readonly IWorkInfoCache _workInfoCache;

    public CachedAsmrApiClient(IAsmrApiClient inner, IWorkInfoCache workInfoCache)
    {
        _inner = inner;
        _workInfoCache = workInfoCache;
    }

    public Task<MetadataSyncPageDto> GetMetadataWorksAsync(int page, int pageSize, bool subtitleOnly = false, CancellationToken cancellationToken = default)
    {
        return _inner.GetMetadataWorksAsync(page, pageSize, subtitleOnly, cancellationToken);
    }

    public async Task<WorkInfoDto> GetWorkInfoAsync(string id, CancellationToken cancellationToken = default)
    {
        if (_workInfoCache.TryGet(id, WorkInfoCacheRequirement.Full, out var cachedWorkInfo) && cachedWorkInfo is not null)
        {
            return cachedWorkInfo;
        }

        var lookupId = id;
        if (_workInfoCache.TryGet(id, WorkInfoCacheRequirement.Any, out var seededWorkInfo) && seededWorkInfo is not null && seededWorkInfo.Id > 0)
        {
            lookupId = seededWorkInfo.Id.ToString(CultureInfo.InvariantCulture);
        }

        var workInfo = await _inner.GetWorkInfoAsync(lookupId, cancellationToken);
        _workInfoCache.Set(id, workInfo, WorkInfoCacheEntryLevel.Full);
        return workInfo;
    }

    public async Task<IReadOnlyList<TrackDto>> GetTracksAsync(string id, CancellationToken cancellationToken = default)
    {
        if (_workInfoCache.TryGet(id, WorkInfoCacheRequirement.Any, out var cachedWorkInfo) && cachedWorkInfo is not null && cachedWorkInfo.Id > 0)
        {
            return await _inner.GetTracksAsync(cachedWorkInfo.Id.ToString(CultureInfo.InvariantCulture), cancellationToken);
        }

        return await _inner.GetTracksAsync(id, cancellationToken);
    }

    public Task DownloadFileAsync(string url, string destinationPath, CancellationToken cancellationToken = default)
    {
        return _inner.DownloadFileAsync(url, destinationPath, cancellationToken);
    }

    public async Task<SearchResultDto> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        var result = await _inner.SearchAsync(query, cancellationToken);
        WarmSummaries(result.Works);
        return result;
    }

    public async Task<IReadOnlyList<SearchWorkDto>> GetPopularAsync(CancellationToken cancellationToken = default)
    {
        var works = await _inner.GetPopularAsync(cancellationToken);
        WarmSummaries(works);
        return works;
    }

    private void WarmSummaries(IReadOnlyList<SearchWorkDto> works)
    {
        if (works.Count == 0)
        {
            return;
        }

        var summaries = works
            .Where(static work => !string.IsNullOrWhiteSpace(work.SourceId))
            .ToDictionary(
                static work => work.SourceId,
                static work => new WorkInfoDto
                {
                    Id = work.Id,
                    SourceId = work.SourceId,
                    Title = work.Title,
                    Release = work.Release,
                    HasSubtitle = work.HasSubtitle,
                },
                StringComparer.OrdinalIgnoreCase);

        _workInfoCache.SetMany(summaries, WorkInfoCacheEntryLevel.Summary);
    }
}