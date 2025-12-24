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
    /// If the data should save backup files when saving.
    /// </summary>
    public bool UseBackups = false;

    /// <summary>
    /// How many backup files to keep alongside the main file. Only used when <see cref="UseBackups"> is enabled.
    /// </summary>
    public int BackupCount = 2;

    /// <summary>
    /// The path to save the file to.
    /// </summary>
    public string SavePath { get; internal set; }

    internal SaveDataNode RootNode { get; private set; } = new();

    internal SaveData(string path) {
        SavePath = path;
    }

    /// <summary>
    /// The root node's stored string values.
    /// </summary>
    public Dictionary<string, string> Strings => RootNode.Strings;

    /// <summary>
    /// The root node's stored float values.
    /// </summary>
    public Dictionary<string, float> Floats => RootNode.Floats;

    /// <summary>
    /// The root node's stored int values.
    /// </summary>
    public Dictionary<string, int> Ints => RootNode.Ints;

    /// <summary>
    /// The root node's stored bool values.
    /// </summary>
    public Dictionary<string, bool> Bools => RootNode.Bools;

    /// <summary>
    /// The root node's stored sub-node values.
    /// </summary>
    public Dictionary<string, SaveDataNode> Children => RootNode.Children;

    /// <summary>
    /// The root node's stored string list values.
    /// </summary>
    public Dictionary<string, List<string>> StringLists => RootNode.StringLists;

    /// <summary>
    /// The root node's stored float list values.
    /// </summary>
    public Dictionary<string, List<float>> FloatLists => RootNode.FloatLists;

    /// <summary>
    /// The root node's stored int list values.
    /// </summary>
    public Dictionary<string, List<int>> IntLists => RootNode.IntLists;

    /// <summary>
    /// The root node's stored bool list values.
    /// </summary>
    public Dictionary<string, List<bool>> BoolLists => RootNode.BoolLists;

    /// <summary>
    /// The root node's stored sub-node list values.
    /// </summary>
    public Dictionary<string, List<SaveDataNode>> ChildrenLists => RootNode.ChildrenLists;

    /// <summary>
    /// Create a new save data storage. <br/>
    /// Automatically populates with data if any exists. Use <see cref="CreateEmpty"/> to ignore existing data.
    /// </summary>
    /// <param name="savePath">The path to using when saving or loading data.</param>
    /// <param name="usesBackups">The save data is stored using backup files.</param>
    /// <returns>The new save data storage.</returns>
    public static SaveData Load(string savePath, bool usesBackups = false) => SaveFileLoader.Load(savePath, usesBackups);

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