using Asmroner.Core.Utils;

namespace Asmroner.Wpf.ViewModels;

public static class DownloadInputNormalizer
{
    public static string NormalizeSingleInputDisplay(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return string.Empty;
        }

        return NormalizeToken(raw.Trim());
    }

    public static string NormalizeBatchInputDisplay(string raw)
    {
        return string.Join(", ", ExtractNormalizedSourceIds(raw));
    }

    public static string NormalizeBatchInputForSubmit(string raw)
    {
        return string.Join(", ", ExtractNormalizedSourceIds(raw));
    }

    public static IReadOnlyList<string> ExtractNormalizedSourceIds(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Array.Empty<string>();
        }

        var separators = new[] { ',', ';', ' ', '\t', '\r', '\n' };
        return raw
            .Split(separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(NormalizeToken)
            .Where(static id => !string.IsNullOrWhiteSpace(id))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static string NormalizeToken(string token)
    {
        return SourceIdNormalizer.Normalize(token);
    }
}