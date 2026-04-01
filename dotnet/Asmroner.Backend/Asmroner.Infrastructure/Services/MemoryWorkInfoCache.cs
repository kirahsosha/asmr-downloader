using System.Collections.Concurrent;
using System.Globalization;
using Asmroner.Core.Api;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Utils;
using Microsoft.Extensions.Caching.Memory;

namespace Asmroner.Infrastructure.Services;

public sealed class MemoryWorkInfoCache : IWorkInfoCache
{
    private static readonly TimeSpan DefaultEntryTtl = TimeSpan.FromHours(1);

    private readonly IMemoryCache _memoryCache;
    private readonly TimeSpan _entryTtl;
    private readonly ConcurrentDictionary<string, string> _lookupToCanonicalSourceId = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, byte> _canonicalSourceIds = new(StringComparer.OrdinalIgnoreCase);

    public MemoryWorkInfoCache(IMemoryCache memoryCache)
        : this(memoryCache, DefaultEntryTtl)
    {
    }

    public MemoryWorkInfoCache(IMemoryCache memoryCache, TimeSpan entryTtl)
    {
        _memoryCache = memoryCache;
        _entryTtl = entryTtl;
    }

    public bool TryGet(string lookupKey, WorkInfoCacheRequirement requirement, out WorkInfoDto? workInfo)
    {
        workInfo = null;
        if (!TryGetEntry(lookupKey, out var entry) || entry is null)
        {
            return false;
        }

        if (!IsRequirementSatisfied(entry.Level, requirement))
        {
            return false;
        }

        workInfo = entry.WorkInfo;
        return true;
    }

    public void Set(string lookupKey, WorkInfoDto workInfo, WorkInfoCacheEntryLevel cacheLevel)
    {
        ArgumentNullException.ThrowIfNull(workInfo);

        var canonicalSourceId = ResolveCanonicalSourceId(lookupKey, workInfo);
        if (string.IsNullOrWhiteSpace(canonicalSourceId))
        {
            return;
        }

        var normalizedIncoming = NormalizeStoredWorkInfo(canonicalSourceId, workInfo);
        var cacheKey = BuildCacheKey(canonicalSourceId);
        if (_memoryCache.TryGetValue<WorkInfoCacheItem>(cacheKey, out var existing) && existing is not null)
        {
            var preferred = existing.Level > cacheLevel ? existing.WorkInfo : normalizedIncoming;
            var fallback = ReferenceEquals(preferred, normalizedIncoming) ? existing.WorkInfo : normalizedIncoming;
            var merged = new WorkInfoCacheItem(MergeWorkInfo(preferred, fallback), MaxLevel(existing.Level, cacheLevel));
            _memoryCache.Set(cacheKey, merged, _entryTtl);
            UpdateAliases(canonicalSourceId, lookupKey, merged.WorkInfo);
            return;
        }

        var created = new WorkInfoCacheItem(normalizedIncoming, cacheLevel);
        _memoryCache.Set(cacheKey, created, _entryTtl);
        UpdateAliases(canonicalSourceId, lookupKey, normalizedIncoming);
    }

    public void SetMany(IReadOnlyDictionary<string, WorkInfoDto> workInfos, WorkInfoCacheEntryLevel cacheLevel)
    {
        ArgumentNullException.ThrowIfNull(workInfos);

        foreach (var (lookupKey, workInfo) in workInfos)
        {
            if (string.IsNullOrWhiteSpace(lookupKey) || workInfo is null)
            {
                continue;
            }

            Set(lookupKey, workInfo, cacheLevel);
        }
    }

    public IReadOnlyDictionary<string, WorkInfoDto> GetSnapshot()
    {
        var snapshot = new Dictionary<string, WorkInfoDto>(StringComparer.OrdinalIgnoreCase);
        foreach (var canonicalSourceId in _canonicalSourceIds.Keys)
        {
            if (_memoryCache.TryGetValue<WorkInfoCacheItem>(BuildCacheKey(canonicalSourceId), out var entry) && entry is not null)
            {
                snapshot[canonicalSourceId] = entry.WorkInfo;
                continue;
            }

            _canonicalSourceIds.TryRemove(canonicalSourceId, out _);
        }

        return snapshot;
    }

    private bool TryGetEntry(string lookupKey, out WorkInfoCacheItem? entry)
    {
        entry = null;
        foreach (var candidate in BuildLookupKeys(lookupKey))
        {
            if (!_lookupToCanonicalSourceId.TryGetValue(candidate, out var canonicalSourceId))
            {
                continue;
            }

            if (_memoryCache.TryGetValue<WorkInfoCacheItem>(BuildCacheKey(canonicalSourceId), out entry) && entry is not null)
            {
                return true;
            }
        }

        return false;
    }

    private void UpdateAliases(string canonicalSourceId, string lookupKey, WorkInfoDto workInfo)
    {
        _canonicalSourceIds[canonicalSourceId] = 0;

        foreach (var alias in BuildLookupKeys(lookupKey, workInfo))
        {
            _lookupToCanonicalSourceId[alias] = canonicalSourceId;
        }
    }

