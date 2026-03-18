using Asmroner.Core.Search;

namespace Asmroner.Core.Interfaces;

public interface ISearchExportService
{
    Task ExportCsvAsync(IReadOnlyList<SearchWorkItem> items, string filePath, CancellationToken cancellationToken = default);

    Task ExportJsonAsync(IReadOnlyList<SearchWorkItem> items, string filePath, CancellationToken cancellationToken = default);
}
