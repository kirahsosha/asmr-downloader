using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Asmroner.Wpf.ViewModels;

public enum ShellPage
{
    Search = 0,
    Download = 1,
    Library = 2,
    Sync = 3,
    Settings = 4,
}

public sealed class ShellViewModel : INotifyPropertyChanged
{
    private const int MaxPageIndex = (int)ShellPage.Settings;

    private int _selectedPageIndex = (int)ShellPage.Settings;
    private string _statusMessage = "初始化中...";
    private bool _isSearchEnabled;
    private bool _isDownloadEnabled;
    private bool _isLibraryEnabled;
    private bool _isSyncEnabled;

    public event PropertyChangedEventHandler? PropertyChanged;

    public int SelectedPageIndex
    {
        get => _selectedPageIndex;
        set
        {
            var normalizedIndex = Math.Clamp(value, 0, MaxPageIndex);
            if (SetProperty(ref _selectedPageIndex, normalizedIndex))
            {
                OnPropertyChanged(nameof(SelectedPage));
            }
        }
    }

    public ShellPage SelectedPage
    {
        get => (ShellPage)_selectedPageIndex;
        set => SelectedPageIndex = (int)value;
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public bool IsSearchEnabled
    {
        get => _isSearchEnabled;
        set => SetProperty(ref _isSearchEnabled, value);
    }

    public bool IsDownloadEnabled
    {
        get => _isDownloadEnabled;
        set => SetProperty(ref _isDownloadEnabled, value);
    }

    public bool IsLibraryEnabled
    {
        get => _isLibraryEnabled;
        set => SetProperty(ref _isLibraryEnabled, value);
    }

    public bool IsSyncEnabled
    {
        get => _isSyncEnabled;
        set => SetProperty(ref _isSyncEnabled, value);
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}