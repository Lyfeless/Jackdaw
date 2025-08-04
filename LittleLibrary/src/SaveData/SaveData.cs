namespace LittleLib;

public abstract class SaveData {
    protected string SavePath;

    public SaveData(string path) {
        SavePath = path;
    }

    protected readonly Dictionary<string, string> Strings = [];
    protected readonly Dictionary<string, int> Ints = [];
    protected readonly Dictionary<string, float> Floats = [];
    protected readonly Dictionary<string, bool> Bools = [];

    public string? GetString(string id) => Strings.TryGetValue(id, out string? value) ? value : null;
    public int? GetInt(string id) => Ints.TryGetValue(id, out int value) ? value : null;
    public float? GetFloat(string id) => Floats.TryGetValue(id, out float value) ? value : null;
    public bool? GetBool(string id) => Bools.TryGetValue(id, out bool value) ? value : null;

    public void SetString(string id, string value) => Strings[id] = value;
    public void SetInt(string id, int value) => Ints[id] = value;
    public void SetFloat(string id, float value) => Floats[id] = value;
    public void SetBool(string id, bool value) => Bools[id] = value;

    public string[] StringKeys() => [.. Strings.Keys];
    public string[] IntKeys() => [.. Ints.Keys];
    public string[] FloatKeys() => [.. Floats.Keys];
    public string[] BoolKeys() => [.. Bools.Keys];

    public abstract void Load();

    public abstract void Save();

    public static SaveData? LoadByVersion(string savePath) {
        BinaryReader? reader = CreateReader(savePath);
        if (reader == null) {
            Console.WriteLine($"SaveData: Failed to read Save Data at path: {savePath}");
            return null;
        }
        int version = reader.ReadInt32();
        return version switch {
            1 => new SaveDataV1(savePath, reader),
            _ => null
        };
    }

    protected static BinaryReader? CreateReader(string savePath) {
        if (!File.Exists(savePath)) { return null; }
        return new(File.Open(savePath, FileMode.Open));
    }

    protected static BinaryWriter CreateWriter(string savePath) {
        return new(File.Open(savePath, FileMode.OpenOrCreate));
    }
}