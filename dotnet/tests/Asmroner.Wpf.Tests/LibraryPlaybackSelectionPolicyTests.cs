using Asmroner.Core.Library;
using Asmroner.Core.Playback;
using Asmroner.Wpf.Views;

namespace Asmroner.Wpf.Tests;

public class LibraryPlaybackSelectionPolicyTests
{
    [Fact]
    public void ShouldReloadContext_ShouldReturnTrue_WhenSelectedFileHasDifferentFullPath()
    {
        var currentWork = new LibraryWorkItem
        {
            SourceId = "RJ5001",
            Title = "Old Root",
        };
        var currentFile = new LibraryFileItem
        {
            Name = "track01.flac",
            RelativePath = "disc1/track01.flac",
            FullPath = "C:/old-root/RJ5001/disc1/track01.flac",
            Extension = ".flac",
            IsPlayable = true,
        };
        var selectedWork = new LibraryWorkItem
        {
            SourceId = "RJ5001",
            Title = "New Root",
        };
        var selectedFile = new LibraryFileItem
        {
            Name = "track01.flac",
            RelativePath = "disc1/track01.flac",
            FullPath = "D:/new-root/RJ5001/disc1/track01.flac",
            Extension = ".flac",
            IsPlayable = true,
        };
        var current = new PlaybackContext
        {
            Work = currentWork,
            File = currentFile,
            State = PlaybackState.Ready,
            Message = "已载入可播放媒体文件，可通过系统默认程序打开。",
        };

        var shouldReload = LibraryPlaybackSelectionPolicy.ShouldReloadContext(current, selectedWork, selectedFile);

        Assert.True(shouldReload);
    }
}