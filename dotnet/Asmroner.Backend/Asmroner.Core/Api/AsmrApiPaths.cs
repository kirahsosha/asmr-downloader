namespace Asmroner.Core.Api;

public static class AsmrApiPaths
{
    public const string AuthLogin = "/api/auth/me";
    public const string Health = "/api/health?cache=false";
    public const string Work = "/api/work/";
    public const string Tracks = "/api/tracks/";
    public const string Search = "/api/search/";
    public const string Popular = "/api/recommender/popular";
    public const string Works = "/api/works?order=create_date&sort=asc&page=1&pageSize=1&subtitle=0&includeTranslationWorks=true";

    public static string BuildWorksQuery(int page, int pageSize, bool subtitleOnly = false)
    {
        return subtitleOnly
            ? $"/api/works?order=create_date&sort=asc&page={page}&pageSize={pageSize}&subtitle=1&includeTranslationWorks=true"
            : $"/api/works?order=create_date&sort=asc&page={page}&pageSize={pageSize}&subtitle=0&includeTranslationWorks=true";
    }
}