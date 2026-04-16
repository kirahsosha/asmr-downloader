using System.IO;

using Asmroner.Core.Library;

namespace Asmroner.Wpf.Views;

public sealed class LibrarySelectionFeedbackResult
{
    public LibraryFileItem? PlayableTarget { get; init; }

    public LibraryFileItem? SuggestedTarget { get; init; }

    public int SuggestedPlayableCount { get; init; }

    public bool CanLoadContext { get; init; }

    public bool CanPlay { get; init; }

    public string Message { get; init; } = string.Empty;
}

public static class LibrarySelectionFeedbackPolicy
{
    public static LibrarySelectionFeedbackResult Evaluate(LibraryFileItem? selectedItem)
    {
        return Evaluate(null, selectedItem, File.Exists);
    }

    public static LibrarySelectionFeedbackResult Evaluate(
        LibraryFileItem? selectedItem,
        Func<string, bool> fileExists)
    {
        return Evaluate(null, selectedItem, fileExists);
    }

    public static LibrarySelectionFeedbackResult Evaluate(
        LibraryWorkItem? selectedWork,
        LibraryFileItem? selectedItem)
    {
        return Evaluate(selectedWork, selectedItem, File.Exists);
    }

    public static LibrarySelectionFeedbackResult Evaluate(
        LibraryWorkItem? selectedWork,
        LibraryFileItem? selectedItem,
        Func<string, bool> fileExists)
    {
        ArgumentNullException.ThrowIfNull(fileExists);

        var suggestedPlayableCount = selectedWork is null
            ? 0
            : LibraryPlayableMediaPolicy.CountPlayableFiles(selectedWork.Files);
        var suggestedTarget = selectedWork is null
            ? null
            : FindSuggestedTarget(selectedWork.Files, selectedItem);

        if (selectedItem is null)
        {
            if (selectedWork is null)
            {
                return new LibrarySelectionFeedbackResult();
            }

            if (suggestedTarget is null || suggestedPlayableCount == 0)
            {
                return new LibrarySelectionFeedbackResult
                {
                    Message = $"当前作品未发现可播放媒体文件。当前支持格式：{LibraryPlayableMediaPolicy.SupportedExtensionsText}",
                };
            }

            return new LibrarySelectionFeedbackResult
            {
                SuggestedTarget = suggestedTarget,
                SuggestedPlayableCount = suggestedPlayableCount,
                Message = $"当前作品包含 {suggestedPlayableCount} 个可播放媒体文件，首个候选：{suggestedTarget.RelativePath}。请在文件树中显式选中后再载入或播放。",
            };
        }

        if (selectedItem.IsDirectory)
        {
            return BuildBlockedSelectionResult(
                "当前选择是目录，请继续选择一个可播放的媒体文件",
                selectedItem,
                suggestedTarget,
                suggestedPlayableCount,
                appendSupportedFormatsWhenNoSuggestion: true);
        }

        if (!LibraryPlayableMediaPolicy.IsPlayableExtension(selectedItem.Extension))
        {
            return BuildBlockedSelectionResult(
                $"当前选择的文件不可播放：{selectedItem.RelativePath}",
                selectedItem,
                suggestedTarget,
                suggestedPlayableCount,
                appendSupportedFormatsWhenNoSuggestion: true);
        }

        if (!fileExists(selectedItem.FullPath))
        {
            return BuildBlockedSelectionResult(
                $"当前选择的媒体文件不存在：{selectedItem.RelativePath}",
                selectedItem,
                suggestedTarget,
                suggestedPlayableCount,
                appendSupportedFormatsWhenNoSuggestion: false,
                suggestionPrefix: "可改选");
        }

        return new LibrarySelectionFeedbackResult
        {
            PlayableTarget = selectedItem,
            SuggestedTarget = selectedItem,
            SuggestedPlayableCount = suggestedPlayableCount > 0 ? suggestedPlayableCount : 1,
            CanLoadContext = true,
            CanPlay = true,
            Message = $"已选择可播放媒体文件：{selectedItem.RelativePath}，可载入或直接播放。",
        };
    }

    private static LibrarySelectionFeedbackResult BuildBlockedSelectionResult(
        string baseMessage,
        LibraryFileItem selectedItem,
        LibraryFileItem? suggestedTarget,
        int suggestedPlayableCount,
        bool appendSupportedFormatsWhenNoSuggestion,
        string suggestionPrefix = "可尝试选择")
    {
        var message = $"{baseMessage}。";
        if (suggestedTarget is not null && !SamePath(selectedItem.FullPath, suggestedTarget.FullPath))
        {
            message = $"{message}{suggestionPrefix}：{suggestedTarget.RelativePath}";
        }
        else if (appendSupportedFormatsWhenNoSuggestion)
        {
            message = $"{message}当前支持格式：{LibraryPlayableMediaPolicy.SupportedExtensionsText}";
        }

        return new LibrarySelectionFeedbackResult
        {
            SuggestedTarget = suggestedTarget,
            SuggestedPlayableCount = suggestedPlayableCount,
            Message = message,
        };
    }

    private static LibraryFileItem? FindSuggestedTarget(
        IReadOnlyList<LibraryFileItem> items,
        LibraryFileItem? selectedItem)
    {
        foreach (var item in items)
        {
            if (item.IsDirectory)
            {
                var nested = FindSuggestedTarget(item.Children, selectedItem);
                if (nested is not null)
                {
                    return nested;
                }

                continue;
            }

            if (!LibraryPlayableMediaPolicy.IsPlayableExtension(item.Extension))
            {
                continue;
            }

            if (selectedItem is not null && SamePath(selectedItem.FullPath, item.FullPath))
            {
                continue;
            }

            return item;
        }

        return null;
    }

    private static bool SamePath(string? left, string? right)
    {
        return string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
    }
}