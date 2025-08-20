using System.Text.Json.Serialization;

namespace LittleLib.Loader.LDTK;

public class EntitySaveData {
    [JsonPropertyName("__identifier")]
    public string NameID { get; set; } = string.Empty;

    [JsonPropertyName("iid")]
    public string InstanceID { get; set; } = string.Empty;

    [JsonPropertyName("px")]
    public int[] Position { get; set; } = [0, 0];

    [JsonPropertyName("__worldX")]
    public int? WorldX { get; set; }

    [JsonPropertyName("__worldY")]
    public int? WorldY { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("fieldInstances")]
    public FieldSaveData[] Fields { get; set; } = [];
}