using System.Collections.Concurrent;
using Asmroner.Core.Api;
using Asmroner.Core.Configuration;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Utils;
using System.Globalization;

namespace Asmroner.Application.Tests;

internal sealed class ScriptedApiClient : IAsmrApiClient
{
    private readonly HashSet<string> _failOnWorkInfoIds;
    private readonly Dictionary<string, WorkInfoDto> _workInfoBySourceId;
    private readonly IReadOnlyList<TrackDto> _tracks;
    private readonly int _trackCount;
    private readonly int _workInfoDelayMilliseconds;
    private readonly bool _throwOnAnyWorkInfoCall;
    private int _failWorkInfoAttempts;
    private int _inflightWorkInfo;
    private int _maxObservedWorkInfoConcurrency;
    private int _workInfoCallCount;
    private readonly ConcurrentQueue<string> _trackRequestIds = new();

    public int WorkInfoCallCount => _workInfoCallCount;
    public int MaxObservedWorkInfoConcurrency => _maxObservedWorkInfoConcurrency;
    public IReadOnlyList<string> TrackRequestIds => _trackRequestIds.ToArray();

    public ScriptedApiClient(
        IEnumerable<string>? failOnWorkInfoIds = null,
        IEnumerable<WorkInfoDto>? workInfos = null,
        IEnumerable<TrackDto>? tracks = null,
        bool throwOnAnyWorkInfoCall = false,
        bool throwOnWorkInfo = false,
        int trackCount = 1,
        int failWorkInfoAttempts = 0,
        int workInfoDelayMilliseconds = 0)
    {
        _failOnWorkInfoIds = new HashSet<string>(
            failOnWorkInfoIds ?? Array.Empty<string>(),
            StringComparer.OrdinalIgnoreCase);
        _workInfoBySourceId = (workInfos ?? Array.Empty<WorkInfoDto>())
            .Where(static item => !string.IsNullOrWhiteSpace(item.SourceId))
            .ToDictionary(static item => item.SourceId, StringComparer.OrdinalIgnoreCase);
        _tracks = tracks?.ToArray() ?? Array.Empty<TrackDto>();
        _throwOnAnyWorkInfoCall = throwOnAnyWorkInfoCall || throwOnWorkInfo;
        _trackCount = Math.Max(1, trackCount);
        _failWorkInfoAttempts = failWorkInfoAttempts;
        _workInfoDelayMilliseconds = Math.Max(0, workInfoDelayMilliseconds);
    }

    public async Task<WorkInfoDto> GetWorkInfoAsync(string id, CancellationToken cancellationToken = default)
    {
        Interlocked.Increment(ref _workInfoCallCount);
        var running = Interlocked.Increment(ref _inflightWorkInfo);
        UpdateMaxConcurrency(running);

        try
        {
            if (_throwOnAnyWorkInfoCall)
            {
                throw new InvalidOperationException("work info call should not happen");
            }

            if (TryConsumeFailureAttempt())
            {
                throw new InvalidOperationException("mock work info error");
            }

            if (_failOnWorkInfoIds.Contains(id))
            {
                throw new InvalidOperationException("mock work info error");
            }

            if (_workInfoDelayMilliseconds > 0)
            {
                await Task.Delay(_workInfoDelayMilliseconds, cancellationToken);
            }

            if (_workInfoBySourceId.TryGetValue(id, out var workInfo))
            {
                return workInfo;
            }

            var matchedByWorkId = _workInfoBySourceId.Values.FirstOrDefault(item =>
                item.Id > 0
                && string.Equals(item.Id.ToString(CultureInfo.InvariantCulture), id, StringComparison.OrdinalIgnoreCase));
            if (matchedByWorkId is not null)
            {
                return matchedByWorkId;
            }

            return new WorkInfoDto
            {
                Id = 1,
                SourceId = id,
                Title = "Default Title",
                Release = "2026-03-15",
                HasSubtitle = false,
            };
        }
        finally
        {
            Interlocked.Decrement(ref _inflightWorkInfo);
        }
    }

