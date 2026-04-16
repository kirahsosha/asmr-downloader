using Asmroner.Core.Library;
using Asmroner.Core.Playback;
using Asmroner.Wpf.Views;

namespace Asmroner.Wpf.Tests;

public class LibraryPlaybackContextTextBuilderTests
{
    [Fact]
    public void Build_ShouldProvideDefaultTexts_WhenNothingIsSelectedOrLoaded()
    {
        var context = new PlaybackContext
        {
            Message = "尚未载入任何可播放媒体文件。",
        };

        var result = LibraryPlaybackContextTextBuilder.Build(context, new LibrarySelectionFeedbackResult());

        Assert.Equal("当前未选择任何作品或文件。请先选择左侧作品，并在文件树中显式选中一个可播放的媒体文件。", result.SelectionText);
        Assert.Equal("状态：未载入\n说明：尚未载入任何可播放媒体文件。", result.LoadedContextText);
    }

    [Fact]
    public void Build_ShouldKeepSelectionFeedbackSeparated_FromLoadedContext()
    {
        var context = new PlaybackContext
        {
            Message = "尚未载入任何可播放媒体文件。",
        };
        var selectionFeedback = new LibrarySelectionFeedbackResult
        {
            Message = "当前作品包含 2 个可播放媒体文件，首个候选：disc1/track01.flac。请在文件树中显式选中后再载入或播放。",
        };

        var result = LibraryPlaybackContextTextBuilder.Build(context, selectionFeedback);

        Assert.Equal(selectionFeedback.Message, result.SelectionText);
        Assert.Equal("状态：未载入\n说明：尚未载入任何可播放媒体文件。", result.LoadedContextText);
    }

    [Fact]
    public void Build_ShouldIncludeLoadedWorkAndFileDetails_WhenContextIsReady()
    {
        var work = new LibraryWorkItem
        {
            SourceId = "RJ6101",
            Title = "Loaded Work",
        };
        var file = new LibraryFileItem
        {
            Name = "track01.flac",
            RelativePath = "disc1/track01.flac",
            FullPath = "C:/library/RJ6101/disc1/track01.flac",
            Extension = ".flac",
            IsPlayable = true,
        };
        var context = new PlaybackContext
        {
            Work = work,
            File = file,
            State = PlaybackState.Ready,
            Message = "已载入可播放媒体文件，可通过系统默认程序打开。",
        };
        var selectionFeedback = new LibrarySelectionFeedbackResult
        {
            Message = "已选择可播放媒体文件：disc1/track01.flac，可载入或直接播放。",
        };

        var result = LibraryPlaybackContextTextBuilder.Build(context, selectionFeedback);

        Assert.Equal(selectionFeedback.Message, result.SelectionText);
        Assert.Equal(
            "状态：已载入\n作品：RJ6101 Loaded Work\n文件：disc1/track01.flac\n说明：已载入可播放媒体文件，可通过系统默认程序打开。",
            result.LoadedContextText);
    }
}