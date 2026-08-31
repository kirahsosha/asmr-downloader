namespace Asmroner.Core.Api;

public static class AsmrApiPaths
{
    public const string AuthLogin = "/api/auth/me";
    public const string Health = "/api/health?cache=false";
    public const string Work = "/api/work/";
    public const string Tracks = "/api/tracks/";
    public const string Search = "/api/search/";
    public const string Popular = "/api/recommender/popular";
    public static readonly string Works = BuildWorksQuery(1, 1, subtitleOnly: false);

    public static string BuildQuery(string path, IEnumerable<KeyValuePair<string, string?>> queryParameters)
    {
        return BuildQuery(path, queryParameters.Select(static pair => new KeyValuePair<string, object?>(pair.Key, pair.Value)));
    }

    public static string BuildQuery(string path, IEnumerable<KeyValuePair<string, object?>> queryParameters)
    {
        var pairs = queryParameters
            .Where(static pair => !string.IsNullOrWhiteSpace(pair.Key) && pair.Value is not null)
            .Select(static pair => $"{pair.Key}={FormatValue(pair.Value!)}")
            .ToArray();

        if (pairs.Length == 0)
        {
            return string.IsNullOrWhiteSpace(path) ? string.Empty : path;
        }

        var query = string.Join("&", pairs);
        return string.IsNullOrWhiteSpace(path) ? $"?{query}" : $"{path}?{query}";
    }

    public static string BuildWorksQuery(int page, int pageSize, bool subtitleOnly = false)
    {
        return BuildQuery("/api/works", new Dictionary<string, object?>
        {
            ["order"] = "create_date",
            ["sort"] = "asc",
            ["page"] = page,
            ["pageSize"] = pageSize,
            ["subtitle"] = subtitleOnly ? 1 : 0,
            ["includeTranslationWorks"] = true,
        });
    }

    private static string FormatValue(object value)
    {
        return value switch
        {
            bool boolValue => boolValue ? "true" : "false",
            IFormattable formattable => formattable.ToString(null, System.Globalization.CultureInfo.InvariantCulture),
            _ => value.ToString() ?? string.Empty,
        };
    }
}