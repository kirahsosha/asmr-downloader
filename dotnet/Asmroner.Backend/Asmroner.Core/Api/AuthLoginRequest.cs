namespace Asmroner.Core.Api;

public sealed class AuthLoginRequest
{
    public required string Name { get; init; }

    public required string Password { get; init; }
}