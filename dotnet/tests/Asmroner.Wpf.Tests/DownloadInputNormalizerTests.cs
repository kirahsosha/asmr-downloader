using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class DownloadInputNormalizerTests
{
    [Fact]
    public void NormalizeSingleInputDisplay_ShouldExtractRjIdFromWorkUrl()
    {
        var normalized = DownloadInputNormalizer.NormalizeSingleInputDisplay("https://www.asmr.one/work/rj778899");

        Assert.Equal("RJ778899", normalized);
    }

    [Fact]
    public void NormalizeBatchInputDisplay_ShouldNormalizeAndDeduplicateSourceIds()
    {
        var normalized = DownloadInputNormalizer.NormalizeBatchInputDisplay("RJ1001; https://api.asmr.one/api/work/rj1001  rj2002 RJ-3003 4004");

        Assert.Equal("RJ1001, RJ2002, RJ3003, RJ4004", normalized);
    }

    [Fact]
    public void NormalizeBatchInputForSubmit_ShouldSupportCommaSemicolonSpaceAndNewline()
    {
        var normalized = DownloadInputNormalizer.NormalizeBatchInputForSubmit("RJ1001, RJ2002;\nRJ3003   rj4004");

        Assert.Equal("RJ1001, RJ2002, RJ3003, RJ4004", normalized);
    }
}
