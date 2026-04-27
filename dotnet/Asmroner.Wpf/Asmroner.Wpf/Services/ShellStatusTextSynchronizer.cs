using System.ComponentModel;
using System.Windows.Controls;

namespace Asmroner.Wpf.Services;

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
            if (textBlock.IsVisible
                && string.Equals(uiMessageService.CurrentMessage, "初始化中...", StringComparison.Ordinal))
            {
                return;
            }

            if (shouldPublish() && !string.IsNullOrWhiteSpace(message))
            {
                uiMessageService.ShowInfo(message);
            }
        }

        var descriptor = DependencyPropertyDescriptor.FromProperty(TextBlock.TextProperty, typeof(TextBlock));
        descriptor?.AddValueChanged(textBlock, (_, _) => PublishCurrentMessage());
    }
}