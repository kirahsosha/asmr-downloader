using Asmroner.Core.Library;

namespace Asmroner.Core.Tests;

public class LibraryPlayableMediaPolicyTests
{
    [Fact]
    public void IsPlayableExtension_ShouldTreatSupportedExtensionsAsPlayable_IgnoringCaseAndDot()
    {
        Assert.True(LibraryPlayableMediaPolicy.IsPlayableExtension(".FlAc"));
        Assert.True(LibraryPlayableMediaPolicy.IsPlayableExtension("OPUS"));
        Assert.False(LibraryPlayableMediaPolicy.IsPlayableExtension(".txt"));
        Assert.False(LibraryPlayableMediaPolicy.IsPlayableExtension(string.Empty));
    }

    [Fact]
    public void CountPlayableFiles_ShouldCountNestedPlayableMediaFiles()
    {
        var items = CreateLibraryTree();

        var count = LibraryPlayableMediaPolicy.CountPlayableFiles(items);

        Assert.Equal(2, count);
    }

    [Fact]
    public void FindFirstPlayableFile_ShouldReturnFirstDepthFirstPlayableFile()
    {
        var items = CreateLibraryTree();

        var result = LibraryPlayableMediaPolicy.FindFirstPlayableFile(items);

        Assert.NotNull(result);
        Assert.Equal("disc1/track01.FLAC", result.RelativePath);
    }

    [Fact]
    public void FindFirstPlayableFile_ShouldReturnNull_WhenNoPlayableFileExists()
    {
        IReadOnlyList<LibraryFileItem> items =
        [
            new LibraryFileItem
            {
                Name = "cover.jpg",
                RelativePath = "cover.jpg",
                FullPath = "C:/library/RJ7002/cover.jpg",
                Extension = ".jpg",
            },
            new LibraryFileItem
            {
                Name = "disc1",
                RelativePath = "disc1",
                FullPath = "C:/library/RJ7002/disc1",
                IsDirectory = true,
                Children =
                [
                    new LibraryFileItem
                    {
                        Name = "readme.txt",
                        RelativePath = "disc1/readme.txt",
                        FullPath = "C:/library/RJ7002/disc1/readme.txt",
                        Extension = ".txt",
                    },
                ],
            },
        ];

        var result = LibraryPlayableMediaPolicy.FindFirstPlayableFile(items);

        Assert.Null(result);
    }

    private static IReadOnlyList<LibraryFileItem> CreateLibraryTree()
    {
        return
        [
            new LibraryFileItem
            {
                Name = "disc1",
                RelativePath = "disc1",
                FullPath = "C:/library/RJ7001/disc1",
                IsDirectory = true,
                Children =
                [
                    new LibraryFileItem
                    {
                        Name = "cover.jpg",
                        RelativePath = "disc1/cover.jpg",
                        FullPath = "C:/library/RJ7001/disc1/cover.jpg",
                        Extension = ".jpg",
                    },
                    new LibraryFileItem
                    {
                        Name = "track01.FLAC",
                        RelativePath = "disc1/track01.FLAC",
                        FullPath = "C:/library/RJ7001/disc1/track01.FLAC",
                        Extension = ".FLAC",
                    },
                ],
            },
            new LibraryFileItem
            {
                Name = "track02.OpUs",
                RelativePath = "track02.OpUs",
                FullPath = "C:/library/RJ7001/track02.OpUs",
                Extension = ".OpUs",
            },
        ];
    }
}