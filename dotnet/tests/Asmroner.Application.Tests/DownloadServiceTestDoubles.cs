using System.Text;
using System.Collections.Concurrent;
using Asmroner.Core.Api;
using Asmroner.Core.Configuration;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Sync;
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
    private readonly Dictionary<string, byte[]> _downloadPayloads;
    private int _failWorkInfoAttempts;
    private int _inflightWorkInfo;
    private int _maxObservedWorkInfoConcurrency;
    private int _workInfoCallCount;
    private readonly ConcurrentQueue<string> _trackRequestIds = new();
    private readonly ConcurrentQueue<string> _downloadFileRequests = new();

    public int WorkInfoCallCount => _workInfoCallCount;
    public int MaxObservedWorkInfoConcurrency => _maxObservedWorkInfoConcurrency;
    public IReadOnlyList<string> TrackRequestIds => _trackRequestIds.ToArray();
    public IReadOnlyList<string> DownloadFileRequests => _downloadFileRequests.ToArray();

    public ScriptedApiClient(
        IEnumerable<string>? failOnWorkInfoIds = null,
        IEnumerable<WorkInfoDto>? workInfos = null,
        IEnumerable<TrackDto>? tracks = null,
        bool throwOnAnyWorkInfoCall = false,
        bool throwOnWorkInfo = false,
        int trackCount = 1,
        int failWorkInfoAttempts = 0,
        int workInfoDelayMilliseconds = 0,
        IReadOnlyDictionary<string, byte[]>? downloadPayloads = null)
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
        _downloadPayloads = downloadPayloads is null
            ? new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase)
            : new Dictionary<string, byte[]>(downloadPayloads, StringComparer.OrdinalIgnoreCase);
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

    public async Task DownloadFileAsync(string url, string destinationPath, CancellationToken cancellationToken = default)
    {
        _downloadFileRequests.Enqueue(url);

        var directory = Path.GetDirectoryName(destinationPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var payload = _downloadPayloads.TryGetValue(url, out var bytes)
            ? bytes
            : Encoding.UTF8.GetBytes($"payload:{url}");
        await File.WriteAllBytesAsync(destinationPath, payload, cancellationToken);
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
        string dataRoot,
        int maxWorkers = 4,
        int maxRetries = 3,
        string? preferFormats = null,
        string syncWantedSize = "5GB",
        bool hdAudioOnly = true,
        string? downloadDataFolder = null,
        string? syncDownloadDataFolder = null,
        int metadataValidityDays = 30)
    {
        _config = new AppConfig
        {
            Downloader = new DownloaderOptions
            {
                DownloadDataFolder = downloadDataFolder ?? dataRoot,
                SyncDataFolder = syncDownloadDataFolder ?? dataRoot,
                MetadataValidityDays = metadataValidityDays,
                SyncWantedSize = syncWantedSize,
                PreferFormats = preferFormats ?? "mp3,wav,flac,jpg,jpeg,png,gif,webp,mp4,mkv,avi,webm,txt,lrc,ass",
                MaxWorkers = maxWorkers,
                MaxRetries = maxRetries,
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
        DefaultDownloadDataDirectory = Path.Combine(root, "download-data");
        DefaultSyncDataDirectory = Path.Combine(root, "sync-data");
        LogsDirectory = Path.Combine(root, "logs");
    }

    public string MetadataDirectory { get; }

    public string DefaultConfigFilePath { get; }

    public string DatabaseFilePath { get; }

    public string DefaultDownloadDataDirectory { get; }

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

internal sealed class InMemoryUiStateStore : IUiStateStore
{
    private readonly IReadOnlyList<string> _emptyQueue = Array.Empty<string>();

    public SearchUiState SearchUiState { get; private set; } = new();

    public DownloadUiState DownloadUiState { get; private set; } = new();

    public MetadataSyncProgressState MetadataSyncProgress { get; private set; } = new();

    public SyncDownloadProgressState SyncDownloadProgress { get; private set; } = new();

    public IReadOnlyList<string> UnfinishedQueue { get; private set; }

    public InMemoryUiStateStore()
    {
        UnfinishedQueue = _emptyQueue;
    }

    public Task<SearchUiState> LoadSearchUiStateAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(SearchUiState);

    public Task SaveSearchUiStateAsync(SearchUiState state, CancellationToken cancellationToken = default)
    {
        SearchUiState = state;
        return Task.CompletedTask;
    }

    public Task<DownloadUiState> LoadDownloadUiStateAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(DownloadUiState);

    public Task SaveDownloadUiStateAsync(DownloadUiState state, CancellationToken cancellationToken = default)
    {
        DownloadUiState = state;
        return Task.CompletedTask;
    }

    public Task<MetadataSyncProgressState> LoadMetadataSyncProgressAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(MetadataSyncProgress);

    public Task SaveMetadataSyncProgressAsync(MetadataSyncProgressState state, CancellationToken cancellationToken = default)
    {
        if (MetadataSyncProgress.StopRequested && string.Equals(state.Status, SyncProgressStatuses.Running, StringComparison.Ordinal))
        {
            state.StopRequested = true;
        }

        if (!string.Equals(state.Status, SyncProgressStatuses.Running, StringComparison.Ordinal))
        {
            state.StopRequested = false;
        }

        MetadataSyncProgress = state;
        return Task.CompletedTask;
    }

    public Task RequestStopMetadataSyncAsync(CancellationToken cancellationToken = default)
    {
        if (string.Equals(MetadataSyncProgress.Status, SyncProgressStatuses.Running, StringComparison.Ordinal))
        {
            MetadataSyncProgress.StopRequested = true;
            MetadataSyncProgress.UpdatedAt = DateTime.UtcNow;
        }

        return Task.CompletedTask;
    }

    public Task<SyncDownloadProgressState> LoadSyncDownloadProgressAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(SyncDownloadProgress);

    public Task SaveSyncDownloadProgressAsync(SyncDownloadProgressState state, CancellationToken cancellationToken = default)
    {
        if (SyncDownloadProgress.StopRequested && string.Equals(state.Status, SyncProgressStatuses.Running, StringComparison.Ordinal))
        {
            state.StopRequested = true;
        }

        if (!string.Equals(state.Status, SyncProgressStatuses.Running, StringComparison.Ordinal))
        {
            state.StopRequested = false;
        }

        SyncDownloadProgress = state;
        return Task.CompletedTask;
    }

    public Task RequestStopSyncDownloadAsync(CancellationToken cancellationToken = default)
    {
        if (string.Equals(SyncDownloadProgress.Status, SyncProgressStatuses.Running, StringComparison.Ordinal))
        {
            SyncDownloadProgress.StopRequested = true;
            SyncDownloadProgress.UpdatedAt = DateTime.UtcNow;
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<string>> LoadUnfinishedQueueAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(UnfinishedQueue);

    public Task SaveUnfinishedQueueAsync(IReadOnlyCollection<string> sourceIds, CancellationToken cancellationToken = default)
    {
        UnfinishedQueue = sourceIds.ToArray();
        return Task.CompletedTask;
    }

    public Task ClearUnfinishedQueueAsync(CancellationToken cancellationToken = default)
    {
        UnfinishedQueue = _emptyQueue;
        return Task.CompletedTask;
    }
}

internal sealed class TestWorkInfoCache : IWorkInfoCache
{
    private readonly Dictionary<string, CachedWorkInfoEntry> _entries = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _lookupToCanonicalSourceId = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _lock = new();

    public bool TryGet(string lookupKey, WorkInfoCacheRequirement requirement, out WorkInfoDto? workInfo)
    {
        lock (_lock)
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
    }

    public void Set(string lookupKey, WorkInfoDto workInfo, WorkInfoCacheEntryLevel cacheLevel)
    {
        ArgumentNullException.ThrowIfNull(workInfo);
        lock (_lock)
        {
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
    }

    public void SetMany(IReadOnlyDictionary<string, WorkInfoDto> workInfos, WorkInfoCacheEntryLevel cacheLevel)
    {
        lock (_lock)
        {
            foreach (var (lookupKey, workInfo) in workInfos)
            {
                if (workInfo is null)
                {
                    continue;
                }

                var canonicalSourceId = SourceIdNormalizer.Normalize(workInfo.SourceId);
                if (string.IsNullOrWhiteSpace(canonicalSourceId))
                {
                    canonicalSourceId = SourceIdNormalizer.Normalize(lookupKey);
                }

                if (string.IsNullOrWhiteSpace(canonicalSourceId))
                {
                    continue;
                }

                if (_entries.TryGetValue(canonicalSourceId, out var existing) && existing.CacheLevel > cacheLevel)
                {
                    UpdateAliases(canonicalSourceId, lookupKey, existing.WorkInfo);
                    continue;
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
        }
    }

    public IReadOnlyDictionary<string, WorkInfoDto> GetSnapshot()
    {
        lock (_lock)
        {
            return _entries.ToDictionary(static item => item.Key, static item => item.Value.WorkInfo, StringComparer.OrdinalIgnoreCase);
        }
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

internal sealed class TestMetadataSyncStore : IMetadataSyncStore
{
    private readonly Dictionary<string, MetadataWorkItem> _metadataWorks = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<int, WorkSyncInfoItem> _syncInfos = [];
    private int _nextSyncInfoId = 1;

    public Task<MetadataSyncSnapshot> GetMetadataSnapshotAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new MetadataSyncSnapshot
        {
            LocalTotalCount = _metadataWorks.Count,
            LocalSubtitleCount = _metadataWorks.Values.Count(static work => work.HasSubtitle),
            LastUpdatedAt = _metadataWorks.Count == 0 ? null : _metadataWorks.Values.Max(static work => work.UpdatedAt),
        });
    }

    public Task<int> UpsertMetadataWorksAsync(IReadOnlyCollection<MetadataWorkItem> works, CancellationToken cancellationToken = default)
    {
        var insertedCount = 0;
        foreach (var work in works)
        {
            if (!_metadataWorks.ContainsKey(work.SourceId))
            {
                insertedCount++;
            }

            _metadataWorks[work.SourceId] = work;
        }

        return Task.FromResult(insertedCount);
    }

    public Task<IReadOnlyDictionary<string, MetadataWorkItem>> GetMetadataWorksBySourceIdsAsync(IReadOnlyCollection<string> sourceIds, CancellationToken cancellationToken = default)
    {
        var items = sourceIds
            .Where(static sourceId => !string.IsNullOrWhiteSpace(sourceId))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(sourceId => _metadataWorks.ContainsKey(sourceId))
            .ToDictionary(sourceId => sourceId, sourceId => _metadataWorks[sourceId], StringComparer.OrdinalIgnoreCase);
        return Task.FromResult<IReadOnlyDictionary<string, MetadataWorkItem>>(items);
    }

    public Task<IReadOnlyList<int>> GetExpiredMetadataWorkIdsAsync(DateTime updatedBefore, CancellationToken cancellationToken = default)
    {
        var items = _metadataWorks.Values
            .Where(work => work.UpdatedAt < updatedBefore)
            .Select(static work => work.Id)
            .OrderBy(static id => id)
            .ToArray();
        return Task.FromResult<IReadOnlyList<int>>(items);
    }

    public Task<IReadOnlyList<MetadataWorkItem>> GetAllMetadataWorksAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<MetadataWorkItem>>(_metadataWorks.Values.OrderBy(static work => work.Id).ToArray());
    }

    public Task<SyncDownloadSnapshot> GetDownloadSnapshotAsync(CancellationToken cancellationToken = default)
    {
        var items = _syncInfos.Values.ToArray();
        return Task.FromResult(new SyncDownloadSnapshot
        {
            PendingCount = items.Count(static item => item.Status == "PENDING"),
            CompletedCount = items.Count(static item => item.Status == "COMPLETED"),
            FailedCount = items.Count(static item => item.Status == "FAILED"),
            RemainingMetadataCount = _metadataWorks.Values.Count(work => !_syncInfos.ContainsKey(work.Id)),
            CompletedSizeBytes = items.Where(static item => item.Status == "COMPLETED").Sum(static item => item.DirSize),
            LastUpdatedAt = items.Length == 0 ? null : items.Max(static item => item.UpdatedAt),
        });
    }

    public Task<IReadOnlyDictionary<int, WorkSyncInfoItem>> GetWorkSyncInfoMapAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyDictionary<int, WorkSyncInfoItem>>(new Dictionary<int, WorkSyncInfoItem>(_syncInfos));
    }

    public Task<int> CleanupPendingSyncDownloadsAsync(CancellationToken cancellationToken = default)
    {
        var removedKeys = _syncInfos
            .Where(static pair => pair.Value.Status == "PENDING")
            .Select(static pair => pair.Key)
            .ToArray();
        foreach (var key in removedKeys)
        {
            _syncInfos.Remove(key);
        }

        return Task.FromResult(removedKeys.Length);
    }

    public Task<IReadOnlyList<MetadataWorkItem>> GetSyncDownloadCandidatesAsync(int count, CancellationToken cancellationToken = default)
    {
        var items = _metadataWorks.Values
            .Where(work => !_syncInfos.ContainsKey(work.Id))
            .OrderBy(static work => work.Id)
            .Take(count)
            .ToArray();
        return Task.FromResult<IReadOnlyList<MetadataWorkItem>>(items);
    }

    public Task<IReadOnlyList<WorkSyncInfoItem>> GetFailedSyncDownloadsAsync(CancellationToken cancellationToken = default)
    {
        var items = _syncInfos.Values
            .Where(static item => item.Status == "FAILED")
            .OrderBy(static item => item.UpdatedAt)
            .ToArray();
        return Task.FromResult<IReadOnlyList<WorkSyncInfoItem>>(items);
    }

    public Task<IReadOnlyList<WorkSyncInfoItem>> GetSyncDownloadsByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        var items = _syncInfos.Values
            .Where(item => string.Equals(item.Status, status, StringComparison.OrdinalIgnoreCase))
            .OrderBy(static item => item.UpdatedAt)
            .ToArray();
        return Task.FromResult<IReadOnlyList<WorkSyncInfoItem>>(items);
    }

    public Task<WorkSyncInfoItem> CreatePendingWorkSyncInfoAsync(MetadataWorkItem work, string filePath, CancellationToken cancellationToken = default)
    {
        var item = new WorkSyncInfoItem
        {
            Id = _nextSyncInfoId++,
            MetadataWorkId = work.Id,
            SourceId = work.SourceId,
            HasSubtitle = work.HasSubtitle,
            Status = "PENDING",
            FilePath = filePath,
            UpdatedAt = DateTime.UtcNow,
        };
        _syncInfos[work.Id] = item;
        return Task.FromResult(item);
    }

    public Task UpdateWorkSyncInfoAsync(WorkSyncInfoItem item, CancellationToken cancellationToken = default)
    {
        _syncInfos[item.MetadataWorkId] = item;
        _nextSyncInfoId = Math.Max(_nextSyncInfoId, item.Id + 1);
        return Task.CompletedTask;
    }

    public MetadataWorkItem? GetMetadataWork(string sourceId)
    {
        return _metadataWorks.TryGetValue(sourceId, out var item) ? item : null;
    }

    public WorkSyncInfoItem? GetSyncInfo(int metadataWorkId)
    {
        return _syncInfos.TryGetValue(metadataWorkId, out var item) ? item : null;
    }
}
