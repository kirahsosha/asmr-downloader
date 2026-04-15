using System.Text.RegularExpressions;

namespace Asmroner.Core.Library;

public sealed class LibraryQuery
{
    public string Keyword { get; init; } = string.Empty;

    public bool SubtitleOnly { get; init; }

    public bool AudioOnly { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}

public sealed class LibraryFileItem
{
    public string Name { get; init; } = string.Empty;

    public string RelativePath { get; init; } = string.Empty;

    public string FullPath { get; init; } = string.Empty;

    public string Extension { get; init; } = string.Empty;

    public bool IsDirectory { get; init; }

    public bool IsPlayable { get; init; }

    public long SizeBytes { get; init; }

    public IReadOnlyList<LibraryFileItem> Children { get; init; } = Array.Empty<LibraryFileItem>();

    public string DisplayName => IsDirectory ? $"{Name}/" : Name;
}

public sealed class LibraryWorkItem
{
    public int WorkId { get; init; }

    public string SourceId { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public string Release { get; init; } = string.Empty;

    public bool HasSubtitle { get; init; }

    public string Tags { get; init; } = string.Empty;

    public string RootDirectory { get; init; } = string.Empty;

    public string SourceRoot { get; init; } = string.Empty;

    public string DirectoryScheme { get; init; } = string.Empty;

    public int TotalFileCount { get; init; }

    public int AudioFileCount { get; init; }

    public IReadOnlyList<LibraryFileItem> Files { get; init; } = Array.Empty<LibraryFileItem>();
}

public sealed class LibraryScanResult
{
    public IReadOnlyList<LibraryWorkItem> Items { get; init; } = Array.Empty<LibraryWorkItem>();

    public IReadOnlyList<string> SkippedDirectories { get; init; } = Array.Empty<string>();

    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();

    public int ScannedRootCount { get; init; }
}

public sealed class LibraryQueryResult
{
    public IReadOnlyList<LibraryWorkItem> Items { get; init; } = Array.Empty<LibraryWorkItem>();

    public IReadOnlyList<string> SkippedDirectories { get; init; } = Array.Empty<string>();

    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();

    public int TotalCount { get; init; }

    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalPages { get; init; }

    public int ScannedRootCount { get; init; }

    public int ScannedWorkCount { get; init; }
}

public sealed class LibraryDirectoryNameParseResult
{
    public string SourceId { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public string Release { get; init; } = string.Empty;

    public bool? HasSubtitle { get; init; }

    public string Scheme { get; init; } = string.Empty;
}

public static class LibraryDirectoryNameParser
{
    private static readonly Regex LegacyPattern = new(
        "^(?<sourceId>[^-\\s]+)-(?<date>\\d{8})-(?<subtitle>sub|nosub)-(?<title>.+)$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase,
        TimeSpan.FromSeconds(1));

    private static readonly Regex BracketPattern = new(
        "^\\[(?<sourceId>[^\\]]+)\\](?<title>.+)$",
        RegexOptions.Compiled,
        TimeSpan.FromSeconds(1));

    public static bool TryParse(string? directoryName, out LibraryDirectoryNameParseResult result)
    {
        result = new LibraryDirectoryNameParseResult();

        if (string.IsNullOrWhiteSpace(directoryName))
        {
            return false;
        }

        var trimmed = directoryName.Trim();

        var bracketMatch = BracketPattern.Match(trimmed);
        if (bracketMatch.Success)
        {
            var sourceId = bracketMatch.Groups["sourceId"].Value.Trim();
            var title = bracketMatch.Groups["title"].Value.Trim();

            if (string.IsNullOrWhiteSpace(sourceId) || string.IsNullOrWhiteSpace(title))
            {
                return false;
            }

            result = new LibraryDirectoryNameParseResult
            {
                SourceId = sourceId,
                Title = title,
                Scheme = "Bracketed",
            };
            return true;
        }

        var legacyMatch = LegacyPattern.Match(trimmed);
        if (!legacyMatch.Success)
        {
            return false;
        }

        var legacySourceId = legacyMatch.Groups["sourceId"].Value.Trim();
        var legacyTitle = legacyMatch.Groups["title"].Value.Trim();
        var date = legacyMatch.Groups["date"].Value.Trim();
        var subtitleFlag = legacyMatch.Groups["subtitle"].Value.Trim();

        if (string.IsNullOrWhiteSpace(legacySourceId) || string.IsNullOrWhiteSpace(legacyTitle) || date.Length != 8)
        {
            return false;
        }

        result = new LibraryDirectoryNameParseResult
        {
            SourceId = legacySourceId,
            Title = legacyTitle,
            Release = FormatLegacyDate(date),
            HasSubtitle = string.Equals(subtitleFlag, "sub", StringComparison.OrdinalIgnoreCase),
            Scheme = "LegacyListen",
        };
        return true;
    }

    private static string FormatLegacyDate(string rawDate)
    {
        return $"{rawDate[..4]}-{rawDate[4..6]}-{rawDate[6..8]}";
    }
}