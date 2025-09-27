using System.Text.Json.Serialization;

namespace Jackdaw.Loader.LDTK;

public class TileSaveData {
    [JsonPropertyName("t")]
    public int TileID { get; set; }

    [JsonPropertyName("a")]
    public float Alpha { get; set; }

    [JsonPropertyName("f")]
    public int Flip { get; set; }

    [JsonPropertyName("px")]
    public int[] Position { get; set; } = [0, 0];

    [JsonPropertyName("src")]
    public int[] Source { get; set; } = [0, 0];
}