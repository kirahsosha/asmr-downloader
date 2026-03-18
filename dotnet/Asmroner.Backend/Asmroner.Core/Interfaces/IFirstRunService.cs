using Asmroner.Core.Configuration;

namespace Asmroner.Core.Interfaces;

public interface IFirstRunService
{
    bool RequiresSetup(AppConfig? config, IReadOnlyList<string> validationErrors);
}