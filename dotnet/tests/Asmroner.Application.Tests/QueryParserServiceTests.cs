using Asmroner.Application.Services;

namespace Asmroner.Application.Tests;

public class QueryParserServiceTests
{
    [Fact]
    public void QueryParser_ShouldParseAdvancedQuery()
    {
        var sut = new QueryParserService();
        var rawQuery = "修女,洗脑,-触手@tag:内射/中出,circle:青春×フェティシズム,va:陽向葵ゅか,duration:1h,rate:4.75,-price:1000,sell:700,age:adult,-lang:JPN?order=dl_count&sort=desc&page=1&pageSize=20&subtitle=0&includeTranslationWorks=true";

        var parsed = sut.Parse(rawQuery);
        var rebuilt = sut.BuildAsmrQuery(parsed);

        Assert.Equal(3, parsed.PlainTexts.Count);
        Assert.Equal("tag:内射/中出", parsed.Filter.Tag);
        Assert.Equal("-price:1000", parsed.Filter.Price);
        Assert.Equal("-lang:JPN", parsed.Filter.Lang);
        Assert.Contains("order=dl_count", rebuilt);
        Assert.Contains("pageSize=20", rebuilt);
    }

    [Fact]
    public void QueryParser_ShouldReturnReadableError_WhenSyntaxInvalid()
    {
        var sut = new QueryParserService();
        var rawQuery = "耳舐め@tag:?order=release&sort=desc&page=1&pageSize=20";

        var ex = Assert.Throws<ArgumentException>(() => sut.Parse(rawQuery));

        Assert.Contains("查询语法无效", ex.Message);
        Assert.Contains("tag:", ex.Message);
    }

    [Fact]
    public void QueryParser_ShouldNotApplyDefaultAge_WhenAgeMissing()
    {
        var sut = new QueryParserService();

        var parsed = sut.Parse("耳舐め@tag:舔耳?order=release&sort=desc&page=1&pageSize=20");
        var rebuilt = Uri.UnescapeDataString(sut.BuildAsmrQuery(parsed).Split('?', 2)[0]);

        Assert.True(string.IsNullOrWhiteSpace(parsed.Filter.Age));
        Assert.DoesNotContain("$age:", rebuilt);
    }

    [Fact]
    public void QueryParser_ShouldParseSemicolonSeparatedFilters()
    {
        var sut = new QueryParserService();

        var parsed = sut.Parse("耳舐め@tag:舔耳;circle:同人サークル;-lang:JPN?order=release&sort=desc&page=1&pageSize=20");

        Assert.Equal("tag:舔耳", parsed.Filter.Tag);
        Assert.Equal("circle:同人サークル", parsed.Filter.Circle);
        Assert.Equal("-lang:JPN", parsed.Filter.Lang);
    }

    [Fact]
    public void QueryParser_ShouldParseOptionOnlyQuery()
    {
        var sut = new QueryParserService();

        var parsed = sut.Parse("?order=price&sort=asc&page=3&pageSize=50&subtitle=1&includeTranslationWorks=false");
        var rebuilt = sut.BuildAsmrQuery(parsed);

        Assert.Empty(parsed.PlainTexts);
        Assert.True(string.IsNullOrWhiteSpace(parsed.Filter.Tag));
        Assert.Equal("price", parsed.PageOptions.Order);
        Assert.Equal("asc", parsed.PageOptions.Sort);
        Assert.Equal("1", parsed.PageOptions.Subtitle);
        Assert.False(parsed.PageOptions.IncludeTranslationWorks);
        Assert.Equal(3, parsed.PageOptions.Page);
        Assert.Equal(50, parsed.PageOptions.PageSize);
        Assert.StartsWith("?order=price&sort=asc&page=3&pageSize=50&subtitle=1&includeTranslationWorks=false", rebuilt, StringComparison.Ordinal);
    }

    [Fact]
    public void QueryParser_ShouldPreserveCommaSeparatedValues_WithinSingleFilter()
    {
        var sut = new QueryParserService();
        var rawQuery = "-tag:女性向,乙女向,记录片/体验谈,女性视角,无插入/无正戏,被NTR,故意被NTR/绿奴,性转换（TS）,虐待/ryona age:adult";

        var parsed = sut.Parse(rawQuery);

        Assert.Equal("-tag:女性向,乙女向,记录片/体验谈,女性视角,无插入/无正戏,被NTR,故意被NTR/绿奴,性转换（TS）,虐待/ryona", parsed.Filter.Tag);
        Assert.Equal("age:adult", parsed.Filter.Age);
        Assert.Empty(parsed.PlainTexts);
        Assert.True(string.IsNullOrWhiteSpace(parsed.Filter.Circle));
        Assert.True(string.IsNullOrWhiteSpace(parsed.Filter.Va));
        Assert.Equal(1, parsed.PageOptions.Page);
        Assert.Equal(20, parsed.PageOptions.PageSize);
    }
}
