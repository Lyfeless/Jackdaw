using Foster.Framework;
using SDL3;

namespace Jackdaw;

internal record struct SaveDataDecomposedPath(string Directory, string Name, string Extension);
internal record struct SaveDataBackupEntry(string FullPath, string Name, string FullName, string Extension, long Timestamp);

internal static class SaveDataHandlerUtils {
    internal const string VERSION_CONTAINER = "version";

    internal static ContentStorage OpenStorage(Game game) {
        Task<ContentStorage> storageTask = game.FileSystem.OpenUserStorageAsync();
        while (!storageTask.IsCompleted) {
            SDL.SDL_Delay(1);
        }
        return storageTask.Result;
    }

    internal static SaveDataDecomposedPath DecomposePath(string path) => new(
        Path.GetDirectoryName(path) ?? string.Empty,
        Path.GetFileNameWithoutExtension(path),
        Path.GetExtension(path)
    );

    internal static SaveDataBackupEntry[] GetBackupFileEntries(ContentStorage storage, string savePath) {
        SaveDataDecomposedPath path = DecomposePath(savePath);
        return GetBackupFileEntries(storage, path);
    }

    internal static SaveDataBackupEntry[] GetBackupFileEntries(ContentStorage storage, SaveDataDecomposedPath path) {
        List<SaveDataBackupEntry> entries = [];
        foreach (string file in storage.EnumerateDirectory("/", $"*{path.Extension}", SearchOption.TopDirectoryOnly)) {
            string name = Path.GetFileNameWithoutExtension(file);
            if (!name.StartsWith(path.Name)) { continue; }

            string[] segments = name.Split('-');
            if (segments.Length < 2 || !long.TryParse(segments[^1], out long fileTime)) { continue; }

            entries.Add(new(
                file,
                path.Name,
                name,
                path.Extension,
                fileTime
            ));
        }

        return [.. entries.OrderByDescending(e => e.Timestamp)];
    }
}