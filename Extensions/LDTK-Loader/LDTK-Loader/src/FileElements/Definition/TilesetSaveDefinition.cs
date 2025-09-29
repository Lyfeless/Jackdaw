using System.Text.Json.Serialization;

namespace Jackdaw.Loader.LDTK;

class TilesetSaveDefinition {
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
    public TileTypeCustomData[] CustomData { get; set; } = [];
}