using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class DownloadOperationContextTests
{
    [Fact]
    public void Create_ShouldNormalizeWhitespaceFilter_ToNull()
    {
        var context = DownloadOperationContext.Create("   ");

        Assert.Null(context.FileFilter);
    }

    [Fact]
    public void Create_ShouldTrimFilter_WhenValueProvided()
    {
        var context = DownloadOperationContext.Create("  +voice;-demo  ");

        Assert.Equal("+voice;-demo", context.FileFilter);
    }
}
