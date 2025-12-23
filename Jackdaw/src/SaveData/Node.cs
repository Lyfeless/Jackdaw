namespace Jackdaw;

public class SaveDataNode {
    /// <summary>
    /// The node's stored string values.
    /// </summary>
    public readonly Dictionary<string, string> Strings = [];

    /// <summary>
    /// The node's stored float values.
    /// </summary>
    public readonly Dictionary<string, float> Floats = [];

    /// <summary>
    /// The node's stored int values.
    /// </summary>
    public readonly Dictionary<string, int> Ints = [];

    /// <summary>
    /// The node's stored bool values.
    /// </summary>
    public readonly Dictionary<string, bool> Bools = [];

    /// <summary>
    /// The node's stored sub-node values.
    /// </summary>
    public readonly Dictionary<string, SaveDataNode> Children = [];

    /// <summary>
    /// The node's stored string list values.
    /// </summary>
    public readonly Dictionary<string, List<string>> StringLists = [];

    /// <summary>
    /// The node's stored float list values.
    /// </summary>
    public readonly Dictionary<string, List<float>> FloatLists = [];

    /// <summary>
    /// The node's stored int list values.
    /// </summary>
    public readonly Dictionary<string, List<int>> IntLists = [];

    /// <summary>
    /// The node's stored bool list values.
    /// </summary>
    public readonly Dictionary<string, List<bool>> BoolLists = [];

    /// <summary>
    /// The node's stored sub-node list values.
    /// </summary>
    public readonly Dictionary<string, List<SaveDataNode>> ChildrenLists = [];
}