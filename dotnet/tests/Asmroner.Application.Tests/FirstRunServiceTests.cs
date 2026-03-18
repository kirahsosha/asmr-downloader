using Asmroner.Application.Services;
using Asmroner.Core.Configuration;

namespace Asmroner.Application.Tests;

public class FirstRunServiceTests
{
    [Fact]
    public void FirstRunService_ShouldRequireSetup_WhenConfigMissing()
    {
        var sut = new FirstRunService();

        var requiresSetup = sut.RequiresSetup(null, Array.Empty<string>());

        Assert.True(requiresSetup);
    }

    [Fact]
    public void FirstRunService_ShouldRequireSetup_WhenValidationFails()
    {
        var sut = new FirstRunService();
        var config = new AppConfig();

        var requiresSetup = sut.RequiresSetup(config, new[] { "账号不能为空" });

        Assert.True(requiresSetup);
    }

    [Fact]
    public void FirstRunService_ShouldNotRequireSetup_WhenConfigValid()
    {
        var sut = new FirstRunService();
        var config = new AppConfig();

        var requiresSetup = sut.RequiresSetup(config, Array.Empty<string>());

        Assert.False(requiresSetup);
    }
}
