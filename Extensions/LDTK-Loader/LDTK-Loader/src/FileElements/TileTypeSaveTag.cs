using System.Text.Json.Serialization;

namespace Jackdaw.Loader.LDTK;

public class TileTypeSaveTag {
    [JsonPropertyName("enumValueId")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("tileIds")]
    public int[] tileIDs { get; set; } = [];
}