using Asmroner.Wpf.Services;
using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class UiMessageServiceTests
{
    [Fact]
    public void ShowInfo_ShouldUpdateShellStatusMessage()
    {
        var viewModel = new ShellViewModel();
        var service = new UiMessageService(viewModel);

        service.ShowInfo("搜索完成，共 42 条结果。");

        Assert.Equal("搜索完成，共 42 条结果。", service.CurrentMessage);
        Assert.Equal("搜索完成，共 42 条结果。", viewModel.StatusMessage);
    }

    [Fact]
    public void ShowWarning_ShouldUpdateShellStatusMessage()
    {
        var viewModel = new ShellViewModel();
        var service = new UiMessageService(viewModel);

        service.ShowWarning("当前连接不稳定，已切换到备用端点。");

        Assert.Equal("当前连接不稳定，已切换到备用端点。", service.CurrentMessage);
        Assert.Equal("当前连接不稳定，已切换到备用端点。", viewModel.StatusMessage);
    }

    [Fact]
    public void ShowError_ShouldUpdateShellStatusMessage()
    {
        var viewModel = new ShellViewModel();
        var service = new UiMessageService(viewModel);

        service.ShowError("初始化失败，请检查设置页后重试。");

        Assert.Equal("初始化失败，请检查设置页后重试。", service.CurrentMessage);
        Assert.Equal("初始化失败，请检查设置页后重试。", viewModel.StatusMessage);
    }

    [Fact]
    public void ShowInfo_ShouldNotOverwriteWarning()
    {
        var viewModel = new ShellViewModel();
        var service = new UiMessageService(viewModel);

        service.ShowWarning("连接不稳定，已切换备用端点。");
        service.ShowInfo("数据加载完成，共 30 条。");

        Assert.Equal("连接不稳定，已切换备用端点。", service.CurrentMessage);
    }

    [Fact]
    public void ShowWarning_ShouldNotOverwriteError()
    {
        var viewModel = new ShellViewModel();
        var service = new UiMessageService(viewModel);

        service.ShowError("同步失败，请检查网络。");
        service.ShowWarning("连接不稳定。");

        Assert.Equal("同步失败，请检查网络。", service.CurrentMessage);
    }

    [Fact]
    public void ShowError_ShouldAlwaysOverwrite()
    {
        var viewModel = new ShellViewModel();
        var service = new UiMessageService(viewModel);

        service.ShowInfo("数据加载完成。");
        service.ShowError("同步失败，请检查网络。");

        Assert.Equal("同步失败，请检查网络。", service.CurrentMessage);
    }

    [Fact]
    public void ShowInfo_ShouldOverwritePreviousInfo()
    {
        var viewModel = new ShellViewModel();
        var service = new UiMessageService(viewModel);

        service.ShowInfo("搜索完成，共 42 条。");
        service.ShowInfo("下载已启动。");

        Assert.Equal("下载已启动。", service.CurrentMessage);
    }

    [Fact]
    public void LastMessageLevel_ShouldTrackHighestLevel()
    {
        var viewModel = new ShellViewModel();
        var service = new UiMessageService(viewModel);

        Assert.Equal(MessageLevel.None, service.LastMessageLevel);

        service.ShowInfo("信息消息");
        Assert.Equal(MessageLevel.Info, service.LastMessageLevel);

        service.ShowWarning("警告消息");
        Assert.Equal(MessageLevel.Warning, service.LastMessageLevel);

        service.ShowError("错误消息");
        Assert.Equal(MessageLevel.Error, service.LastMessageLevel);
    }
}