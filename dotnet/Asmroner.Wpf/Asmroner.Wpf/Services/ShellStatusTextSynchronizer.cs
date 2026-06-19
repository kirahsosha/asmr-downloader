using System.ComponentModel;
using System.Windows.Controls;

namespace Asmroner.Wpf.Services;

public static class ShellStatusRelayPolicy
{
    public static bool ShouldPublish(
        string message,
        bool isTextBlockVisible,
        string currentShellMessage,
        bool canPublishFromView)
    {
        if (!canPublishFromView)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            return false;
        }

        if (isTextBlockVisible
            && string.Equals(currentShellMessage, "初始化中...", StringComparison.Ordinal))
        {
            return false;
        }

        return true;
    }
}

internal static class ShellStatusTextSynchronizer
{
    public static void Attach(TextBlock textBlock, IUiMessageService uiMessageService, Func<bool>? shouldPublish = null)
    {
        ArgumentNullException.ThrowIfNull(textBlock);
        ArgumentNullException.ThrowIfNull(uiMessageService);

        shouldPublish ??= () => textBlock.IsVisible;

        void PublishCurrentMessage()
        {
            var message = textBlock.Text;
            if (ShellStatusRelayPolicy.ShouldPublish(
                    message,
                    textBlock.IsVisible,
                    uiMessageService.CurrentMessage,
                    shouldPublish()))
            {
                uiMessageService.ShowInfo(message);
            }
        }

        var descriptor = DependencyPropertyDescriptor.FromProperty(TextBlock.TextProperty, typeof(TextBlock));
        descriptor?.AddValueChanged(textBlock, (_, _) => PublishCurrentMessage());
    }
}