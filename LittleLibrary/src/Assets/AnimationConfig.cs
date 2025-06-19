using System.Text.Json.Serialization;

namespace LittleLib;

internal class AnimationGroupConfig {
    [JsonPropertyName("entries")]
    public AnimationConfigEntry[] Entries { get; set; } = [];
}

internal class AnimationConfigEntry {
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public AnimationConfig Animation = new();
}

//! FIXME (Alex): These should NOT be public, handle them somewhere before passing the DATA to animations
public class AnimationConfig {
    [JsonPropertyName("textures")]
    public string[] Textures = [];

    // Automatically divide frames into clips of texture 0 if both of these properties are >0, otherwise read from frames
    [JsonPropertyName("horizontalFrames")]
    public int HorizontalFrames { get; set; } = 0;

    [JsonPropertyName("verticalFrames")]
    public int VerticalFrames { get; set; } = 0;

    // Used only if auto-clip is used
    [JsonPropertyName("frameTime")]
    public float FrameTime { get; set; } = 100;

    [JsonPropertyName("frames")]
    public AnimationConfigFrame[] Frames { get; set; } = [];

    [JsonPropertyName("startDelay")]
    public float StartDelay { get; set; } = 0;

    [JsonPropertyName("looping")]
    public bool Looping { get; set; } = false;

    [JsonPropertyName("positionOffsetX")]
    public int PositionOffsetX { get; set; } = 0;

    [JsonPropertyName("positionOffsetY")]
    public int PositionOffsetY { get; set; } = 0;
}

public class AnimationConfigFrame {
    [JsonPropertyName("texture")]
    public int Texture { get; set; } = 0;

    [JsonPropertyName("duration")]
    public int Duration { get; set; } = 100;

    [JsonPropertyName("positionOffsetX")]
    public int PositionOffsetX { get; set; } = 0;

    [JsonPropertyName("positionOffsetY")]
    public int PositionOffsetY { get; set; } = 0;

    [JsonPropertyName("clipX")]
    public int ClipX { get; set; } = 0;

    [JsonPropertyName("clipY")]
    public int ClipY { get; set; } = 0;

    [JsonPropertyName("clipWidth")]
    public int ClipWidth { get; set; } = 0;

    [JsonPropertyName("clipHeight")]
    public int ClipHeight { get; set; } = 0;
}