using System.Text.Json;
using System.Text.Json.Serialization;

namespace LittleLib;

public struct LittleGameConfig() {
    [JsonPropertyName("applicationName")]
    public string ApplicationName = "LittleGame";
    [JsonPropertyName("windowTitle")]
    public string WindowTitle = "Little Game";

    [JsonPropertyName("viewWidth")]
    public int ViewWidth = 352;
    [JsonPropertyName("viewHeight")]
    public int ViewHeight = 256;

    [JsonPropertyName("screenWidth")]
    public int ScreenWidth = 600;
    [JsonPropertyName("screenHeight")]
    public int ScreenHeight = 600;

    [JsonPropertyName("content")]
    public LittleGameContentConfig Content = new();
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