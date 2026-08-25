using System.Diagnostics;
using Foster.Framework;
using SDL3;

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
        // SDL default for accessing the project's root directory will default to using the dotnet runtime location while debugging.
        //  This workaround exists until I have a better method of opening a storage object directly in the root
        if (Debugger.IsAttached) { game.FileSystem.OpenTitleStorage(AppContext.BaseDirectory, Callback); }
        else { game.FileSystem.OpenTitleStorage(Callback); }
    }

    void Callback(ContentStorage storage) {
        FileStorage = new ZipStorage(storage, path);
    }
}