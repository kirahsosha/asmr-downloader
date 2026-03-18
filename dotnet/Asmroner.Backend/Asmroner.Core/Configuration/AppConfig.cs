namespace Asmroner.Core.Configuration;

public sealed class AppConfig
{
    public UserOptions User { get; set; } = new();

    public DownloaderOptions Downloader { get; set; } = new();

    public LimitOptions Limit { get; set; } = new();
}