using System.Text;
using System.Text.Json;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Search;

namespace Asmroner.Application.Services;

public sealed class SearchExportService : ISearchExportService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
    };

    public async Task ExportCsvAsync(IReadOnlyList<SearchWorkItem> items, string filePath, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? ".");

        var lines = new List<string>(items.Count + 1)
        {
            "source_id,title,has_subtitle,release,tags",
        };

        foreach (var item in items)
        {
            lines.Add(string.Join(',',
                EscapeCsv(item.SourceId),
                EscapeCsv(item.Title),
                item.HasSubtitle ? "true" : "false",
                EscapeCsv(item.Release),
                EscapeCsv(item.Tags)));
        }

        await File.WriteAllLinesAsync(filePath, lines, Encoding.UTF8, cancellationToken);
    }

    public async Task ExportJsonAsync(IReadOnlyList<SearchWorkItem> items, string filePath, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? ".");

        await using var stream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(stream, items, JsonOptions, cancellationToken);
    }

    private static string EscapeCsv(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }
}
