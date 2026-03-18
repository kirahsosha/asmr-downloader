namespace Asmroner.Wpf.ViewModels;

public static class DownloadDirectoryPathPolicy
{
    public static string Resolve(string? configuredPath, string defaultPath)
    {
        return string.IsNullOrWhiteSpace(configuredPath)
            ? defaultPath
            : configuredPath.Trim();
    }
}
