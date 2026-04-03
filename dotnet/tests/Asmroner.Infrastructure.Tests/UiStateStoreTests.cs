using Asmroner.Core.Configuration;
using Asmroner.Core.Sync;
using Asmroner.Infrastructure.Services;

namespace Asmroner.Infrastructure.Tests;

public class UiStateStoreTests
{
    [Fact]
    public async Task SaveSearchUiStateAsync_ShouldRoundTrip()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var pathService = new AppPathService(tempRoot);
            var store = new UiStateStore(pathService);

            var expected = new SearchUiState
            {
                IncludeTranslationWorks = false,
                QueueTranslationWorks = false,
                Tag = "tag-a,tag-b",
                TagExclude = true,
                Circle = "circle-a",
                Lang = "zh-CN",
            };

            await store.SaveSearchUiStateAsync(expected);
            var actual = await store.LoadSearchUiStateAsync();

            Assert.False(actual.IncludeTranslationWorks);
            Assert.False(actual.QueueTranslationWorks);
            Assert.Equal("tag-a,tag-b", actual.Tag);
            Assert.True(actual.TagExclude);
            Assert.Equal("circle-a", actual.Circle);
            Assert.Equal("zh-CN", actual.Lang);
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    [Fact]
    public async Task SaveDownloadUiStateAsync_ShouldRoundTrip()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var pathService = new AppPathService(tempRoot);
            var store = new UiStateStore(pathService);

            var expected = new DownloadUiState
            {
                FileFilter = "+voice;-cover",
                HdAudioOnly = false,
                QueueTranslationWorks = false,
            };

            await store.SaveDownloadUiStateAsync(expected);
            var actual = await store.LoadDownloadUiStateAsync();

            Assert.Equal("+voice;-cover", actual.FileFilter);
            Assert.False(actual.HdAudioOnly);
            Assert.False(actual.QueueTranslationWorks);
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    [Fact]
    public async Task LoadDownloadUiStateAsync_ShouldReturnDefaults_WhenNoUiStatePersisted()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var pathService = new AppPathService(tempRoot);
            var store = new UiStateStore(pathService);

            var actual = await store.LoadDownloadUiStateAsync();

            Assert.Equal(string.Empty, actual.FileFilter);
            Assert.True(actual.HdAudioOnly);
            Assert.True(actual.QueueTranslationWorks);
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    [Fact]
    public async Task SaveUnfinishedQueueAsync_ShouldNormalizeDeduplicate_AndClear()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var pathService = new AppPathService(tempRoot);
            var store = new UiStateStore(pathService);

            await store.SaveUnfinishedQueueAsync(new[]
            {
                "RJ1001",
                "rj1001",
                "https://api.asmr.one/api/work/RJ1002",
                " ",
                "RJ-1003",
            });

            var loaded = await store.LoadUnfinishedQueueAsync();
            Assert.Equal(new[] { "RJ1001", "RJ1002", "RJ1003" }, loaded);

            await store.ClearUnfinishedQueueAsync();
            var cleared = await store.LoadUnfinishedQueueAsync();
            Assert.Empty(cleared);
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    [Fact]
    public async Task SaveMetadataSyncProgressAsync_ShouldRoundTrip_AndPreserveStopRequest()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var pathService = new AppPathService(tempRoot);
            var store = new UiStateStore(pathService);

            await store.SaveMetadataSyncProgressAsync(new MetadataSyncProgressState
            {
                Status = SyncProgressStatuses.Running,
                NextPage = 3,
                ProcessedPageCount = 2,
                TotalPageCount = 10,
                RemoteTotalCount = 500,
                RemoteSubtitleCount = 30,
                InsertedCount = 200,
                StartedAt = DateTime.UtcNow.AddMinutes(-5),
                UpdatedAt = DateTime.UtcNow.AddMinutes(-1),
            });

            await store.RequestStopMetadataSyncAsync();
            await store.SaveMetadataSyncProgressAsync(new MetadataSyncProgressState
            {
                Status = SyncProgressStatuses.Running,
                NextPage = 4,
                ProcessedPageCount = 3,
                TotalPageCount = 10,
                RemoteTotalCount = 500,
                RemoteSubtitleCount = 30,
                InsertedCount = 280,
                UpdatedAt = DateTime.UtcNow,
            });

            var actual = await store.LoadMetadataSyncProgressAsync();

            Assert.Equal(SyncProgressStatuses.Running, actual.Status);
            Assert.True(actual.StopRequested);
            Assert.Equal(4, actual.NextPage);
            Assert.Equal(3, actual.ProcessedPageCount);
            Assert.Equal(10, actual.TotalPageCount);
            Assert.Equal(500, actual.RemoteTotalCount);
            Assert.Equal(30, actual.RemoteSubtitleCount);
            Assert.Equal(280, actual.InsertedCount);

