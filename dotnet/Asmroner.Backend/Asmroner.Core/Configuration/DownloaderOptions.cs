namespace Asmroner.Core.Configuration;

public sealed class DownloaderOptions
{
    public string ApiUrl { get; set; } = string.Empty;

    public string ProxyUrl { get; set; } = string.Empty;

    public int MaxWorkers { get; set; } = 4;

    public int MaxRetries { get; set; } = 3;

    public string SyncDataFolder { get; set; } = string.Empty;

    public string SyncWantedSize { get; set; } = "5GB";

    /// <summary>
    /// 统一格式优先级（逗号分隔）。留空表示不按扩展名过滤，即全部下载。
    /// 示例：mp3,m4a,wav,flac,jpg,png,mp4,txt,lrc,json。
    /// </summary>
    public string PreferFormats { get; set; } = "mp3,wav,flac,jpg,jpeg,png,gif,webp,mp4,mkv,avi,webm,txt,lrc,ass";

    // legacy: 兼容旧配置字段
    public string PreferMedia { get; set; } = "mp3,m4a,wav,flac";

    /// <summary>
    /// 图片格式扩展名（逗号分隔），如 jpg,jpeg,png,gif,webp。留空则不下载图片。
    /// </summary>
    public string PreferImage { get; set; } = string.Empty;

    /// <summary>
    /// 视频格式扩展名（逗号分隔），如 mp4,mkv,avi,mov,webm。留空则不下载视频。
    /// </summary>
    public string PreferVideo { get; set; } = string.Empty;

    /// <summary>
    /// 下载文件筛选规则（分号分隔）。无前缀或 + 前缀为必含条件，- 前缀为排除条件。
    /// 例：+voice;-cover 表示路径必须含 voice 且不含 cover。
    /// </summary>
    public string FileFilter { get; set; } = string.Empty;

    /// <summary>
    /// 全局搜索规则（高级筛选语法）。应用启动后自动填充到 Search 页高级筛选输入。
    /// 示例：tag:舔耳;lang:zh-CN;-age:r15。
    /// </summary>
    public string GlobalSearchRule { get; set; } = string.Empty;
}