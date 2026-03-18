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

        var unified = options.PreferFormats;
        if (string.IsNullOrWhiteSpace(unified))
        {
            unified = string.Join(",", new[] { options.PreferMedia, options.PreferImage, options.PreferVideo }
                .Where(static s => !string.IsNullOrWhiteSpace(s)));
        }

        return ParseFormatGroup(unified);
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
}