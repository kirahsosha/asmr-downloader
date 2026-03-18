using Asmroner.Core.Api;

namespace Asmroner.Wpf.ViewModels;

public static class DownloadWorkInfoTitlePolicy
{
    public static Dictionary<string, string> BuildNonEmptyTitleMap(IReadOnlyDictionary<string, WorkInfoDto> workInfos)
    {
        var titles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var (sourceId, info) in workInfos)
        {
            if (string.IsNullOrWhiteSpace(info.Title))
            {
                continue;
            }

            titles[sourceId] = info.Title;
        }

        return titles;
    }
}
