using Asmroner.Core.Search;
using Asmroner.Core.Interfaces;

namespace Asmroner.Wpf.Services;

public sealed class SearchService : ISearchService
{
	public Task<SearchExecutionResult> SearchAsync(string rawQuery, int count, CancellationToken cancellationToken = default)
	{
		throw new NotSupportedException("WPF 占位 SearchService 已弃用，请使用 Asmroner.Application.Services.SearchService。");
	}
}