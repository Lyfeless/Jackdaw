using System.Text.Json.Serialization;

namespace LittleLib.Loader.LDTK;

public class TileTypeCustomData {
    [JsonPropertyName("tileId")]
    public int ID { get; set; } = 0;

    [JsonPropertyName("data")]
    public string Data { get; set; } = string.Empty;
}