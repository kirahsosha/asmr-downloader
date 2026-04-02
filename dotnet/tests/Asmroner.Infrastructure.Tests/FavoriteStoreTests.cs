using Asmroner.Core.Favorites;
using Asmroner.Infrastructure.Services;

namespace Asmroner.Infrastructure.Tests;

public class FavoriteStoreTests
{
    [Fact]
    public async Task SaveFavoriteFolderItemsAsync_ShouldRoundTripAndSkipExistingDuplicates()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var pathService = new AppPathService(tempRoot);
            var store = new FavoriteStore(pathService);

            var firstResult = await store.SaveFavoriteFolderItemsAsync(" 常听 ", new[]
            {
                new FavoriteWorkItem { SourceId = "RJ1001", WorkId = 1001, Title = "作品A" },
                new FavoriteWorkItem { SourceId = "rj1001", WorkId = 1001, Title = "作品A重复" },
                new FavoriteWorkItem { SourceId = "RJ1002", WorkId = 1002, Title = "作品B" },
            });

            var secondResult = await store.SaveFavoriteFolderItemsAsync("常听", new[]
            {
                new FavoriteWorkItem { SourceId = "RJ1002", WorkId = 2002, Title = "作品B-更新" },
                new FavoriteWorkItem { SourceId = "RJ1003", WorkId = 1003, Title = "作品C" },
            });

            var titles = await store.LoadFavoriteFolderTitlesAsync();
            var items = await store.LoadFavoriteFolderItemsAsync("常听");

            Assert.Equal(2, firstResult.AddedCount);
            Assert.Equal(1, firstResult.SkippedCount);
            Assert.Equal(1, secondResult.AddedCount);
            Assert.Equal(1, secondResult.SkippedCount);
            Assert.Equal(new[] { "常听" }, titles);
            Assert.Equal(new[] { "RJ1001", "RJ1002", "RJ1003" }, items.Select(static item => item.SourceId).ToArray());
            Assert.Equal(2002, items.Single(static item => item.SourceId == "RJ1002").WorkId);
            Assert.Equal("作品B-更新", items.Single(static item => item.SourceId == "RJ1002").Title);
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    [Fact]
    public async Task LoadFavoriteFolderTitlesAsync_ShouldDistinctTrimAndSortCaseInsensitive()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var pathService = new AppPathService(tempRoot);
            var store = new FavoriteStore(pathService);

            await store.SaveFavoriteFolderItemsAsync(" Beta ", new[]
            {
                new FavoriteWorkItem { SourceId = "RJ2001", WorkId = 2001, Title = "作品1" },
            });
            await store.SaveFavoriteFolderItemsAsync("alpha", new[]
            {
                new FavoriteWorkItem { SourceId = "RJ2002", WorkId = 2002, Title = "作品2" },
            });
            await store.SaveFavoriteFolderItemsAsync("ALPHA", new[]
            {
                new FavoriteWorkItem { SourceId = "RJ2003", WorkId = 2003, Title = "作品3" },
            });

            var titles = await store.LoadFavoriteFolderTitlesAsync();

            Assert.Equal(new[] { "alpha", "Beta" }, titles);
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