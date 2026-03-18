namespace Asmroner.Core.Api;

public sealed class ApiError
{
    public string Code { get; init; } = "unknown_error";

    public int HttpStatus { get; init; }

    public string Message { get; init; } = string.Empty;

    public string? RawBody { get; init; }
}