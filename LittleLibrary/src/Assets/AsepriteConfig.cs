using System.Text.Json.Serialization;

namespace LittleLib;

internal class AsepriteConfig {
    [JsonPropertyName("startDelay")]
    public float StartDelay { get; set; } = 0;

    [JsonPropertyName("looping")]
    public bool Looping { get; set; } = true;

    [JsonPropertyName("positionOffsetX")]
    public int PositionOffsetX { get; set; } = 0;

    [JsonPropertyName("positionOffsetY")]
    public int PositionOffsetY { get; set; } = 0;
}