using Asmroner.Core.Api;
using Asmroner.Core.Configuration;
using Asmroner.Core.Interfaces;

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

    public int WorkInfoCallCount { get; private set; }
    public int MaxObservedWorkInfoConcurrency => _maxObservedWorkInfoConcurrency;

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
        WorkInfoCallCount++;
        var running = Interlocked.Increment(ref _inflightWorkInfo);
        UpdateMaxConcurrency(running);

        try
        {
            if (_throwOnAnyWorkInfoCall)
            {
                throw new InvalidOperationException("work info call should not happen");
            }

            if (_failWorkInfoAttempts > 0)
            {
                _failWorkInfoAttempts--;
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

    public Task<IReadOnlyList<TrackDto>> GetTracksAsync(string id, CancellationToken cancellationToken = default)
    {
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

    public Task<IReadOnlyList<HotWorkDto>> GetPopularAsync(CancellationToken cancellationToken = default)
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
}

internal sealed class TestConfigurationService : IConfigurationService
{
    private readonly AppConfig _config;

    public TestConfigurationService(
        string syncDataFolder,
        int maxWorkers = 4,
        string? preferFormats = null,
        string preferMedia = "mp3,m4a")
    {
        _config = new AppConfig
        {
            Downloader = new DownloaderOptions
            {
                SyncDataFolder = syncDataFolder,
                PreferFormats = preferFormats ?? "mp3,wav,flac,jpg,jpeg,png,gif,webp,mp4,mkv,avi,webm,txt,lrc,ass",
                PreferMedia = preferMedia,
                MaxWorkers = maxWorkers,
            },
            Limit = new LimitOptions
            {
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
        ConfigFilePath = Path.Combine(root, "config.toml");
        DatabaseFilePath = Path.Combine(root, "asmroner.db");
        DefaultSyncDataDirectory = Path.Combine(root, "sync-data");
    }

    public string MetadataDirectory { get; }

    public string ConfigFilePath { get; }

    public string DatabaseFilePath { get; }

    public string DefaultSyncDataDirectory { get; }

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
