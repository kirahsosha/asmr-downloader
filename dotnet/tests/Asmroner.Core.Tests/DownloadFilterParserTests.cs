using Asmroner.Core.Configuration;
using Asmroner.Core.Download;

namespace Asmroner.Core.Tests;

public class DownloadFilterParserTests
{
    [Fact]
    public void ParsePreferExtensions_ShouldUseUnifiedPreferFormatsFirst()
    {
        var options = new DownloaderOptions
        {
            PreferFormats = "mp3,m4a,TXT",
            PreferMedia = "flac",
            PreferImage = "jpg",
            PreferVideo = "mp4",
        };

        var result = DownloadFilterParser.ParsePreferExtensions(options);

        Assert.Equal(new[] { ".mp3", ".m4a", ".txt" }, result);
    }

    [Fact]
    public void ParsePreferExtensions_ShouldFallbackToLegacyFields()
    {
        var options = new DownloaderOptions
        {
            PreferFormats = string.Empty,
            PreferMedia = "mp3,m4a",
            PreferImage = "jpg,png",
            PreferVideo = "mp4",
        };

        var result = DownloadFilterParser.ParsePreferExtensions(options);

        Assert.Equal(new[] { ".mp3", ".m4a", ".jpg", ".png", ".mp4" }, result);
    }

    [Fact]
    public void ParseFileFilter_ShouldParseIncludeAndExcludeTerms()
    {
        var result = DownloadFilterParser.ParseFileFilter("+voice;-demo;chapter");

        Assert.Equal(3, result.Count);
        Assert.Equal(("voice", false), result[0]);
        Assert.Equal(("demo", true), result[1]);
        Assert.Equal(("chapter", false), result[2]);
    }

    [Fact]
    public void MatchesFileFilter_ShouldApplyAllTerms()
    {
        var filter = DownloadFilterParser.ParseFileFilter("+main;-demo");

        Assert.True(DownloadFilterParser.MatchesFileFilter("main_story/track01", filter));
        Assert.False(DownloadFilterParser.MatchesFileFilter("side_story/track01", filter));
        Assert.False(DownloadFilterParser.MatchesFileFilter("main_demo/track01", filter));
    }
}