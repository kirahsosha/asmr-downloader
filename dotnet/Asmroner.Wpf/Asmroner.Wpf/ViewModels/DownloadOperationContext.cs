namespace Asmroner.Wpf.ViewModels;

public readonly record struct DownloadOperationContext(string? FileFilter, bool HdAudioOnly)
{
    public static DownloadOperationContext Create(string? rawFileFilter, bool hdAudioOnly = false)
    {
        return new DownloadOperationContext(
            DownloadExecutionArgs.NormalizeFileFilter(rawFileFilter),
            hdAudioOnly);
    }
}
