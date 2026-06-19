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

    [Theory]
    [InlineData("初始化中...")]
    [InlineData("正在处理，请稍候...")]
    [InlineData("Download 正在处理，请稍候...")]
    [InlineData("Search 正在处理，请稍候...")]
    [InlineData("Library 正在处理，请稍候...")]
    [InlineData("Sync 正在处理，请稍候...")]
    [InlineData("Settings 正在处理，请稍候...")]
    public void ShouldPublish_ShouldReturnFalse_WhenCurrentShellMessageIsKnownPlaceholder(string placeholder)
    {
        var result = ShellStatusRelayPolicy.ShouldPublish(
            message: "搜索完成",
            isTextBlockVisible: true,
            currentShellMessage: placeholder,
            canPublishFromView: true);

        Assert.False(result);
    }

    [Fact]
    public void ShouldPublish_ShouldReturnTrue_WhenCurrentShellMessageIsNotKnownPlaceholder()
    {
        var result = ShellStatusRelayPolicy.ShouldPublish(
            message: "下载完成",
            isTextBlockVisible: true,
            currentShellMessage: "下载失败，重试中...",
            canPublishFromView: true);

        Assert.True(result);
    }
}
