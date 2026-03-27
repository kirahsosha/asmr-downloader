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

    [Fact]
    public void FilterHdAudioOnly_ShouldRemoveMp3_WhenFlacExists()
    {
        var entries = new[] { "track01.mp3", "track01.flac" };

        var result = DownloadFilterParser.FilterHdAudioOnly(entries, static url => url);

        Assert.DoesNotContain("track01.mp3", result);
        Assert.Contains("track01.flac", result);
    }

    [Fact]
    public void FilterHdAudioOnly_ShouldRemoveMp3_WhenWavExists()
    {
        var entries = new[] { "bgm.mp3", "bgm.wav" };

        var result = DownloadFilterParser.FilterHdAudioOnly(entries, static url => url);

        Assert.DoesNotContain("bgm.mp3", result);
        Assert.Contains("bgm.wav", result);
    }

    [Fact]
    public void FilterHdAudioOnly_ShouldKeepMp3_WhenNoHdAudioExists()
    {
        var entries = new[] { "track01.mp3", "cover.jpg" };

        var result = DownloadFilterParser.FilterHdAudioOnly(entries, static url => url);

        Assert.Contains("track01.mp3", result);
        Assert.Contains("cover.jpg", result);
    }

    [Fact]
    public void FilterHdAudioOnly_ShouldReturnUnchanged_WhenHdAudioOnlyIsFalse()
    {
        var entries = new[] { "track01.mp3", "track01.flac" };

        var result = DownloadFilterParser.FilterHdAudioOnly(entries, static url => url, hdAudioOnly: false);

        Assert.Equal(entries, result);
    }
}