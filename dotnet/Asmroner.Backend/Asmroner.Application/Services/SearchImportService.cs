using System.Text;
using System.Text.Json;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Search;

namespace Asmroner.Application.Services;

public sealed class SearchImportService : ISearchImportService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<IReadOnlyList<SearchWorkItem>> ParseCsvAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        var lines = await File.ReadAllLinesAsync(filePath, Encoding.UTF8, cancellationToken);

        if (lines.Length == 0)
        {
            return Array.Empty<SearchWorkItem>();
        }

        var header = ParseCsvRow(lines[0]);
        var colSourceId = IndexOf(header, "source_id");
        var colTitle = IndexOf(header, "title");
        var colRelease = IndexOf(header, "release");
        var colTags = IndexOf(header, "tags");
        var colHasSub = IndexOf(header, "has_subtitle");

        if (colSourceId < 0)
        {
            return Array.Empty<SearchWorkItem>();
        }

        var results = new List<SearchWorkItem>(lines.Length - 1);
        for (var i = 1; i < lines.Length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var line = lines[i];
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var cols = ParseCsvRow(line);
            if (cols.Count <= colSourceId)
            {
                continue;
            }

            var sourceId = cols[colSourceId];
            if (string.IsNullOrWhiteSpace(sourceId))
            {
                continue;
            }

            results.Add(new SearchWorkItem
            {
                SourceId = sourceId,
                Title = colTitle >= 0 && colTitle < cols.Count ? cols[colTitle] : string.Empty,
                Release = colRelease >= 0 && colRelease < cols.Count ? cols[colRelease] : string.Empty,
                Tags = colTags >= 0 && colTags < cols.Count ? cols[colTags] : string.Empty,
                HasSubtitle = colHasSub >= 0 && colHasSub < cols.Count
                    && string.Equals(cols[colHasSub], "true", StringComparison.OrdinalIgnoreCase),
            });
        }

        return results;
    }

    public async Task<IReadOnlyList<SearchWorkItem>> ParseJsonAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        await using var stream = File.OpenRead(filePath);
        var items = await JsonSerializer.DeserializeAsync<List<SearchWorkItem>>(
            stream, JsonOptions, cancellationToken);

        return items is null || items.Count == 0
            ? Array.Empty<SearchWorkItem>()
            : items.Where(static item => !string.IsNullOrWhiteSpace(item.SourceId)).ToArray();
    }

    /// <summary>
    /// Parses a single CSV row, handling RFC 4180 double-quoted fields.
    /// </summary>
    private static List<string> ParseCsvRow(string line)
    {
        var result = new List<string>();
        var current = new StringBuilder();
        var inQuotes = false;
        var i = 0;

        while (i < line.Length)
        {
            var ch = line[i];

            if (inQuotes)
            {
                if (ch == '"')
                {
                    // Peek next char: if also '"', it's an escaped quote
                    if (i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i += 2;
                        continue;
                    }

                    inQuotes = false;
                }
                else
                {
                    current.Append(ch);
                }
            }
            else
            {
                if (ch == '"')
                {
                    inQuotes = true;
                }
                else if (ch == ',')
                {
                    result.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(ch);
                }
            }

            i++;
        }

        result.Add(current.ToString());
        return result;
    }

    private static int IndexOf(List<string> header, string name)
    {
        for (var i = 0; i < header.Count; i++)
        {
            if (string.Equals(header[i].Trim(), name, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }

        return -1;
    }
}
