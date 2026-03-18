using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Asmroner.Core.Download;

public static class DownloadTaskStatusExtensions
{
    /// <summary>
    /// Gets the display name from the Display attribute for the status.
    /// </summary>
    public static string GetDisplayName(this DownloadTaskStatus status)
    {
        var field = typeof(DownloadTaskStatus).GetField(status.ToString());
        if (field == null)
        {
            return status.ToString();
        }

        var displayAttribute = field.GetCustomAttribute<DisplayAttribute>();
        return displayAttribute?.Name ?? status.ToString();
    }

    /// <summary>
    /// Gets the sort order from the Order attribute for the status.
    /// Lower values sort first.
    /// </summary>
    public static int GetSortOrder(this DownloadTaskStatus status)
    {
        var field = typeof(DownloadTaskStatus).GetField(status.ToString());
        if (field == null)
        {
            return int.MaxValue;
        }

        var orderAttribute = field.GetCustomAttribute<OrderAttribute>();
        return orderAttribute?.Order ?? int.MaxValue;
    }
}
