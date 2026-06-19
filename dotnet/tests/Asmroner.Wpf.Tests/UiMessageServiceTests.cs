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
}