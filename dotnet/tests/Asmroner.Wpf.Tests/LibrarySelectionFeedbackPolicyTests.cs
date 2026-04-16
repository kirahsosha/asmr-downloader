using Asmroner.Core.Library;
using Asmroner.Wpf.Views;

namespace Asmroner.Wpf.Tests;

public class LibrarySelectionFeedbackPolicyTests
{
    [Fact]
    public void Evaluate_ShouldGuide_WhenWorkIsSelectedWithoutTreeItem()
    {
        var selectedWork = CreatePlayableWork();

        var result = LibrarySelectionFeedbackPolicy.Evaluate(selectedWork, selectedItem: null, static path => !path.EndsWith("readme.txt", StringComparison.OrdinalIgnoreCase));

        Assert.False(result.CanLoadContext);
        Assert.False(result.CanPlay);
        Assert.Null(result.PlayableTarget);
        Assert.NotNull(result.SuggestedTarget);
        Assert.Equal(1, result.SuggestedPlayableCount);
        Assert.Equal("disc1/track01.flac", result.SuggestedTarget.RelativePath);
        Assert.Equal("当前作品包含 1 个可播放媒体文件，首个候选：disc1/track01.flac。请在文件树中显式选中后再载入或播放。", result.Message);
    }

    [Fact]
    public void Evaluate_ShouldExplainSupportedFormats_WhenWorkHasNoPlayableFiles()
    {
        var selectedWork = CreateNonPlayableWork();

        var result = LibrarySelectionFeedbackPolicy.Evaluate(selectedWork, selectedItem: null, static _ => true);

        Assert.False(result.CanLoadContext);
        Assert.False(result.CanPlay);
        Assert.Null(result.PlayableTarget);
        Assert.Null(result.SuggestedTarget);
        Assert.Equal($"当前作品未发现可播放媒体文件。当前支持格式：{LibraryPlayableMediaPolicy.SupportedExtensionsText}", result.Message);
    }

    [Fact]
    public void Evaluate_ShouldWarn_WhenDirectoryIsSelected()
    {
        var selectedWork = CreatePlayableWork();
        var selectedItem = new LibraryFileItem
        {
            Name = "disc1",
            RelativePath = "disc1",
            FullPath = "C:/library/RJ6001/disc1",
            IsDirectory = true,
        };

        var result = LibrarySelectionFeedbackPolicy.Evaluate(selectedWork, selectedItem, static path => !path.EndsWith("readme.txt", StringComparison.OrdinalIgnoreCase));

        Assert.False(result.CanLoadContext);
        Assert.False(result.CanPlay);
        Assert.Null(result.PlayableTarget);
        Assert.Equal("当前选择是目录，请继续选择一个可播放的媒体文件。可尝试选择：disc1/track01.flac", result.Message);
    }

    [Fact]
    public void Evaluate_ShouldWarn_WhenNonPlayableFileIsSelected()
    {
        var selectedWork = CreatePlayableWork();
        var selectedItem = new LibraryFileItem
        {
            Name = "readme.txt",
            RelativePath = "disc1/readme.txt",
            FullPath = "C:/library/RJ6001/disc1/readme.txt",
            Extension = ".txt",
            IsPlayable = false,
        };

        var result = LibrarySelectionFeedbackPolicy.Evaluate(selectedWork, selectedItem, static path => !path.EndsWith("readme.txt", StringComparison.OrdinalIgnoreCase));

        Assert.False(result.CanLoadContext);
        Assert.False(result.CanPlay);
        Assert.Null(result.PlayableTarget);
        Assert.Equal("当前选择的文件不可播放：disc1/readme.txt。可尝试选择：disc1/track01.flac", result.Message);
    }

    [Fact]
    public void Evaluate_ShouldWarn_WhenPlayableFileDoesNotExist()
    {
        var selectedWork = CreateWorkWithMissingPlayableSelection();
        var selectedItem = new LibraryFileItem
        {
            Name = "track01.flac",
            RelativePath = "disc1/track01.flac",
            FullPath = "C:/library/RJ6003/disc1/track01.flac",
            Extension = ".flac",
            IsPlayable = true,
        };

        var result = LibrarySelectionFeedbackPolicy.Evaluate(
            selectedWork,
            selectedItem,
            static path => path.EndsWith("track02.flac", StringComparison.OrdinalIgnoreCase));

        Assert.False(result.CanLoadContext);
        Assert.False(result.CanPlay);
        Assert.Null(result.PlayableTarget);
        Assert.Equal("当前选择的媒体文件不存在：disc1/track01.flac。可改选：disc1/track02.flac", result.Message);
    }

