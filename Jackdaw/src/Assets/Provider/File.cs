using Foster.Framework;
using SDL3;

namespace Jackdaw;

/// <summary>
/// An asset provider for loading data from individual files on disc.
/// Uses the folder structure to create groups and names.
/// </summary>
/// <param name="rootFolder">The folder all asset groups and items are contained within.</param>
public class FileFolderAssetProvider(string rootFolder) : StorageObjectAssetProvider {
    protected override void AssignStorage(Game game) {
        Task<ContentStorage> storageTask = game.FileSystem.OpenTitleStorageAsync(rootFolder);
        while (!storageTask.IsCompleted) {
            SDL.SDL_Delay(1);
        }

        FileStorage = storageTask.Result;
    }
}