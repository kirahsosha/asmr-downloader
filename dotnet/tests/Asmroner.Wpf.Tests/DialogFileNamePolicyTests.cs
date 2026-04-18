using Asmroner.Wpf.Services;

namespace Asmroner.Wpf.Tests;

public class DialogFileNamePolicyTests
{
    [Theory]
    [InlineData("export.csv", 1, "csv")]
    [InlineData("export.json", 1, "json")]
    [InlineData("export", 1, "csv")]
    [InlineData("export", 2, "json")]
    public void ResolveExtension_ShouldRespectFileNameAndFilterSelection(string filePath, int filterIndex, string expectedExtension)
    {
        var actual = DialogFileNamePolicy.ResolveExtension(filePath, filterIndex);

        Assert.Equal(expectedExtension, actual);
    }

    [Theory]
    [InlineData("export", "csv", "export.csv")]
    [InlineData("export.txt", "json", "export.json")]
    [InlineData("export.csv", "csv", "export.csv")]
    public void EnsureExtension_ShouldAppendOrReplaceExtension(string filePath, string extension, string expectedPath)
    {
        var actual = DialogFileNamePolicy.EnsureExtension(filePath, extension);

        Assert.Equal(expectedPath, actual);
    }
}