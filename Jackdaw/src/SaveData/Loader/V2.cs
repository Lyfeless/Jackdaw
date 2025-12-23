using System.Text.Json;
using System.Text.Json.Nodes;

namespace Jackdaw;

internal class SaveDataFileLoaderV2 : ISaveDataFileVersion {
    /*
        Binary Layout:
            int32: Version (already read if skipVersion is true)
            int32: String Count
            for String Count value:
                string: String Entry Key
                string: String Entry Value
            int32: String Array Count
            for String Array Count value:
                string: String Array Element Key
                int32: String Array Element Count
                for String Array Element Count value:
                    string: String Array Element Entry Value
            int32: Float Count
            for Float Count value:
                string: Float Entry Key
                float: Float Entry Value
            int32: Float Array Count
            for Float Array Count value:
                string: Float Array Element Key
                int32: Float Array Element Count
                for Float Array Element Count value:
                    float: Float Array Element Entry Value
            int32: Int Count
            for Int Count value:
                string: Int Entry Key
                int32: Int Entry Value
            int32: Int Array Count
            for Int Array Count value:
                string: Int Array Element Key
                int32: Int Array Element Count
                for Int Array Element Count value:
                    int32: Int Array Element Entry Value
            int32: Bool Count
            for Bool Count value:
                string: Bool Entry Key
                bool: Bool Entry Value
            int32: Bool Array Count
            for Bool Array Count value:
                string: Bool Array Element Key
                int32: Bool Array Element Count
                for Bool Array Element Count value:
                    bool: Bool Array Element Entry Value
            int32: Sub-Node Count
            for Sub-Node Count value:
                Parse sub-node as String Count onwards
            int32: Sub-Node Array Count
            for Sub-Node Array Count value:
                string: Sub-Node Array Element Key
                int32: Sub-Node Array Element Count
                for Sub-Node Array Element Count value:
                    Parse sub-node as String Count onwards
    */

    const string STRING_CONTAINER = "strings";
    const string STRING_ARRAY_CONTAINER = "stringlists";
    const string FLOAT_CONTAINER = "floats";
    const string FLOAT_ARRAY_CONTAINER = "floatlists";
    const string INT_CONTAINER = "ints";
    const string INT_ARRAY_CONTAINER = "intlists";
    const string BOOL_CONTAINER = "bools";
    const string BOOL_ARRAY_CONTAINER = "boollists";
    const string CHILD_CONTAINER = "children";
    const string CHILD_ARRAY_CONTAINER = "childrenlists";

    #region Binary reading
    public SaveData LoadBinary(SaveData savedata, BinaryReader reader, bool skippedVersion = true) {
        if (!skippedVersion) { reader.ReadInt32(); }

        ReadNodeBinary(savedata.RootNode, reader);

        reader.Close();
        return savedata;
    }

    static void ReadNodeBinary(SaveDataNode savedata, BinaryReader reader) {
        ReadElementBinary(reader, savedata.Strings, savedata.StringLists, reader.ReadString);
        ReadElementBinary(reader, savedata.Floats, savedata.FloatLists, reader.ReadSingle);
        ReadElementBinary(reader, savedata.Ints, savedata.IntLists, reader.ReadInt32);
        ReadElementBinary(reader, savedata.Bools, savedata.BoolLists, reader.ReadBoolean);
        ReadElementBinary(reader, savedata.Children, savedata.ChildrenLists, () => ReadNewNodeBinary(reader));
    }

    static SaveDataNode ReadNewNodeBinary(BinaryReader reader) {
        SaveDataNode node = new();
        ReadNodeBinary(node, reader);
        return node;
    }

    static void ReadElementBinary<T>(BinaryReader reader, Dictionary<string, T> singleDict, Dictionary<string, List<T>> arrayDict, Func<T> readFunc) {
        ReadSingleElementBinary(reader, singleDict, readFunc);
        ReadArrayElementBinary(reader, arrayDict, readFunc);
    }

    static void ReadSingleElementBinary<T>(BinaryReader reader, Dictionary<string, T> dict, Func<T> readFunc) {
        int count = reader.ReadInt32();
        for (int i = 0; i < count; ++i) {
            string key = reader.ReadString();
            T value = readFunc();
            dict.Add(key, value);
        }
    }

