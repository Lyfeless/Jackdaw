using System.Drawing;
using System.Text.Json.Serialization;

namespace LittleLib;

public struct LittleGameConfig() {
    [JsonPropertyName("applicationName")]
    public string ApplicationName = "LittleGame";
    [JsonPropertyName("windowTitle")]
    public string WindowTitle = "Little Game";
    [JsonPropertyName("window")]
    public LittleGameWindowConfig Window = new();
    [JsonPropertyName("content")]
    public LittleGameContentConfig Content = new();
}

public struct LittleGameWindowConfig() {
    public enum RendererType {
        FULL_WINDOW,
        FIXED_VIEWPORT
    }

    public RendererType Renderer = RendererType.FIXED_VIEWPORT;

    [JsonPropertyName("windowWidth")]
    public int WindowWidth = 600;
    [JsonPropertyName("windowHeight")]
    public int WindowHeight = 600;
    [JsonPropertyName("clearColor")]
    public string ClearColor = "0xffffff";

    // FIXED_VIEWPORT specific options
    [JsonPropertyName("viewportWidth")]
    public int ViewportWidth = 352;
    [JsonPropertyName("viewportHeight")]
    public int ViewportHeight = 256;
    //? Taking the color from foster's cornflower blue
    [JsonPropertyName("viewportColor")]
    public string ViewportColor = "0x6495ed";
}

public struct LittleGameContentConfig() {
    [JsonPropertyName("rootFolder")]
    public string RootFolder = "Content";

    [JsonPropertyName("textureFolder")]
    public string TextureFolder = "Textures";

    [JsonPropertyName("fontFolder")]
    public string FontFolder = "Fonts";

    [JsonPropertyName("fontConfig")]
    public string FontConfig = "Fonts/config.json";

    [JsonPropertyName("soundFolder")]
    public string SoundFolder = "Sounds";

    [JsonPropertyName("soundConfig")]
    public string SoundConfig = "Sounds/config.json";

    [JsonPropertyName("shaderFolder")]
    public string ShaderFolder = "Shaders";

    [JsonPropertyName("shaderConfig")]
    public string ShaderConfig = "Shaders/config.json";

    //! FIXME (Alex): Maybe animations should be split into seperate files instead? Would that be easier to read?
    [JsonPropertyName("animationConfig")]
    public string AnimationConfig = "Animations/config.json";
}