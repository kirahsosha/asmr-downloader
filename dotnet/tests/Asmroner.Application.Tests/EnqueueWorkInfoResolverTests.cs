using Asmroner.Application.Services;
using Asmroner.Core.Api;
using Asmroner.Core.Configuration;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Sync;

namespace Asmroner.Application.Tests;

public class EnqueueWorkInfoResolverTests
{
    [Fact]
    public async Task ResolvePreferTranslatedAsync_ShouldCreateSelectedEditionWorkInfo_WhenPreferredEditionOnlyExistsInRelatedEditions()
    {
        var apiClient = new ScriptedApiClient(workInfos:
        [
            new WorkInfoDto
            {
                SourceId = "RJ2001",
                Title = "日文原版",
                TranslationInfo = new WorkTranslationInfoDto
                {
                    IsOriginal = true,
                },
                OtherLanguageEditionsInDb =
                [
                    new WorkOtherLanguageEditionDto
                    {
                        Id = 2002,
                        SourceId = "RJ2002",
                        Lang = "简体中文",
                        Title = "简中版",
                    },
                ],
            },
        ]);
        var sut = new EnqueueWorkInfoResolver(apiClient, new StubConfigurationService(), new NoopMetadataSyncStore());

        var result = await sut.ResolvePreferTranslatedAsync([
            new EnqueueWorkInfoRequest { SourceId = "RJ2001" },
        ], TestContext.Current.CancellationToken);

        var resolved = Assert.Single(result.WorkInfos);
        Assert.Equal("RJ2002", resolved.Key);
        Assert.Equal(2002, resolved.Value.Id);
        Assert.Equal("RJ2002", resolved.Value.SourceId);
        Assert.Equal("简中版", resolved.Value.Title);
        Assert.Empty(result.FailedSourceIds);
        Assert.Equal(1, result.SwitchedSourceCount);
    }

    [Fact]
    public async Task ResolvePreferTranslatedAsync_ShouldReuseFetchedPreferredEdition_AndDeduplicateFinalSourceIds()
    {
        var apiClient = new ScriptedApiClient(workInfos:
        [
            new WorkInfoDto
            {
                SourceId = "RJ2101",
                Title = "日文原版",
                TranslationInfo = new WorkTranslationInfoDto
                {
                    IsOriginal = true,
                },
                OtherLanguageEditionsInDb =
                [
                    new WorkOtherLanguageEditionDto
                    {
                        SourceId = "RJ2102",
                        Lang = "简体中文",
                        Title = "简中版",
                    },
                ],
            },
            new WorkInfoDto
            {
                SourceId = "RJ2102",
                Title = "简中完整版",
                TranslationInfo = new WorkTranslationInfoDto
                {
                    Lang = "CHI_HANS",
                },
            },
        ]);
        var sut = new EnqueueWorkInfoResolver(apiClient, new StubConfigurationService(), new NoopMetadataSyncStore());

        var result = await sut.ResolvePreferTranslatedAsync([
            new EnqueueWorkInfoRequest { SourceId = "RJ2101" },
            new EnqueueWorkInfoRequest { SourceId = "RJ2102" },
        ], TestContext.Current.CancellationToken);

        var resolved = Assert.Single(result.WorkInfos);
        Assert.Equal("RJ2102", resolved.Key);
        Assert.Equal("简中完整版", resolved.Value.Title);
        Assert.Empty(result.FailedSourceIds);
        Assert.Equal(1, result.SwitchedSourceCount);
    }

    [Fact]
    public async Task ResolvePreferTranslatedAsync_ShouldReturnFailedSourceIds_WhenFetchFails()
    {
        var apiClient = new ScriptedApiClient(
            failOnWorkInfoIds: ["RJ2202"],
            workInfos:
            [
                new WorkInfoDto
                {
                    SourceId = "RJ2201",
                    Title = "日文原版",
                    TranslationInfo = new WorkTranslationInfoDto
                    {
                        IsOriginal = true,
                    },
                },
            ]);
        var sut = new EnqueueWorkInfoResolver(apiClient, new StubConfigurationService(), new NoopMetadataSyncStore());

        var result = await sut.ResolvePreferTranslatedAsync([
            new EnqueueWorkInfoRequest { SourceId = "RJ2201" },
            new EnqueueWorkInfoRequest { SourceId = "RJ2202" },
        ], TestContext.Current.CancellationToken);

        var resolved = Assert.Single(result.WorkInfos);
        Assert.Equal("RJ2201", resolved.Key);
        Assert.Equal(["RJ2202"], result.FailedSourceIds);
        Assert.Equal(0, result.SwitchedSourceCount);
    }

