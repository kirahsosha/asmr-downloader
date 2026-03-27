using Asmroner.Core.Download;
using Asmroner.Wpf.ViewModels;

namespace Asmroner.Wpf.Tests;

public class DownloadEnqueueDuplicatePolicyTests
{
    [Theory]
    [InlineData(DownloadTaskStatus.Queued)]
    [InlineData(DownloadTaskStatus.Running)]
    [InlineData(DownloadTaskStatus.Completed)]
    [InlineData(DownloadTaskStatus.Failed)]
    [InlineData(DownloadTaskStatus.Canceled)]
    public void FilterAlreadyPresent_ShouldExclude_WhenSourceIdExistsWithAnyStatus(DownloadTaskStatus status)
    {
        var existing = new[] { new DownloadTaskItem { SourceId = "RJ001", Status = status } };
        var incoming = new[] { "RJ001" };

        var result = DownloadEnqueueDuplicatePolicy.FilterAlreadyPresent(existing, incoming);

        Assert.Empty(result);
    }

    [Fact]
    public void FilterAlreadyPresent_ShouldInclude_WhenSourceIdNotInTaskList()
    {
        var existing = new[] { new DownloadTaskItem { SourceId = "RJ001" } };
        var incoming = new[] { "RJ002", "RJ003" };

        var result = DownloadEnqueueDuplicatePolicy.FilterAlreadyPresent(existing, incoming);

        Assert.Equal(new[] { "RJ002", "RJ003" }, result);
    }

    [Fact]
    public void FilterAlreadyPresent_ShouldIgnoreCase()
    {
        var existing = new[] { new DownloadTaskItem { SourceId = "rj001" } };
        var incoming = new[] { "RJ001", "RJ002" };

        var result = DownloadEnqueueDuplicatePolicy.FilterAlreadyPresent(existing, incoming);

        Assert.Equal(new[] { "RJ002" }, result);
    }

    [Fact]
    public void FilterAlreadyPresent_ShouldReturnAll_WhenNoExistingTasks()
    {
        var existing = Array.Empty<DownloadTaskItem>();
        var incoming = new[] { "RJ001", "RJ002" };

        var result = DownloadEnqueueDuplicatePolicy.FilterAlreadyPresent(existing, incoming);

        Assert.Equal(new[] { "RJ001", "RJ002" }, result);
    }

    [Fact]
    public void FilterAlreadyPresent_ShouldReturnEmpty_WhenAllAlreadyExist()
    {
        var existing = new[]
        {
            new DownloadTaskItem { SourceId = "RJ001" },
            new DownloadTaskItem { SourceId = "RJ002" },
        };
        var incoming = new[] { "RJ001", "RJ002" };

        var result = DownloadEnqueueDuplicatePolicy.FilterAlreadyPresent(existing, incoming);

        Assert.Empty(result);
    }
}
