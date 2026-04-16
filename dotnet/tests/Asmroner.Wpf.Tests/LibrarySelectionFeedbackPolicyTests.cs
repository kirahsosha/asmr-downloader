using Asmroner.Core.Library;
using Asmroner.Wpf.Views;

namespace Asmroner.Wpf.Tests;

public class LibrarySelectionFeedbackPolicyTests
{
    [Fact]
    public void Evaluate_ShouldWarn_WhenDirectoryIsSelected()
    {
        var selectedItem = new LibraryFileItem
        {
            Name = "disc1",
            RelativePath = "disc1",
            FullPath = "C:/library/RJ6001/disc1",
            IsDirectory = true,
        };

        var result = LibrarySelectionFeedbackPolicy.Evaluate(selectedItem, static _ => true);

        Assert.False(result.CanLoadContext);
        Assert.False(result.CanPlay);
        Assert.Null(result.PlayableTarget);
        Assert.Equal("当前选择是目录，请继续选择一个可播放的媒体文件。", result.Message);
    }

    [Fact]
    public void Evaluate_ShouldWarn_WhenNonPlayableFileIsSelected()
    {
        var selectedItem = new LibraryFileItem
        {
            Name = "readme.txt",
            RelativePath = "disc1/readme.txt",
            FullPath = "C:/library/RJ6001/disc1/readme.txt",
            Extension = ".txt",
            IsPlayable = false,
        };

        var result = LibrarySelectionFeedbackPolicy.Evaluate(selectedItem, static _ => true);

        Assert.False(result.CanLoadContext);
        Assert.False(result.CanPlay);
        Assert.Null(result.PlayableTarget);
        Assert.Equal("当前选择的文件不可播放：disc1/readme.txt", result.Message);
    }

    [Fact]
    public void Evaluate_ShouldWarn_WhenPlayableFileDoesNotExist()
    {
        var selectedItem = new LibraryFileItem
        {
            Name = "track01.flac",
            RelativePath = "disc1/track01.flac",
            FullPath = "C:/library/RJ6001/disc1/track01.flac",
            Extension = ".flac",
            IsPlayable = true,
        };

        var result = LibrarySelectionFeedbackPolicy.Evaluate(selectedItem, static _ => false);

        Assert.False(result.CanLoadContext);
        Assert.False(result.CanPlay);
        Assert.Null(result.PlayableTarget);
        Assert.Equal("当前选择的媒体文件不存在：disc1/track01.flac", result.Message);
    }

    [Fact]
    public void Evaluate_ShouldEnableActions_WhenPlayableFileExists()
    {
        var selectedItem = new LibraryFileItem
        {
            Name = "track01.flac",
            RelativePath = "disc1/track01.flac",
            FullPath = "C:/library/RJ6001/disc1/track01.flac",
            Extension = ".flac",
            IsPlayable = true,
        };

        var result = LibrarySelectionFeedbackPolicy.Evaluate(selectedItem, static _ => true);

        Assert.True(result.CanLoadContext);
        Assert.True(result.CanPlay);
        Assert.Equal(selectedItem, result.PlayableTarget);
        Assert.Equal("已选择可播放媒体文件：disc1/track01.flac，可载入或直接播放。", result.Message);
    }
}