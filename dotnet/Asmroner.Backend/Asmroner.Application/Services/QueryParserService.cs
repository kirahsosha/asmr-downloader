using System.Text;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Search;

namespace Asmroner.Application.Services;

public sealed class QueryParserService : IQueryParserService
{
    public SearchQuery Parse(string rawQuery)
    {
        if (string.IsNullOrWhiteSpace(rawQuery))
        {
            throw new ArgumentException("查询字符串不能为空。", nameof(rawQuery));
        }

        var query = rawQuery.Trim();
        var pageOptions = SearchPageOptions.CreateDefault();
        var plainTexts = new List<string>();

        string filterSegment;
        string? pageSegment = null;

        var atIndex = query.IndexOf('@');
        if (atIndex >= 0)
        {
            var plainSegment = query[..atIndex];
            plainTexts.AddRange(ParsePlainTexts(plainSegment));
            query = query[(atIndex + 1)..];
        }

        var questionIndex = query.IndexOf('?');
        if (questionIndex >= 0)
        {
            filterSegment = query[..questionIndex];
            pageSegment = query[(questionIndex + 1)..];
        }
        else
        {
            filterSegment = query;
        }

        var filter = ParseFilter(filterSegment);
        var hasFilter = HasAnyFilter(filter);

        if (pageSegment is not null)
        {
            pageOptions = ParsePageOptions(pageSegment, pageOptions);
        }

        if (plainTexts.Count == 0 && !hasFilter)
        {
            var fallbackPlain = filterSegment.Trim();
            if (!string.IsNullOrWhiteSpace(fallbackPlain))
            {
                plainTexts.Add(fallbackPlain);
            }
        }

        return new SearchQuery
        {
            RawQuery = rawQuery,
            PlainTexts = plainTexts,
            Filter = filter,
            PageOptions = pageOptions,
        };
    }

    public string BuildAsmrQuery(SearchQuery query)
    {
        var builder = new StringBuilder();

        if (query.PlainTexts.Count > 0)
        {
            builder.Append(' ');
            builder.Append(string.Join(' ', query.PlainTexts));
        }

        AppendFilterToken(builder, query.Filter.Tag);
        AppendFilterToken(builder, query.Filter.Circle);
        AppendFilterToken(builder, query.Filter.Va);
        AppendFilterToken(builder, query.Filter.Duration);
        AppendFilterToken(builder, query.Filter.Rate);
        AppendFilterToken(builder, query.Filter.Price);
        AppendFilterToken(builder, query.Filter.Sell);
        AppendFilterToken(builder, query.Filter.Age);
        AppendFilterToken(builder, query.Filter.Lang);

        var encoded = Uri.EscapeDataString(builder.ToString());

        var page = query.PageOptions;
        var queryTail = $"?order={page.Order}&sort={page.Sort}&page={page.Page}&pageSize={page.PageSize}&subtitle={page.Subtitle}&includeTranslationWorks={page.IncludeTranslationWorks.ToString().ToLowerInvariant()}";

        return encoded + queryTail;
    }

    private static void AppendFilterToken(StringBuilder builder, string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return;
        }

        builder.Append(' ');
        builder.Append('$');
        builder.Append(token);
        builder.Append('$');
    }

    private static IReadOnlyList<string> ParsePlainTexts(string input)
    {
        return input
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(static item => item.Trim())
            .Where(static item => !string.IsNullOrWhiteSpace(item))
            .ToArray();
    }

    private static SearchFilter ParseFilter(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return new SearchFilter();
        }

        var tag = string.Empty;
        var circle = string.Empty;
        var va = string.Empty;
        var duration = string.Empty;
        var rate = string.Empty;
        var price = string.Empty;
        var sell = string.Empty;
        var age = string.Empty;
        var lang = string.Empty;
        var parts = input.Split([',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var part in parts)
        {
            ValidateFilterToken(part);

            if (StartsWithAny(part, "tag:", "-tag:")) tag = part;
            else if (StartsWithAny(part, "circle:", "-circle:")) circle = part;
            else if (StartsWithAny(part, "va:", "-va:")) va = part;
            else if (StartsWithAny(part, "duration:", "-duration:")) duration = part;
            else if (StartsWithAny(part, "rate:", "-rate:")) rate = part;
            else if (StartsWithAny(part, "price:", "-price:")) price = part;
            else if (StartsWithAny(part, "sell:", "-sell:")) sell = part;
            else if (StartsWithAny(part, "age:", "-age:")) age = part;
            else if (StartsWithAny(part, "lang:", "-lang:")) lang = part;
        }

        return new SearchFilter
        {
            Tag = tag,
            Circle = circle,
            Va = va,
            Duration = duration,
            Rate = rate,
            Price = price,
            Sell = sell,
            Age = age,
            Lang = lang,
        };
    }

    private static SearchPageOptions ParsePageOptions(string input, SearchPageOptions defaults)
    {
        var values = ParseQueryString(input);

        return new SearchPageOptions
        {
            Order = GetOrDefault(values, "order", defaults.Order),
            Sort = GetOrDefault(values, "sort", defaults.Sort),
            Subtitle = GetOrDefault(values, "subtitle", defaults.Subtitle),
            IncludeTranslationWorks = bool.TryParse(GetOrDefault(values, "includeTranslationWorks", defaults.IncludeTranslationWorks.ToString()), out var includeTranslation)
                ? includeTranslation
                : defaults.IncludeTranslationWorks,
            Page = TryReadPositiveInt(values, "page", defaults.Page),
            PageSize = TryReadPositiveInt(values, "pageSize", defaults.PageSize),
        };
    }

    private static Dictionary<string, string> ParseQueryString(string input)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var segments = input.Split('&', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var segment in segments)
        {
            var parts = segment.Split('=', 2);
            if (parts.Length != 2)
            {
                continue;
            }

            result[parts[0]] = parts[1];
        }

        return result;
    }

    private static int TryReadPositiveInt(Dictionary<string, string> values, string key, int fallback)
    {
        if (values.TryGetValue(key, out var raw) && int.TryParse(raw, out var parsed) && parsed > 0)
        {
            return parsed;
        }

        return fallback;
    }

    private static string GetOrDefault(Dictionary<string, string> values, string key, string fallback)
    {
        return values.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : fallback;
    }

    private static bool StartsWithAny(string value, params string[] prefixes)
    {
        return prefixes.Any(prefix => value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
    }

    private static void ValidateFilterToken(string token)
    {
        var colonIndex = token.IndexOf(':');
        if (colonIndex < 0)
        {
            return;
        }

        var valuePart = token[(colonIndex + 1)..].Trim();
        if (valuePart.Length == 0)
        {
            throw new ArgumentException($"查询语法无效：筛选条件 `{token}` 缺少取值。", nameof(token));
        }
    }

    private static bool HasAnyFilter(SearchFilter filter)
    {
        var tokens = new[]
        {
            filter.Tag,
            filter.Circle,
            filter.Va,
            filter.Duration,
            filter.Rate,
            filter.Price,
            filter.Sell,
            filter.Age,
            filter.Lang,
        };

        return tokens.Any(token => !string.IsNullOrWhiteSpace(token));
    }
}
