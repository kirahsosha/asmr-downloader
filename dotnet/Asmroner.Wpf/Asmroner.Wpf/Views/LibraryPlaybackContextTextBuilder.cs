using Asmroner.Core.Playback;

namespace Asmroner.Wpf.Views;

public sealed class LibraryPlaybackContextTexts
{
    public string SelectionText { get; init; } = string.Empty;

    public string LoadedContextText { get; init; } = string.Empty;
}

public static class LibraryPlaybackContextTextBuilder
{
    private const string DefaultSelectionText = "当前未选择任何作品或文件。请先选择左侧作品，并在文件树中显式选中一个可播放的媒体文件。";
    private const string DefaultLoadedContextMessage = "尚未载入任何可播放媒体文件。";

    public static LibraryPlaybackContextTexts Build(
        PlaybackContext context,
        LibrarySelectionFeedbackResult selectionFeedback)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(selectionFeedback);

        return new LibraryPlaybackContextTexts
        {
            SelectionText = BuildSelectionText(selectionFeedback),
            LoadedContextText = BuildLoadedContextText(context),
        };
    }

    private static string BuildSelectionText(LibrarySelectionFeedbackResult selectionFeedback)
    {
        return string.IsNullOrWhiteSpace(selectionFeedback.Message)
            ? DefaultSelectionText
            : selectionFeedback.Message;
    }

    private static string BuildLoadedContextText(PlaybackContext context)
    {
        var message = string.IsNullOrWhiteSpace(context.Message)
            ? DefaultLoadedContextMessage
            : context.Message;

        if (context.Work is null || context.File is null)
        {
            return $"状态：{BuildPlaybackStateText(context.State)}\n说明：{message}";
        }

        var workText = string.IsNullOrWhiteSpace(context.Work.Title)
            ? context.Work.SourceId
            : $"{context.Work.SourceId} {context.Work.Title}";

        return $"状态：{BuildPlaybackStateText(context.State)}\n作品：{workText}\n文件：{context.File.RelativePath}\n说明：{message}";
    }

    private static string BuildPlaybackStateText(PlaybackState state)
    {
        return state switch
        {
            PlaybackState.None => "未载入",
            PlaybackState.Ready => "已载入",
            PlaybackState.Launched => "已调用系统打开",
            PlaybackState.Failed => "打开失败",
            _ => state.ToString(),
        };
    }
}