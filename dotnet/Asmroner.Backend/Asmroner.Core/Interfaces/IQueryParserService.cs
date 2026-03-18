using Asmroner.Core.Search;

namespace Asmroner.Core.Interfaces;

public interface IQueryParserService
{
    SearchQuery Parse(string rawQuery);

    string BuildAsmrQuery(SearchQuery query);
}
