using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// All configuration data for creating a game.
/// </summary>
public struct GameConfig() {
    /// <summary>
    /// The name of the application. Also used when saving data to the userpath.
    /// </summary>
    public string ApplicationName = "JackdawGame";

    /// <summary>
    /// Configuration data for the window.
    /// </summary>
    public GameWindowConfig Window = new();

    /// <summary>
    /// Configuration data for the asset loader.
    /// </summary>
    public GameContentConfig Content = new();
}

/// <summary>
/// All configuration data related to the game window.
/// </summary>
public struct GameWindowConfig() {
    /// <summary>
    /// The title to display on the window.
    /// </summary>
    public string Title = "Jackdaw Game";

    /// <summary>
    /// Initial size of the full window.
    /// </summary>
    public Point2 Size = new(600, 600);

    /// <summary>
    /// Whether or not the window can be resized by the user.
    /// </summary>
    public bool Resizable = true;

    /// <summary>
    /// The background color of the window. <br/>
    /// Used as the letterbox color when using fixed viewport.
    /// </summary>
    public Color ClearColor = Color.Black;
}

/// <summary>
/// All configuration data related to asset loading.
/// </summary>
public struct GameContentConfig() {
    /// <summary>
    /// The source to load assets from. <br/>
    /// Defaults to loading assets from folders in the root folder "Content".
    /// </summary>
    public IAssetProvider AssetProvider = new FileFolderAssetProvider("Content");

    /// <summary>
    /// Any custom content loaders to run when initializing assets. <br/>
    /// If left empty only textures, animations, fonts and shaders will be loaded.
    /// </summary>
    public AssetLoaderStage[] CustomAssetLoaders = [];

    /// <summary>
    /// The provider group name to search for texture files. <br/>
    /// Defaults to "Textures".
    /// </summary>
    public string TextureGroup = "Textures";

    /// <summary>
    /// The provider group to search for font files. <br/>
    /// Defaults to "Fonts".
    /// </summary>
    public string FontGroup = "Fonts";

    /// <summary>
    /// The name of the font config data. <br/>
    /// Must be stored inside the font group to be found. <br/>
    /// Defaults to "config.json".
    /// </summary>
    public string FontConfig = "config.json";

    /// <summary>
    /// The provider group to search for shader files. <br/>
    /// Defaults to "Shaders".
    /// </summary>
    public string ShaderGroup = "Shaders";

    /// <summary>
    /// The name of the shader config data. <br/>
    /// Must be stored inside the shader group to be found. <br/>
    /// Defaults to "config.json".
    /// </summary>
    public string ShaderConfig = "config.json";

    /// <summary>
    /// The provider group to search for animation files. <br/>
    /// Defaults to "Animations".
    /// </summary>
    public string AnimationGroup = "Animations";

    /// <summary>
    /// The file extension for single animations.
    /// Defaults to ".jda".
    /// </summary>
    public string AnimationExtension = ".jda";

    /// <summary>
    /// The file extension for groups of animations.
    /// Defaults to ".jdga".
    /// </summary>
    public string AnimationGroupExtension = ".jdga";

    /// <summary>
    /// The file extension for aseprite sidecar files.
    /// Defaults to ".asedef".
    /// </summary>
    public string AsepriteConfigExtension = ".asedef";
}