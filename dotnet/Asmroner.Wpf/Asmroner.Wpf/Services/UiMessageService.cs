using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Services;

public enum MessageLevel
{
    None = 0,
    Info = 1,
    Warning = 2,
    Error = 3,
}

public interface IUiMessageService
{
    string CurrentMessage { get; }

    MessageLevel LastMessageLevel { get; }

    void ShowInfo(string message);

    void ShowWarning(string message);

    void ShowError(string message);
}

public sealed class UiMessageService : IUiMessageService
{
    private readonly ShellViewModel _shellViewModel;
    private MessageLevel _lastMessageLevel = MessageLevel.None;

    public UiMessageService(ShellViewModel shellViewModel)
    {
        _shellViewModel = shellViewModel;
    }

    public string CurrentMessage => _shellViewModel.StatusMessage;

    public MessageLevel LastMessageLevel => _lastMessageLevel;

    public void ShowInfo(string message)
    {
        if (_lastMessageLevel >= MessageLevel.Warning)
        {
            return;
        }

        _lastMessageLevel = MessageLevel.Info;
        _shellViewModel.StatusMessage = message;
    }

    public void ShowWarning(string message)
    {
        if (_lastMessageLevel >= MessageLevel.Error)
        {
            return;
        }

        _lastMessageLevel = MessageLevel.Warning;
        _shellViewModel.StatusMessage = message;
    }

    public void ShowError(string message)
    {
        _lastMessageLevel = MessageLevel.Error;
        _shellViewModel.StatusMessage = message;
    }
}

public sealed class NoOpUiMessageService : IUiMessageService
{
    public static NoOpUiMessageService Instance { get; } = new();

    public string CurrentMessage => string.Empty;

    public MessageLevel LastMessageLevel => MessageLevel.None;

    public void ShowInfo(string message)
    {
    }

    public void ShowWarning(string message)
    {
    }

    public void ShowError(string message)
    {
    }
}