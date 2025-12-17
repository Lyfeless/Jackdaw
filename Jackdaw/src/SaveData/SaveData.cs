using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Simple system for storing key/value pairs as save data. Automatically handles saving and loading into a userpath folder.
/// </summary>
/// <param name="path">The file path to read the data from.</param>
public abstract class SaveData(string path) {
    protected string SavePath = path;
    protected readonly Dictionary<string, string> Strings = [];
    protected readonly Dictionary<string, int> Ints = [];
    protected readonly Dictionary<string, float> Floats = [];
    protected readonly Dictionary<string, bool> Bools = [];

    /// <summary>
    /// Find a stored string value.
    /// </summary>
    /// <param name="id">The value's assigned name.</param>
    /// <returns>The string assigned to the name, or null if no string value has that name.</returns>
    public string? GetString(string id) => Strings.TryGetValue(id, out string? value) ? value : null;

    /// <summary>
    /// Find a stored int value.
    /// </summary>
    /// <param name="id">The value's assigned name.</param>
    /// <returns>The int assigned to the name, or null if no int value has that name.</returns>
    public int? GetInt(string id) => Ints.TryGetValue(id, out int value) ? value : null;

    /// <summary>
    /// Find a stored float value.
    /// </summary>
    /// <param name="id">The value's assigned name.</param>
    /// <returns>The float assigned to the name, or null if no float value has that name.</returns>
    public float? GetFloat(string id) => Floats.TryGetValue(id, out float value) ? value : null;

    /// <summary>
    /// Find a stored bool value.
    /// </summary>
    /// <param name="id">The value's assigned name.</param>
    /// <returns>The bool assigned to the name, or null if no bool value has that name.</returns>
    public bool? GetBool(string id) => Bools.TryGetValue(id, out bool value) ? value : null;

    /// <summary>
    /// Set a string in the save data using a name.
    /// </summary>
    /// <param name="id">The value's name.</param>
    /// <param name="value">The string value.</param>
    public void SetString(string id, string value) => Strings[id] = value;

    /// <summary>
    /// Set a int in the save data using a name.
    /// </summary>
    /// <param name="id">The value's name.</param>
    /// <param name="value">The int value.</param>
    public void SetInt(string id, int value) => Ints[id] = value;

    /// <summary>
    /// Set a float in the save data using a name.
    /// </summary>
    /// <param name="id">The value's name.</param>
    /// <param name="value">The float value.</param>
    public void SetFloat(string id, float value) => Floats[id] = value;

    /// <summary>
    /// Set a bool in the save data using a name.
    /// </summary>
    /// <param name="id">The value's name.</param>
    /// <param name="value">The bool value.</param>
    public void SetBool(string id, bool value) => Bools[id] = value;

    /// <summary>
    /// Get the names of all existing string values.
    /// </summary>
    /// <returns>An array of value names.</returns>
    public string[] StringKeys() => [.. Strings.Keys];

    /// <summary>
    /// Get the names of all existing int values.
    /// </summary>
    /// <returns>An array of value names.</returns>
    public string[] IntKeys() => [.. Ints.Keys];

    /// <summary>
    /// Get the names of all existing float values.
    /// </summary>
    /// <returns>An array of value names.</returns>
    public string[] FloatKeys() => [.. Floats.Keys];

    /// <summary>
    /// Get the names of all existing bool values.
    /// </summary>
    /// <returns>An array of value names.</returns>
    public string[] BoolKeys() => [.. Bools.Keys];

    /// <summary>
    /// Load savedata from the the assigned location. <br/>
    /// It's not recommended to use this function directly,
    /// use <see cref="LoadByVersion" /> to get a loader with the correct version for the save data.
    /// </summary>
    public abstract void Load();

    /// <summary>
    /// Save all stored data to file.
    /// </summary>
    public abstract void Save();

    /// <summary>
    /// Determine the save data version and create the appropriate manager.
    /// </summary>
    /// <param name="savePath">The path to read from, relative to the program's save location.</param>
    /// <returns>A new savedata instance, or null if the file can't load.</returns>
    public static SaveData? LoadByVersion(string savePath) {
        BinaryReader? reader = CreateReader(savePath);
        if (reader == null) {
            Log.Warning($"SaveData: Failed to read Save Data at path: {savePath}");
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