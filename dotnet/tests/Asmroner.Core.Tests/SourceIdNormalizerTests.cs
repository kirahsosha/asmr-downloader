using Asmroner.Core.Utils;

namespace Asmroner.Core.Tests;

public class SourceIdNormalizerTests
{
    [Fact]
    public void Normalize_ShouldExtractBjId_FromDlsiteBooksUrl()
    {
        var result = SourceIdNormalizer.Normalize("https://www.dlsite.com/books/work/=/product_id/BJ02370869.html");

        Assert.Equal("BJ02370869", result);
    }

    [Fact]
    public void ToApiNumericId_ShouldNotTreatBjSourceIdSuffix_AsNumericWorkId()
    {
        var result = SourceIdNormalizer.ToApiNumericId("BJ02370869");

        Assert.Equal("BJ02370869", result);
    }
}