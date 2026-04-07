using System.Collections.Concurrent;
using System.Globalization;
using Asmroner.Core.Api;
using Asmroner.Core.Constants;
using Asmroner.Core.Interfaces;
using Asmroner.Core.Utils;

namespace Asmroner.Application.Services;

public sealed class EnqueueWorkInfoResolver : IEnqueueWorkInfoResolver
{
    private readonly IAsmrApiClient _asmrApiClient;
    private readonly IMetadataSyncStore _metadataSyncStore;

    public EnqueueWorkInfoResolver(IAsmrApiClient asmrApiClient, IMetadataSyncStore metadataSyncStore)
    {
        _asmrApiClient = asmrApiClient;
        _metadataSyncStore = metadataSyncStore;
    }

    public async Task<EnqueueWorkInfoResolutionResult> ResolvePreferTranslatedAsync(IReadOnlyCollection<EnqueueWorkInfoRequest> requests, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requests);

        var normalizedRequests = requests
            .Select(static request => new EnqueueWorkInfoRequest
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
            return new EnqueueWorkInfoResolutionResult();
        }

        var failedSourceIds = new ConcurrentBag<string>();
        var fetchedWorkInfos = await FetchWorkInfosAsync(normalizedRequests, failedSourceIds, cancellationToken);
        var resolvedWorkInfos = new Dictionary<string, WorkInfoDto>(StringComparer.OrdinalIgnoreCase);
        var switchedSourceCount = 0;

        foreach (var request in normalizedRequests)
        {
            var sourceId = request.SourceId;
            if (!fetchedWorkInfos.TryGetValue(sourceId, out var workInfo))
            {
                continue;
            }

            var selected = WorkLanguageSelectionPolicy.SelectPreferredEdition(workInfo);
            if (!string.Equals(selected.SelectedSourceId, sourceId, StringComparison.OrdinalIgnoreCase))
            {
                switchedSourceCount++;
            }

            if (resolvedWorkInfos.ContainsKey(selected.SelectedSourceId))
            {
                continue;
            }

            if (fetchedWorkInfos.TryGetValue(selected.SelectedSourceId, out var selectedWorkInfo))
            {
                resolvedWorkInfos[selected.SelectedSourceId] = selectedWorkInfo;
                continue;
            }

            resolvedWorkInfos[selected.SelectedSourceId] = new WorkInfoDto
            {
                Id = selected.SelectedWorkId,
                SourceId = selected.SelectedSourceId,
                Title = string.IsNullOrWhiteSpace(selected.SelectedTitle) ? workInfo.Title : selected.SelectedTitle,
                Release = workInfo.Release,
                HasSubtitle = workInfo.HasSubtitle,
                MainCoverUrl = workInfo.MainCoverUrl,
                WorkAttributes = workInfo.WorkAttributes,
                LanguageEditions = workInfo.LanguageEditions,
                OtherLanguageEditionsInDb = workInfo.OtherLanguageEditionsInDb,
                TranslationInfo = new WorkTranslationInfoDto
                {
                    Lang = selected.SelectedLanguage,
                    IsOriginal = selected.IsCurrentWork && workInfo.TranslationInfo.IsOriginal,
                },
            };
        }

        await UpsertMetadataSummariesAsync(resolvedWorkInfos, cancellationToken);

        return new EnqueueWorkInfoResolutionResult
        {
            WorkInfos = resolvedWorkInfos,
            FailedSourceIds = failedSourceIds
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(static sourceId => sourceId)
                .ToArray(),
            SwitchedSourceCount = switchedSourceCount,
        };
    }

    private async Task<IReadOnlyDictionary<string, WorkInfoDto>> FetchWorkInfosAsync(
        IReadOnlyList<EnqueueWorkInfoRequest> requests,
        ConcurrentBag<string> failedSourceIds,
        CancellationToken cancellationToken)
    {
        var result = new ConcurrentDictionary<string, WorkInfoDto>(StringComparer.OrdinalIgnoreCase);
        using var limiter = new SemaphoreSlim(AsmronerConstants.Download.WorkInfoFetchMaxConcurrency);

        var jobs = requests.Select(async request =>
        {
            await limiter.WaitAsync(cancellationToken);
            try
            {
                var lookupId = request.WorkId is > 0
                    ? request.WorkId.Value.ToString(CultureInfo.InvariantCulture)
                    : request.SourceId;
                var workInfo = await _asmrApiClient.GetWorkInfoAsync(lookupId, cancellationToken);
                result[request.SourceId] = workInfo;
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
        return new Dictionary<string, WorkInfoDto>(result, StringComparer.OrdinalIgnoreCase);
    }

    private async Task UpsertMetadataSummariesAsync(IReadOnlyDictionary<string, WorkInfoDto> workInfos, CancellationToken cancellationToken)
    {
        if (workInfos.Count == 0)
        {
            return;
        }

        var existingWorks = await _metadataSyncStore.GetMetadataWorksBySourceIdsAsync(workInfos.Keys.ToArray(), cancellationToken);
        var updatedAt = DateTime.UtcNow;
        var upsertItems = workInfos
            .Select(item => MetadataWorkSummaryMapper.MergeFromWorkInfo(
                item.Value,
                existingWorks.TryGetValue(item.Key, out var existing) ? existing : null,
                updatedAt))
            .Where(static item => item.Id > 0 && !string.IsNullOrWhiteSpace(item.SourceId))
            .ToArray();

        if (upsertItems.Length > 0)
        {
            await _metadataSyncStore.UpsertMetadataWorksAsync(upsertItems, cancellationToken);
        }
    }
}