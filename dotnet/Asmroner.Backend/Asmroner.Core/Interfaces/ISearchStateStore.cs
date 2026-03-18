namespace Asmroner.Core.Interfaces;

public interface ISearchStateStore
{
    void EnqueueForDownload(IEnumerable<string> sourceIds);

    IReadOnlyList<string> GetQueuedSourceIds();

    void RemoveFromQueue(IEnumerable<string> sourceIds);
}
