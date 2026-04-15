using Asmroner.Core.Library;

namespace Asmroner.Core.Tests;

public class LibraryDirectoryNameParserTests
{
    [Fact]
    public void TryParse_ShouldParseBracketedDirectoryName()
    {
        var success = LibraryDirectoryNameParser.TryParse("[RJ123456]Sample Title", out var result);

        Assert.True(success);
        Assert.Equal("RJ123456", result.SourceId);
        Assert.Equal("Sample Title", result.Title);
        Assert.Equal(string.Empty, result.Release);
        Assert.Null(result.HasSubtitle);
        Assert.Equal("Bracketed", result.Scheme);
    }

    [Fact]
    public void TryParse_ShouldParseLegacyDirectoryName()
    {
        var success = LibraryDirectoryNameParser.TryParse("RJ223344-20240203-sub-LegacyTitle", out var result);

        Assert.True(success);
        Assert.Equal("RJ223344", result.SourceId);
        Assert.Equal("LegacyTitle", result.Title);
        Assert.Equal("2024-02-03", result.Release);
        Assert.True(result.HasSubtitle);
        Assert.Equal("LegacyListen", result.Scheme);
    }

    [Fact]
    public void TryParse_ShouldParseLegacyDirectoryName_WhenTitleContainsHyphen()
    {
        var success = LibraryDirectoryNameParser.TryParse("RJ223344-20240203-nosub-Legacy-Title-Part2", out var result);

        Assert.True(success);
        Assert.Equal("RJ223344", result.SourceId);
        Assert.Equal("Legacy-Title-Part2", result.Title);
        Assert.Equal("2024-02-03", result.Release);
        Assert.False(result.HasSubtitle);
        Assert.Equal("LegacyListen", result.Scheme);
    }

    [Fact]
    public void TryParse_ShouldReturnFalse_ForInvalidDirectoryName()
    {
        var success = LibraryDirectoryNameParser.TryParse("not-a-library-work", out var result);

        Assert.False(success);
        Assert.Equal(string.Empty, result.SourceId);
        Assert.Equal(string.Empty, result.Title);
    }
}