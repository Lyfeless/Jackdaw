using System.Text.Json.Serialization;

namespace LittleLib.Loader.LDTK;

public class TileTypeSaveTag {
    [JsonPropertyName("enumValueId")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("tileIds")]
    public int[] tileIDs { get; set; } = [];
}