using Asmroner.Core.Search;

namespace Asmroner.Core.Interfaces;

public interface ISearchService
{
	Task<SearchExecutionResult> SearchAsync(string rawQuery, int count, CancellationToken cancellationToken = default);
}