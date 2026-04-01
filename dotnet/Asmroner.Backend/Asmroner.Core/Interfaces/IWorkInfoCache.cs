using Asmroner.Core.Api;

namespace Asmroner.Core.Interfaces;

public interface IWorkInfoCache
{
    bool TryGet(string lookupKey, WorkInfoCacheRequirement requirement, out WorkInfoDto? workInfo);

    void Set(string lookupKey, WorkInfoDto workInfo, WorkInfoCacheEntryLevel cacheLevel);

    void SetMany(IReadOnlyDictionary<string, WorkInfoDto> workInfos, WorkInfoCacheEntryLevel cacheLevel);

    IReadOnlyDictionary<string, WorkInfoDto> GetSnapshot();
}

public enum WorkInfoCacheEntryLevel
{
    Summary = 1,
    Full = 2,
}

public enum WorkInfoCacheRequirement
{
    Any = 1,
    Full = 2,
}