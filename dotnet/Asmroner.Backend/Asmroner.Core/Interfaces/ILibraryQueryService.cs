using Asmroner.Core.Library;

namespace Asmroner.Core.Interfaces;

public interface ILibraryQueryService
{
    Task<LibraryQueryResult> QueryAsync(LibraryQuery query, CancellationToken cancellationToken = default);
}