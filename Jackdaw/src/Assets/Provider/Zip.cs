using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// An asset provider for loading data from a zip archive on disc.
/// Uses the archive's internal folder structure to create groups and names.
/// </summary>
/// <param name="path">
/// The relative file path to the zip archive's location, including the file name and extension. <br/>
/// The file does not need to use the .zip extension to work.
/// </param>
public class ZipArchiveAssetProvider(string path) : StorageObjectAssetProvider {
    protected override void AssignStorage(Game game) {
        game.FileSystem.OpenTitleStorage(Callback);
    }

    void Callback(ContentStorage storage) {
        FileStorage = new ZipStorage(storage, path);
    }
}