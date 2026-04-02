namespace Asmroner.Core.Favorites;

public sealed class FavoriteWorkItem
{
    public string SourceId { get; init; } = string.Empty;

    public int WorkId { get; init; }

    public string Title { get; init; } = string.Empty;
}