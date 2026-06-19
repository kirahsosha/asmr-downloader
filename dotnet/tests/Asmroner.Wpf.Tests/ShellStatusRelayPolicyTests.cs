using Asmroner.Wpf.Services;

namespace Asmroner.Wpf.Tests;

public class ShellStatusRelayPolicyTests
{
    [Fact]
    public void ShouldPublish_ShouldReturnFalse_WhenViewPolicyDisallows()
    {
        var result = ShellStatusRelayPolicy.ShouldPublish(
            message: "同步完成",
            isTextBlockVisible: true,
            currentShellMessage: "同步中...",
            canPublishFromView: false);

        Assert.False(result);
    }

    [Fact]
    public void ShouldPublish_ShouldReturnFalse_WhenMessageIsEmpty()
    {
        var result = ShellStatusRelayPolicy.ShouldPublish(
            message: " ",
            isTextBlockVisible: true,
            currentShellMessage: "同步中...",
            canPublishFromView: true);

        Assert.False(result);
    }

    [Fact]
    public void ShouldPublish_ShouldReturnFalse_WhenCurrentShellMessageIsInitializing()
    {
        var result = ShellStatusRelayPolicy.ShouldPublish(
            message: "搜索完成",
            isTextBlockVisible: true,
            currentShellMessage: "初始化中...",
            canPublishFromView: true);

        Assert.False(result);
    }

    [Fact]
    public void ShouldPublish_ShouldReturnTrue_WhenMessageIsValidAndCanPublish()
    {
        var result = ShellStatusRelayPolicy.ShouldPublish(
            message: "下载队列刷新完成",
            isTextBlockVisible: true,
            currentShellMessage: "同步中...",
            canPublishFromView: true);

        Assert.True(result);
    }
}
