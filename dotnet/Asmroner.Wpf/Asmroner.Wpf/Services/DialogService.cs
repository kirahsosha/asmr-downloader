using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace Asmroner.Wpf.Services;

public interface IDialogService
{
    bool ConfirmQuestion(string message, string title, MessageBoxResult defaultResult = MessageBoxResult.No);

    string? ShowOpenFileDialog(OpenFileDialogOptions options);

    SaveFileDialogResult? ShowSaveFileDialog(SaveFileDialogOptions options);
}

public sealed record OpenFileDialogOptions
{
    public required string Title { get; init; }

    public string? InitialDirectory { get; init; }

    public string Filter { get; init; } = "所有文件 (*.*)|*.*";

    public string DefaultExt { get; init; } = string.Empty;
}

public sealed record SaveFileDialogOptions
{
    public required string Title { get; init; }

    public string? InitialDirectory { get; init; }

    public required string FileName { get; init; }

    public string DefaultExt { get; init; } = ".csv";

    public bool AddExtension { get; init; } = true;

    public bool OverwritePrompt { get; init; } = true;

    public string Filter { get; init; } = "CSV 文件 (*.csv)|*.csv|JSON 文件 (*.json)|*.json|所有文件 (*.*)|*.*";

    public int FilterIndex { get; init; } = 1;
}

public readonly record struct SaveFileDialogResult(string FullPath, string Extension);

public sealed class DialogService : IDialogService
{
    public bool ConfirmQuestion(string message, string title, MessageBoxResult defaultResult = MessageBoxResult.No)
    {
        var result = MessageBox.Show(
            message,
            title,
            MessageBoxButton.YesNo,
            MessageBoxImage.Question,
            defaultResult);

        return result == MessageBoxResult.Yes;
    }

    public string? ShowOpenFileDialog(OpenFileDialogOptions options)
    {
        var dialog = new OpenFileDialog
        {
            Title = options.Title,
            InitialDirectory = options.InitialDirectory,
            Filter = options.Filter,
            DefaultExt = options.DefaultExt,
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    public SaveFileDialogResult? ShowSaveFileDialog(SaveFileDialogOptions options)
    {
        var dialog = new SaveFileDialog
        {
            Title = options.Title,
            InitialDirectory = options.InitialDirectory,
            FileName = options.FileName,
            DefaultExt = options.DefaultExt,
            AddExtension = options.AddExtension,
            OverwritePrompt = options.OverwritePrompt,
            Filter = options.Filter,
            FilterIndex = options.FilterIndex,
        };

        if (dialog.ShowDialog() != true)
        {
            return null;
        }

        var extension = DialogFileNamePolicy.ResolveExtension(dialog.FileName, dialog.FilterIndex);
        var fullPath = DialogFileNamePolicy.EnsureExtension(dialog.FileName, extension);
        return new SaveFileDialogResult(fullPath, extension);
    }
}

public static class DialogFileNamePolicy
{
    public static string ResolveExtension(string filePath, int filterIndex)
    {
        var extension = Path.GetExtension(filePath);
        if (extension.Equals(".csv", StringComparison.OrdinalIgnoreCase))
        {
            return "csv";
        }

        if (extension.Equals(".json", StringComparison.OrdinalIgnoreCase))
        {
            return "json";
        }

        return filterIndex == 2 ? "json" : "csv";
    }

    public static string EnsureExtension(string filePath, string extension)
    {
        var normalizedExtension = "." + extension;
        var currentExtension = Path.GetExtension(filePath);
        if (currentExtension.Equals(normalizedExtension, StringComparison.OrdinalIgnoreCase))
        {
            return filePath;
        }

        if (string.IsNullOrWhiteSpace(currentExtension))
        {
            return filePath + normalizedExtension;
        }

        return Path.ChangeExtension(filePath, extension);
    }
}