using Asmroner.Core.Interfaces;
using Asmroner.Core.Utils;

namespace Asmroner.Application.Services;

public sealed class SearchStateStore : ISearchStateStore
{
    private readonly HashSet<string> _queuedSourceIds = new(StringComparer.OrdinalIgnoreCase);

    public void EnqueueForDownload(IEnumerable<string> sourceIds)
    {
        foreach (var sourceId in sourceIds)
        {
            var normalized = NormalizeSourceId(sourceId);
            if (string.IsNullOrWhiteSpace(normalized))
            {
                continue;
            }

            _queuedSourceIds.Add(normalized);
        }
    }

    public IReadOnlyList<string> GetQueuedSourceIds()
    {
        return _queuedSourceIds.OrderBy(static id => id).ToArray();
    }

    public void RemoveFromQueue(IEnumerable<string> sourceIds)
    {
        foreach (var sourceId in sourceIds)
        {
            var normalized = NormalizeSourceId(sourceId);
            if (string.IsNullOrWhiteSpace(normalized))
            {
                continue;
            }

            _queuedSourceIds.Remove(normalized);
        }
    }

    private static string NormalizeSourceId(string? raw)
    {
        return SourceIdNormalizer.Normalize(raw);
    }
}
