using System.Net;

namespace Asmroner.Infrastructure.Services;

public static class EndpointDiscoveryHttpTransport
{
    public const string ProbeClientName = "AsmrProbe";
    public const string ProbeUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/135.0.0.0 Safari/537.36";

    public static void ConfigureProbeClient(HttpClient client)
    {
        client.DefaultRequestVersion = HttpVersion.Version11;
        client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
    }

    public static void ApplyProbeRequestHeaders(HttpRequestMessage request)
    {
        if (request.Headers.UserAgent.Count == 0)
        {
            request.Headers.UserAgent.ParseAdd(ProbeUserAgent);
        }
    }
}