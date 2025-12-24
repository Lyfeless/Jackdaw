using System.Text.Json.Nodes;

namespace Jackdaw;

internal class SaveDataFileLoaderV1 : ISaveDataFileVersion {
    /*
        Binary Layout:
            int32: Version (already read if skipVersion is true)
            int32: String Count
            for String Count value:
                string: String Entry Key
                string: String Entry Value
            int32: Float Count
            for Float Count value:
                string: Float Entry Key
                float: Float Entry Value
            int32: Int Count
            for Int Count value:
                string: Int Entry Key
                int32: Int Entry Value
            int32: Bool Count
            for Bool Count value:
                string: Bool Entry Key
                bool: Bool Entry Value
    */

    public SaveData LoadBinary(SaveData savedata, BinaryReader reader, bool skippedVersion = true) {
        // Version
        if (!skippedVersion) { reader.ReadInt32(); }

        // Strings
        int stringCount = reader.ReadInt32();
        for (int i = 0; i < stringCount; ++i) {
            string key = reader.ReadString();
            string value = reader.ReadString();
            savedata.Strings.Add(key, value);
        }

        // Floats
        int floatCount = reader.ReadInt32();
        for (int i = 0; i < floatCount; ++i) {
            string key = reader.ReadString();
            float value = reader.ReadSingle();
            savedata.Floats.Add(key, value);
        }

        // Ints
        int intCount = reader.ReadInt32();
        for (int i = 0; i < intCount; ++i) {
            string key = reader.ReadString();
            int value = reader.ReadInt32();
            savedata.Ints.Add(key, value);
        }

        // Bools
        int boolCount = reader.ReadInt32();
        for (int i = 0; i < boolCount; ++i) {
            string key = reader.ReadString();
            bool value = reader.ReadBoolean();
            savedata.Bools.Add(key, value);
        }

        reader.Close();

        return savedata;
    }

    public SaveData LoadJSON(SaveData savedata, JsonNode rootNode) {
        JsonObject? strings = rootNode["strings"]?.AsObject();
        if (strings != null) {
            foreach ((string key, JsonNode? node) in strings) {
                if (node == null) { continue; }
                savedata.Strings[key] = node.GetValue<string>();
            }
        }

        JsonObject? floats = rootNode["floats"]?.AsObject();
        if (floats != null) {
            foreach ((string key, JsonNode? node) in floats) {
                if (node == null) { continue; }
                savedata.Floats[key] = node.GetValue<float>();
            }
        }

        JsonObject? ints = rootNode["ints"]?.AsObject();
        if (ints != null) {
            foreach ((string key, JsonNode? node) in ints) {
                if (node == null) { continue; }
                savedata.Ints[key] = node.GetValue<int>();
            }
        }

        JsonObject? bools = rootNode["bools"]?.AsObject();
        if (bools != null) {
            foreach ((string key, JsonNode? node) in bools) {
                if (node == null) { continue; }
                savedata.Bools[key] = node.GetValue<bool>();
            }
        }

        return savedata;
    }

    public void SaveBinary(SaveData savedata, string savePath) {
        BinaryWriter writer = SaveFileLoader.CreateBinaryWriter(savePath);

        // Version
        writer.Write(1);

        // Strings
        writer.Write(savedata.Strings.Count);
        foreach ((string key, string value) in savedata.Strings) {
            writer.Write(key);
            writer.Write(value);
        }

        // Floats
        writer.Write(savedata.Floats.Count);
        foreach ((string key, float value) in savedata.Floats) {
            writer.Write(key);
            writer.Write(value);
        }

        // Ints
        writer.Write(savedata.Ints.Count);
        foreach ((string key, int value) in savedata.Ints) {
            writer.Write(key);
            writer.Write(value);
        }

        // Bools
        writer.Write(savedata.Bools.Count);
        foreach ((string key, bool value) in savedata.Bools) {
            writer.Write(key);
            writer.Write(value);
        }

        writer.Close();
    }

    public void SaveJson(SaveData savedata, string savePath) {
        JsonObject node = [];
        node[SaveFileLoader.VERSION_CONTAINER] = 1;

        JsonObject strings = [];
        foreach ((string key, string value) in savedata.Strings) {
            strings[key] = value;
        }

        JsonObject floats = [];
        foreach ((string key, float value) in savedata.Floats) {
            strings[key] = value;
        }

        JsonObject ints = [];
        foreach ((string key, int value) in savedata.Ints) {
            strings[key] = value;
        }

        JsonObject bools = [];
        foreach ((string key, bool value) in savedata.Bools) {
            strings[key] = value;
        }

        node["strings"] = strings;
        node["floats"] = floats;
        node["ints"] = ints;
        node["bools"] = bools;

        SaveFileLoader.WriteJsonObject(node, savePath);
    }
}