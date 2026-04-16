namespace Asmroner.Core.Library;

public static class LibraryPlayableMediaPolicy
{
    private static readonly string[] SupportedExtensions =
    [
        ".mp3",
        ".wav",
        ".flac",
        ".m4a",
        ".ogg",
        ".aac",
        ".opus",
        ".wma",
        ".webm",
    ];

    private static readonly HashSet<string> SupportedExtensionSet = new(SupportedExtensions, StringComparer.OrdinalIgnoreCase);

    public static string SupportedExtensionsText { get; } = string.Join(", ", SupportedExtensions);

    public static bool IsPlayableExtension(string? extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
        {
            return false;
        }

        return SupportedExtensionSet.Contains(NormalizeExtension(extension));
    }

    public static int CountPlayableFiles(IReadOnlyList<LibraryFileItem> items)
    {
        var count = 0;
        foreach (var item in items)
        {
            count += item.IsDirectory
                ? CountPlayableFiles(item.Children)
                : IsPlayableExtension(item.Extension) ? 1 : 0;
        }

        return count;
    }

    public static LibraryFileItem? FindFirstPlayableFile(IReadOnlyList<LibraryFileItem> items)
    {
        foreach (var item in items)
        {
            if (item.IsDirectory)
            {
                var nested = FindFirstPlayableFile(item.Children);
                if (nested is not null)
                {
                    return nested;
                }

                continue;
            }

            if (IsPlayableExtension(item.Extension))
            {
                return item;
            }
        }

        return null;
    }

    private static string NormalizeExtension(string extension)
    {
        var trimmed = extension.Trim();
        if (trimmed.Length == 0)
        {
            return string.Empty;
        }

        return trimmed.StartsWith(".", StringComparison.Ordinal)
            ? trimmed
            : $".{trimmed}";
    }
}