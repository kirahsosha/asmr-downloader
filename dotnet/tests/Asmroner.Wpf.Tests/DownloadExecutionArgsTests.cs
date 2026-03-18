using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class DownloadExecutionArgsTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void NormalizeFileFilter_ShouldReturnNull_ForNullOrWhitespace(string? raw)
    {
        var normalized = DownloadExecutionArgs.NormalizeFileFilter(raw);

        Assert.Null(normalized);
    }

    [Fact]
    public void NormalizeFileFilter_ShouldTrimAndReturnValue_ForNonEmptyInput()
    {
        var normalized = DownloadExecutionArgs.NormalizeFileFilter("  +voice;-demo  ");

        Assert.Equal("+voice;-demo", normalized);
    }
}
