using Asmroner.Core.Library;

namespace Asmroner.Core.Interfaces;

public interface ILibraryScannerService
{
    Task<LibraryScanResult> ScanAsync(CancellationToken cancellationToken = default);
}