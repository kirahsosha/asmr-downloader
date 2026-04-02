namespace Asmroner.Wpf.ViewModels;

public static class FavoriteFolderSelectionPolicy
{
    public static string NormalizeFolderTitle(string? folderTitle)
    {
        return (folderTitle ?? string.Empty).Trim();
    }

    public static IReadOnlyList<string> BuildFolderTitles(IEnumerable<string> folderTitles)
    {
        return folderTitles
            .Select(NormalizeFolderTitle)
            .Where(static title => !string.IsNullOrWhiteSpace(title))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(static title => title, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public static bool CanConfirm(string? folderTitle, bool allowCustomInput, IReadOnlyCollection<string> existingFolderTitles)
    {
        var normalizedFolderTitle = NormalizeFolderTitle(folderTitle);
        if (string.IsNullOrWhiteSpace(normalizedFolderTitle))
        {
            return false;
        }

        return allowCustomInput
            || existingFolderTitles.Contains(normalizedFolderTitle, StringComparer.OrdinalIgnoreCase);
    }
}