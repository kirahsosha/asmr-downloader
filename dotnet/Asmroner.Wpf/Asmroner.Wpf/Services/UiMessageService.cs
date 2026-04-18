using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Services;

public interface IUiMessageService
{
    string CurrentMessage { get; }

    void ShowInfo(string message);

    void ShowWarning(string message);

    void ShowError(string message);
}

public sealed class UiMessageService : IUiMessageService
{
    private readonly ShellViewModel _shellViewModel;

    public UiMessageService(ShellViewModel shellViewModel)
    {
        _shellViewModel = shellViewModel;
    }

    public string CurrentMessage => _shellViewModel.StatusMessage;

    public void ShowInfo(string message)
    {
        _shellViewModel.StatusMessage = message;
    }

    public void ShowWarning(string message)
    {
        _shellViewModel.StatusMessage = message;
    }

    public void ShowError(string message)
    {
        _shellViewModel.StatusMessage = message;
    }
}