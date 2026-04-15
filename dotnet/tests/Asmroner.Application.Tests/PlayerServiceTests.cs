using Asmroner.Application.Services;
using Asmroner.Core.Library;
using Asmroner.Core.Playback;

namespace Asmroner.Application.Tests;

public class PlayerServiceTests
{
    [Fact]
    public void LoadContext_ShouldSelectFirstPlayableFile_FromNestedTree_WhenFileIsNotSpecified()
    {
        var sut = new PlayerService();
        var work = new LibraryWorkItem
        {
            SourceId = "RJ4001",
            Title = "Nested Audio Work",
            Files =
            [
                new LibraryFileItem
                {
                    Name = "disc1",
                    RelativePath = "disc1",
                    FullPath = "C:/library/RJ4001/disc1",
                    IsDirectory = true,
                    Children =
                    [
                        new LibraryFileItem
                        {
                            Name = "cover.jpg",
                            RelativePath = "disc1/cover.jpg",
                            FullPath = "C:/library/RJ4001/disc1/cover.jpg",
                            Extension = ".jpg",
                        },
                        new LibraryFileItem
                        {
                            Name = "track01.flac",
                            RelativePath = "disc1/track01.flac",
                            FullPath = "C:/library/RJ4001/disc1/track01.flac",
                            Extension = ".flac",
                            IsPlayable = true,
                        },
                    ],
                },
            ],
        };

        sut.LoadContext(work);

        var context = sut.GetCurrentContext();
        Assert.Equal(PlaybackState.Ready, context.State);
        Assert.Equal(work, context.Work);
        Assert.NotNull(context.File);
        Assert.Equal("disc1/track01.flac", context.File.RelativePath);
        Assert.Equal("已载入播放上下文，播放器将在后续批次接入。", context.Message);
    }

    [Fact]
    public void LoadContext_ShouldRemainEmpty_WhenNoPlayableFileExists_AndClearContextShouldReset()
    {
        var sut = new PlayerService();
        var work = new LibraryWorkItem
        {
            SourceId = "RJ4002",
            Title = "Text Only Work",
            Files =
            [
                new LibraryFileItem
                {
                    Name = "readme.txt",
                    RelativePath = "readme.txt",
                    FullPath = "C:/library/RJ4002/readme.txt",
                    Extension = ".txt",
                    IsPlayable = false,
                },
            ],
        };

        sut.LoadContext(work);

        var loadedContext = sut.GetCurrentContext();
        Assert.Equal(PlaybackState.None, loadedContext.State);
        Assert.Equal(work, loadedContext.Work);
        Assert.Null(loadedContext.File);
        Assert.Equal("当前作品没有可载入的本地音频文件。", loadedContext.Message);

        sut.ClearContext();

        var clearedContext = sut.GetCurrentContext();
        Assert.Null(clearedContext.Work);
        Assert.Null(clearedContext.File);
        Assert.Equal(PlaybackState.None, clearedContext.State);
        Assert.Equal("尚未载入任何本地音频。", clearedContext.Message);
    }
}