using Asmroner.Core.Sync;

namespace Asmroner.Core.Interfaces;

public interface ISyncExportService
{
    Task<SyncExportResult> ExportAsync(SyncExportStatus status, string filePath, CancellationToken cancellationToken = default);
}