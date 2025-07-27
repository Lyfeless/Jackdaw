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

    [JsonPropertyName("frameData")]
    public AsepriteFrameConfig[] FrameData { get; set; } = [];
}

internal class AsepriteFrameConfig {
    [JsonPropertyName("frameNumber")]
    public int Frame { get; set; } = 0;

    [JsonPropertyName("flipX")]
    public bool FlipX { get; set; } = false;

    [JsonPropertyName("flipY")]
    public bool FlipY { get; set; } = false;

    [JsonPropertyName("embeddedData")]
    public string EmbeddedData { get; set; } = string.Empty;
}