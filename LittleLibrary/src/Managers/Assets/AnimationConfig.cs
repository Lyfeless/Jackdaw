using System.Text.Json.Serialization;

namespace LittleLib;

public class AnimationConfig {
    [JsonPropertyName("entries")]
    public AnimationConfigEntry[] AnimationConfigs { get; set; } = [];
}

public class AnimationConfigEntry {
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("frameWidth")]
    public int FrameWidth { get; set; } = 1;

    [JsonPropertyName("frameHeight")]
    public int FrameHeight { get; set; } = 1;

    [JsonPropertyName("frames")]
    public AnimationConfigFrame[] Frames { get; set; } = [];

    [JsonPropertyName("horizontalFrames")]
    public int HorizontalFrames { get; set; } = 1;

    [JsonPropertyName("verticalFrames")]
    public int VerticalFrames { get; set; } = 0;

    [JsonPropertyName("defaultFrameTime")]
    public float DefaultFrameTime { get; set; } = 100;

    [JsonPropertyName("defaultStartDelay")]
    public float DefaultStartDelay { get; set; } = 0;

    [JsonPropertyName("defaultLooping")]
    public bool DefaultLooping { get; set; } = false;

    [JsonPropertyName("defaultOffset")]
    public AnimationDefaultOffset DefaultOffset { get; set; } = new() { X = 0, Y = 0 };
}

public class AnimationConfigFrame {
    [JsonPropertyName("x")]
    public int X { get; set; } = 0;

    [JsonPropertyName("y")]
    public int Y { get; set; } = 0;
}

public class AnimationDefaultOffset {
    [JsonPropertyName("x")]
    public int X { get; set; } = 0;

    [JsonPropertyName("y")]
    public int Y { get; set; } = 0;
}