using Asmroner.Application.Services;
using Asmroner.Core.Configuration;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Sync;

namespace Asmroner.Application.Tests;

public class LibraryScannerServiceTests
{
    [Fact]
    public async Task ScanAsync_ShouldCollectBracketedAndLegacyDirectories_AndOverlayMetadata()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var tempRoot = CreateTempRoot();
        try
        {
            var downloadRoot = Path.Combine(tempRoot, "download");
            var syncRoot = Path.Combine(tempRoot, "sync");
            Directory.CreateDirectory(downloadRoot);
            Directory.CreateDirectory(syncRoot);

            var bracketedDirectory = Path.Combine(downloadRoot, "[RJ1001]Bracket Title");
            Directory.CreateDirectory(Path.Combine(bracketedDirectory, "disc1"));
            await File.WriteAllTextAsync(Path.Combine(bracketedDirectory, "track01.mp3"), "audio", cancellationToken);
            await File.WriteAllTextAsync(Path.Combine(bracketedDirectory, "cover.jpg"), "image", cancellationToken);
            await File.WriteAllTextAsync(Path.Combine(bracketedDirectory, "disc1", "track02.wav"), "audio", cancellationToken);

            var legacyDirectory = Path.Combine(syncRoot, "RJ1002-20240203-sub-LegacyTitle");
            Directory.CreateDirectory(legacyDirectory);
            await File.WriteAllTextAsync(Path.Combine(legacyDirectory, "readme.txt"), "note", cancellationToken);

            var invalidDirectory = Path.Combine(downloadRoot, "invalid-directory-name");
            Directory.CreateDirectory(invalidDirectory);

            var sut = new LibraryScannerService(
                new StaticConfigurationService(downloadRoot, syncRoot),
                new StaticMetadataSyncStore(
                [
                    new MetadataWorkItem
                    {
                        Id = 1001,
                        SourceId = "RJ1001",
                        Title = "Metadata Title",
                        Release = "2024-05-06",
                        HasSubtitle = true,
                        Tags = "voice,healing",
                        UpdatedAt = DateTime.UtcNow,
                    },
                ]));

            var result = await sut.ScanAsync(cancellationToken);

            Assert.Equal(2, result.ScannedRootCount);
            Assert.Empty(result.Errors);
            Assert.Contains(invalidDirectory, result.SkippedDirectories, StringComparer.OrdinalIgnoreCase);
            Assert.Equal(2, result.Items.Count);

            Assert.Collection(result.Items,
                first =>
                {
                    Assert.Equal("RJ1001", first.SourceId);
                    Assert.Equal(1001, first.WorkId);
                    Assert.Equal("Metadata Title", first.Title);
                    Assert.Equal("2024-05-06", first.Release);
                    Assert.True(first.HasSubtitle);
                    Assert.Equal("voice,healing", first.Tags);
                    Assert.Equal(3, first.TotalFileCount);
                    Assert.Equal(2, first.AudioFileCount);
                    Assert.Equal("Bracketed", first.DirectoryScheme);
                    Assert.Contains(first.Files, static item => item.IsDirectory && item.Name == "disc1");
                    Assert.Contains(first.Files, static item => !item.IsDirectory && item.IsPlayable && item.Name == "track01.mp3");
                },
                second =>
                {
                    Assert.Equal("RJ1002", second.SourceId);
                    Assert.Equal("LegacyTitle", second.Title);
                    Assert.Equal("2024-02-03", second.Release);
                    Assert.True(second.HasSubtitle);
                    Assert.Equal(1, second.TotalFileCount);
                    Assert.Equal(0, second.AudioFileCount);
                    Assert.Equal("LegacyListen", second.DirectoryScheme);
                });
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    [Fact]
    public async Task ScanAsync_ShouldPreferDuplicateWorkEntry_WithMorePlayableFiles()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var tempRoot = CreateTempRoot();
        try
        {
            var downloadRoot = Path.Combine(tempRoot, "download");
            var syncRoot = Path.Combine(tempRoot, "sync");
            Directory.CreateDirectory(downloadRoot);
            Directory.CreateDirectory(syncRoot);

            var downloadDirectory = Path.Combine(downloadRoot, "[RJ2001]Download Title");
            Directory.CreateDirectory(downloadDirectory);
            await File.WriteAllTextAsync(Path.Combine(downloadDirectory, "note.txt"), "text", cancellationToken);

            var syncDirectory = Path.Combine(syncRoot, "[RJ2001]Sync Title");
            Directory.CreateDirectory(syncDirectory);
            await File.WriteAllTextAsync(Path.Combine(syncDirectory, "track01.mp3"), "audio", cancellationToken);
            await File.WriteAllTextAsync(Path.Combine(syncDirectory, "track02.flac"), "audio", cancellationToken);

            var sut = new LibraryScannerService(
                new StaticConfigurationService(downloadRoot, syncRoot),
                new StaticMetadataSyncStore(Array.Empty<MetadataWorkItem>()));

            var result = await sut.ScanAsync(cancellationToken);

            var item = Assert.Single(result.Items);
            Assert.Equal("RJ2001", item.SourceId);
            Assert.Equal("Sync Title", item.Title);
            Assert.Equal(syncRoot, item.SourceRoot);
            Assert.Equal(syncDirectory, item.RootDirectory);
            Assert.Equal(2, item.TotalFileCount);
            Assert.Equal(2, item.AudioFileCount);
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    [Fact]
    public async Task ScanAsync_ShouldKeepReadableFiles_WhenNestedDirectoryEnumerationThrows()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var tempRoot = CreateTempRoot();
        try
        {
            var downloadRoot = Path.Combine(tempRoot, "download");
            var syncRoot = Path.Combine(tempRoot, "sync");
            Directory.CreateDirectory(downloadRoot);
            Directory.CreateDirectory(syncRoot);

            var workDirectory = Path.Combine(downloadRoot, "[RJ3001]Partial Work");
            var brokenDirectory = Path.Combine(workDirectory, "disc2");
            Directory.CreateDirectory(workDirectory);
            Directory.CreateDirectory(brokenDirectory);
            await File.WriteAllTextAsync(Path.Combine(workDirectory, "track01.mp3"), "audio", cancellationToken);
            await File.WriteAllTextAsync(Path.Combine(brokenDirectory, "track02.flac"), "audio", cancellationToken);

            var sut = new LibraryScannerService(
                new StaticConfigurationService(downloadRoot, syncRoot),
                new StaticMetadataSyncStore(Array.Empty<MetadataWorkItem>()),
                Directory.Exists,
                path =>
                {
                    if (string.Equals(path, brokenDirectory, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new IOException("拒绝访问。");
                    }

                    return Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
                },
                static path => Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly),
                static path => new FileInfo(path).Length);

            var result = await sut.ScanAsync(cancellationToken);

            var item = Assert.Single(result.Items);
            Assert.Equal("RJ3001", item.SourceId);
            Assert.Equal(2, item.TotalFileCount);
            Assert.Equal(2, item.AudioFileCount);
            var nestedDirectory = Assert.Single(item.Files.Where(static file => file.IsDirectory && file.Name == "disc2"));
            Assert.Contains(item.Files, static file => !file.IsDirectory && file.Name == "track01.mp3");
            Assert.Contains(nestedDirectory.Children, static file => !file.IsDirectory && file.Name == "track02.flac");
            Assert.Contains(result.Errors, error => error.Contains("读取目录失败：disc2", StringComparison.Ordinal));
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    [Fact]
    public async Task ScanAsync_ShouldSkipUnreadableFile_WhenFileInspectionThrows()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var tempRoot = CreateTempRoot();
        try
        {
            var downloadRoot = Path.Combine(tempRoot, "download");
            var syncRoot = Path.Combine(tempRoot, "sync");
            Directory.CreateDirectory(downloadRoot);
            Directory.CreateDirectory(syncRoot);

            var workDirectory = Path.Combine(downloadRoot, "[RJ3002]Unreadable File Work");
            Directory.CreateDirectory(workDirectory);
            var readableFile = Path.Combine(workDirectory, "track01.mp3");
            var unreadableFile = Path.Combine(workDirectory, "track02.flac");
            await File.WriteAllTextAsync(readableFile, "audio", cancellationToken);
            await File.WriteAllTextAsync(unreadableFile, "audio", cancellationToken);

            var sut = new LibraryScannerService(
                new StaticConfigurationService(downloadRoot, syncRoot),
                new StaticMetadataSyncStore(Array.Empty<MetadataWorkItem>()),
                Directory.Exists,
                static path => Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly),
                static path => Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly),
                path =>
                {
                    if (string.Equals(path, unreadableFile, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new IOException("文件被占用。");
                    }

                    return new FileInfo(path).Length;
                });

            var result = await sut.ScanAsync(cancellationToken);

            var item = Assert.Single(result.Items);
            Assert.Equal("RJ3002", item.SourceId);
            Assert.Equal(1, item.TotalFileCount);
            Assert.Equal(1, item.AudioFileCount);
            Assert.Contains(item.Files, static file => file.Name == "track01.mp3");
            Assert.DoesNotContain(item.Files, static file => file.Name == "track02.flac");
            Assert.Contains(result.Errors, error => error.Contains("读取文件失败：track02.flac", StringComparison.Ordinal));
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    [Fact]
    public async Task ScanAsync_ShouldTreatSupportedExtensionsAsPlayable_IgnoringCase()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var tempRoot = CreateTempRoot();
        try
        {
            var downloadRoot = Path.Combine(tempRoot, "download");
            var syncRoot = Path.Combine(tempRoot, "sync");
            Directory.CreateDirectory(downloadRoot);
            Directory.CreateDirectory(syncRoot);

            var workDirectory = Path.Combine(downloadRoot, "[RJ3003]Case Extension Work");
            var nestedDirectory = Path.Combine(workDirectory, "disc1");
            Directory.CreateDirectory(workDirectory);
            Directory.CreateDirectory(nestedDirectory);
            await File.WriteAllTextAsync(Path.Combine(workDirectory, "track01.FLAC"), "audio", cancellationToken);
            await File.WriteAllTextAsync(Path.Combine(nestedDirectory, "TRACK02.OpUs"), "audio", cancellationToken);
            await File.WriteAllTextAsync(Path.Combine(workDirectory, "note.TXT"), "text", cancellationToken);

            var sut = new LibraryScannerService(
                new StaticConfigurationService(downloadRoot, syncRoot),
                new StaticMetadataSyncStore(Array.Empty<MetadataWorkItem>()));

            var result = await sut.ScanAsync(cancellationToken);

            var item = Assert.Single(result.Items);
            Assert.Equal("RJ3003", item.SourceId);
            Assert.Equal(3, item.TotalFileCount);
            Assert.Equal(2, item.AudioFileCount);
            Assert.Empty(result.Errors);
            Assert.Contains(item.Files, static file => !file.IsDirectory && file.Name == "track01.FLAC" && file.IsPlayable);
            var nestedDirectoryItem = Assert.Single(item.Files.Where(static file => file.IsDirectory && file.Name == "disc1"));
            Assert.Contains(nestedDirectoryItem.Children, static file => !file.IsDirectory && file.Name == "TRACK02.OpUs" && file.IsPlayable);
            Assert.Contains(item.Files, static file => !file.IsDirectory && file.Name == "note.TXT" && !file.IsPlayable);
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    private static string CreateTempRoot()
    {
        var path = Path.Combine(Path.GetTempPath(), "asmroner-library-scanner-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }

    private static void CleanupTempRoot(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }

    private sealed class StaticConfigurationService : IConfigurationService
    {
        private readonly AppConfig _config;

        public StaticConfigurationService(string downloadRoot, string syncRoot)
        {
            _config = new AppConfig
            {
                Downloader = new DownloaderOptions
                {
                    DownloadDataFolder = downloadRoot,
                    SyncDataFolder = syncRoot,
                },
            };
        }

        public Task<AppConfig?> LoadAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<AppConfig?>(_config);
        }

        public Task SaveAsync(AppConfig config, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
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

    private sealed class StaticMetadataSyncStore : IMetadataSyncStore
    {
        private readonly IReadOnlyList<MetadataWorkItem> _items;

        public StaticMetadataSyncStore(IReadOnlyList<MetadataWorkItem> items)
        {
            _items = items;
        }

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
            return Task.FromResult(_items);
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
}