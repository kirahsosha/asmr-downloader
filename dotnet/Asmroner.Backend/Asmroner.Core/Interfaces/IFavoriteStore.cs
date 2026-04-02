using Asmroner.Core.Favorites;

namespace Asmroner.Core.Interfaces;

public interface IFavoriteStore
{
    Task<IReadOnlyList<string>> LoadFavoriteFolderTitlesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FavoriteWorkItem>> LoadFavoriteFolderItemsAsync(string folderTitle, CancellationToken cancellationToken = default);

    Task<FavoriteSaveResult> SaveFavoriteFolderItemsAsync(
        string folderTitle,
        IReadOnlyCollection<FavoriteWorkItem> items,
        CancellationToken cancellationToken = default);
}