using System.Text.Json.Nodes;
using Foster.Framework;
using SDL3;

namespace Jackdaw;

internal static class SaveDataLoadHandler {
    internal static SaveFileInfo GetFileInfo(Game game, string savePath) {
        ContentStorage storage = SaveDataHandlerUtils.OpenStorage(game);
        return new(
            storage.FileExists(savePath),
            SaveDataHandlerUtils.GetBackupFileEntries(storage, savePath).Length
        );
    }

    internal static SaveData Load(Game game, string savePath, int backup) {
        ContentStorage storage = SaveDataHandlerUtils.OpenStorage(game);

        bool singleExists = storage.FileExists(savePath);
        if (backup == -1 && singleExists) { return LoadSaveData(storage, savePath); }

        SaveDataBackupEntry[] backupFiles = SaveDataHandlerUtils.GetBackupFileEntries(storage, savePath);

        if (backupFiles.Length == 0) {
            if (singleExists) { return LoadSaveData(storage, savePath); }
            Log.Info($"SaveData: Save file {savePath} has no primary or backup files, creating new.");
            return new(savePath);
        }

        if (backup < 0 || backup >= backupFiles.Length) {
            int newBackup = Calc.Clamp(backup, 0, backupFiles.Length - 1);
            Log.Info($"SaveData: Backup index {backup} for {savePath} does not exist, loading backup {newBackup} instead. Use Savedata.GetFileInfo to check how many backups exist.");
            backup = newBackup;
        }

        SaveData data = LoadSaveData(storage, backupFiles[backup].FullPath);
        data.SavePath = savePath;
        data.UseBackups = true;
        data.BackupCount = backupFiles.Length;
        return data;
    }

    static SaveData LoadSaveData(ContentStorage storage, string savePath) {
        SaveData saveData = new(savePath);

        Stream textCheck = storage.OpenRead(savePath);
        bool isFileText = IsText(textCheck, out char firstChar);
        textCheck.Close();

        using Stream stream = storage.OpenRead(savePath);
        if (isFileText) { LoadText(stream, saveData, firstChar); }
        else { LoadBinary(stream, saveData); }

        return saveData;
    }

    static void LoadBinary(Stream stream, SaveData saveData) {
        saveData.SaveFormat = SaveData.Format.BINARY;

        using BinaryReader reader = new(stream);
        int version = reader.ReadInt32();
        GetLoaderFromVersion(version)?.LoadBinary(saveData, reader, true);
    }

    static void LoadText(Stream stream, SaveData saveData, char firstChar) {
        switch (firstChar) {
            case '{':
                if (!LoadJSON(stream, saveData)) {
                    Log.Warning($"SaveData: Invalid data for json save file {saveData.SavePath}, could not load");
                }
                break;
        }
    }

    static bool LoadJSON(Stream stream, SaveData saveData) {
        saveData.SaveFormat = SaveData.Format.JSON;

        JsonNode? jsonObj = ReadJsonObject(stream);
        if (jsonObj == null) { return false; }

        JsonNode? version = jsonObj[SaveDataHandlerUtils.VERSION_CONTAINER];
        if (version == null) { return false; }

        GetLoaderFromVersion((int)version)?.LoadJSON(saveData, jsonObj);
        return true;
    }

    static ISaveDataFileVersion? GetLoaderFromVersion(int version) => version switch {
        1 => new SaveDataFileLoaderV1(),
        2 => new SaveDataFileLoaderV2(),
        _ => null
    };

    internal static JsonNode? ReadJsonObject(Stream stream) => JsonNode.Parse(stream);

    /// Adapted from https://stackoverflow.com/a/64038750 and Git's binary check approach
    internal static bool IsText(Stream stream, out char firstChar) {
        const int checkCount = 8000;
        const char nulChar = '\0';
        firstChar = ' ';
        using StreamReader reader = new(stream);
        for (int i = 0; i < checkCount; ++i) {
            if (reader.EndOfStream) { return true; }
            char c = (char)reader.Read();
            if (i == 0) { firstChar = c; }
            if (c == nulChar) { return false; }
        }
        return true;
    }
}