    public Task<MetadataSyncPageDto> GetMetadataWorksAsync(int page, int pageSize, bool subtitleOnly = false, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new MetadataSyncPageDto
        {
            Pagination = new MetadataSyncPaginationDto
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = 0,
            },
        });
    }

    public Task<IReadOnlyList<TrackDto>> GetTracksAsync(string id, CancellationToken cancellationToken = default)
    {
        _trackRequestIds.Enqueue(id);

        if (_tracks.Count > 0)
        {
            return Task.FromResult(_tracks);
        }

        var tracks = new List<TrackDto>(_trackCount);
        for (var index = 0; index < _trackCount; index++)
        {
            tracks.Add(new TrackDto
            {
                Title = $"track-{index + 1}",
                MediaDownloadUrl = index % 2 == 0
                    ? $"https://cdn.example.com/a/track-{index + 1}.mp3"
                    : $"https://cdn.example.com/a/track-{index + 1}.m4a",
            });
        }

        return Task.FromResult<IReadOnlyList<TrackDto>>(tracks);
    }

    public Task<SearchResultDto> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<SearchWorkDto>> GetPopularAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private void UpdateMaxConcurrency(int candidate)
    {
        while (true)
        {
            var observed = _maxObservedWorkInfoConcurrency;
            if (candidate <= observed)
            {
                return;
            }

            if (Interlocked.CompareExchange(ref _maxObservedWorkInfoConcurrency, candidate, observed) == observed)
            {
                return;
            }
        }
    }

    private bool TryConsumeFailureAttempt()
    {
        while (true)
        {
            var remaining = Volatile.Read(ref _failWorkInfoAttempts);
            if (remaining <= 0)
            {
                return false;
            }

            if (Interlocked.CompareExchange(ref _failWorkInfoAttempts, remaining - 1, remaining) == remaining)
            {
                return true;
            }
        }
    }
}

internal sealed class TestConfigurationService : IConfigurationService
{
    private readonly AppConfig _config;

    public TestConfigurationService(
        string syncDataFolder,
        int maxWorkers = 4,
        string? preferFormats = null,
        string syncWantedSize = "5GB",
        bool hdAudioOnly = true)
    {
        _config = new AppConfig
        {
            Downloader = new DownloaderOptions
            {
                SyncDataFolder = syncDataFolder,
                SyncWantedSize = syncWantedSize,
                PreferFormats = preferFormats ?? "mp3,wav,flac,jpg,jpeg,png,gif,webp,mp4,mkv,avi,webm,txt,lrc,ass",
                MaxWorkers = maxWorkers,
                HdAudioOnly = hdAudioOnly,
            },
            Limit = new LimitOptions
            {
                SyncQps = 1000,
                SyncJitterMin = 0,
                SyncJitterMax = 0,
                DownloadQps = 1000,
                DownloadJitterMin = 0,
                DownloadJitterMax = 0,
            },
        };
    }

    public Task<AppConfig?> LoadAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<AppConfig?>(_config);

    public Task SaveAsync(AppConfig config, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public IReadOnlyList<string> Validate(AppConfig config)
        => Array.Empty<string>();

    public bool Exists() => true;
}

internal sealed class TestAppPathService : IAppPathService
{
    public TestAppPathService(string root)
    {
        MetadataDirectory = root;
        DefaultConfigFilePath = Path.Combine(root, "config.json");
        DatabaseFilePath = Path.Combine(root, "asmroner.db");
        DefaultSyncDataDirectory = Path.Combine(root, "sync-data");
        LogsDirectory = Path.Combine(root, "logs");
    }

    public string MetadataDirectory { get; }

    public string DefaultConfigFilePath { get; }

    public string DatabaseFilePath { get; }

    public string DefaultSyncDataDirectory { get; }

    public string LogsDirectory { get; }

    public void EnsureMetadataDirectory()
    {
        Directory.CreateDirectory(MetadataDirectory);
    }
}

internal sealed class NoopRateLimiterService : IRateLimiterService
{
    public Task WaitAsync(double qps, int jitterMin, int jitterMax, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}

internal sealed class DelayRateLimiterService : IRateLimiterService
{
    private readonly int _delayMilliseconds;

