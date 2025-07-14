using System.Text.Json.Serialization;

namespace LittleLib;

public struct LittleGameConfig() {
    [JsonPropertyName("applicationName")]
    public string ApplicationName { get; set; } = "LittleGame";

    [JsonPropertyName("windowTitle")]
    public string WindowTitle { get; set; } = "Little Game";

    [JsonPropertyName("window")]
    public LittleGameWindowConfig Window { get; set; } = new();

    [JsonPropertyName("content")]
    public LittleGameContentConfig Content { get; set; } = new();

    [JsonPropertyName("audio")]
    public LittleGameAudioConfig Audio { get; set; } = new();
}

public struct LittleGameWindowConfig() {
    public enum RendererType {
        FULL_WINDOW,
        FIXED_VIEWPORT
    }

    [JsonPropertyName("Renderer")]
    [JsonConverter(typeof(JsonStringEnumConverter<RendererType>))]
    public RendererType Renderer { get; set; } = RendererType.FIXED_VIEWPORT;

    [JsonPropertyName("windowWidth")]
    public int WindowWidth { get; set; } = 600;
    [JsonPropertyName("windowHeight")]
    public int WindowHeight { get; set; } = 600;
    [JsonPropertyName("clearColor")]
    public string ClearColor { get; set; } = "0x000000";

    // FIXED_VIEWPORT specific options
    [JsonPropertyName("viewportWidth")]
    public int ViewportWidth { get; set; } = 352;
    [JsonPropertyName("viewportHeight")]
    public int ViewportHeight { get; set; } = 256;
    //? Taking the color from foster's cornflower blue
    [JsonPropertyName("viewportColor")]
    public string ViewportColor { get; set; } = "0x6495ed";
}

public struct LittleGameContentConfig() {
    [JsonPropertyName("rootFolder")]
    public string RootFolder { get; set; } = "Content";

    [JsonPropertyName("textureFolder")]
    public string TextureFolder { get; set; } = "Textures";

    [JsonPropertyName("fontFolder")]
    public string FontFolder { get; set; } = "Fonts";

    [JsonPropertyName("fontConfig")]
    public string FontConfig { get; set; } = "Fonts/config.json";

    [JsonPropertyName("soundFolder")]
    public string SoundFolder { get; set; } = "Sounds";

    [JsonPropertyName("soundConfig")]
    public string SoundConfig { get; set; } = "Sounds/config.json";

    [JsonPropertyName("shaderFolder")]
    public string ShaderFolder { get; set; } = "Shaders";

    [JsonPropertyName("shaderConfig")]
    public string ShaderConfig { get; set; } = "Shaders/config.json";

    [JsonPropertyName("animationFolder")]
    public string AnimationFolder { get; set; } = "Animations";

    [JsonPropertyName("animationExtension")]
    public string AnimationExtension { get; set; } = ".llanim";

    [JsonPropertyName("animationGroupExtension")]
    public string AnimationGroupExtension { get; set; } = ".llganim";

    [JsonPropertyName("asepriteConfigExtension")]
    public string AsepriteConfigExtension { get; set; } = ".asedef";
}

public struct LittleGameAudioConfig() {
    [JsonPropertyName("buses")]
    public LittleGameAudioBusConfig[] Buses { get; set; } = [];

    [JsonPropertyName("defaultBus")]
    public string DefaultBus { get; set; } = string.Empty;
}

public struct LittleGameAudioBusConfig() {
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("parent")]
    public string Parent { get; set; } = string.Empty;

    [JsonPropertyName("defaultVolume")]
    public float DefaultVolume { get; set; } = 0.5f;
}