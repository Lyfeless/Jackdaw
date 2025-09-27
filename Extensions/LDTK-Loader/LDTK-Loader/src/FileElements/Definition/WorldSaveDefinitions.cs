using System.Text.Json.Serialization;

namespace Jackdaw.Loader.LDTK;

class WorldSaveDefinitions {
    [JsonPropertyName("entities")]
    public EntitySaveDefinition[] Entities { get; set; } = [];

    [JsonPropertyName("enums")]
    public EnumSaveDefinition[] Enums { get; set; } = [];

    [JsonPropertyName("layers")]
    public LayerSaveDefinition[] Layers { get; set; } = [];

    [JsonPropertyName("levelFields")]
    public FieldSaveDefinition[] Fields { get; set; } = [];

    [JsonPropertyName("tilesets")]
    public TilesetSaveDefinition[] Tilesets { get; set; } = [];
}