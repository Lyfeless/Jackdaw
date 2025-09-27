using System.Text.Json.Serialization;

namespace Jackdaw;

/// <summary>
/// All configuration data for creating a game.
/// </summary>
public struct GameConfig() {
    /// <summary>
    /// The name of the application. Also used when saving data to the userpath.
    /// </summary>
    [JsonPropertyName("applicationName")]
    public string ApplicationName { get; set; } = "JackdawGame";

    /// <summary>
    /// The title to display on the window.
    /// </summary>
    [JsonPropertyName("windowTitle")]
    public string WindowTitle { get; set; } = "Jackdaw Game";

    /// <summary>
    /// Configuration data for the window.
    /// </summary>
    [JsonPropertyName("window")]
    public GameWindowConfig Window { get; set; } = new();

    /// <summary>
    /// Configuration data for the asset loader.
    /// </summary>
    [JsonPropertyName("content")]
    public GameContentConfig Content { get; set; } = new();
}

/// <summary>
/// All configuration data related to the game window.
/// </summary>
public struct GameWindowConfig() {
    /// <summary>
    /// Rendering configuration options.
    /// </summary>
    public enum RendererType {
        /// <summary>
        /// Scene tree is rendered directly to the screen, 1 window pixel => 1 ingame unit.
        /// </summary>
        FULL_WINDOW,

        /// <summary>
        /// Scene is letterboxed to keep a consistent resolution and aspect ratio.
        /// </summary>
        FIXED_VIEWPORT
    }

    /// <summary>
    /// Display method for rending the scene tree to the window.
    /// </summary>
    [JsonPropertyName("Renderer")]
    [JsonConverter(typeof(JsonStringEnumConverter<RendererType>))]
    public RendererType Renderer { get; set; } = RendererType.FIXED_VIEWPORT;

    /// <summary>
    /// Initial width of the full window.
    /// </summary>
    [JsonPropertyName("windowWidth")]
    public int WindowWidth { get; set; } = 600;

    /// <summary>
    /// Initial height of the full window.
    /// </summary>
    [JsonPropertyName("windowHeight")]
    public int WindowHeight { get; set; } = 600;

    /// <summary>
    /// Whether or not the window can be resized by the user.
    /// </summary>
    [JsonPropertyName("resizable")]
    public bool Resizable { get; set; } = true;

    /// <summary>
    /// The background color of the window. <br/>
    /// Used as the letterbox color when using fixed viewport.
    /// </summary>
    [JsonPropertyName("clearColor")]
    public string ClearColor { get; set; } = "0x000000";
}

/// <summary>
/// All configuration data related to asset loading.
/// </summary>
public struct GameContentConfig() {
    /// <summary>
    /// The root folder all asset data is stored in.<br/>
    /// Defaults to "Content".
    /// </summary>
    [JsonPropertyName("rootFolder")]
    public string RootFolder { get; set; } = "Content";

    /// <summary>
    /// The folder to search for texture files. <br/>
    /// Defaults to "Textures".
    /// Relative to root folder.
    /// </summary>
    [JsonPropertyName("textureFolder")]
    public string TextureFolder { get; set; } = "Textures";

    /// <summary>
    /// The folder to search for font files. <br/>
    /// Defaults to "Fonts".
    /// Relative to root folder.
    /// </summary>
    [JsonPropertyName("fontFolder")]
    public string FontFolder { get; set; } = "Fonts";

    /// <summary>
    /// The location of the font config data.
    /// Defaults to "Fonts/config.json".
    /// Relative to root folder.
    /// </summary>
    [JsonPropertyName("fontConfig")]
    public string FontConfig { get; set; } = "Fonts/config.json";

    /// <summary>
    /// The folder to search for shader files. <br/>
    /// Defaults to "Shaders".
    /// Relative to root folder.
    /// </summary>
    [JsonPropertyName("shaderFolder")]
    public string ShaderFolder { get; set; } = "Shaders";

    /// <summary>
    /// The location of the shader config data.
    /// Defaults to "Shaders/config.json".
    /// Relative to root folder.
    /// </summary>
    [JsonPropertyName("shaderConfig")]
    public string ShaderConfig { get; set; } = "Shaders/config.json";

    /// <summary>
    /// The folder to search for animation files. <br/>
    /// Defaults to "Animations".
    /// Relative to root folder.
    /// </summary>
    [JsonPropertyName("animationFolder")]
    public string AnimationFolder { get; set; } = "Animations";

    /// <summary>
    /// The file extension for single animations.
    /// Defaults to ".llanim".
    /// </summary>
    [JsonPropertyName("animationExtension")]
    public string AnimationExtension { get; set; } = ".llanim";

    /// <summary>
    /// The file extension for groups of animations.
    /// Defaults to ".llganim".
    /// </summary>
    [JsonPropertyName("animationGroupExtension")]
    public string AnimationGroupExtension { get; set; } = ".llganim";

    /// <summary>
    /// The file extension for aseprite sidecar files.
    /// Defaults to ".asedef".
    /// </summary>
    [JsonPropertyName("asepriteConfigExtension")]
    public string AsepriteConfigExtension { get; set; } = ".asedef";
}