    static void ReadArrayElementBinary<T>(BinaryReader reader, Dictionary<string, List<T>> dict, Func<T> readFunc)
        => ReadSingleElementBinary(reader, dict, () => ArrayElementBinaryReadFunc(reader, readFunc));

    static List<T> ArrayElementBinaryReadFunc<T>(BinaryReader reader, Func<T> readFunc) {
        List<T> elements = [];
        int count = reader.ReadInt32();
        for (int j = 0; j < count; ++j) {
            T value = readFunc();
            elements.Add(value);
        }
        return elements;
    }
    #endregion

    #region JSON reading
    public SaveData LoadJSON(SaveData savedata, JsonNode rootNode) {
        ReadNodeJSON(savedata.RootNode, rootNode);
        return savedata;
    }

    static void ReadNodeJSON(SaveDataNode savedata, JsonNode rootNode) {
        ReadElementJSON(rootNode, STRING_CONTAINER, STRING_ARRAY_CONTAINER, savedata.Strings, savedata.StringLists, e => e.GetValue<string>());
        ReadElementJSON(rootNode, FLOAT_CONTAINER, FLOAT_ARRAY_CONTAINER, savedata.Floats, savedata.FloatLists, e => e.GetValue<float>());
        ReadElementJSON(rootNode, INT_CONTAINER, INT_ARRAY_CONTAINER, savedata.Ints, savedata.IntLists, e => e.GetValue<int>());
        ReadElementJSON(rootNode, BOOL_CONTAINER, BOOL_ARRAY_CONTAINER, savedata.Bools, savedata.BoolLists, e => e.GetValue<bool>());
        ReadElementJSON(rootNode, CHILD_CONTAINER, CHILD_ARRAY_CONTAINER, savedata.Children, savedata.ChildrenLists, e => ReadNewNodeJSON(e));
    }

    static SaveDataNode ReadNewNodeJSON(JsonNode rootNode) {
        SaveDataNode node = new();
        ReadNodeJSON(node, rootNode);
        return node;
    }

    static void ReadElementJSON<T>(JsonNode rootNode, string singleElement, string arrayElement, Dictionary<string, T> singleDict, Dictionary<string, List<T>> arrayDict, Func<JsonNode, T> readFunc) {
        ReadSingleElementJSON(rootNode, singleElement, singleDict, readFunc);
        ReadArrayElementJSON(rootNode, arrayElement, arrayDict, readFunc);
    }

    static void ReadSingleElementJSON<T>(JsonNode rootNode, string element, Dictionary<string, T> dict, Func<JsonNode, T> readFunc) {
        JsonObject? node = rootNode[element]?.AsObject();
        if (node == null) { return; }
        foreach ((string key, JsonNode? value) in node) {
            if (value == null) { continue; }
            dict[key] = readFunc(value);
        }
    }

    static void ReadArrayElementJSON<T>(JsonNode rootNode, string element, Dictionary<string, List<T>> dict, Func<JsonNode, T> readFunc)
        => ReadSingleElementJSON(rootNode, element, dict, e => ArrayElementJSONReadFunc(e, readFunc));

    static List<T> ArrayElementJSONReadFunc<T>(JsonNode node, Func<JsonNode, T> readFunc) {
        List<T> elements = [];
        foreach (JsonNode? value in node.AsArray()) {
            if (value == null) { continue; }
            elements.Add(readFunc(value));
        }
        return elements;
    }
    #endregion

    #region Binary saving
    public void SaveBinary(SaveData savedata) {
        using BinaryWriter writer = SaveFileLoader.CreateBinaryWriter(savedata.SavePath);

        // Version
        writer.Write(2);

        SaveNodeBinary(savedata.RootNode, writer);
    }

    static void SaveNodeBinary(SaveDataNode savedata, BinaryWriter writer) {
        WriteElementBinary(writer, savedata.Strings, savedata.StringLists, writer.Write);
        WriteElementBinary(writer, savedata.Floats, savedata.FloatLists, writer.Write);
        WriteElementBinary(writer, savedata.Ints, savedata.IntLists, writer.Write);
        WriteElementBinary(writer, savedata.Bools, savedata.BoolLists, writer.Write);
        WriteElementBinary(writer, savedata.Children, savedata.ChildrenLists, e => { SaveNodeBinary(e, writer); });
    }

