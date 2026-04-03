namespace Asmroner.Core.Api;

public static class AsmrApiPaths
{
    public const string AuthLogin = "/api/auth/me";
    public const string Work = "/api/work/";
    public const string Tracks = "/api/tracks/";
    public const string Search = "/api/search/";
    public const string Popular = "/api/recommender/popular";
    public const string Works = "/api/works?order=release&sort=desc&page=1&pageSize=1";

    public static string BuildWorksQuery(int page, int pageSize, bool subtitleOnly = false)
    {
        return subtitleOnly
            ? $"/api/works?order=release&sort=desc&page={page}&pageSize={pageSize}&subtitle=1"
            : $"/api/works?order=release&sort=desc&page={page}&pageSize={pageSize}";
    }
}