using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Asmroner.Wpf.Services;

public interface IPageLoadStateService
{
    PageLoadState Create(string pageName);
}

public sealed class PageLoadStateService : IPageLoadStateService
{
    public PageLoadState Create(string pageName)
    {
        return new PageLoadState(pageName);
    }
}

public sealed class PageLoadState : INotifyPropertyChanged
{
    private readonly string _defaultBusyMessage;
    private int _busyCount;
    private bool _isBusy;
    private string _busyMessage = string.Empty;
    private bool _isEmptyVisible;
    private string _emptyTitle = string.Empty;
    private string _emptyDescription = string.Empty;

    public PageLoadState(string pageName)
    {
        PageName = pageName;
        _defaultBusyMessage = $"{PageName} 正在处理，请稍候...";
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string PageName { get; }

    public bool IsBusy
    {
        get => _isBusy;
        private set => SetProperty(ref _isBusy, value);
    }

    public string BusyMessage
    {
        get => _busyMessage;
        private set => SetProperty(ref _busyMessage, value);
    }

    public bool IsEmptyVisible
    {
        get => _isEmptyVisible;
        private set => SetProperty(ref _isEmptyVisible, value);
    }

    public string EmptyTitle
    {
        get => _emptyTitle;
        private set => SetProperty(ref _emptyTitle, value);
    }

    public string EmptyDescription
    {
        get => _emptyDescription;
        private set => SetProperty(ref _emptyDescription, value);
    }

    public void ShowBusy(string message)
    {
        _busyCount++;
        BusyMessage = string.IsNullOrWhiteSpace(message) ? _defaultBusyMessage : message;
        IsBusy = true;
    }

    public void HideBusy()
    {
        if (_busyCount <= 0)
        {
            IsBusy = false;
            BusyMessage = string.Empty;
            return;
        }

        _busyCount--;
        if (_busyCount == 0)
        {
            IsBusy = false;
            BusyMessage = string.Empty;
            return;
        }

        IsBusy = true;
        BusyMessage = _defaultBusyMessage;
    }

    public void ShowEmpty(string title, string description)
    {
        EmptyTitle = title;
        EmptyDescription = description;
        IsEmptyVisible = true;
    }

    public void ClearEmpty()
    {
        IsEmptyVisible = false;
        EmptyTitle = string.Empty;
        EmptyDescription = string.Empty;
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}