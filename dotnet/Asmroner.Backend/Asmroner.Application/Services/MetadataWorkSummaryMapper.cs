using Asmroner.Core.Api;
using Asmroner.Core.Sync;
using Asmroner.Core.Utils;

namespace Asmroner.Application.Services;

internal static class MetadataWorkSummaryMapper
{
    public static WorkInfoDto ToWorkInfoDto(MetadataWorkItem item)
    {
        return new WorkInfoDto
        {
            Id = item.Id,
            Title = item.Title,
            Release = item.Release,
            HasSubtitle = item.HasSubtitle,
            SourceId = SourceIdNormalizer.Normalize(item.SourceId),
        };
    }

    public static MetadataWorkItem MergeFromWorkInfo(WorkInfoDto workInfo, MetadataWorkItem? existing, DateTime updatedAt)
    {
        var normalizedSourceId = SourceIdNormalizer.Normalize(workInfo.SourceId);
        return new MetadataWorkItem
        {
            Id = workInfo.Id > 0 ? workInfo.Id : existing?.Id ?? 0,
            Title = string.IsNullOrWhiteSpace(workInfo.Title) ? existing?.Title ?? string.Empty : workInfo.Title.Trim(),
            CircleId = existing?.CircleId ?? 0,
            CircleName = existing?.CircleName ?? string.Empty,
            Nsfw = existing?.Nsfw ?? false,
            Release = string.IsNullOrWhiteSpace(workInfo.Release) ? existing?.Release ?? string.Empty : workInfo.Release.Trim(),
            DownloadCount = existing?.DownloadCount ?? 0,
            Price = existing?.Price ?? 0,
            ReviewCount = existing?.ReviewCount ?? 0,
            RateCount = existing?.RateCount ?? 0,
            RateAverage = existing?.RateAverage ?? 0,
            HasSubtitle = workInfo.HasSubtitle,
            CreateDate = existing?.CreateDate ?? string.Empty,
            Vas = existing?.Vas ?? string.Empty,
            Tags = existing?.Tags ?? string.Empty,
            Duration = existing?.Duration ?? 0,
            SourceType = existing?.SourceType ?? string.Empty,
            SourceId = string.IsNullOrWhiteSpace(normalizedSourceId)
                ? existing?.SourceId ?? string.Empty
                : normalizedSourceId,
            UpdatedAt = updatedAt,
        };
    }
}