    [Fact]
    public async Task ResolvePreferTranslatedAsync_ShouldUseWorkIdWhenProvided()
    {
        var apiClient = new ScriptedApiClient(workInfos:
        [
            new WorkInfoDto
            {
                Id = 100000062,
                SourceId = "BJ02370869",
                Title = "BJ original",
                TranslationInfo = new WorkTranslationInfoDto
                {
                    IsOriginal = true,
                },
            },
        ]);
        var sut = new EnqueueWorkInfoResolver(apiClient, new StubConfigurationService(), new NoopMetadataSyncStore());

        var result = await sut.ResolvePreferTranslatedAsync([
            new EnqueueWorkInfoRequest { SourceId = "BJ02370869", WorkId = 100000062 },
        ], TestContext.Current.CancellationToken);

        var resolved = Assert.Single(result.WorkInfos);
        Assert.Equal("BJ02370869", resolved.Key);
        Assert.Equal(100000062, resolved.Value.Id);
        Assert.Empty(result.FailedSourceIds);
        Assert.Equal(0, result.SwitchedSourceCount);
    }

    [Fact]
    public async Task ResolvePreferTranslatedAsync_ShouldUseConfiguredPreferredLanguages()
    {
        var apiClient = new ScriptedApiClient(workInfos:
        [
            new WorkInfoDto
            {
                SourceId = "RJ2301",
                Title = "日文原版",
                TranslationInfo = new WorkTranslationInfoDto
                {
                    IsOriginal = true,
                },
                OtherLanguageEditionsInDb =
                [
                    new WorkOtherLanguageEditionDto { SourceId = "RJ2302", Lang = "简体中文", Title = "简中版" },
                    new WorkOtherLanguageEditionDto { SourceId = "RJ2303", Lang = "繁体中文", Title = "繁中版" },
                ],
            },
        ]);
        var configService = new StubConfigurationService("繁体中文,简体中文");
        var sut = new EnqueueWorkInfoResolver(apiClient, configService, new NoopMetadataSyncStore());

        var result = await sut.ResolvePreferTranslatedAsync([
            new EnqueueWorkInfoRequest { SourceId = "RJ2301" },
        ], TestContext.Current.CancellationToken);

        var resolved = Assert.Single(result.WorkInfos);
        Assert.Equal("RJ2303", resolved.Key);
        Assert.Equal("繁中版", resolved.Value.Title);
        Assert.Equal(1, result.SwitchedSourceCount);
    }

    private sealed class NoopMetadataSyncStore : IMetadataSyncStore
    {
        public Task<MetadataSyncSnapshot> GetMetadataSnapshotAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new MetadataSyncSnapshot());
        }

        public Task<int> UpsertMetadataWorksAsync(IReadOnlyCollection<MetadataWorkItem> works, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }

        public Task<IReadOnlyDictionary<string, MetadataWorkItem>> GetMetadataWorksBySourceIdsAsync(IReadOnlyCollection<string> sourceIds, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyDictionary<string, MetadataWorkItem>>(
                new Dictionary<string, MetadataWorkItem>(StringComparer.OrdinalIgnoreCase));
        }

        public Task<IReadOnlyList<int>> GetExpiredMetadataWorkIdsAsync(DateTime updatedBefore, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<int>>(Array.Empty<int>());
        }

        public Task<IReadOnlyList<MetadataWorkItem>> GetAllMetadataWorksAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<MetadataWorkItem>>(Array.Empty<MetadataWorkItem>());
        }

        public Task<SyncDownloadSnapshot> GetDownloadSnapshotAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new SyncDownloadSnapshot());
        }

        public Task<IReadOnlyDictionary<int, WorkSyncInfoItem>> GetWorkSyncInfoMapAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyDictionary<int, WorkSyncInfoItem>>(
                new Dictionary<int, WorkSyncInfoItem>());
        }

        public Task<int> CleanupPendingSyncDownloadsAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }

        public Task<IReadOnlyList<MetadataWorkItem>> GetSyncDownloadCandidatesAsync(int count, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<MetadataWorkItem>>(Array.Empty<MetadataWorkItem>());
        }

        public Task<IReadOnlyList<WorkSyncInfoItem>> GetFailedSyncDownloadsAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<WorkSyncInfoItem>>(Array.Empty<WorkSyncInfoItem>());
        }

        public Task<IReadOnlyList<WorkSyncInfoItem>> GetSyncDownloadsByStatusAsync(string status, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<WorkSyncInfoItem>>(Array.Empty<WorkSyncInfoItem>());
        }

        public Task<WorkSyncInfoItem> CreatePendingWorkSyncInfoAsync(MetadataWorkItem work, string filePath, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task UpdateWorkSyncInfoAsync(WorkSyncInfoItem item, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class StubConfigurationService : IConfigurationService
    {
        private readonly string _preferredLanguages;

        public StubConfigurationService(string preferredLanguages = "简体中文,繁体中文,日本語")
        {
            _preferredLanguages = preferredLanguages;
        }

        public Task<AppConfig?> LoadAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<AppConfig?>(new AppConfig
            {
                Downloader = new DownloaderOptions
                {
                    PreferredLanguages = _preferredLanguages,
                },
            });
        }

        public Task SaveAsync(AppConfig config, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public IReadOnlyList<string> Validate(AppConfig config)
        {
            return Array.Empty<string>();
        }

        public bool Exists()
        {
            return true;
        }
    }
}