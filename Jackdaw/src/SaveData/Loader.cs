using System.Text.Json;
using System.Text.Json.Nodes;
using Foster.Framework;

namespace Jackdaw;

internal static class SaveFileLoader {
    public const string VERSION_CONTAINER = "version";

    record struct DecomposedPath(string Directory, string Name, string Extension);
    record struct FileBackupEntry(string FullPath, string Name, string FullName, string Extension, long Timestamp);

    public static SaveData Load(string savePath, int backup) {
        bool singleExists = File.Exists(savePath);
        if (backup == -1 && singleExists) { return LoadSaveData(savePath); }

        FileBackupEntry[] backupFiles = GetBackupFileEntries(savePath);

        if (backupFiles.Length == 0) {
            if (singleExists) { return LoadSaveData(savePath); }
            Log.Info($"Save file {savePath} has no primary or backup files, creating new.");
            return new(savePath);
        }

        if (backup < 0 || backup >= backupFiles.Length) {
            int newBackup = Calc.Clamp(backup, 0, backupFiles.Length - 1);
            Log.Info($"SaveData: Backup index {backup} for {savePath} does not exist, loading backup {newBackup} instead. Use Savedata.GetFileInfo to check how many backups exist.");
            backup = newBackup;
        }

        SaveData data = LoadSaveData(backupFiles[backup].FullPath);
        data.SavePath = savePath;
        data.UseBackups = true;
        data.BackupCount = backupFiles.Length;
        return data;
    }

    public static SaveFileInfo GetFileInfo(string savePath) => new(
        File.Exists(savePath),
        GetBackupFileEntries(savePath).Length
    );

    static SaveData LoadSaveData(string savePath) {
        if (!File.Exists(savePath)) { return new(savePath); }
        if (IsText(savePath, out char firstChar)) { return LoadText(savePath, firstChar); }
        return LoadBinary(savePath);
    }

    public static void Save(SaveData saveData) => Save(saveData, saveData.SaveFormat);

    public static void Save(SaveData saveData, SaveData.Format format) {
        string savePath = saveData.SavePath;
        if (saveData.UseBackups) {
            DecomposedPath pathInfo = DecomposePath(saveData.SavePath);
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            savePath = Path.Join(pathInfo.Directory, $"{pathInfo.Name}-{timestamp}{pathInfo.Extension}");

            // Remove old paths
            FileBackupEntry[] backupFiles = GetBackupFileEntries(saveData.SavePath);
            int diff = backupFiles.Length - saveData.BackupCount + 1;
            if (diff > 0) {
                for (int i = 0; i < diff; ++i) {
                    File.Delete(backupFiles[^(1 + i)].FullPath);
                }
            }
        }

        SaveDataFileLoaderV2 loader = new();
        switch (format) {
            case SaveData.Format.BINARY: loader.SaveBinary(saveData, savePath); break;
            case SaveData.Format.JSON: loader.SaveJson(saveData, savePath); break;
        }
    }

    static SaveData LoadBinary(string savePath) {
        BinaryReader? reader = CreateBinaryReader(savePath);
        if (reader == null) {
            Log.Warning($"SaveData: Failed to read data at path {savePath}, could not load");
            return new(savePath);
        }

        int version = reader.ReadInt32();

        SaveData data = new(savePath);
        GetLoaderFromVersion(version)?.LoadBinary(data, reader, true);
        return data;
    }

    static SaveData LoadText(string savePath, char firstChar) => firstChar switch {
        '{' => LoadJSON(savePath),
        _ => new(savePath),
    };

    static SaveData LoadJSON(string savePath) {
        string json = File.ReadAllText(savePath);

        JsonNode? jsonObj = JsonNode.Parse(json);
        if (jsonObj == null) { return JsonDataInvalid(savePath); }

        JsonNode? version = jsonObj[VERSION_CONTAINER];
        if (version == null) { return JsonDataInvalid(savePath); }

        SaveData data = new(savePath);
        GetLoaderFromVersion((int)version)?.LoadJSON(data, jsonObj);
        return data;
    }

    static SaveData JsonDataInvalid(string savePath) {
        Log.Warning($"SaveData: Invalid data for json save file {savePath}, could not load");
        return new(savePath);
    }

    static ISaveDataFileVersion? GetLoaderFromVersion(int version) => version switch {
        1 => new SaveDataFileLoaderV1(),
        2 => new SaveDataFileLoaderV2(),
        _ => null
    };

    /// Adapted from https://stackoverflow.com/a/64038750 and Git's binary check approach
    internal static bool IsText(string path, out char firstChar) {
        const int checkCount = 8000;
        const char nulChar = '\0';
        firstChar = ' ';
        using StreamReader reader = new(path);
        for (int i = 0; i < checkCount; ++i) {
            if (reader.EndOfStream) { return true; }
            char c = (char)reader.Read();
            if (i == 0) { firstChar = c; }
            if (c == nulChar) { return false; }
        }
        return true;
    }

    internal static BinaryReader? CreateBinaryReader(string savePath) {
        if (!File.Exists(savePath)) { return null; }
        return new(File.Open(savePath, FileMode.Open));
    }

    internal static BinaryWriter CreateBinaryWriter(string savePath) {
        FileStream file = File.Open(savePath, FileMode.OpenOrCreate);
        file.SetLength(0);
        return new(file);
    }


    internal static void WriteJsonObject(JsonNode node, string path) {
        using FileStream file = new(path, FileMode.OpenOrCreate);
        file.SetLength(0);

        using Utf8JsonWriter writer = new(file);
        node.WriteTo(writer);
    }

    static DecomposedPath DecomposePath(string path) => new(
        Path.GetDirectoryName(path) ?? string.Empty,
        Path.GetFileNameWithoutExtension(path),
        Path.GetExtension(path)
    );

    static FileBackupEntry[] GetBackupFileEntries(string savePath) {
        DecomposedPath path = DecomposePath(savePath);
        return GetBackupFileEntries(path);
    }

    static FileBackupEntry[] GetBackupFileEntries(DecomposedPath path) {
        List<FileBackupEntry> entries = [];
        foreach (string file in Directory.EnumerateFiles(path.Directory, $"*{path.Extension}", SearchOption.TopDirectoryOnly)) {
            string name = Path.GetFileNameWithoutExtension(file);
            if (!name.StartsWith(path.Name)) { continue; }

            string[] segments = name.Split('-');
            if (segments.Length < 2 || !long.TryParse(segments[^1], out long fileTime)) { continue; }

            entries.Add(new FileBackupEntry(
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