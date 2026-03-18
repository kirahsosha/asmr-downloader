namespace Asmroner.Core.Api;

public sealed class ConnectivityProbeResult
{
    public bool IsReachable { get; init; }

    public bool IsAuthenticated { get; init; }

    public string BaseUrl { get; init; } = string.Empty;

    public long LatencyMs { get; init; }

    public string Message { get; init; } = string.Empty;
}