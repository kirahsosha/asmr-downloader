using Asmroner.Application.Services;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Library;

namespace Asmroner.Application.Tests;

public class LibraryQueryServiceTests
{
    [Fact]
    public async Task QueryAsync_ShouldFilterByKeywordSubtitleAndAudio_AndReturnPagingMetadata()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var sut = new LibraryQueryService(new StubLibraryScannerService(new LibraryScanResult
        {
            Items =
            [
                CreateWorkItem("RJ3001", "Quiet Night", "2024-04-02", hasSubtitle: true, audioFileCount: 2, tags: "night,healing"),
                CreateWorkItem("RJ3002", "Morning Voice", "2024-04-03", hasSubtitle: false, audioFileCount: 1, tags: "daytime"),
                CreateWorkItem("BJ3003", "Night Archive", "2024-01-01", hasSubtitle: true, audioFileCount: 0, tags: "night,archive"),
            ],
            SkippedDirectories = ["skip-a"],
            Errors = ["error-a"],
            ScannedRootCount = 2,
        }));

        var result = await sut.QueryAsync(new LibraryQuery
        {
            Keyword = "  night  ",
            SubtitleOnly = true,
            AudioOnly = true,
            Page = 1,
            PageSize = 1,
        }, cancellationToken);

        var item = Assert.Single(result.Items);
        Assert.Equal("RJ3001", item.SourceId);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(1, result.PageSize);
        Assert.Equal(1, result.TotalPages);
        Assert.Equal(2, result.ScannedRootCount);
        Assert.Equal(3, result.ScannedWorkCount);
        Assert.Equal(["skip-a"], result.SkippedDirectories);
        Assert.Equal(["error-a"], result.Errors);
    }

    [Fact]
    public async Task QueryAsync_ShouldClampPageToLastPage_WhenRequestedPageExceedsRange()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var sut = new LibraryQueryService(new StubLibraryScannerService(new LibraryScanResult
        {
            Items =
            [
                CreateWorkItem("RJ3101", "Newest", "2024-05-03", hasSubtitle: true, audioFileCount: 1),
                CreateWorkItem("RJ3102", "Middle", "2024-05-02", hasSubtitle: true, audioFileCount: 1),
                CreateWorkItem("RJ3103", "Oldest", "2024-05-01", hasSubtitle: true, audioFileCount: 1),
            ],
            ScannedRootCount = 1,
        }));

        var result = await sut.QueryAsync(new LibraryQuery
        {
            Page = 9,
            PageSize = 2,
        }, cancellationToken);

        var item = Assert.Single(result.Items);
        Assert.Equal(2, result.Page);
        Assert.Equal(2, result.TotalPages);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal("RJ3103", item.SourceId);
    }

    private static LibraryWorkItem CreateWorkItem(
        string sourceId,
        string title,
        string release,
        bool hasSubtitle,
        int audioFileCount,
        string tags = "")
    {
        return new LibraryWorkItem
        {
            SourceId = sourceId,
            Title = title,
            Release = release,
            HasSubtitle = hasSubtitle,
            AudioFileCount = audioFileCount,
            TotalFileCount = Math.Max(audioFileCount, 1),
            Tags = tags,
            RootDirectory = $"C:/library/{sourceId}",
        };
    }

    private sealed class StubLibraryScannerService : ILibraryScannerService
    {
        private readonly LibraryScanResult _result;

        public StubLibraryScannerService(LibraryScanResult result)
        {
            _result = result;
        }

        public Task<LibraryScanResult> ScanAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_result);
        }
    }
}