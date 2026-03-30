using Asmroner.Infrastructure.Services;
using NLog;
using NLog.Layouts;
using NLog.Targets;

namespace Asmroner.Infrastructure.Tests;

/// <summary>
/// NLogAppLogService 单元测试。
/// 每个 Fact 独立构造服务实例并在测试完成后清除全局 NLog 配置，
/// 避免会话间状态泄漏。
/// </summary>
public class NLogAppLogServiceTests : IDisposable
{
    private readonly string _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

    public void Dispose()
    {
        LogManager.Configuration = null;
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    [Fact]
    public void Configure_ShouldCreateFileTarget()
    {
        var sut = new NLogAppLogService();

        sut.Configure(_tempDir);

        var target = LogManager.Configuration.FindTargetByName<FileTarget>("jsonFile");
        Assert.NotNull(target);
    }

    [Fact]
    public void Configure_ShouldUseJsonLayout()
    {
        var sut = new NLogAppLogService();

        sut.Configure(_tempDir);

        var target = LogManager.Configuration.FindTargetByName<FileTarget>("jsonFile");
        Assert.NotNull(target);
        Assert.IsType<JsonLayout>(target.Layout);
    }

    [Fact]
    public void Configure_ShouldSetArchiveAboveSize()
    {
        var sut = new NLogAppLogService();

        sut.Configure(_tempDir);

        var target = LogManager.Configuration.FindTargetByName<FileTarget>("jsonFile");
        Assert.NotNull(target);
        Assert.Equal(NLogAppLogService.ArchiveSizeThreshold, target.ArchiveAboveSize);
    }

    [Fact]
    public void Configure_ShouldArchiveEveryDay()
    {
        var sut = new NLogAppLogService();

        sut.Configure(_tempDir);

        var target = LogManager.Configuration.FindTargetByName<FileTarget>("jsonFile");
        Assert.NotNull(target);
        Assert.Equal(FileArchivePeriod.Day, target.ArchiveEvery);
    }

    [Fact]
    public void Configure_ShouldCreateLogDirectory_WhenNotExists()
    {
        var sut = new NLogAppLogService();
        var nestedDir = Path.Combine(_tempDir, "sub1", "sub2");

        sut.Configure(nestedDir);

        Assert.True(Directory.Exists(nestedDir));
    }

    [Fact]
    public void Configure_ShouldWriteLogEntry_WhenStaticLoggerIsUsed()
    {
        var sut = new NLogAppLogService();
        sut.Configure(_tempDir);

        var logger = LogManager.GetCurrentClassLogger();
        logger.Info("test-message-{Guid}", Guid.NewGuid().ToString("N"));
        LogManager.Flush();
        LogManager.Shutdown();

        var files = Directory.GetFiles(_tempDir, "*.json*");
        Assert.NotEmpty(files);
        using var stream = new FileStream(files[0], FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(stream);
        var content = reader.ReadToEnd();
        Assert.Contains("test-message-", content);
    }
}
