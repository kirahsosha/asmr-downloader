using System.Collections.Concurrent;
using System.Globalization;
using Asmroner.Core.Api;
using Asmroner.Core.Configuration;
using Asmroner.Core.Constants;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Sync;
using Asmroner.Core.Utils;

namespace Asmroner.Application.Services;

public sealed class MetadataWorkInfoResolver : IMetadataWorkInfoResolver
{
    private readonly IAsmrApiClient _asmrApiClient;
    private readonly IConfigurationService _configurationService;
    private readonly IMetadataSyncStore _metadataSyncStore;

    public MetadataWorkInfoResolver(
        IAsmrApiClient asmrApiClient,
        IConfigurationService configurationService,
        IMetadataSyncStore metadataSyncStore)
    {
        _asmrApiClient = asmrApiClient;
        _configurationService = configurationService;
        _metadataSyncStore = metadataSyncStore;
    }

    public async Task<MetadataWorkInfoResolutionResult> ResolveAsync(IReadOnlyCollection<WorkInfoResolutionRequest> requests, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requests);

        var normalizedRequests = requests
            .Select(static request => new WorkInfoResolutionRequest
            {
                SourceId = SourceIdNormalizer.Normalize(request.SourceId),
                WorkId = request.WorkId is > 0 ? request.WorkId : null,
            })
            .Where(static request => !string.IsNullOrWhiteSpace(request.SourceId))
            .GroupBy(static request => request.SourceId, StringComparer.OrdinalIgnoreCase)
            .Select(static group => group
                .OrderByDescending(static request => request.WorkId.HasValue)
                .First())
            .ToArray();

        if (normalizedRequests.Length == 0)
        {
            return new MetadataWorkInfoResolutionResult();
        }

        var config = await _configurationService.LoadAsync(cancellationToken) ?? new AppConfig();
        var validDays = Math.Max(1, config.Downloader.MetadataValidityDays);
        var freshnessThreshold = DateTime.UtcNow.AddDays(-validDays);
        var storedWorks = await _metadataSyncStore.GetMetadataWorksBySourceIdsAsync(
            normalizedRequests.Select(static request => request.SourceId).ToArray(),
            cancellationToken);

        var result = new Dictionary<string, WorkInfoDto>(StringComparer.OrdinalIgnoreCase);
        var pendingFetches = new List<WorkInfoResolutionRequest>();
        foreach (var request in normalizedRequests)
        {
            if (storedWorks.TryGetValue(request.SourceId, out var metadataWork) && metadataWork.UpdatedAt >= freshnessThreshold)
            {
                result[request.SourceId] = MetadataWorkSummaryMapper.ToWorkInfoDto(metadataWork);
                continue;
            }

            pendingFetches.Add(request);
        }

        if (pendingFetches.Count == 0)
        {
            return new MetadataWorkInfoResolutionResult
            {
                WorkInfos = result,
            };
        }

        var fetchedWorkInfos = new ConcurrentDictionary<string, WorkInfoDto>(StringComparer.OrdinalIgnoreCase);
        var failedSourceIds = new ConcurrentBag<string>();
        using var limiter = new SemaphoreSlim(AsmronerConstants.Download.WorkInfoFetchMaxConcurrency);

        var jobs = pendingFetches.Select(async request =>
        {
            await limiter.WaitAsync(cancellationToken);
            try
            {
                var lookupId = request.WorkId is > 0
                    ? request.WorkId.Value.ToString(CultureInfo.InvariantCulture)
                    : request.SourceId;
                var workInfo = await _asmrApiClient.GetWorkInfoAsync(lookupId, cancellationToken);
                fetchedWorkInfos[request.SourceId] = workInfo;
            }
            catch
            {
                failedSourceIds.Add(request.SourceId);
            }
            finally
            {
                limiter.Release();
            }
        });

        await Task.WhenAll(jobs);

        var updatedAt = DateTime.UtcNow;
        var upsertItems = new List<MetadataWorkItem>(fetchedWorkInfos.Count);
        foreach (var request in pendingFetches)
        {
            if (!fetchedWorkInfos.TryGetValue(request.SourceId, out var workInfo))
            {
                continue;
            }

            var normalizedSourceId = SourceIdNormalizer.Normalize(workInfo.SourceId);
            if (string.IsNullOrWhiteSpace(normalizedSourceId))
            {
                normalizedSourceId = request.SourceId;
            }

            var normalizedWorkInfo = new WorkInfoDto
            {
                Id = workInfo.Id,
                SourceId = normalizedSourceId,
                Title = workInfo.Title,
                Release = workInfo.Release,
                HasSubtitle = workInfo.HasSubtitle,
                MainCoverUrl = workInfo.MainCoverUrl,
                WorkAttributes = workInfo.WorkAttributes,
                LanguageEditions = workInfo.LanguageEditions,
                OtherLanguageEditionsInDb = workInfo.OtherLanguageEditionsInDb,
                TranslationInfo = workInfo.TranslationInfo,
            };

            result[normalizedSourceId] = normalizedWorkInfo;
            storedWorks.TryGetValue(request.SourceId, out var existing);
            upsertItems.Add(MetadataWorkSummaryMapper.MergeFromWorkInfo(normalizedWorkInfo, existing, updatedAt));
        }

        var validUpserts = upsertItems
            .Where(static item => item.Id > 0 && !string.IsNullOrWhiteSpace(item.SourceId))
            .ToArray();
        if (validUpserts.Length > 0)
        {
            await _metadataSyncStore.UpsertMetadataWorksAsync(validUpserts, cancellationToken);
        }

        return new MetadataWorkInfoResolutionResult
        {
            WorkInfos = result,
            FailedSourceIds = failedSourceIds
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(static sourceId => sourceId)
                .ToArray(),
        };
    }
}