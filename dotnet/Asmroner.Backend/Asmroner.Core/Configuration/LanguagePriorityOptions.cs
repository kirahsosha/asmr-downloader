namespace Asmroner.Core.Configuration;

public static class LanguagePriorityOptions
{
    public static readonly string[] DefaultOrder = ["简体中文", "繁体中文", "日本語"];

    public static IReadOnlyList<string> ParseOrDefault(string? raw)
    {
        var parsed = Parse(raw);
        return parsed.Count == 0 ? DefaultOrder : parsed;
    }

    public static IReadOnlyList<string> Parse(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Array.Empty<string>();
        }

        var result = new List<string>();
        foreach (var token in raw.Split([',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var normalized = NormalizeLanguage(token);
            if (string.IsNullOrWhiteSpace(normalized))
            {
                continue;
            }

            if (result.Contains(normalized, StringComparer.Ordinal))
            {
                continue;
            }

            result.Add(normalized);
        }

        return result;
    }

    public static IReadOnlyList<string> FindUnsupportedLanguages(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Array.Empty<string>();
        }

        var unsupported = new List<string>();
        foreach (var token in raw.Split([',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (!string.IsNullOrWhiteSpace(NormalizeLanguage(token)))
            {
                continue;
            }

            if (unsupported.Contains(token, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            unsupported.Add(token);
        }

        return unsupported;
    }

    public static string NormalizeLanguage(string? rawLanguage)
    {
        if (string.IsNullOrWhiteSpace(rawLanguage))
        {
            return string.Empty;
        }

        var normalized = rawLanguage.Trim().Replace('-', '_').ToUpperInvariant();

        return normalized switch
        {
            "简体中文" or "简中" or "ZH_CN" or "ZH_HANS" or "CHI_HANS" or "CHS" => "简体中文",
            "繁体中文" or "繁中" or "ZH_TW" or "ZH_HANT" or "CHI_HANT" or "CHT" => "繁体中文",
            "日本語" or "日语" or "日文" or "JA" or "JA_JP" or "JPN" => "日本語",
            _ => string.Empty,
        };
    }
}
