using Asmroner.Core.Configuration;
using Asmroner.Infrastructure.Services;

namespace Asmroner.Infrastructure.Tests;

public class ConfigurationServiceTests
{
    [Fact]
    public void ConfigurationService_ShouldReturnValidationErrors_WhenRequiredFieldsMissing()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), $"asmroner-batch04-config-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempRoot);

        var sut = new ConfigurationService(new AppPathService(tempRoot));
        var config = new AppConfig
        {
            User = new UserOptions
            {
                Account = string.Empty,
                Password = string.Empty,
            },
            Downloader = new DownloaderOptions
            {
                SyncDataFolder = string.Empty,
                MaxWorkers = 0,
                MaxRetries = -1,
            },
            Limit = new LimitOptions
            {
                SyncQps = 0,
                DownloadQps = 0,
                SyncJitterMin = 20,
                SyncJitterMax = 10,
                DownloadJitterMin = 20,
                DownloadJitterMax = 10,
            },
        };

        var errors = sut.Validate(config);

        Assert.Contains("账号不能为空。", errors);
        Assert.Contains("密码不能为空。", errors);
        Assert.Contains("并发工作数必须大于 0。", errors);
        Assert.Contains("重试次数不能小于 0。", errors);
        Assert.Contains("同步目录不能为空。", errors);
        Assert.Contains("QPS 必须大于 0。", errors);
        Assert.Contains("同步抖动最小值不能大于最大值。", errors);
        Assert.Contains("下载抖动最小值不能大于最大值。", errors);
    }

    [Fact]
    public async Task ConfigurationService_ShouldSaveAndLoadConfig()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var pathService = new AppPathService(tempRoot);
            var sut = new ConfigurationService(pathService);
            var config = new AppConfig
            {
                User = new UserOptions
                {
                    Account = "tester",
                    Password = "secret",
                },
                Downloader = new DownloaderOptions
                {
                    ApiUrl = "https://api-custom.example.com",
                    ApiCandidateUrls = "https://api-custom.example.com;https://api-fallback.example.com",
                    PublishSourceUrls = "https://publish-a.example.com;https://publish-b.example.com",
                    WorkPageUrlTemplate = "https://example.com/work/{RJID}",
                    SyncDataFolder = Path.Combine(tempRoot, "sync"),
                    MaxWorkers = 8,
                    MaxRetries = 5,
                    GlobalSearchRule = "tag:舔耳;lang:zh-CN",
                },
                Limit = new LimitOptions
                {
                    SyncQps = 6,
                    DownloadQps = 4,
                },
            };

            await sut.SaveAsync(config);
            var loaded = await sut.LoadAsync();

            Assert.NotNull(loaded);
            Assert.Equal("tester", loaded!.User.Account);
            Assert.Equal("https://api-custom.example.com", loaded.Downloader.ApiUrl);
            Assert.Equal("https://api-custom.example.com;https://api-fallback.example.com", loaded.Downloader.ApiCandidateUrls);
            Assert.Equal("https://publish-a.example.com;https://publish-b.example.com", loaded.Downloader.PublishSourceUrls);
            Assert.Equal("https://example.com/work/{RJID}", loaded.Downloader.WorkPageUrlTemplate);
            Assert.Equal(8, loaded.Downloader.MaxWorkers);
            Assert.Equal(5, loaded.Downloader.MaxRetries);
            Assert.Equal("tag:舔耳;lang:zh-CN", loaded.Downloader.GlobalSearchRule);
        }
        finally
        {
            CleanupTempRoot(tempRoot);
        }
    }

    private static string CreateTempRoot()
    {
        var path = Path.Combine(Path.GetTempPath(), "asmroner-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }

    private static void CleanupTempRoot(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }
}
