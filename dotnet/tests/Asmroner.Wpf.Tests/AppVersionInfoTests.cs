using System.Text.RegularExpressions;
using Asmroner.Wpf.Services;

namespace Asmroner.Wpf.Tests;

public class AppVersionInfoTests
{
    [Fact]
    public void GetDisplayVersion_ShouldReturnThreePartAssemblyVersion()
    {
        var version = AppVersionInfo.GetDisplayVersion();

        Assert.Matches(new Regex("^\\d+\\.\\d+\\.\\d+$"), version);
    }

    [Fact]
    public void BuildSettingsVersionText_AndStartupMessage_ShouldUseDisplayVersionWithoutSuffix()
    {
        var version = AppVersionInfo.GetDisplayVersion();

        Assert.Equal($"版本：v{version}", AppVersionInfo.BuildSettingsVersionText());
        Assert.Equal($"Asmroner v{version} startup completed.", AppVersionInfo.BuildStartupCompletedMessage());
    }
}