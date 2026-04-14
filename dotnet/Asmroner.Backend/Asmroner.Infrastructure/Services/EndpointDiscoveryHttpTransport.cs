using System.Net;

namespace Asmroner.Infrastructure.Services;

public static class EndpointDiscoveryHttpTransport
{
    public const string ProbeClientName = "AsmrProbe";
    public const string PublishClientName = "AsmrPublish";
    public const string ProbeUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/135.0.0.0 Safari/537.36";
    public const string PublishAcceptHeader = "text/html,application/xhtml+xml,application/javascript,*/*;q=0.8";

    public static void ConfigureProbeClient(HttpClient client)
    {
        client.DefaultRequestVersion = HttpVersion.Version11;
        client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
    }

    public static void ConfigurePublishClient(HttpClient client)
    {
        ConfigureProbeClient(client);
    }

    public static HttpMessageHandler CreatePublishHandler()
    {
        return new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = static (_, _, _, _) => true,
        };
    }

    public static void ApplyProbeRequestHeaders(HttpRequestMessage request)
    {
        if (request.Headers.UserAgent.Count == 0)
        {
            request.Headers.UserAgent.ParseAdd(ProbeUserAgent);
        }
    }

    public static void ApplyPublishRequestHeaders(HttpRequestMessage request)
    {
        ApplyProbeRequestHeaders(request);

        if (request.Headers.Accept.Count == 0)
        {
            request.Headers.Accept.ParseAdd(PublishAcceptHeader);
        }
    }
}