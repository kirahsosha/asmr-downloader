using Asmroner.Application.Services;

namespace Asmroner.Application.Tests;

public class SearchImportServiceTests
{
    private readonly SearchImportService _sut = new();

    [Fact]
    public async Task ParseCsvAsync_ShouldReturnItems_FromValidCsv()
    {
        var csv = "source_id,has_subtitle,release,tags,title\nRJ001,false,2024-01-01,音声;耳かき,テスト作品";
        var path = WriteTempFile(csv);

        var result = await _sut.ParseCsvAsync(path);

        Assert.Single(result);
        Assert.Equal("RJ001", result[0].SourceId);
        Assert.Equal("テスト作品", result[0].Title);
        Assert.Equal("2024-01-01", result[0].Release);
        Assert.Equal("音声;耳かき", result[0].Tags);
        Assert.False(result[0].HasSubtitle);
    }

    [Fact]
    public async Task ParseCsvAsync_ShouldSkipHeaderAndEmptyLines()
    {
        var csv = "source_id,has_subtitle,release,tags,title\n\nRJ001,false,2024-01-01,tag1,Title\n\n";
        var path = WriteTempFile(csv);

        var result = await _sut.ParseCsvAsync(path);

        Assert.Single(result);
    }

    [Fact]
    public async Task ParseCsvAsync_ShouldHandleQuotedTitle_WithComma()
    {
        var csv = "source_id,has_subtitle,release,tags,title\nRJ002,false,2024-01-01,,\"Title, with comma\"";
        var path = WriteTempFile(csv);

        var result = await _sut.ParseCsvAsync(path);

        Assert.Single(result);
        Assert.Equal("Title, with comma", result[0].Title);
    }

    [Fact]
    public async Task ParseCsvAsync_ShouldHandleEmbeddedDoubleQuote_InTitle()
    {
        var csv = "source_id,has_subtitle,release,tags,title\nRJ003,false,2024-01-01,,\"He said \"\"hello\"\"\"";
        var path = WriteTempFile(csv);

        var result = await _sut.ParseCsvAsync(path);

        Assert.Single(result);
        Assert.Equal("He said \"hello\"", result[0].Title);
    }

    [Fact]
    public async Task ParseJsonAsync_ShouldDeserializeItems_FromValidJson()
    {
        var json = "[{\"sourceId\":\"RJ010\",\"title\":\"JSON作品\",\"release\":\"2024-06-01\",\"tags\":\"音声;耳かき\",\"hasSubtitle\":true}]";
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
