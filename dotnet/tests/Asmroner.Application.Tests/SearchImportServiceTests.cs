using Asmroner.Application.Services;

namespace Asmroner.Application.Tests;

public class SearchImportServiceTests
{
    private readonly SearchImportService _sut = new();

    [Fact]
    public async Task ParseCsvAsync_ShouldReturnItems_FromValidCsv()
    {
        var csv = "source_id,release,rate_average_2dp,dl_count,has_subtitle,title\nRJ001,2024-01-01,4.50,100,false,テスト作品";
        var path = WriteTempFile(csv);

        var result = await _sut.ParseCsvAsync(path);

        Assert.Single(result);
        Assert.Equal("RJ001", result[0].SourceId);
        Assert.Equal("テスト作品", result[0].Title);
        Assert.Equal("2024-01-01", result[0].Release);
        Assert.Equal(4.5, result[0].RateAverage, precision: 2);
        Assert.Equal(100, result[0].DownloadCount);
    }

    [Fact]
    public async Task ParseCsvAsync_ShouldSkipHeaderAndEmptyLines()
    {
        var csv = "source_id,release,rate_average_2dp,dl_count,has_subtitle,title\n\nRJ001,2024-01-01,4.50,100,false,Title\n\n";
        var path = WriteTempFile(csv);

        var result = await _sut.ParseCsvAsync(path);

        Assert.Single(result);
    }

    [Fact]
    public async Task ParseCsvAsync_ShouldHandleQuotedTitle_WithComma()
    {
        var csv = "source_id,release,rate_average_2dp,dl_count,has_subtitle,title\nRJ002,2024-01-01,4.00,50,false,\"Title, with comma\"";
        var path = WriteTempFile(csv);

        var result = await _sut.ParseCsvAsync(path);

        Assert.Single(result);
        Assert.Equal("Title, with comma", result[0].Title);
    }

    [Fact]
    public async Task ParseCsvAsync_ShouldHandleEmbeddedDoubleQuote_InTitle()
    {
        var csv = "source_id,release,rate_average_2dp,dl_count,has_subtitle,title\nRJ003,2024-01-01,3.50,10,false,\"He said \"\"hello\"\"\"";
        var path = WriteTempFile(csv);

        var result = await _sut.ParseCsvAsync(path);

        Assert.Single(result);
        Assert.Equal("He said \"hello\"", result[0].Title);
    }

    [Fact]
    public async Task ParseJsonAsync_ShouldDeserializeItems_FromValidJson()
    {
        var json = "[{\"sourceId\":\"RJ010\",\"title\":\"JSON作品\",\"release\":\"2024-06-01\",\"rateAverage\":3.8,\"downloadCount\":200,\"hasSubtitle\":true}]";
        var path = WriteTempFile(json, ".json");

        var result = await _sut.ParseJsonAsync(path);

        Assert.Single(result);
        Assert.Equal("RJ010", result[0].SourceId);
        Assert.Equal("JSON作品", result[0].Title);
    }

    [Fact]
    public async Task ParseJsonAsync_ShouldReturnEmpty_ForEmptyJsonArray()
    {
        var path = WriteTempFile("[]", ".json");

        var result = await _sut.ParseJsonAsync(path);

        Assert.Empty(result);
    }

    [Fact]
    public async Task ParseJsonAsync_ShouldSkipEntries_WithEmptySourceId()
    {
        var json = "[{\"sourceId\":\"\",\"title\":\"No ID\"},{\"sourceId\":\"RJ020\",\"title\":\"Valid\"}]";
        var path = WriteTempFile(json, ".json");

        var result = await _sut.ParseJsonAsync(path);

        Assert.Single(result);
        Assert.Equal("RJ020", result[0].SourceId);
    }

    private static string WriteTempFile(string content, string extension = ".csv")
    {
        var path = Path.Combine(Path.GetTempPath(), $"asmroner-import-test-{Guid.NewGuid():N}{extension}");
        File.WriteAllText(path, content);
        return path;
    }
}
