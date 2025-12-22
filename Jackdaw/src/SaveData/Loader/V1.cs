using System.Text.Json.Nodes;

namespace Jackdaw;

internal class SaveDataFileLoaderV1 : ISaveDataFileVersion {
    /*
        Layout:
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
            savedata.SetString(key, value);
        }

        // Floats
        int floatCount = reader.ReadInt32();
        for (int i = 0; i < floatCount; ++i) {
            string key = reader.ReadString();
            float value = reader.ReadSingle();
            savedata.SetFloat(key, value);
        }

        // Ints
        int intCount = reader.ReadInt32();
        for (int i = 0; i < intCount; ++i) {
            string key = reader.ReadString();
            int value = reader.ReadInt32();
            savedata.SetInt(key, value);
        }

        // Bools
        int boolCount = reader.ReadInt32();
        for (int i = 0; i < boolCount; ++i) {
            string key = reader.ReadString();
            bool value = reader.ReadBoolean();
            savedata.SetBool(key, value);
        }

        reader.Close();

        return savedata;
    }

    public SaveData LoadJson(SaveData savedata, JsonNode rootNode) {
        JsonObject? strings = rootNode["strings"]?.AsObject();
        if (strings != null) {
            foreach ((string key, JsonNode? node) in strings) {
                if (node == null) { continue; }
                savedata.SetString(key, node.GetValue<string>());
            }
        }

        JsonObject? floats = rootNode["floats"]?.AsObject();
        if (floats != null) {
            foreach ((string key, JsonNode? node) in floats) {
                if (node == null) { continue; }
                savedata.SetFloat(key, node.GetValue<float>());
            }
        }

        JsonObject? ints = rootNode["ints"]?.AsObject();
        if (ints != null) {
            foreach ((string key, JsonNode? node) in ints) {
                if (node == null) { continue; }
                savedata.SetInt(key, node.GetValue<int>());
            }
        }

        JsonObject? bools = rootNode["bools"]?.AsObject();
        if (bools != null) {
            foreach ((string key, JsonNode? node) in bools) {
                if (node == null) { continue; }
                savedata.SetBool(key, node.GetValue<bool>());
            }
        }

        return savedata;
    }

    public void SaveBinary(SaveData savedata) {
        BinaryWriter writer = SaveFileLoader.CreateBinaryWriter(savedata.SavePath);

        // Version
        writer.Write(1);

        // Strings
        string[] strings = savedata.StringKeys;
        writer.Write(strings.Length);
        foreach (string key in strings) {
            writer.Write(key);
            writer.Write(savedata.GetString(key) ?? "");
        }

        // Floats
        string[] floats = savedata.FloatKeys;
        writer.Write(floats.Length);
        foreach (string key in floats) {
            writer.Write(key);
            writer.Write(savedata.GetFloat(key) ?? 0);
        }

        // Ints
        string[] ints = savedata.IntKeys;
        writer.Write(ints.Length);
        foreach (string key in ints) {
            writer.Write(key);
            writer.Write(savedata.GetInt(key) ?? 0);
        }

        // Bools
        string[] bools = savedata.BoolKeys;
        writer.Write(savedata.BoolCount);
        foreach (string key in bools) {
            writer.Write(key);
            writer.Write(savedata.GetBool(key) ?? false);
        }

        writer.Close();
    }

    public void SaveJson(SaveData savedata) {
        JsonObject node = [];
        node["version"] = 1;

        JsonObject strings = [];
        foreach (string key in savedata.StringKeys) {
            strings[key] = savedata.GetString(key);
        }

        JsonObject floats = [];
        foreach (string key in savedata.FloatKeys) {
            floats[key] = savedata.GetFloat(key);
        }

        JsonObject ints = [];
        foreach (string key in savedata.IntKeys) {
            ints[key] = savedata.GetInt(key);
        }

        JsonObject bools = [];
        foreach (string key in savedata.BoolKeys) {
            bools[key] = savedata.GetBool(key);
        }

        node["strings"] = strings;
        node["floats"] = floats;
        node["ints"] = ints;
        node["bools"] = bools;

        SaveFileLoader.WriteJsonObject(node, savedata.SavePath);
    }
}