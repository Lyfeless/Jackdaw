using System.Text.Json.Serialization;

namespace LittleLib.Loader.LDTK;

public class TilesetSaveDefinition {
    [JsonPropertyName("uid")]
    public int DefinitionID { get; set; }

    [JsonPropertyName("identifier")]
    public string Identifier { get; set; } = string.Empty;

    [JsonPropertyName("relPath")]
    public string TexturePath { get; set; } = string.Empty;

    [JsonPropertyName("__cWid")]
    public int TileCountX { get; set; }

    [JsonPropertyName("__cHei")]
    public int TileCountY { get; set; }

    [JsonPropertyName("tileGridSize")]
    public int GridSize { get; set; }

    [JsonPropertyName("padding")]
    public int Padding { get; set; }

    [JsonPropertyName("enumTags")]
    public TileTypeSaveTag[] TileTypes { get; set; } = [];

    [JsonPropertyName("customData")]
    public TilesetCustomData[] CustomData { get; set; } = [];
}

public class TilesetCustomData {
    [JsonPropertyName("tileId")]
    public int ID { get; set; } = 0;

    [JsonPropertyName("data")]
    public string Data { get; set; } = string.Empty;
}