    [Fact]
    public void Evaluate_ShouldEnableActions_WhenPlayableFileExists()
    {
        var selectedWork = CreatePlayableWork();
        var selectedItem = new LibraryFileItem
        {
            Name = "track01.flac",
            RelativePath = "disc1/track01.flac",
            FullPath = "C:/library/RJ6001/disc1/track01.flac",
            Extension = ".flac",
            IsPlayable = true,
        };

        var result = LibrarySelectionFeedbackPolicy.Evaluate(selectedWork, selectedItem, static path => !path.EndsWith("readme.txt", StringComparison.OrdinalIgnoreCase));

        Assert.True(result.CanLoadContext);
        Assert.True(result.CanPlay);
        Assert.Equal(selectedItem, result.PlayableTarget);
        Assert.Equal(1, result.SuggestedPlayableCount);
        Assert.Equal("已选择可播放媒体文件：disc1/track01.flac，可载入或直接播放。", result.Message);
    }

    private static LibraryWorkItem CreatePlayableWork()
    {
        return new LibraryWorkItem
        {
            SourceId = "RJ6001",
            Title = "Playable Work",
            Files =
            [
                new LibraryFileItem
                {
                    Name = "disc1",
                    RelativePath = "disc1",
                    FullPath = "C:/library/RJ6001/disc1",
                    IsDirectory = true,
                    Children =
                    [
                        new LibraryFileItem
                        {
                            Name = "readme.txt",
                            RelativePath = "disc1/readme.txt",
                            FullPath = "C:/library/RJ6001/disc1/readme.txt",
                            Extension = ".txt",
                        },
                        new LibraryFileItem
                        {
                            Name = "track01.flac",
                            RelativePath = "disc1/track01.flac",
                            FullPath = "C:/library/RJ6001/disc1/track01.flac",
                            Extension = ".flac",
                            IsPlayable = true,
                        },
                    ],
                },
            ],
        };
    }

    private static LibraryWorkItem CreateNonPlayableWork()
    {
        return new LibraryWorkItem
        {
            SourceId = "RJ6002",
            Title = "Text Only Work",
            Files =
            [
                new LibraryFileItem
                {
                    Name = "cover.jpg",
                    RelativePath = "cover.jpg",
                    FullPath = "C:/library/RJ6002/cover.jpg",
                    Extension = ".jpg",
                },
                new LibraryFileItem
                {
                    Name = "disc1",
                    RelativePath = "disc1",
                    FullPath = "C:/library/RJ6002/disc1",
                    IsDirectory = true,
                    Children =
                    [
                        new LibraryFileItem
                        {
                            Name = "readme.txt",
                            RelativePath = "disc1/readme.txt",
                            FullPath = "C:/library/RJ6002/disc1/readme.txt",
                            Extension = ".txt",
                        },
                    ],
                },
            ],
        };
    }

    private static LibraryWorkItem CreateWorkWithMissingPlayableSelection()
    {
        return new LibraryWorkItem
        {
            SourceId = "RJ6003",
            Title = "Missing Playable Work",
            Files =
            [
                new LibraryFileItem
                {
                    Name = "disc1",
                    RelativePath = "disc1",
                    FullPath = "C:/library/RJ6003/disc1",
                    IsDirectory = true,
                    Children =
                    [
                        new LibraryFileItem
                        {
                            Name = "track01.flac",
                            RelativePath = "disc1/track01.flac",
                            FullPath = "C:/library/RJ6003/disc1/track01.flac",
                            Extension = ".flac",
                            IsPlayable = true,
                        },
                        new LibraryFileItem
                        {
                            Name = "track02.flac",
                            RelativePath = "disc1/track02.flac",
                            FullPath = "C:/library/RJ6003/disc1/track02.flac",
                            Extension = ".flac",
                            IsPlayable = true,
                        },
                    ],
                },
            ],
        };
    }
}