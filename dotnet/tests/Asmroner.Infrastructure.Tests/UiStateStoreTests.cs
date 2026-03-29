using Asmroner.Core.Configuration;
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
                Tag = "tag-a,tag-b",
                TagExclude = true,
                Circle = "circle-a",
                Lang = "zh-CN",
            };

            await store.SaveSearchUiStateAsync(expected);
            var actual = await store.LoadSearchUiStateAsync();

            Assert.False(actual.IncludeTranslationWorks);
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
