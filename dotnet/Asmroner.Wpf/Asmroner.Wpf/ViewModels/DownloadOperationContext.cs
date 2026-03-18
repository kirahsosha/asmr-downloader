namespace Asmroner.Wpf.ViewModels;

public readonly record struct DownloadOperationContext(string? FileFilter)
{
    public static DownloadOperationContext Create(string? rawFileFilter)
    {
        return new DownloadOperationContext(
            DownloadExecutionArgs.NormalizeFileFilter(rawFileFilter));
    }
}
