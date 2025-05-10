using System.Text.Json.Serialization;

namespace LittleLib.Loader.LDTK;

public class LayerSaveData {
    [JsonPropertyName("__type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("__identifier")]
    public string NameID { get; set; } = string.Empty;

    [JsonPropertyName("iid")]
    public string InstanceID { get; set; } = string.Empty;

    [JsonPropertyName("__cWid")]
    public int Width { get; set; }

    [JsonPropertyName("__cHei")]
    public int Height { get; set; }

    [JsonPropertyName("__gridSize")]
    public int TileSize { get; set; }

    [JsonPropertyName("__tilesetDefUid")]
    public int? Tileset { get; set; }

    [JsonPropertyName("__pxTotalOffsetX")]
    public int OffsetX { get; set; }

    [JsonPropertyName("__pxTotalOffsetY")]
    public int OffsetY { get; set; }

    [JsonPropertyName("autoLayerTiles")]
    public TileSaveData[] AutoTiles { get; set; } = [];

    [JsonPropertyName("gridTiles")]
    public TileSaveData[] Tiles { get; set; } = [];

    [JsonPropertyName("entityInstances")]
    public EntitySaveData[] Entities { get; set; } = [];
}