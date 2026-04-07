using Asmroner.Core.Api;

namespace Asmroner.Core.Interfaces;

public interface IAsmrApiClient
{
    Task<MetadataSyncPageDto> GetMetadataWorksAsync(int page, int pageSize, bool subtitleOnly = false, CancellationToken cancellationToken = default);

    Task<WorkInfoDto> GetWorkInfoAsync(string id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TrackDto>> GetTracksAsync(string id, CancellationToken cancellationToken = default);

    Task DownloadFileAsync(string url, string destinationPath, CancellationToken cancellationToken = default);

    Task<SearchResultDto> SearchAsync(string query, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SearchWorkDto>> GetPopularAsync(CancellationToken cancellationToken = default);
}