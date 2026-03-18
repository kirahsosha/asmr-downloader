namespace Asmroner.Core.Configuration;

public sealed class LimitOptions
{
    public double SyncQps { get; set; } = 5;

    public int SyncJitterMin { get; set; } = 50;

    public int SyncJitterMax { get; set; } = 200;

    public double DownloadQps { get; set; } = 3;

    public int DownloadJitterMin { get; set; } = 50;

    public int DownloadJitterMax { get; set; } = 300;
}