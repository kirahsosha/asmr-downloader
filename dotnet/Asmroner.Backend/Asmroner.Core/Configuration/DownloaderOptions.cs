namespace Asmroner.Core.Configuration;

public sealed class DownloaderOptions
{
    /// <summary>
    /// 当前生效的 API 基础地址。启动或测试连接发现成功后会回写该值。
    /// </summary>
    public string ApiUrl { get; set; } = "https://api.asmr-300.com";

    /// <summary>
    /// 候选 API 基础地址列表（分号/逗号分隔）。
    /// </summary>
    public string ApiCandidateUrls { get; set; } = "https://api.asmr-300.com;https://api.asmr.one";

    /// <summary>
    /// 发布源地址列表（分号/逗号分隔），用于动态发现候选 API 地址。
    /// </summary>
    public string PublishSourceUrls { get; set; } = "https://as.mr;https://as.131433.xyz";

    /// <summary>
    /// 作品页面 URL 模板，供外部跳转等场景复用。
    /// 示例：https://www.asmr.one/work/{RJID}
    /// </summary>
    public string WorkPageUrlTemplate { get; set; } = "https://www.asmr.one/work/{RJID}";

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

    /// <summary>
    /// 是否仅下载高清音频。开启后在同时存在 wav/flac 与 mp3 的情况下会跳过 mp3。
    /// </summary>
    public bool HdAudioOnly { get; set; } = true;
}