    private static IReadOnlyCollection<string> BuildLookupKeys(string lookupKey, WorkInfoDto? workInfo = null)
    {
        var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        AddLookupKey(keys, lookupKey);
        AddLookupKey(keys, SourceIdNormalizer.Normalize(lookupKey));
        AddLookupKey(keys, ExtractNumericAlias(lookupKey));

        if (workInfo is not null)
        {
            AddLookupKey(keys, workInfo.SourceId);
            AddLookupKey(keys, SourceIdNormalizer.Normalize(workInfo.SourceId));
            AddLookupKey(keys, ExtractNumericAlias(workInfo.SourceId));

            if (workInfo.Id > 0)
            {
                AddLookupKey(keys, workInfo.Id.ToString(CultureInfo.InvariantCulture));
            }
        }

        return keys;
    }

    private static void AddLookupKey(ISet<string> keys, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        keys.Add(value.Trim());
    }

    private static string ResolveCanonicalSourceId(string lookupKey, WorkInfoDto workInfo)
    {
        var sourceId = SourceIdNormalizer.Normalize(workInfo.SourceId);
        if (!string.IsNullOrWhiteSpace(sourceId))
        {
            return sourceId;
        }

        return SourceIdNormalizer.Normalize(lookupKey);
    }

    private static WorkInfoDto NormalizeStoredWorkInfo(string canonicalSourceId, WorkInfoDto workInfo)
    {
        if (string.Equals(workInfo.SourceId, canonicalSourceId, StringComparison.OrdinalIgnoreCase))
        {
            return workInfo;
        }

        return new WorkInfoDto
        {
            Id = workInfo.Id,
            Title = workInfo.Title,
            Release = workInfo.Release,
            HasSubtitle = workInfo.HasSubtitle,
            SourceId = canonicalSourceId,
            MainCoverUrl = workInfo.MainCoverUrl,
            WorkAttributes = workInfo.WorkAttributes,
            LanguageEditions = workInfo.LanguageEditions,
            OtherLanguageEditionsInDb = workInfo.OtherLanguageEditionsInDb,
            TranslationInfo = workInfo.TranslationInfo,
        };
    }

    private static WorkInfoDto MergeWorkInfo(WorkInfoDto preferred, WorkInfoDto fallback)
    {
        return new WorkInfoDto
        {
            Id = preferred.Id > 0 ? preferred.Id : fallback.Id,
            Title = string.IsNullOrWhiteSpace(preferred.Title) ? fallback.Title : preferred.Title,
            Release = string.IsNullOrWhiteSpace(preferred.Release) ? fallback.Release : preferred.Release,
            HasSubtitle = preferred.HasSubtitle,
            SourceId = string.IsNullOrWhiteSpace(preferred.SourceId) ? fallback.SourceId : preferred.SourceId,
            MainCoverUrl = string.IsNullOrWhiteSpace(preferred.MainCoverUrl) ? fallback.MainCoverUrl : preferred.MainCoverUrl,
            WorkAttributes = string.IsNullOrWhiteSpace(preferred.WorkAttributes) ? fallback.WorkAttributes : preferred.WorkAttributes,
            LanguageEditions = preferred.LanguageEditions.Count > 0 ? preferred.LanguageEditions : fallback.LanguageEditions,
            OtherLanguageEditionsInDb = preferred.OtherLanguageEditionsInDb.Count > 0 ? preferred.OtherLanguageEditionsInDb : fallback.OtherLanguageEditionsInDb,
            TranslationInfo = HasTranslationInfo(preferred.TranslationInfo) ? preferred.TranslationInfo : fallback.TranslationInfo,
        };
    }

    private static bool HasTranslationInfo(WorkTranslationInfoDto translationInfo)
    {
        return translationInfo.IsOriginal || !string.IsNullOrWhiteSpace(translationInfo.Lang);
    }

    private static bool IsRequirementSatisfied(WorkInfoCacheEntryLevel cacheLevel, WorkInfoCacheRequirement requirement)
    {
        return (int)cacheLevel >= (int)requirement;
    }

    private static WorkInfoCacheEntryLevel MaxLevel(WorkInfoCacheEntryLevel left, WorkInfoCacheEntryLevel right)
    {
        return left >= right ? left : right;
    }

    private static string BuildCacheKey(string canonicalSourceId)
    {
        return string.Create(CultureInfo.InvariantCulture, $"workinfo:{canonicalSourceId}");
    }

    private static string ExtractNumericAlias(string lookupKey)
    {
        var numeric = SourceIdNormalizer.ToApiNumericId(lookupKey);
        return numeric.All(char.IsDigit) ? numeric : string.Empty;
    }

    private sealed record WorkInfoCacheItem(WorkInfoDto WorkInfo, WorkInfoCacheEntryLevel Level);
}