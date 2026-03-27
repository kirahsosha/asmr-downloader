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

    [Fact]
    public void Create_ShouldCarryHdAudioOnly_WhenFlagIsTrue()
    {
        var context = DownloadOperationContext.Create(null, hdAudioOnly: true);

        Assert.True(context.HdAudioOnly);
    }

    [Fact]
    public void Create_ShouldDefaultHdAudioOnly_ToFalse()
    {
        var context = DownloadOperationContext.Create(null);

        Assert.False(context.HdAudioOnly);
    }
}
