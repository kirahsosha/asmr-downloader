namespace Asmroner.Core.Api;

public sealed class AsmrApiException : Exception
{
    public AsmrApiException(ApiError error)
        : base(error.Message)
    {
        Error = error;
    }

    public ApiError Error { get; }
}