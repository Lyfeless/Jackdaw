using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Foster.Framework;

namespace Jackdaw;

internal static class SaveDataSaveHandler {
    public static void Save(Game game, SaveData saveData) => Save(game, saveData, saveData.SaveFormat);

    public static void Save(Game game, SaveData saveData, SaveData.Format format) {
        ContentStorage storage = SaveDataHandlerUtils.OpenStorage(game);

        string savePath = saveData.SavePath;
        if (saveData.UseBackups) {
            SaveDataDecomposedPath pathInfo = SaveDataHandlerUtils.DecomposePath(saveData.SavePath);
            savePath = GetBackupPathName(pathInfo);
            RemoveExtraBackups(storage, pathInfo, saveData.BackupCount);
        }

        switch (format) {
            case SaveData.Format.BINARY: SaveBinary(storage, savePath, saveData); break;
            case SaveData.Format.JSON: SaveJson(storage, savePath, saveData); break;
        }
    }

    static void SaveBinary(ContentStorage storage, string savePath, SaveData saveData) {
        using MemoryStream memory = new();
        using BinaryWriter writer = new(memory);

        SaveDataFileLoaderV2 loader = new();
        loader.SaveBinary(saveData, writer);

        storage.WriteAllBytes(savePath, memory.GetBuffer());
    }

    static void SaveJson(ContentStorage storage, string savePath, SaveData saveData) {
        using MemoryStream memory = new();
        using Utf8JsonWriter writer = new(memory);

        SaveDataFileLoaderV2 loader = new();
        loader.SaveJson(saveData, writer);

        writer.Flush();
        storage.WriteAllText(savePath, Encoding.UTF8.GetString(memory.ToArray()));
    }

    static string GetBackupPathName(SaveDataDecomposedPath pathInfo) {
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return Path.Join(pathInfo.Directory, $"{pathInfo.Name}-{timestamp}{pathInfo.Extension}");
    }

    static void RemoveExtraBackups(ContentStorage storage, SaveDataDecomposedPath pathInfo, int maxBackups) {
        SaveDataBackupEntry[] backupFiles = SaveDataHandlerUtils.GetBackupFileEntries(storage, pathInfo);
        int diff = backupFiles.Length - maxBackups + 1;
        if (diff == 0) { return; }

        for (int i = 0; i < diff; ++i) {
            storage.Remove(backupFiles[^(1 + i)].FullPath);
        }
    }
}