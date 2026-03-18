using Asmroner.Application.Services;
using Asmroner.Core.Search;

namespace Asmroner.Application.Tests;

public class SearchExportServiceTests
{
    [Fact]
    public async Task SearchExportService_ShouldExportCsvAndJson()
    {
        var sut = new SearchExportService();
        var tempDir = Path.Combine(Path.GetTempPath(), $"asmroner-stage3-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);

        var items = new[]
        {
            new SearchWorkItem
            {
                SourceId = "RJ2001",
                Release = "2026-03-15",
                RateAverage = 4.5,
                DownloadCount = 321,
                HasSubtitle = true,
                Title = "示例作品",
            },
        };

        var csvPath = Path.Combine(tempDir, "result.csv");
        var jsonPath = Path.Combine(tempDir, "result.json");

        await sut.ExportCsvAsync(items, csvPath);
        await sut.ExportJsonAsync(items, jsonPath);

        Assert.True(File.Exists(csvPath));
        Assert.True(File.Exists(jsonPath));
        Assert.Contains("source_id", await File.ReadAllTextAsync(csvPath));
        Assert.Contains("RJ2001", await File.ReadAllTextAsync(jsonPath));
    }

    [Fact]
    public async Task SearchExportService_ShouldEscapeCsvFields_WhenTextContainsCommaOrQuote()
    {
        var sut = new SearchExportService();
        var tempDir = Path.Combine(Path.GetTempPath(), $"asmroner-batch02-csv-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);

        var outputPath = Path.Combine(tempDir, "escaped.csv");
        var items = new[]
        {
            new SearchWorkItem
            {
                SourceId = "RJ7201",
                Release = "2026-03-15",
                RateAverage = 4.7,
                DownloadCount = 999,
                HasSubtitle = true,
                Title = "A,\"B\"\nC",
            },
        };

        await sut.ExportCsvAsync(items, outputPath);

        var content = await File.ReadAllTextAsync(outputPath);
        Assert.Contains("source_id,release,rate_average_2dp,dl_count,has_subtitle,title", content);
        Assert.Contains("\"A,\"\"B\"\"", content);
        Assert.Contains("C\"", content);
    }
}
