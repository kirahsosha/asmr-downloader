using System.IO;

using Asmroner.Core.Library;

namespace Asmroner.Wpf.Views;

public sealed class LibrarySelectionFeedbackResult
{
    public LibraryFileItem? PlayableTarget { get; init; }

    public bool CanLoadContext { get; init; }

    public bool CanPlay { get; init; }

    public string Message { get; init; } = string.Empty;
}

public static class LibrarySelectionFeedbackPolicy
{
    public static LibrarySelectionFeedbackResult Evaluate(LibraryFileItem? selectedItem)
    {
        return Evaluate(selectedItem, File.Exists);
    }

    public static LibrarySelectionFeedbackResult Evaluate(
        LibraryFileItem? selectedItem,
        Func<string, bool> fileExists)
    {
        ArgumentNullException.ThrowIfNull(fileExists);

        if (selectedItem is null)
        {
            return new LibrarySelectionFeedbackResult();
        }

        if (selectedItem.IsDirectory)
        {
            return new LibrarySelectionFeedbackResult
            {
                Message = "当前选择是目录，请继续选择一个可播放的媒体文件。",
            };
        }

        if (!selectedItem.IsPlayable)
        {
            return new LibrarySelectionFeedbackResult
            {
                Message = $"当前选择的文件不可播放：{selectedItem.RelativePath}",
            };
        }

        if (!fileExists(selectedItem.FullPath))
        {
            return new LibrarySelectionFeedbackResult
            {
                Message = $"当前选择的媒体文件不存在：{selectedItem.RelativePath}",
            };
        }

        return new LibrarySelectionFeedbackResult
        {
            PlayableTarget = selectedItem,
            CanLoadContext = true,
            CanPlay = true,
            Message = $"已选择可播放媒体文件：{selectedItem.RelativePath}，可载入或直接播放。",
        };
    }
}