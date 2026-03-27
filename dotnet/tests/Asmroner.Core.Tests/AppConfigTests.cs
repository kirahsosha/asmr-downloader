using Asmroner.Core.Configuration;

namespace Asmroner.Core.Tests;

public class AppConfigTests
{
    [Fact]
    public void AppConfig_ShouldHaveReasonableDefaults()
    {
        var config = new AppConfig();

        Assert.Equal(4, config.Downloader.MaxWorkers);
        Assert.Equal(3, config.Downloader.MaxRetries);
        Assert.Equal("5GB", config.Downloader.SyncWantedSize);
        Assert.Equal("mp3,wav,flac,jpg,jpeg,png,gif,webp,mp4,mkv,avi,webm,txt,lrc,ass", config.Downloader.PreferFormats);
        Assert.Equal("mp3,m4a,wav,flac", config.Downloader.PreferMedia);
        Assert.True(config.Downloader.HdAudioOnly);
    }
}
