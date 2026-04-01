using Asmroner.Core.Api;
using Asmroner.Core.Interfaces;
using Asmroner.Infrastructure.Services;
using Microsoft.Extensions.Caching.Memory;

namespace Asmroner.Infrastructure.Tests;

public class MemoryWorkInfoCacheTests
{
    [Fact]
    public void TryGet_ShouldRequireFullEntry_WhenRequirementIsFull()
    {
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var sut = new MemoryWorkInfoCache(memoryCache);

        sut.Set("RJ1001", new WorkInfoDto
        {
            Id = 1001,
            SourceId = "RJ1001",
            Title = "Summary",
        }, WorkInfoCacheEntryLevel.Summary);

        Assert.True(sut.TryGet("RJ1001", WorkInfoCacheRequirement.Any, out var summary));
        Assert.Equal("Summary", summary!.Title);
        Assert.False(sut.TryGet("RJ1001", WorkInfoCacheRequirement.Full, out _));
    }

    [Fact]
    public void Set_ShouldPreserveFullEntry_WhenSummaryArrivesLater()
    {
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var sut = new MemoryWorkInfoCache(memoryCache);

        sut.Set("RJ1002", new WorkInfoDto
        {
            Id = 1002,
            SourceId = "RJ1002",
            Title = "Full Title",
            Release = "2026-04-02",
            WorkAttributes = "RG01020616,JPN,DLP",
            TranslationInfo = new WorkTranslationInfoDto
            {
                IsOriginal = true,
            },
        }, WorkInfoCacheEntryLevel.Full);

        sut.Set("RJ1002", new WorkInfoDto
        {
            Id = 1002,
            SourceId = "RJ1002",
            Title = "Summary Title",
        }, WorkInfoCacheEntryLevel.Summary);

        Assert.True(sut.TryGet("RJ1002", WorkInfoCacheRequirement.Full, out var cached));
        Assert.Equal("Full Title", cached!.Title);
        Assert.Equal("RG01020616,JPN,DLP", cached.WorkAttributes);
        Assert.True(cached.TranslationInfo.IsOriginal);
    }

    [Fact]
    public void TryGet_ShouldResolveNumericAlias_WhenWorkIdKnown()
    {
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var sut = new MemoryWorkInfoCache(memoryCache);

        sut.Set("BJ02370869", new WorkInfoDto
        {
            Id = 100000062,
            SourceId = "BJ02370869",
            Title = "BJ Full",
        }, WorkInfoCacheEntryLevel.Full);

        Assert.True(sut.TryGet("100000062", WorkInfoCacheRequirement.Full, out var cached));
        Assert.Equal("BJ02370869", cached!.SourceId);
        Assert.Equal("BJ Full", cached.Title);
    }

    [Fact]
    public async Task Entries_ShouldExpirePerItem_AfterConfiguredTtl()
    {
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var sut = new MemoryWorkInfoCache(memoryCache, TimeSpan.FromMilliseconds(200));

        sut.Set("RJ2001", new WorkInfoDto
        {
            Id = 2001,
            SourceId = "RJ2001",
            Title = "First",
        }, WorkInfoCacheEntryLevel.Full);

        await Task.Delay(150);

        sut.Set("RJ2002", new WorkInfoDto
        {
            Id = 2002,
            SourceId = "RJ2002",
            Title = "Second",
        }, WorkInfoCacheEntryLevel.Full);

        await Task.Delay(100);

        Assert.False(sut.TryGet("RJ2001", WorkInfoCacheRequirement.Full, out _));
        Assert.True(sut.TryGet("RJ2002", WorkInfoCacheRequirement.Full, out var cached));
        Assert.Equal("Second", cached!.Title);
    }
}