using System.Text.Json;
using System.Text.Json.Nodes;
using Foster.Framework;

namespace Jackdaw;

internal static class SaveFileLoader {
    public static SaveData Load(string savePath) {
        if (!File.Exists(savePath)) {
            Log.Warning($"SaveData: Failed to find save data at path {savePath}, could not load");
            return new(savePath);
        }
        if (IsText(savePath, out char firstChar)) { return LoadText(savePath, firstChar); }
        return LoadBinary(savePath);
    }

    public static void Save(SaveData saveData) => Save(saveData, saveData.SaveFormat);

    public static void Save(SaveData saveData, SaveData.Format format) {
        SaveDataFileLoaderV1 loader = new();
        switch (format) {
            case SaveData.Format.BINARY: loader.SaveBinary(saveData); break;
            case SaveData.Format.JSON: loader.SaveJson(saveData); break;
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

        JsonNode? version = jsonObj["version"];
        if (version == null) { return JsonDataInvalid(savePath); }

        SaveData data = new(savePath);
        GetLoaderFromVersion((int)version)?.LoadJson(data, jsonObj);
        return data;
    }

    static SaveData JsonDataInvalid(string savePath) {
        Log.Warning($"SaveData: Invalid data for json save file {savePath}, could not load");
        return new(savePath);
    }

    static ISaveDataFileVersion? GetLoaderFromVersion(int version) => version switch {
        1 => new SaveDataFileLoaderV1(),
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
        return new(File.Open(savePath, FileMode.OpenOrCreate));
    }


    internal static void WriteJsonObject(JsonNode node, string path) {
        using FileStream file = new(path, FileMode.OpenOrCreate);
        using Utf8JsonWriter writer = new(file);
        node.WriteTo(writer);
    }
}