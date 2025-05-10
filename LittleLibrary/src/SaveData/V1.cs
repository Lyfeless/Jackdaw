namespace LittleLib;

public class SaveDataV1(string savePath) : SaveData(savePath) {
    public SaveDataV1(string savePath, BinaryReader reader) : this(savePath) {
        Load(reader, true);
    }

    public override void Load() {
        BinaryReader? reader = CreateReader(SavePath);
        if (reader == null) {
            Console.WriteLine($"SaveData: Failed to read Save Data at path: {SavePath}");
            return;
        }
        Load(reader);
    }

    void Load(BinaryReader reader, bool skippedVersion = false) {
        // Version
        if (!skippedVersion) { reader.ReadInt32(); }

        // Strings
        int stringCount = reader.ReadInt32();
        for (int i = 0; i < stringCount; ++i) {
            string key = reader.ReadString();
            string value = reader.ReadString();
            Strings.Add(key, value);
        }

        // Floats
        int floatCount = reader.ReadInt32();
        for (int i = 0; i < floatCount; ++i) {
            string key = reader.ReadString();
            float value = reader.ReadSingle();
            Floats.Add(key, value);
        }

        // Ints
        int intCount = reader.ReadInt32();
        for (int i = 0; i < intCount; ++i) {
            string key = reader.ReadString();
            int value = reader.ReadInt32();
            Ints.Add(key, value);
        }

        // Bools
        int boolCount = reader.ReadInt32();
        for (int i = 0; i < boolCount; ++i) {
            string key = reader.ReadString();
            bool value = reader.ReadBoolean();
            Bools.Add(key, value);
        }
    }

    public override void Save() {
        BinaryWriter writer = CreateWriter(SavePath);

        // Version
        writer.Write(1);

        // Strings
        writer.Write(Strings.Count);
        foreach ((string key, string value) in Strings) {
            writer.Write(key);
            writer.Write(value);
        }

        // Floats
        writer.Write(Floats.Count);
        foreach ((string key, float value) in Floats) {
            writer.Write(key);
            writer.Write(value);
        }

        // Ints
        writer.Write(Ints.Count);
        foreach ((string key, int value) in Ints) {
            writer.Write(key);
            writer.Write(value);
        }

        // Bools
        writer.Write(Bools.Count);
        foreach ((string key, bool value) in Bools) {
            writer.Write(key);
            writer.Write(value);
        }
    }
}