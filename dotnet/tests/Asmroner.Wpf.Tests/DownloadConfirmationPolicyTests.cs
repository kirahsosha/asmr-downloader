using System.Windows;
using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class DownloadConfirmationPolicyTests
{
    [Fact]
    public void Evaluate_ShouldContinue_WhenResultIsYes()
    {
        var decision = DownloadConfirmationPolicy.Evaluate(MessageBoxResult.Yes);

        Assert.True(decision.ShouldContinue);
        Assert.Null(decision.StatusText);
    }

    [Theory]
    [InlineData(MessageBoxResult.No)]
    [InlineData(MessageBoxResult.Cancel)]
    [InlineData(MessageBoxResult.None)]
    public void Evaluate_ShouldCancelOperation_WhenResultIsNotYes(MessageBoxResult result)
    {
        var decision = DownloadConfirmationPolicy.Evaluate(result);

        Assert.False(decision.ShouldContinue);
        Assert.Equal("已取消本次操作。", decision.StatusText);
    }
}
