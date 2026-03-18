using Asmroner.Application.Services;

namespace Asmroner.Application.Tests;

public class SearchStateStoreTests
{
    [Fact]
    public void EnqueueForDownload_ShouldNormalizeSourceIdExtractedFromUrl()
    {
        var sut = new SearchStateStore();

        sut.EnqueueForDownload(new[]
        {
            "https://www.asmr.one/work/RJ987654",
            "  rj987654  ",
            "https://api.asmr-300.com/api/work/rj123",
            "RJ-5555",
            "6666"
        });

        var queued = sut.GetQueuedSourceIds();

        Assert.Equal(4, queued.Count);
        Assert.Contains("RJ987654", queued);
        Assert.Contains("RJ123", queued);
        Assert.Contains("RJ5555", queued);
        Assert.Contains("RJ6666", queued);
    }

    [Fact]
    public void RemoveFromQueue_ShouldMatchNormalizedInput()
    {
        var sut = new SearchStateStore();
        sut.EnqueueForDownload(new[] { "RJ7788" });

        sut.RemoveFromQueue(new[] { "https://api.asmr.one/api/work/rj7788" });

        Assert.Empty(sut.GetQueuedSourceIds());
    }
}