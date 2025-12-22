namespace Jackdaw;

/// <summary>
/// Simple system for storing key/value pairs as save data. Automatically handles saving and loading into a userpath folder. <br/>
/// Create a new save data storage using <see cref="Load"/>
/// </summary>
public class SaveData {
    public enum Format {
        BINARY,
        JSON
    }

    /// <summary>
    /// The save format to use when saving to file.
    /// </summary>
    public Format SaveFormat = Format.BINARY;

    /// <summary>
    /// The path to save the file to.
    /// </summary>
    public string SavePath { get; private set; }
    protected readonly Dictionary<string, string> Strings = [];
    protected readonly Dictionary<string, float> Floats = [];
    protected readonly Dictionary<string, int> Ints = [];
    protected readonly Dictionary<string, bool> Bools = [];

    internal SaveData(string path) {
        SavePath = path;
    }

    /// <summary>
    /// Find a stored string value.
    /// </summary>
    /// <param name="id">The value's assigned name.</param>
    /// <returns>The string assigned to the name, or null if no string value has that name.</returns>
    public string? GetString(string id) => Strings.TryGetValue(id, out string? value) ? value : null;

    /// <summary>
    /// Find a stored float value.
    /// </summary>
    /// <param name="id">The value's assigned name.</param>
    /// <returns>The float assigned to the name, or null if no float value has that name.</returns>
    public float? GetFloat(string id) => Floats.TryGetValue(id, out float value) ? value : null;

    /// <summary>
    /// Find a stored int value.
    /// </summary>
    /// <param name="id">The value's assigned name.</param>
    /// <returns>The int assigned to the name, or null if no int value has that name.</returns>
    public int? GetInt(string id) => Ints.TryGetValue(id, out int value) ? value : null;

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
    /// Set a float in the save data using a name.
    /// </summary>
    /// <param name="id">The value's name.</param>
    /// <param name="value">The float value.</param>
    public void SetFloat(string id, float value) => Floats[id] = value;

    /// <summary>
    /// Set a int in the save data using a name.
    /// </summary>
    /// <param name="id">The value's name.</param>
    /// <param name="value">The int value.</param>
    public void SetInt(string id, int value) => Ints[id] = value;

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
    public string[] StringKeys => [.. Strings.Keys];

    /// <summary>
    /// Get the names of all existing float values.
    /// </summary>
    /// <returns>An array of value names.</returns>
    public string[] FloatKeys => [.. Floats.Keys];

    /// <summary>
    /// Get the names of all existing int values.
    /// </summary>
    /// <returns>An array of value names.</returns>
    public string[] IntKeys => [.. Ints.Keys];

    /// <summary>
    /// Get the names of all existing bool values.
    /// </summary>
    /// <returns>An array of value names.</returns>
    public string[] BoolKeys => [.. Bools.Keys];

    /// <summary>
    /// Get the number of stored string values.
    /// </summary>
    public int StringCount => Strings.Count;

    /// <summary>
    /// Get the number of stored float values.
    /// </summary>
    public int FloatCount => Floats.Count;

    /// <summary>
    /// Get the number of stored int values.
    /// </summary>
    public int IntCount => Ints.Count;

    /// <summary>
    /// Get the number of stored bool values.
    /// </summary>
    public int BoolCount => Bools.Count;

    /// <summary>
    /// Create a new save data storage. <br/>
    /// Automatically populates with data if any exists. Use <see cref="CreateEmpty"/> to ignore existing data.
    /// </summary>
    /// <param name="savePath">The path to using when saving or loading data.</param>
    /// <returns>The new save data storage.</returns>
    public static SaveData Load(string savePath) => SaveFileLoader.Load(savePath);

    /// <summary>
    /// Create a new save data storage, ignoring any data currently stored at the save path.
    /// </summary>
    /// <param name="savePath">The path to using when saving or loading data.</param>
    /// <returns>The new save data storage.</returns>
    public static SaveData CreateEmpty(string savePath) => new(savePath);

    /// <summary>
    /// Save all stored data to file in the save data's <see cref="SaveFormat" />.
    /// </summary>
    public void Save() => SaveFileLoader.Save(this);

    /// <summary>
    /// Save all stored data to file in the given format.
    /// </summary>
    /// <param name="fileFormat">The format to save the file in.</param>
    public void Save(Format fileFormat) => SaveFileLoader.Save(this, fileFormat);
}