using Asmroner.Core.Api;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Search;

namespace Asmroner.Application.Services;

public sealed class SearchService : ISearchService
{
    private readonly IAsmrApiClient _apiClient;
    private readonly IQueryParserService _queryParserService;

    public SearchService(IAsmrApiClient apiClient, IQueryParserService queryParserService)
    {
        _apiClient = apiClient;
        _queryParserService = queryParserService;
    }

    public async Task<SearchExecutionResult> SearchAsync(string rawQuery, int count, CancellationToken cancellationToken = default)
    {
        var limit = Math.Max(1, count);
        var parsed = _queryParserService.Parse(rawQuery);
        var requestedPage = Math.Max(1, parsed.PageOptions.Page);

        var firstPageQuery = new SearchQuery
        {
            RawQuery = parsed.RawQuery,
            PlainTexts = parsed.PlainTexts,
            Filter = parsed.Filter,
            PageOptions = new SearchPageOptions
            {
                Order = parsed.PageOptions.Order,
                Sort = parsed.PageOptions.Sort,
                Subtitle = parsed.PageOptions.Subtitle,
                IncludeTranslationWorks = parsed.PageOptions.IncludeTranslationWorks,
                Page = requestedPage,
                PageSize = parsed.PageOptions.PageSize,
            },
        };

        var firstQueryString = _queryParserService.BuildAsmrQuery(firstPageQuery);
        var firstResult = await _apiClient.SearchAsync(firstQueryString, cancellationToken);

        var totalCount = firstResult.Pagination.TotalCount;
        var pageSize = Math.Max(1, firstResult.Pagination.PageSize);
        var targetCount = Math.Min(totalCount, limit);
        var works = new List<SearchWorkDto>(firstResult.Works);

        if (targetCount > pageSize)
        {
            var requiredPages = (int)Math.Ceiling(targetCount / (double)pageSize);
            for (var offset = 1; offset < requiredPages; offset++)
            {
                var page = requestedPage + offset;
                var pageQuery = new SearchQuery
                {
                    RawQuery = parsed.RawQuery,
                    PlainTexts = parsed.PlainTexts,
                    Filter = parsed.Filter,
                    PageOptions = new SearchPageOptions
                    {
                        Order = parsed.PageOptions.Order,
                        Sort = parsed.PageOptions.Sort,
                        Subtitle = parsed.PageOptions.Subtitle,
                        IncludeTranslationWorks = parsed.PageOptions.IncludeTranslationWorks,
                        Page = page,
                        PageSize = pageSize,
                    },
                };

                var pageQueryString = _queryParserService.BuildAsmrQuery(pageQuery);
                var pageResult = await _apiClient.SearchAsync(pageQueryString, cancellationToken);
                works.AddRange(pageResult.Works);
            }
        }

        var mapped = works
            .Take(targetCount)
            .Select(static work => new SearchWorkItem
            {
                SourceId = work.SourceId,
                Title = work.Title,
                Release = work.Release,
                HasSubtitle = work.HasSubtitle,
                Tags = string.Join(";", work.Tags.OrderBy(t => t.Id).Select(t => t.Name)),
            })
            .ToArray();

        return new SearchExecutionResult
        {
            Items = mapped,
            TotalCount = totalCount,
            ReturnedCount = mapped.Length,
            ResolvedQuery = firstQueryString,
        };
    }
}
