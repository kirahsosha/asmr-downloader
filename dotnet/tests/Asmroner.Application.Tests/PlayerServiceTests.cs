using System.IO;
using Asmroner.Application.Services;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Library;
using Asmroner.Core.Playback;

namespace Asmroner.Application.Tests;

public class PlayerServiceTests
{
    [Fact]
    public void LoadContext_ShouldLoadSelectedPlayableFile_WhenFileIsProvided()
    {
        using var tempMediaFile = new TempMediaFile(".flac");
        var launcher = new FakeMediaLauncher();
        var sut = new PlayerService(launcher);
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
                            FullPath = tempMediaFile.FilePath,
                            Extension = ".flac",
                            IsPlayable = true,
                        },
                    ],
                },
            ],
        };
        var file = work.Files[0].Children[1];

        sut.LoadContext(work, file);

        var context = sut.GetCurrentContext();
        Assert.Equal(PlaybackState.Ready, context.State);
        Assert.Equal(work, context.Work);
        Assert.NotNull(context.File);
        Assert.Equal("disc1/track01.flac", context.File.RelativePath);
        Assert.Equal("已载入可播放媒体文件，可通过系统默认程序打开。", context.Message);
    }

    [Fact]
    public void LoadContext_ShouldKeepEmptyContext_WhenSelectedFileIsNotPlayable()
    {
        var sut = new PlayerService(new FakeMediaLauncher());
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
        var selectedFile = work.Files[0];

        sut.LoadContext(work, selectedFile);

        var loadedContext = sut.GetCurrentContext();
        Assert.Equal(PlaybackState.None, loadedContext.State);
        Assert.Equal(work, loadedContext.Work);
        Assert.Null(loadedContext.File);
        Assert.Equal("请先在文件树中选择一个可播放的媒体文件。", loadedContext.Message);
    }

    [Fact]
    public void LoadContext_ShouldMarkFailed_WhenSelectedFileDoesNotExist()
    {
        var sut = new PlayerService(new FakeMediaLauncher());
        var work = new LibraryWorkItem
        {
            SourceId = "RJ4003",
            Title = "Missing Audio Work",
            Files =
            [
                new LibraryFileItem
                {
                    Name = "track01.flac",
                    RelativePath = "track01.flac",
                    FullPath = Path.Combine(Path.GetTempPath(), $"missing-{Guid.NewGuid():N}.flac"),
                    Extension = ".flac",
                    IsPlayable = true,
                },
            ],
        };
        var file = work.Files[0];

        sut.LoadContext(work, file);

        var context = sut.GetCurrentContext();
        Assert.Equal(PlaybackState.Failed, context.State);
        Assert.Equal(work, context.Work);
        Assert.NotNull(context.File);
        Assert.Contains("本地媒体文件不存在", context.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Play_ShouldOpenLoadedFile_WithSystemLauncher()
    {
        using var tempMediaFile = new TempMediaFile(".mp3");
        var launcher = new FakeMediaLauncher();
        var sut = new PlayerService(launcher);
        var work = CreateSingleFileWork("RJ4004", "Playable Work", tempMediaFile.FilePath);

        sut.LoadContext(work, work.Files[0]);

        sut.Play();

        var context = sut.GetCurrentContext();
        Assert.Equal(PlaybackState.Launched, context.State);
        Assert.Equal(tempMediaFile.FilePath, launcher.LastOpenedFilePath);
        Assert.Equal("已调用系统默认程序打开当前媒体文件。", context.Message);
    }

    [Fact]
    public void Play_ShouldMarkFailed_WhenLauncherThrows()
    {
        using var tempMediaFile = new TempMediaFile(".wav");
        var launcher = new FakeMediaLauncher
        {
            ExceptionToThrow = new InvalidOperationException("未找到关联程序。"),
        };
        var sut = new PlayerService(launcher);
        var work = CreateSingleFileWork("RJ4005", "Broken Work", tempMediaFile.FilePath);

        sut.LoadContext(work, work.Files[0]);
        sut.Play();

        var context = sut.GetCurrentContext();
        Assert.Equal(PlaybackState.Failed, context.State);
        Assert.Equal(work, context.Work);
        Assert.NotNull(context.File);
        Assert.Equal("调用系统默认程序打开失败：未找到关联程序。", context.Message);
    }

    [Fact]
    public void Play_ShouldRequireReload_WhenCurrentContextIsFailed()
    {
        var sut = new PlayerService(new FakeMediaLauncher());
        var work = CreateSingleFileWork(
            "RJ4006",
            "Reload Required Work",
            Path.Combine(Path.GetTempPath(), $"missing-reload-{Guid.NewGuid():N}.wav"));

        sut.LoadContext(work, work.Files[0]);

        sut.Play();

        var context = sut.GetCurrentContext();
        Assert.Equal(PlaybackState.Failed, context.State);
        Assert.Equal("请先重新载入文件后再打开。", context.Message);
    }

    [Fact]
    public void Play_ShouldKeepLoadedContext_WhenCurrentFileMatchesSelection()
    {
        using var tempMediaFile = new TempMediaFile(".flac");
        var launcher = new FakeMediaLauncher();
        var sut = new PlayerService(launcher);
        var work = CreateSingleFileWork("RJ4007", "Loaded Work", tempMediaFile.FilePath);

        sut.LoadContext(work, work.Files[0]);
        var readyContext = sut.GetCurrentContext();

        sut.Play();

        var context = sut.GetCurrentContext();
        Assert.Equal(work, context.Work);
        Assert.Equal(readyContext.File, context.File);
        Assert.Equal(PlaybackState.Launched, context.State);
    }

    [Fact]
    public void ClearContext_ShouldResetInitialState()
    {
        using var tempMediaFile = new TempMediaFile(".mp3");
        var sut = new PlayerService(new FakeMediaLauncher());
        var work = CreateSingleFileWork("RJ4008", "Clearable Work", tempMediaFile.FilePath);

        sut.LoadContext(work, work.Files[0]);
        sut.ClearContext();

        var context = sut.GetCurrentContext();
        Assert.Null(context.Work);
        Assert.Null(context.File);
        Assert.Equal(PlaybackState.None, context.State);
        Assert.Equal("尚未载入任何可播放媒体文件。", context.Message);
    }

    private static LibraryWorkItem CreateSingleFileWork(string sourceId, string title, string filePath)
    {
        return new LibraryWorkItem
        {
            SourceId = sourceId,
            Title = title,
            AudioFileCount = 1,
            Files =
            [
                new LibraryFileItem
                {
                    Name = Path.GetFileName(filePath),
                    RelativePath = Path.GetFileName(filePath),
                    FullPath = filePath,
                    Extension = Path.GetExtension(filePath),
                    IsPlayable = true,
                },
            ],
        };
    }

    private sealed class FakeMediaLauncher : IMediaLauncher
    {
        public Exception? ExceptionToThrow { get; init; }

        public string? LastOpenedFilePath { get; private set; }

        public void Open(string filePath)
        {
            if (ExceptionToThrow is not null)
            {
                throw ExceptionToThrow;
            }

            LastOpenedFilePath = filePath;
        }
    }

    private sealed class TempMediaFile : IDisposable
    {
        public TempMediaFile(string extension)
        {
            var fileName = $"asmroner-media-{Guid.NewGuid():N}{extension}";
            FilePath = Path.Combine(Path.GetTempPath(), fileName);
            File.WriteAllBytes(FilePath, [0x00]);
        }

        public string FilePath { get; }

        public void Dispose()
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
        }
    }
}