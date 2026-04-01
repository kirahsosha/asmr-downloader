using Asmroner.Core.Api;
using Asmroner.Core.Interfaces;
using Asmroner.Infrastructure.Services;
using Microsoft.Extensions.Caching.Memory;

namespace Asmroner.Infrastructure.Tests;

public class CachedAsmrApiClientTests
{
    [Fact]
    public async Task GetWorkInfoAsync_ShouldReturnCachedFullEntry_WithoutCallingInnerApi()
    {
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var workInfoCache = new MemoryWorkInfoCache(memoryCache);
        workInfoCache.Set("RJ3001", new WorkInfoDto
        {
            Id = 3001,
            SourceId = "RJ3001",
            Title = "Cached Full",
        }, WorkInfoCacheEntryLevel.Full);

        var inner = new RecordingInnerAsmrApiClient();
        var sut = new CachedAsmrApiClient(inner, workInfoCache);

        var result = await sut.GetWorkInfoAsync("RJ3001");

        Assert.Equal("Cached Full", result.Title);
        Assert.Empty(inner.WorkInfoRequests);
    }

    [Fact]
    public async Task SearchAsync_ShouldWarmSummaryCache_AndGetWorkInfoAsync_ShouldPromoteToFullUsingCachedId()
    {
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var workInfoCache = new MemoryWorkInfoCache(memoryCache);
        var inner = new RecordingInnerAsmrApiClient
        {
            SearchResult = new SearchResultDto
            {
                Works = new[]
                {
                    new SearchWorkDto
                    {
                        Id = 100000062,
                        SourceId = "BJ02370869",
                        Title = "Summary Title",
                        Release = "2026-04-02",
                        HasSubtitle = true,
                    },
                },
                Pagination = new SearchPaginationDto
                {
                    CurrentPage = 1,
                    PageSize = 20,
                    TotalCount = 1,
                },
            },
            WorkInfos =
            {
                ["100000062"] = new WorkInfoDto
                {
                    Id = 100000062,
                    SourceId = "BJ02370869",
                    Title = "Full Title",
                    WorkAttributes = "RG01020616,JPN,DLP",
                },
            },
        };
        var sut = new CachedAsmrApiClient(inner, workInfoCache);

        _ = await sut.SearchAsync("dummy-query");

        Assert.True(workInfoCache.TryGet("BJ02370869", WorkInfoCacheRequirement.Any, out var seededSummary));
        Assert.Equal("Summary Title", seededSummary!.Title);
        Assert.False(workInfoCache.TryGet("BJ02370869", WorkInfoCacheRequirement.Full, out _));

        var full = await sut.GetWorkInfoAsync("BJ02370869");

        Assert.Equal(["100000062"], inner.WorkInfoRequests);
        Assert.Equal("Full Title", full.Title);
        Assert.True(workInfoCache.TryGet("BJ02370869", WorkInfoCacheRequirement.Full, out var cachedFull));
        Assert.Equal("RG01020616,JPN,DLP", cachedFull!.WorkAttributes);
    }

    [Fact]
    public async Task GetPopularAsync_ShouldWarmSummaryCache_ForSubsequentTrackLookup()
    {
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var workInfoCache = new MemoryWorkInfoCache(memoryCache);
        var inner = new RecordingInnerAsmrApiClient
        {
            PopularWorks = new[]
            {
                new SearchWorkDto
                {
                    Id = 4001,
                    SourceId = "RJ4001",
                    Title = "Popular Summary",
                },
            },
        };
        var sut = new CachedAsmrApiClient(inner, workInfoCache);

        _ = await sut.GetPopularAsync();
        _ = await sut.GetTracksAsync("RJ4001");

        Assert.Equal(["4001"], inner.TrackRequests);
    }

    private sealed class RecordingInnerAsmrApiClient : IAsmrApiClient
    {
        public SearchResultDto SearchResult { get; init; } = new();

        public IReadOnlyList<SearchWorkDto> PopularWorks { get; init; } = Array.Empty<SearchWorkDto>();

        public Dictionary<string, WorkInfoDto> WorkInfos { get; } = new(StringComparer.OrdinalIgnoreCase);

        public List<string> WorkInfoRequests { get; } = new();

        public List<string> TrackRequests { get; } = new();

        public Task<WorkInfoDto> GetWorkInfoAsync(string id, CancellationToken cancellationToken = default)
        {
            WorkInfoRequests.Add(id);
            if (WorkInfos.TryGetValue(id, out var workInfo))
            {
                return Task.FromResult(workInfo);
            }

            throw new InvalidOperationException($"Unexpected work info request: {id}");
        }

        public Task<IReadOnlyList<TrackDto>> GetTracksAsync(string id, CancellationToken cancellationToken = default)
        {
            TrackRequests.Add(id);
            return Task.FromResult<IReadOnlyList<TrackDto>>(Array.Empty<TrackDto>());
        }

        public Task<SearchResultDto> SearchAsync(string query, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(SearchResult);
        }

        public Task<IReadOnlyList<SearchWorkDto>> GetPopularAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(PopularWorks);
        }
    }
}