using Asmroner.Core.Search;

namespace Asmroner.Core.Interfaces;

public interface ISearchImportService
{
    /// <summary>
    /// Parses a CSV file exported by <see cref="ISearchExportService"/> and returns the work items.
    /// Expected header: source_id,release,rate_average_2dp,dl_count,has_subtitle,title
    /// </summary>
    Task<IReadOnlyList<SearchWorkItem>> ParseCsvAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Parses a JSON file exported by <see cref="ISearchExportService"/> and returns the work items.
    /// Expected format: JSON array of SearchWorkItem objects with camelCase field names.
    /// </summary>
    Task<IReadOnlyList<SearchWorkItem>> ParseJsonAsync(string filePath, CancellationToken cancellationToken = default);
}
