using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// An asset provider for loading data from individual files on disc.
/// Uses the folder structure to create groups and names.
/// </summary>
/// <param name="rootFolder">The folder all asset groups and items are contained within.</param>
public class FileFolderAssetProvider(string rootFolder) : StorageObjectAssetProvider {
    protected override void AssignStorage(Game game) {
        game.FileSystem.OpenTitleStorage(rootFolder, Callback);
    }

    void Callback(ContentStorage storage) {
        FileStorage = storage;
    }
}