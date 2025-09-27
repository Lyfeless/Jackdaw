using System.Text.Json.Serialization;

namespace Jackdaw.Loader.LDTK;

internal class LevelSaveReference {
    [JsonPropertyName("identifier")]
    public string NameID { get; set; } = string.Empty;

    [JsonPropertyName("iid")]
    public string InstanceID { get; set; } = string.Empty;

    [JsonPropertyName("worldX")]
    public int X { get; set; }

    [JsonPropertyName("worldY")]
    public int Y { get; set; }

    [JsonPropertyName("pxWid")]
    public int Width { get; set; }

    [JsonPropertyName("pxHei")]
    public int Height { get; set; }

    [JsonPropertyName("__bgColor")]
    public string BackgroundColor { get; set; } = string.Empty;

    [JsonPropertyName("fieldInstances")]
    public FieldSaveData[] Fields { get; set; } = [];
}