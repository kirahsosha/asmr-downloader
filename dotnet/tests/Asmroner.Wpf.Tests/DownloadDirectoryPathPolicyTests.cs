using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class DownloadDirectoryPathPolicyTests
{
    [Fact]
    public void Resolve_ShouldReturnDefaultPath_WhenConfiguredPathIsNull()
    {
        var path = DownloadDirectoryPathPolicy.Resolve(null, "C:/downloads/default");

        Assert.Equal("C:/downloads/default", path);
    }

    [Fact]
    public void Resolve_ShouldReturnTrimmedConfiguredPath_WhenConfiguredPathProvided()
    {
        var path = DownloadDirectoryPathPolicy.Resolve("  C:/downloads/custom  ", "C:/downloads/default");

        Assert.Equal("C:/downloads/custom", path);
    }

    [Fact]
    public void Resolve_ShouldReturnDefaultPath_WhenConfiguredPathIsWhitespace()
    {
        var path = DownloadDirectoryPathPolicy.Resolve("   ", "C:/downloads/default");

        Assert.Equal("C:/downloads/default", path);
    }
}