            await store.SaveMetadataSyncProgressAsync(new MetadataSyncProgressState
            {
                Status = SyncProgressStatuses.Completed,
                NextPage = 1,
                ProcessedPageCount = 10,
                TotalPageCount = 10,
                RemoteTotalCount = 500,
                RemoteSubtitleCount = 30,
                InsertedCount = 500,
                UpdatedAt = DateTime.UtcNow,
            });

            var completed = await store.LoadMetadataSyncProgressAsync();
            Assert.Equal(SyncProgressStatuses.Completed, completed.Status);
            Assert.False(completed.StopRequested);
            Assert.Equal(1, completed.NextPage);
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    [Fact]
    public async Task LoadMetadataSyncProgressAsync_ShouldReturnDefaults_WhenNoStatePersisted()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var pathService = new AppPathService(tempRoot);
            var store = new UiStateStore(pathService);

            var actual = await store.LoadMetadataSyncProgressAsync();

            Assert.Equal(SyncProgressStatuses.Idle, actual.Status);
            Assert.False(actual.StopRequested);
            Assert.Equal(1, actual.NextPage);
            Assert.Equal(0, actual.ProcessedPageCount);
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    [Fact]
    public async Task SaveSyncDownloadProgressAsync_ShouldRoundTrip_AndPreserveStopRequest()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var pathService = new AppPathService(tempRoot);
            var store = new UiStateStore(pathService);

            await store.SaveSyncDownloadProgressAsync(new SyncDownloadProgressState
            {
                Status = SyncProgressStatuses.Running,
                LastProcessedSourceId = "RJ1001",
                ProcessedCount = 1,
                CompletedCount = 1,
                FailedCount = 0,
                RemainingMetadataCountAfter = 9,
                CompletedSizeBytesBefore = 0,
                CompletedSizeBytesAfter = 2048,
                SizeLimitBytes = 4096,
                StartedAt = DateTime.UtcNow.AddMinutes(-3),
                UpdatedAt = DateTime.UtcNow.AddMinutes(-1),
            });

            await store.RequestStopSyncDownloadAsync();
            await store.SaveSyncDownloadProgressAsync(new SyncDownloadProgressState
            {
                Status = SyncProgressStatuses.Running,
                LastProcessedSourceId = "RJ1002",
                ProcessedCount = 2,
                CompletedCount = 1,
                FailedCount = 1,
                RemainingMetadataCountAfter = 8,
                CompletedSizeBytesBefore = 0,
                CompletedSizeBytesAfter = 2048,
                SizeLimitBytes = 4096,
                UpdatedAt = DateTime.UtcNow,
            });

            var actual = await store.LoadSyncDownloadProgressAsync();

            Assert.Equal(SyncProgressStatuses.Running, actual.Status);
            Assert.True(actual.StopRequested);
            Assert.Equal("RJ1002", actual.LastProcessedSourceId);
            Assert.Equal(2, actual.ProcessedCount);
            Assert.Equal(1, actual.CompletedCount);
            Assert.Equal(1, actual.FailedCount);
            Assert.Equal(8, actual.RemainingMetadataCountAfter);
            Assert.Equal(2048, actual.CompletedSizeBytesAfter);
            Assert.Equal(4096, actual.SizeLimitBytes);

            await store.SaveSyncDownloadProgressAsync(new SyncDownloadProgressState
            {
                Status = SyncProgressStatuses.Stopped,
                LastProcessedSourceId = "RJ1002",
                ProcessedCount = 2,
                CompletedCount = 1,
                FailedCount = 1,
                RemainingMetadataCountAfter = 8,
                CompletedSizeBytesBefore = 0,
                CompletedSizeBytesAfter = 2048,
                SizeLimitBytes = 4096,
                UpdatedAt = DateTime.UtcNow,
            });

            var stopped = await store.LoadSyncDownloadProgressAsync();
            Assert.Equal(SyncProgressStatuses.Stopped, stopped.Status);
            Assert.False(stopped.StopRequested);
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    [Fact]
    public async Task LoadSyncDownloadProgressAsync_ShouldReturnDefaults_WhenNoStatePersisted()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var pathService = new AppPathService(tempRoot);
            var store = new UiStateStore(pathService);

            var actual = await store.LoadSyncDownloadProgressAsync();

            Assert.Equal(SyncProgressStatuses.Idle, actual.Status);
            Assert.False(actual.StopRequested);
            Assert.Equal(string.Empty, actual.LastProcessedSourceId);
            Assert.Equal(0, actual.ProcessedCount);
            Assert.Equal(0, actual.SizeLimitBytes);
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    private static string CreateTempRoot()
    {
        var path = Path.Combine(Path.GetTempPath(), "asmroner-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }

    private static void CleanupTempRoot(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }
}
