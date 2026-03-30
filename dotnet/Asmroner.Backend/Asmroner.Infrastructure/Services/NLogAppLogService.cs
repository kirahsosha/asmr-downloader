using Asmroner.Core.Interfaces;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;

namespace Asmroner.Infrastructure.Services;

/// <summary>
/// 使用 NLog 实现应用级日志服务。
/// 日志以 JSON Lines 格式写入滚动文件，文件超过 10 MB 或日期变更时自动归档。
/// </summary>
public sealed class NLogAppLogService : IAppLogService
{
    /// <summary>日志文件超过此大小（字节）时创建新文件。</summary>
    public const long ArchiveSizeThreshold = 10 * 1024 * 1024; // 10 MB

    /// <inheritdoc />
    public void Configure(string logDirectory)
    {
        Directory.CreateDirectory(logDirectory);

        var fileTarget = BuildFileTarget(logDirectory);

        var config = new LoggingConfiguration();
        config.AddTarget(fileTarget);
        config.AddRuleForAllLevels(fileTarget);

        LogManager.Configuration = config;
        LogManager.ReconfigExistingLoggers();
    }

    // ── Internal ──────────────────────────────────────────────────────────────

    internal static FileTarget BuildFileTarget(string logDirectory)
    {
        var layout = new JsonLayout();
        layout.Attributes.Add(new JsonAttribute("timestamp", "${longdate}"));
        layout.Attributes.Add(new JsonAttribute("level", "${level:upperCase=true}"));
        layout.Attributes.Add(new JsonAttribute("logger", "${logger}"));
        layout.Attributes.Add(new JsonAttribute("message", "${message:withException=false}"));
        layout.Attributes.Add(new JsonAttribute("exception", "${exception:format=tostring}", false));
        layout.Attributes.Add(new JsonAttribute("threadId", "${threadid}"));

        var fileTarget = new FileTarget("jsonFile")
        {
            FileName = Path.Combine(logDirectory, "asmroner-${shortdate}.json"),
            Layout = layout,
            ArchiveEvery = FileArchivePeriod.Day,
            ArchiveAboveSize = ArchiveSizeThreshold,
            ArchiveSuffixFormat = "yyyyMMdd_HHmmss",
            ArchiveFileName = Path.Combine(logDirectory, "archive", "asmroner-{#}.json"),
            MaxArchiveFiles = 30,
            KeepFileOpen = true,
        };

        return fileTarget;
    }
}
