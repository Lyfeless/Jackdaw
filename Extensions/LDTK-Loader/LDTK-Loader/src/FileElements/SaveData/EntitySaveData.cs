using System.Text.Json.Serialization;

namespace Jackdaw.Loader.LDTK;

/// <summary>
/// Save data for an instance of an entity.
/// </summary>
public class EntitySaveData {
    /// <summary>
    /// The name of the entity's type
    /// </summary>
    [JsonPropertyName("__identifier")]
    public string NameID { get; set; } = string.Empty;

    /// <summary>
    /// The entity's unique instance identifier.
    /// </summary>
    [JsonPropertyName("iid")]
    public string InstanceID { get; set; } = string.Empty;

    /// <summary>
    /// The entity's position, with x as index 0 and y as index 1.
    /// </summary>
    [JsonPropertyName("px")]
    public int[] Position { get; set; } = [0, 0];

    /// <summary>
    /// The entity's x position in world space.
    /// </summary>
    [JsonPropertyName("__worldX")]
    public int? WorldX { get; set; }

    /// <summary>
    /// The entity's y position in world space.
    /// </summary>
    [JsonPropertyName("__worldY")]
    public int? WorldY { get; set; }

    /// <summary>
    /// The entity's width.
    /// </summary>
    [JsonPropertyName("width")]
    public int Width { get; set; }

    /// <summary>
    /// The entity's height.
    /// </summary>
    [JsonPropertyName("height")]
    public int Height { get; set; }

    /// <summary>
    /// All data stored in custom fields on the entity. <br/>
    /// Use <seealso cref="LDTKFieldGetter"/> to get variable data.
    /// </summary>
    [JsonPropertyName("fieldInstances")]
    public FieldSaveData[] Fields { get; set; } = [];
}