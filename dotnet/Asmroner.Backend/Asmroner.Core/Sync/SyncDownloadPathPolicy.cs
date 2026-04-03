namespace Asmroner.Core.Sync;

public static class SyncDownloadPathPolicy
{
    public static string BuildTargetDirectory(string targetRoot, string sourceId, string title)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(targetRoot);

        var normalizedSourceId = string.IsNullOrWhiteSpace(sourceId)
            ? "UNKNOWN"
            : sourceId.Trim();
        var sanitizedTitle = SanitizePathPart(title);
        return Path.Combine(targetRoot, $"[{normalizedSourceId}]{sanitizedTitle}");
    }

    public static string SanitizePathPart(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "untitled";
        }

        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = value
            .Select(ch => invalidChars.Contains(ch) ? '_' : ch)
            .ToArray();

        return new string(sanitized).Trim();
    }
}