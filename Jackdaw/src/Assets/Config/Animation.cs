using System.Text.Json.Serialization;

namespace Jackdaw;

internal enum AnimationType {
    SPRITESHEET,
    MULTI_TEXTURE
}

internal class AnimationGroupConfig {
    [JsonPropertyName("entries")]
    public AnimationConfigEntry[] Entries { get; set; } = [];
}

internal class AnimationConfigEntry {
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public AnimationConfig Animation { get; set; } = new();
}

internal class AnimationConfig {
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter<AnimationType>))]
    public AnimationType AnimationType { get; set; } = AnimationType.SPRITESHEET;

    [JsonPropertyName("spritesheet")]
    public string Spritesheet { get; set; } = "";

    [JsonPropertyName("textures")]
    public string[] Textures { get; set; } = [];

    // Automatically divide frames into clips of texture 0 if both of these properties are >0, otherwise read from frames
    [JsonPropertyName("horizontalFrames")]
    public int HorizontalFrames { get; set; } = 0;

    [JsonPropertyName("verticalFrames")]
    public int VerticalFrames { get; set; } = 0;

    // Used only if auto-clip is used
    [JsonPropertyName("frameTime")]
    public float FrameTime { get; set; } = 100;

    // Frame count limit, used only if auto-clip is used
    //      exists in case spritesheet has empty tiles at the end of the animation that need to be ignored
    [JsonPropertyName("maxFrameCount")]
    public int MaxFrameCount { get; set; } = 0;

    [JsonPropertyName("frames")]
    public AnimationConfigFrame[] Frames { get; set; } = [];

    [JsonPropertyName("looping")]
    public bool Looping { get; set; } = true;

    [JsonPropertyName("positionOffsetX")]
    public int PositionOffsetX { get; set; } = 0;

    [JsonPropertyName("positionOffsetY")]
    public int PositionOffsetY { get; set; } = 0;
}

internal class AnimationConfigFrame {
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

    [JsonPropertyName("frameX")]
    public int FrameX { get; set; } = 0;

    [JsonPropertyName("frameY")]
    public int FrameY { get; set; } = 0;

    [JsonPropertyName("flipX")]
    public bool FlipX { get; set; } = false;

    [JsonPropertyName("flipY")]
    public bool FlipY { get; set; } = false;

    [JsonPropertyName("embeddedData")]
    public string EmbeddedData { get; set; } = string.Empty;
}