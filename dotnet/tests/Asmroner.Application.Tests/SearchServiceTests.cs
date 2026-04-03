using Asmroner.Application.Services;
using Asmroner.Core.Api;
using Asmroner.Core.Interfaces;

namespace Asmroner.Application.Tests;

public class SearchServiceTests
{
    [Fact]
    public async Task SearchService_ShouldAggregateMultiplePages()
    {
        var parser = new QueryParserService();
        var apiClient = new PagedSearchApiClient();
        var sut = new SearchService(apiClient, parser);

        var result = await sut.SearchAsync("耳舐め?order=release&sort=desc&page=1&pageSize=2&subtitle=0&includeTranslationWorks=true", 5);

        Assert.Equal(6, result.TotalCount);
        Assert.Equal(5, result.ReturnedCount);
        Assert.Equal(3, apiClient.Calls.Count);
        Assert.Equal(100, result.Items[0].WorkId);
        Assert.Equal("RJ1001", result.Items[0].SourceId);
        Assert.Equal(300, result.Items[4].WorkId);
        Assert.Equal("RJ1005", result.Items[4].SourceId);
    }

    [Fact]
    public async Task SearchService_ShouldRespectRequestedPageAndKeepFilters()
    {
        var parser = new QueryParserService();
        var apiClient = new PagedSearchApiClient();
        var sut = new SearchService(apiClient, parser);

        var rawQuery = "耳舐め@tag:舔耳,va:狐坂めぐ,lang:zh?order=dl_count&sort=asc&page=2&pageSize=2&subtitle=1&includeTranslationWorks=false";
        var result = await sut.SearchAsync(rawQuery, 2);

        Assert.Single(apiClient.Calls);
        Assert.Equal(6, result.TotalCount);
        Assert.Equal(2, result.ReturnedCount);
        Assert.Equal("RJ1003", result.Items[0].SourceId);
        Assert.Equal("RJ1004", result.Items[1].SourceId);

        var query = apiClient.Calls[0];
        Assert.Equal("2", ReadQueryValue(query, "page"));
        Assert.Equal("2", ReadQueryValue(query, "pageSize"));
        Assert.Equal("dl_count", ReadQueryValue(query, "order"));
        Assert.Equal("asc", ReadQueryValue(query, "sort"));
        Assert.Equal("1", ReadQueryValue(query, "subtitle"));
        Assert.Equal("false", ReadQueryValue(query, "includeTranslationWorks"));

        var decoded = Uri.UnescapeDataString(query.Split('?', 2)[0]);
        Assert.Contains("耳舐め", decoded);
        Assert.Contains("$tag:舔耳$", decoded);
        Assert.Contains("$va:狐坂めぐ$", decoded);
        Assert.Contains("$lang:zh$", decoded);
    }

    [Fact]
    public async Task SearchService_ShouldReturnEmpty_WhenApiReturnsNoWorkItems()
    {
        var sut = new SearchService(new EmptySearchApiClient(), new QueryParserService());

        var result = await sut.SearchAsync("耳舐め?order=release&sort=desc&page=1&pageSize=20", 10);

        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.ReturnedCount);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task SearchService_ShouldSearch_WhenOnlyPageOptionsProvided()
    {
        var parser = new QueryParserService();
        var apiClient = new PagedSearchApiClient();
        var sut = new SearchService(apiClient, parser);

        var result = await sut.SearchAsync("?order=release&sort=desc&page=1&pageSize=2&subtitle=0&includeTranslationWorks=true", 2);

        Assert.Single(apiClient.Calls);
        Assert.Equal(6, result.TotalCount);
        Assert.Equal(2, result.ReturnedCount);
        Assert.Equal("RJ1001", result.Items[0].SourceId);
        Assert.Equal("RJ1002", result.Items[1].SourceId);
    }

    private sealed class EmptySearchApiClient : IAsmrApiClient
    {
        public Task<MetadataSyncPageDto> GetMetadataWorksAsync(int page, int pageSize, bool subtitleOnly = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<WorkInfoDto> GetWorkInfoAsync(string id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<TrackDto>> GetTracksAsync(string id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<SearchWorkDto>> GetPopularAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<SearchResultDto> SearchAsync(string query, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new SearchResultDto
            {
                Works = Array.Empty<SearchWorkDto>(),
                Pagination = new SearchPaginationDto
                {
                    CurrentPage = 1,
                    PageSize = 20,
                    TotalCount = 0,
                },
            });
        }
    }

    private sealed class PagedSearchApiClient : IAsmrApiClient
    {
        public List<string> Calls { get; } = new();

        public Task<MetadataSyncPageDto> GetMetadataWorksAsync(int page, int pageSize, bool subtitleOnly = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<WorkInfoDto> GetWorkInfoAsync(string id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<TrackDto>> GetTracksAsync(string id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<SearchWorkDto>> GetPopularAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<SearchResultDto> SearchAsync(string query, CancellationToken cancellationToken = default)
        {
            Calls.Add(query);
            var page = ReadPage(query);

            return Task.FromResult(page switch
            {
                1 => BuildPage(page, new[] { "RJ1001", "RJ1002" }),
                2 => BuildPage(page, new[] { "RJ1003", "RJ1004" }),
                3 => BuildPage(page, new[] { "RJ1005", "RJ1006" }),
                _ => BuildPage(page, Array.Empty<string>()),
            });
        }

        private static SearchResultDto BuildPage(int page, IReadOnlyList<string> sourceIds)
        {
            return new SearchResultDto
            {
                Works = sourceIds.Select((sourceId, index) => new SearchWorkDto
                {
                    Id = page * 100 + index,
                    SourceId = sourceId,
                    Title = $"Title-{sourceId}",
                    Release = "2026-03-15",
                    DownloadCount = 10 + index,
                    RateAverage = 4.2,
                    HasSubtitle = false,
                    Tags = new[] { new TagDto { Id = 1, Name = "tag1" } },
                }).ToArray(),
                Pagination = new SearchPaginationDto
                {
                    CurrentPage = page,
                    PageSize = 2,
                    TotalCount = 6,
                },
            };
        }

        private static int ReadPage(string query)
        {
            var marker = "&page=";
            var index = query.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (index < 0)
            {
                return 1;
            }

            var start = index + marker.Length;
            var end = query.IndexOf('&', start);
            var text = end < 0 ? query[start..] : query[start..end];
            return int.TryParse(text, out var page) ? page : 1;
        }
    }

    private static string ReadQueryValue(string query, string key)
    {
        var marker = key + "=";
        var index = query.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (index < 0)
        {
            return string.Empty;
        }

        var start = index + marker.Length;
        var end = query.IndexOf('&', start);
        return end < 0 ? query[start..] : query[start..end];
    }
}
