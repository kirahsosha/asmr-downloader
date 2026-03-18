using Asmroner.Core.Configuration;
using Asmroner.Core.Interfaces;
using Tomlyn;
using Tomlyn.Model;

namespace Asmroner.Infrastructure.Services;

public sealed class ConfigurationService : IConfigurationService
{
    private readonly IAppPathService _appPathService;

    public ConfigurationService(IAppPathService appPathService)
    {
        _appPathService = appPathService;
    }

    public bool Exists()
    {
        return File.Exists(_appPathService.ConfigFilePath);
    }

    public async Task<AppConfig?> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!Exists())
        {
            return null;
        }

        var content = await File.ReadAllTextAsync(_appPathService.ConfigFilePath, cancellationToken);
        var parseResult = Toml.Parse(content);
        if (parseResult.HasErrors)
        {
            var errorMessage = string.Join("; ", parseResult.Diagnostics.Select(diagnostic => diagnostic.ToString()));
            throw new InvalidOperationException($"配置文件格式无效: {errorMessage}");
        }

        var model = parseResult.ToModel();
        if (model is not TomlTable root)
        {
            throw new InvalidOperationException("配置文件格式无效: TOML 根节点不是对象。");
        }

        return new AppConfig
        {
            User = new UserOptions
            {
                Account = GetString(root, "user", "account"),
                Password = GetString(root, "user", "password"),
            },
            Downloader = new DownloaderOptions
            {
                ApiUrl = GetString(root, "downloader", "api_url", new DownloaderOptions().ApiUrl),
                ApiCandidateUrls = GetString(root, "downloader", "api_candidate_urls", new DownloaderOptions().ApiCandidateUrls),
                PublishSourceUrls = GetString(root, "downloader", "publish_source_urls", new DownloaderOptions().PublishSourceUrls),
                WorkPageUrlTemplate = GetString(root, "downloader", "work_page_url_template", new DownloaderOptions().WorkPageUrlTemplate),
                ProxyUrl = GetString(root, "downloader", "proxy_url"),
                MaxWorkers = GetInt(root, "downloader", "max_workers", 4),
                MaxRetries = GetInt(root, "downloader", "max_retries", 3),
                SyncDataFolder = GetString(root, "downloader", "sync_data_folder"),
                SyncWantedSize = GetString(root, "downloader", "sync_wanted_size", "5GB"),
                PreferFormats = ResolvePreferFormats(root),
                PreferMedia = GetString(root, "downloader", "prefer_media", "mp3,m4a,wav,flac"),
                PreferImage = GetString(root, "downloader", "prefer_image"),
                PreferVideo = GetString(root, "downloader", "prefer_video"),
                FileFilter = GetString(root, "downloader", "file_filter"),
                GlobalSearchRule = GetString(root, "downloader", "global_search_rule"),
            },
            Limit = new LimitOptions
            {
                SyncQps = GetDouble(root, "limit", "sync_qps", 5),
                SyncJitterMin = GetInt(root, "limit", "sync_jitter_min", 50),
                SyncJitterMax = GetInt(root, "limit", "sync_jitter_max", 200),
                DownloadQps = GetDouble(root, "limit", "download_qps", 3),
                DownloadJitterMin = GetInt(root, "limit", "download_jitter_min", 50),
                DownloadJitterMax = GetInt(root, "limit", "download_jitter_max", 300),
            },
        };
    }

    public async Task SaveAsync(AppConfig config, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(config);

        _appPathService.EnsureMetadataDirectory();

        var lines = new[]
        {
            "[user]",
            $"account = \"{Escape(config.User.Account)}\"",
            $"password = \"{Escape(config.User.Password)}\"",
            string.Empty,
            "[downloader]",
            $"api_url = \"{Escape(config.Downloader.ApiUrl)}\"",
            $"api_candidate_urls = \"{Escape(config.Downloader.ApiCandidateUrls)}\"",
            $"publish_source_urls = \"{Escape(config.Downloader.PublishSourceUrls)}\"",
            $"work_page_url_template = \"{Escape(config.Downloader.WorkPageUrlTemplate)}\"",
            $"proxy_url = \"{Escape(config.Downloader.ProxyUrl)}\"",
            $"max_workers = {config.Downloader.MaxWorkers}",
            $"max_retries = {config.Downloader.MaxRetries}",
            $"sync_data_folder = \"{Escape(config.Downloader.SyncDataFolder)}\"",
            $"sync_wanted_size = \"{Escape(config.Downloader.SyncWantedSize)}\"",
            $"prefer_formats = \"{Escape(config.Downloader.PreferFormats)}\"",
            $"prefer_media = \"{Escape(config.Downloader.PreferMedia)}\"",
            $"prefer_image = \"{Escape(config.Downloader.PreferImage)}\"",
            $"prefer_video = \"{Escape(config.Downloader.PreferVideo)}\"",
            $"file_filter = \"{Escape(config.Downloader.FileFilter)}\"",
            $"global_search_rule = \"{Escape(config.Downloader.GlobalSearchRule)}\"",
            string.Empty,
            "[limit]",
            $"sync_qps = {config.Limit.SyncQps}",
            $"sync_jitter_min = {config.Limit.SyncJitterMin}",
            $"sync_jitter_max = {config.Limit.SyncJitterMax}",
            $"download_qps = {config.Limit.DownloadQps}",
            $"download_jitter_min = {config.Limit.DownloadJitterMin}",
            $"download_jitter_max = {config.Limit.DownloadJitterMax}",
            string.Empty,
        };

        var content = string.Join(Environment.NewLine, lines);
        await File.WriteAllTextAsync(_appPathService.ConfigFilePath, content, cancellationToken);
    }

    public IReadOnlyList<string> Validate(AppConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(config.User.Account))
        {
            errors.Add("账号不能为空。");
        }

        if (string.IsNullOrWhiteSpace(config.User.Password))
        {
            errors.Add("密码不能为空。");
        }

        if (config.Downloader.MaxWorkers <= 0)
        {
            errors.Add("并发工作数必须大于 0。");
        }

        if (config.Downloader.MaxRetries < 0)
        {
            errors.Add("重试次数不能小于 0。");
        }

        if (string.IsNullOrWhiteSpace(config.Downloader.SyncDataFolder))
        {
            errors.Add("同步目录不能为空。");
        }

        if (config.Limit.SyncQps <= 0 || config.Limit.DownloadQps <= 0)
        {
            errors.Add("QPS 必须大于 0。");
        }

        if (config.Limit.SyncJitterMin > config.Limit.SyncJitterMax)
        {
            errors.Add("同步抖动最小值不能大于最大值。");
        }

        if (config.Limit.DownloadJitterMin > config.Limit.DownloadJitterMax)
        {
            errors.Add("下载抖动最小值不能大于最大值。");
        }

        return errors;
    }

    private static string Escape(string input)
    {
        return (input ?? string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\"");
    }

    private static string ResolvePreferFormats(TomlTable root)
    {
        var preferFormats = GetString(root, "downloader", "prefer_formats");
        if (!string.IsNullOrWhiteSpace(preferFormats))
        {
            return preferFormats;
        }

        var merged = string.Join(",", new[]
        {
            GetString(root, "downloader", "prefer_media"),
            GetString(root, "downloader", "prefer_image"),
            GetString(root, "downloader", "prefer_video"),
        }.Where(static value => !string.IsNullOrWhiteSpace(value)));

        return string.IsNullOrWhiteSpace(merged)
            ? "mp3,wav,flac,jpg,jpeg,png,gif,webp,mp4,mkv,avi,webm,txt,lrc,ass"
            : merged;
    }

    private static TomlTable? GetSection(TomlTable root, string sectionName)
    {
        if (!root.TryGetValue(sectionName, out var sectionValue))
        {
            return null;
        }

        return sectionValue as TomlTable;
    }

    private static string GetString(TomlTable root, string sectionName, string key, string defaultValue = "")
    {
        var section = GetSection(root, sectionName);
        if (section is null || !section.TryGetValue(key, out var value) || value is null)
        {
            return defaultValue;
        }

        return value.ToString() ?? defaultValue;
    }

    private static int GetInt(TomlTable root, string sectionName, string key, int defaultValue)
    {
        var section = GetSection(root, sectionName);
        if (section is null || !section.TryGetValue(key, out var value) || value is null)
        {
            return defaultValue;
        }

        if (value is long longValue)
        {
            return (int)longValue;
        }

        return int.TryParse(value.ToString(), out var parsed) ? parsed : defaultValue;
    }

    private static double GetDouble(TomlTable root, string sectionName, string key, double defaultValue)
    {
        var section = GetSection(root, sectionName);
        if (section is null || !section.TryGetValue(key, out var value) || value is null)
        {
            return defaultValue;
        }

        if (value is double doubleValue)
        {
            return doubleValue;
        }

        if (value is long longValue)
        {
            return longValue;
        }

        return double.TryParse(value.ToString(), out var parsed) ? parsed : defaultValue;
    }
}