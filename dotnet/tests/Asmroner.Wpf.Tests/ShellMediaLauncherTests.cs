using System.Diagnostics;
using Asmroner.Wpf.Services;

namespace Asmroner.Wpf.Tests;

public class ShellMediaLauncherTests
{
    [Fact]
    public void Open_ShouldTreatNullProcessAsSuccess_WhenShellStartDoesNotThrow()
    {
        ProcessStartInfo? capturedStartInfo = null;
        var sut = new ShellMediaLauncher(startInfo =>
        {
            capturedStartInfo = startInfo;
            return null;
        });

        var exception = Record.Exception(() => sut.Open("C:/library/RJ5001/disc1/track01.flac"));

        Assert.Null(exception);
        Assert.NotNull(capturedStartInfo);
        Assert.Equal("C:/library/RJ5001/disc1/track01.flac", capturedStartInfo!.FileName);
        Assert.True(capturedStartInfo.UseShellExecute);
    }

    [Fact]
    public void Open_ShouldRethrow_WhenShellStartThrows()
    {
        var sut = new ShellMediaLauncher(_ => throw new InvalidOperationException("启动失败。"));

        var exception = Assert.Throws<InvalidOperationException>(() => sut.Open("C:/library/RJ5001/disc1/track01.flac"));

        Assert.Equal("启动失败。", exception.Message);
    }
}