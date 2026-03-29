using Asmroner.Core.Configuration;

namespace Asmroner.Core.Download;

public static class DownloadFilterParser
{
    public static IReadOnlyList<string> ParsePreferExtensions(DownloaderOptions? options)
    {
        if (options is null)
        {
            return Array.Empty<string>();
        }

        return ParseFormatGroup(options.PreferFormats);
    }

    public static IReadOnlyList<(string Term, bool IsExclude)> ParseFileFilter(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Array.Empty<(string, bool)>();
        }

        return raw
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(static token =>
            {
                if (token.StartsWith('-'))
                {
                    return (token[1..].Trim(), true);
                }

                if (token.StartsWith('+'))
                {
                    return (token[1..].Trim(), false);
                }

                return (token, false);
            })
            .Where(static item => !string.IsNullOrWhiteSpace(item.Item1))
            .ToArray();
    }

    public static bool MatchesFileFilter(string path, IReadOnlyList<(string Term, bool IsExclude)> filter)
    {
        if (filter.Count == 0)
        {
            return true;
        }

        foreach (var (term, isExclude) in filter)
        {
            var contains = path.Contains(term, StringComparison.OrdinalIgnoreCase);
            if (isExclude && contains)
            {
                return false;
            }

            if (!isExclude && !contains)
            {
                return false;
            }
        }

        return true;
    }

    private static IReadOnlyList<string> ParseFormatGroup(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            // 留空表示全部下载（不限扩展名）
            return Array.Empty<string>();
        }

        return raw
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(static item => item.StartsWith('.') ? item.ToLowerInvariant() : "." + item.ToLowerInvariant())
            .ToArray();
    }

    /// <summary>
    /// When <paramref name="hdAudioOnly"/> is true and <paramref name="entries"/> contain at least one
    /// HD audio file (wav/flac), removes all MP3 entries and returns the filtered list.
    /// If no HD audio is present, or <paramref name="hdAudioOnly"/> is false, the original list is returned unchanged.
    /// </summary>
    public static IReadOnlyList<T> FilterHdAudioOnly<T>(
        IReadOnlyList<T> entries,
        Func<T, string> urlSelector,
        bool hdAudioOnly = true)
    {
        if (!hdAudioOnly)
        {
            return entries;
        }

        bool HasExtension(T entry, string ext) =>
            urlSelector(entry).EndsWith(ext, StringComparison.OrdinalIgnoreCase);

        var hasHdAudio = entries.Any(e => HasExtension(e, ".wav") || HasExtension(e, ".flac"));
        if (!hasHdAudio)
        {
            return entries;
        }

        return entries.Where(e => !HasExtension(e, ".mp3")).ToArray();
    }
}