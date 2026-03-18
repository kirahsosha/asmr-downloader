namespace Asmroner.Wpf.ViewModels;

public static class SearchFilterValuePolicy
{
    private static readonly char[] ValueSeparators = [' ', '\t', '\r', '\n', ';', ','];

    public static string MergeDistinct(string? existingText, string? candidateValue)
    {
        var normalizedExisting = existingText ?? string.Empty;
        var normalizedCandidate = candidateValue?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(normalizedCandidate))
        {
            return normalizedExisting;
        }

        var existingValues = normalizedExisting
            .Split(ValueSeparators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (existingValues.Contains(normalizedCandidate, StringComparer.OrdinalIgnoreCase))
        {
            return normalizedExisting;
        }

        if (string.IsNullOrWhiteSpace(normalizedExisting))
        {
            return normalizedCandidate;
        }

        return string.Concat(normalizedExisting, ",", normalizedCandidate);
    }
}