    public DelayRateLimiterService(int delayMilliseconds)
    {
        _delayMilliseconds = delayMilliseconds;
    }

    public Task WaitAsync(double qps, int jitterMin, int jitterMax, CancellationToken cancellationToken = default)
        => Task.Delay(_delayMilliseconds, cancellationToken);
}

internal sealed class TestWorkInfoCache : IWorkInfoCache
{
    private readonly Dictionary<string, CachedWorkInfoEntry> _entries = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _lookupToCanonicalSourceId = new(StringComparer.OrdinalIgnoreCase);

    public bool TryGet(string lookupKey, WorkInfoCacheRequirement requirement, out WorkInfoDto? workInfo)
    {
        workInfo = null;
        if (string.IsNullOrWhiteSpace(lookupKey))
        {
            return false;
        }

        foreach (var candidate in BuildLookupKeys(lookupKey))
        {
            if (!_lookupToCanonicalSourceId.TryGetValue(candidate, out var canonicalSourceId))
            {
                continue;
            }

            if (!_entries.TryGetValue(canonicalSourceId, out var entry))
            {
                continue;
            }

            if ((int)entry.CacheLevel < (int)requirement)
            {
                return false;
            }

            workInfo = entry.WorkInfo;
            return true;
        }

        return false;
    }

    public void Set(string lookupKey, WorkInfoDto workInfo, WorkInfoCacheEntryLevel cacheLevel)
    {
        ArgumentNullException.ThrowIfNull(workInfo);

        var canonicalSourceId = SourceIdNormalizer.Normalize(workInfo.SourceId);
        if (string.IsNullOrWhiteSpace(canonicalSourceId))
        {
            canonicalSourceId = SourceIdNormalizer.Normalize(lookupKey);
        }

        if (string.IsNullOrWhiteSpace(canonicalSourceId))
        {
            return;
        }

        if (_entries.TryGetValue(canonicalSourceId, out var existing) && existing.CacheLevel > cacheLevel)
        {
            UpdateAliases(canonicalSourceId, lookupKey, existing.WorkInfo);
            return;
        }

        var normalized = new WorkInfoDto
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

        _entries[canonicalSourceId] = new CachedWorkInfoEntry(normalized, cacheLevel);
        UpdateAliases(canonicalSourceId, lookupKey, normalized);
    }

    public void SetMany(IReadOnlyDictionary<string, WorkInfoDto> workInfos, WorkInfoCacheEntryLevel cacheLevel)
    {
        foreach (var (lookupKey, workInfo) in workInfos)
        {
            if (workInfo is null)
            {
                continue;
            }

            Set(lookupKey, workInfo, cacheLevel);
        }
    }

    public IReadOnlyDictionary<string, WorkInfoDto> GetSnapshot()
    {
        return _entries.ToDictionary(static item => item.Key, static item => item.Value.WorkInfo, StringComparer.OrdinalIgnoreCase);
    }

    private void UpdateAliases(string canonicalSourceId, string lookupKey, WorkInfoDto workInfo)
    {
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

        var numericAlias = SourceIdNormalizer.ToApiNumericId(lookupKey);
        if (numericAlias.All(char.IsDigit))
        {
            AddLookupKey(keys, numericAlias);
        }

        if (workInfo is not null)
        {
            AddLookupKey(keys, workInfo.SourceId);
            AddLookupKey(keys, SourceIdNormalizer.Normalize(workInfo.SourceId));

            var sourceIdNumericAlias = SourceIdNormalizer.ToApiNumericId(workInfo.SourceId);
            if (sourceIdNumericAlias.All(char.IsDigit))
            {
                AddLookupKey(keys, sourceIdNumericAlias);
            }

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

    private sealed record CachedWorkInfoEntry(WorkInfoDto WorkInfo, WorkInfoCacheEntryLevel CacheLevel);
}
