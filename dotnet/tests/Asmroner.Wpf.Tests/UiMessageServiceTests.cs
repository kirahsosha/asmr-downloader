using Asmroner.Wpf.Services;
using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class UiMessageServiceTests
{
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