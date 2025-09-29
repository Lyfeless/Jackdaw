using System.Text.Json.Serialization;

namespace Jackdaw.Loader.LDTK;

class TileTypeCustomData {
    [JsonPropertyName("tileId")]
    public int ID { get; set; } = 0;

    [JsonPropertyName("data")]
    public string Data { get; set; } = string.Empty;
}