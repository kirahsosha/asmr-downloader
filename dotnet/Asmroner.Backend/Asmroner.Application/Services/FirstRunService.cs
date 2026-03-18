using Asmroner.Core.Configuration;
using Asmroner.Core.Interfaces;

namespace Asmroner.Application.Services;

public sealed class FirstRunService : IFirstRunService
{
    public bool RequiresSetup(AppConfig? config, IReadOnlyList<string> validationErrors)
    {
        if (config is null)
        {
            return true;
        }

        return validationErrors.Count > 0;
    }
}