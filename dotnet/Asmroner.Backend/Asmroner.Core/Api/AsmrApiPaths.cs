namespace Asmroner.Core.Api;

public static class AsmrApiPaths
{
    public const string DefaultBaseUrl = "https://api.asmr-300.com";
    public const string FallbackBaseUrl = "https://api.asmr.one";
    public const string LatestPublishUrl = "https://as.mr";
    public const string LatestPublishProxyUrl = "https://as.131433.xyz";

    public const string AuthLogin = "/api/auth/me";
    public const string Work = "/api/work/";
    public const string Tracks = "/api/tracks/";
    public const string Search = "/api/search/";
    public const string Popular = "/api/recommender/popular";
    public const string Works = "/api/works?order=release&sort=desc&page=1&pageSize=1";
}