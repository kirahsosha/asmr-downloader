namespace Asmroner.Wpf.ViewModels;

public static class DownloadExecutionArgs
{
    public static string? NormalizeFileFilter(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        var value = raw.Trim();
        return value.Length == 0 ? null : value;
    }
}