    static void WriteElementBinary<T>(BinaryWriter writer, Dictionary<string, T> singleDict, Dictionary<string, List<T>> arrayDict, Action<T> writeFunc) {
        WriteSingleElementBinary(writer, singleDict, writeFunc);
        WriteArrayElementBinary(writer, arrayDict, writeFunc);
    }

    static void WriteSingleElementBinary<T>(BinaryWriter writer, Dictionary<string, T> dict, Action<T> writeFunc) {
        writer.Write(dict.Count);
        foreach ((string key, T value) in dict) {
            writer.Write(key);
            writeFunc(value);
        }
    }

    static void WriteArrayElementBinary<T>(BinaryWriter writer, Dictionary<string, List<T>> dict, Action<T> writeFunc) {
        WriteSingleElementBinary(writer, dict, e => { ArrayElementBinaryWriteFunc(writer, e, writeFunc); });
    }

    static void ArrayElementBinaryWriteFunc<T>(BinaryWriter writer, List<T> values, Action<T> writeFunc) {
        writer.Write(values.Count);
        foreach (T element in values) {
            writeFunc(element);
        }
    }
    #endregion

    #region JSON saving
    public void SaveJson(SaveData savedata) {
        JsonNode node = SaveNodeJson(savedata.RootNode);
        node[SaveFileLoader.VERSION_CONTAINER] = 2;
        SaveFileLoader.WriteJsonObject(node, savedata.SavePath);
    }

    static JsonNode SaveNodeJson(SaveDataNode savedata) {
        JsonObject rootNode = [];
        WriteElementJSON(rootNode, STRING_CONTAINER, STRING_ARRAY_CONTAINER, savedata.Strings, savedata.StringLists, e => (JsonNode)e);
        WriteElementJSON(rootNode, FLOAT_CONTAINER, FLOAT_ARRAY_CONTAINER, savedata.Floats, savedata.FloatLists, e => (JsonNode)e);
        WriteElementJSON(rootNode, INT_CONTAINER, INT_ARRAY_CONTAINER, savedata.Ints, savedata.IntLists, e => (JsonNode)e);
        WriteElementJSON(rootNode, BOOL_CONTAINER, BOOL_ARRAY_CONTAINER, savedata.Bools, savedata.BoolLists, e => (JsonNode)e);
        WriteElementJSON(rootNode, CHILD_CONTAINER, CHILD_ARRAY_CONTAINER, savedata.Children, savedata.ChildrenLists, SaveNodeJson);
        return rootNode;
    }

    static void WriteElementJSON<T>(JsonObject rootNode, string singleElement, string arrayElement, Dictionary<string, T> singleDict, Dictionary<string, List<T>> arrayDict, Func<T, JsonNode> writeFunc) {
        WriteSingleElementJSON(rootNode, singleElement, singleDict, writeFunc);
        WriteArrayElementJSON(rootNode, arrayElement, arrayDict, writeFunc);
    }

    static void WriteSingleElementJSON<T>(JsonObject rootNode, string element, Dictionary<string, T> dict, Func<T, JsonNode> writeFunc) {
        if (dict.Count == 0) { return; }
        JsonObject node = [];
        foreach ((string key, T value) in dict) {
            node[key] = writeFunc(value);
        }
        rootNode[element] = node;
    }

    static void WriteArrayElementJSON<T>(JsonObject rootNode, string element, Dictionary<string, List<T>> dict, Func<T, JsonNode> writeFunc)
        => WriteSingleElementJSON(rootNode, element, dict, e => { return ArrayElementJSONWriteFunc(rootNode, e, writeFunc); });

    static JsonNode ArrayElementJSONWriteFunc<T>(JsonObject rootNode, List<T> values, Func<T, JsonNode> writeFunc) {
        JsonArray node = [];
        foreach (T element in values) {
            node.Add(writeFunc(element));
        }
        return node;
    }
    #endregion
}