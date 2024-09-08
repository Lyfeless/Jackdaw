using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;
using Foster.Audio;
using Foster.Framework;

namespace LittleLib;

/// <summary>
/// Global class used for storing and loading game assets including textures, sounds, fonts, etc
/// </summary>
public static class Assets {
    #region Config classes

    public class FontConfig {
        public const int DefaultFontSize = 16;

        [JsonPropertyName("entries")]
        public FontConfigEntry[] FontConfigs { get; set; } = [];
    }

    public class FontConfigEntry {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("size")]
        public int Size { get; set; }
    }

    public class SoundConfig {
        public const SoundLoadingMethod DefaultLoadingMethod = SoundLoadingMethod.Preload;

        [JsonPropertyName("entries")]
        public SoundConfigEntry[] SoundConfigs { get; set; } = [];
    }

    public class SoundConfigEntry {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("loadingMethod")]
        [JsonConverter(typeof(JsonStringEnumConverter<SoundLoadingMethod>))]
        public SoundLoadingMethod LoadingMethod { get; set; } = SoundConfig.DefaultLoadingMethod;
    }

    public class ShaderConfig {

        [JsonPropertyName("entries")]
        public ShaderConfigEntry[] ShaderConfigs { get; set; } = [];
    }

    public class ShaderConfigEntry {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("vertex")]
        public string Vertex { get; set; } = string.Empty;

        [JsonPropertyName("fragment")]
        public string Fragment { get; set; } = string.Empty;

        [JsonPropertyName("attributes")]
        public ShaderConfigAttribute[] Attributes { get; set; } = [];
    }

    public class ShaderConfigAttribute {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("index")]
        public int Index { get; set; } = 0;
    }

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
        public float DefaultFrametime { get; set; } = 100;

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

    #endregion

    #region File/path definitions

    public const string AssetPath = "Content";
    public const string TextureFolder = "Textures";
    public const string FontFolder = "Fonts";
    public const string SoundFolder = "Sounds";
    public const string ShaderFolder = "Shaders";

    public const string ConfigName = "Config.json";

    public const string AnimationDataName = "AnimData.json";

    const string FallbackFolder = "Fallback";

    public static readonly string[] TextureExtensions = [".png", ".jpg"];
    public static readonly string[] FontExtensions = [".ttf", ".otf", ".fnt"];
    public static readonly string[] SoundExtensions = [".wav", ".mp3", ".ogg"];
    public const string ShaderExtension = ".hlsl";

    #endregion

    #region Assets storage and accessors

    static readonly Dictionary<string, Subtexture> Textures = [];
    public static Subtexture GetTexture(string name) => Textures.TryGetValue(name, out Subtexture output) ? output : Textures["error"];

    static readonly Dictionary<string, SpriteFont> Fonts = [];
    public static SpriteFont GetFont(string name) => Fonts.TryGetValue(name, out SpriteFont? output) ? output : Fonts["error"];

    static readonly Dictionary<string, Shader> Shaders = [];
    public static Shader GetShader(string name) => Shaders.TryGetValue(name, out Shader? output) ? output : Shaders["error"];

    static readonly Dictionary<string, AnimationData> Animations = [];
    public static AnimationData GetAnimation(string name) => Animations.TryGetValue(name, out AnimationData? output) ? output : Animations["error"];


    static readonly Dictionary<string, Sound> Sounds = [];
    public static Sound GetSound(string name) => Sounds.TryGetValue(name, out Sound? output) ? output : Sounds["error"];

    #endregion

    #region Default shader

    static readonly Shader FallbackShader = new(
        // This data is grabbed directly from the internal foster code
        new(
            // Vertex
            @"
                #version 330
                uniform mat4 u_matrix;
                layout(location=0) in vec2 a_position;
                layout(location=1) in vec2 a_tex;
                layout(location=2) in vec4 a_color;
                layout(location=3) in vec4 a_type;
                out vec2 v_tex;
                out vec4 v_col;
                out vec4 v_type;
                void main(void)
                {
                    gl_Position = u_matrix * vec4(a_position.xy, 0, 1);
                    v_tex = a_tex;
                    v_col = a_color;
                    v_type = a_type;
                }
            ",
            // Fragment
            @"
                #version 330
				uniform sampler2D u_texture;
				in vec2 v_tex;
				in vec4 v_col;
				in vec4 v_type;
				out vec4 o_color;
				void main(void)
				{
					vec4 color = texture(u_texture, v_tex);
					o_color =
						v_type.x * color * v_col +
						v_type.y * color.a * v_col +
						v_type.z * v_col;
				}
            "
        )
    );

    #endregion

    /// <summary>
    /// Load and initialize all asset types
    /// </summary>
    public static void Init() {
        // Textures
        {
            // Create asset packer
            Packer packer = new() {
                Trim = false,
                CombineDuplicates = false,
                Padding = 1
            };

            // Load fallback texture
            packer.Add("error", Path.Join(FallbackFolder, "texture.png"));

            // Load all textures in asset directory
            string texturePath = Path.Join(AssetPath, TextureFolder);
            if (Directory.Exists(texturePath)) {

                foreach (string file in Directory.EnumerateFiles(texturePath, "*.*", SearchOption.AllDirectories).Where(e => TextureExtensions.Any(e.EndsWith))) {
                    string name = GetAssetName(texturePath, file);
                    packer.Add(name, file);
                }
            }

            // Run packer on processed textures
            var output = packer.Pack();
            List<Texture> pages = [];

            foreach (var page in output.Pages) {
                // page.Premultiply();
                pages.Add(new Texture(page));
            }

            foreach (var entry in output.Entries) {
                Textures.Add(entry.Name, new Subtexture(pages[entry.Page], entry.Source, entry.Frame));
            }
        }

        // Fonts
        {
            // Load fallback font
            Fonts.Add("error", new SpriteFont(Path.Join(FallbackFolder, "font.ttf"), 16));

            // Load all fonts in asset directory
            string fontPath = Path.Join(AssetPath, FontFolder);
            if (Directory.Exists(fontPath)) {
                string configPath = Path.Join(fontPath, ConfigName);
                FontConfig? config = Path.Exists(configPath) ? JsonSerializer.Deserialize(File.ReadAllText(configPath), SourceGenerationContext.Default.FontConfig) : null;

                foreach (string file in Directory.EnumerateFiles(fontPath, "*.*", SearchOption.AllDirectories).Where(e => FontExtensions.Any(e.EndsWith))) {
                    string name = GetAssetName(fontPath, file);
                    FontConfigEntry? configEntry = config?.FontConfigs.FirstOrDefault(e => e.Name == name);
                    Fonts.Add(name, new SpriteFont(file, configEntry?.Size ?? FontConfig.DefaultFontSize));
                }
            }
        }

        // Sounds
        {
            //! FIXME (Alex): Load config for sound settings, mostly for sounds that should be streamed

            // Load fallback sound
            Sounds.Add("error", new Sound(Path.Join(FallbackFolder, "sound.ogg")));

            string soundPath = Path.Join(AssetPath, SoundFolder);
            if (Directory.Exists(soundPath)) {
                string configPath = Path.Join(soundPath, ConfigName);
                SoundConfig? config = Path.Exists(configPath) ? JsonSerializer.Deserialize(File.ReadAllText(configPath), SourceGenerationContext.Default.SoundConfig) : null;

                foreach (string file in Directory.EnumerateFiles(soundPath, "*.*", SearchOption.AllDirectories).Where(e => SoundExtensions.Any(e.EndsWith))) {
                    string name = GetAssetName(soundPath, file);
                    SoundConfigEntry? configEntry = config?.SoundConfigs.FirstOrDefault(e => e.Name == name);
                    Sounds.Add(name, new Sound(file, configEntry?.LoadingMethod ?? SoundConfig.DefaultLoadingMethod));
                }
            }
        }

        // Shaders
        {
            // Load fallback shader
            //! FIXME (Alex): I could see an argument for just crashing if a shader doesn't load correctly, might be harder to identify bugs if it doesn't crash on fail
            Shaders.Add("error", FallbackShader);

            // Load shaders from config
            //      Shaders are built from more data than just files, so read directly off the config
            string shaderPath = Path.Join(AssetPath, ShaderFolder);
            string shaderConfigPath = Path.Join(shaderPath, ConfigName);
            if (Path.Exists(shaderConfigPath)) {
                ShaderConfig? config = JsonSerializer.Deserialize(File.ReadAllText(shaderConfigPath), SourceGenerationContext.Default.ShaderConfig);

                if (config != null) {
                    foreach (ShaderConfigEntry entry in config.ShaderConfigs) {
                        // Find files from config entry
                        string vertexPath = Path.Join(shaderPath, entry.Vertex + ShaderExtension);
                        string fragmentPath = Path.Join(shaderPath, entry.Fragment + ShaderExtension);
                        if (!Path.Exists(vertexPath) || !Path.Exists(fragmentPath)) { continue; }

                        // Build shader info
                        string vertexData = File.ReadAllText(vertexPath);
                        string fragmentData = File.ReadAllText(fragmentPath);
                        ShaderCreateInfo createInfo = new(
                            vertexData,
                            fragmentData,
                            (entry.Attributes?.Length ?? 0) == 0
                                ? []
                                : entry.Attributes?.Select<ShaderConfigAttribute, ShaderCreateInfo.Attribute>(e => new(e.Name, e.Index)).ToArray()
                        );

                        // Add shader to dict
                        Shaders.Add(entry.Name, new Shader(createInfo));
                    }
                }
            }
        }

        // Animation Data
        {
            Animations.Add("error", new([Point2.Zero], Point2.One));
            string path = Path.Join(AssetPath, AnimationDataName);
            if (Path.Exists(path)) {
                AnimationConfig? config = JsonSerializer.Deserialize(File.ReadAllText(path), SourceGenerationContext.Default.AnimationConfig);
                if (config != null) {
                    foreach (AnimationConfigEntry entry in config.AnimationConfigs) {
                        if (entry.Frames == null) {
                            Animations.Add(entry.Name, new(entry.HorizontalFrames, entry.VerticalFrames, new(entry.FrameWidth, entry.FrameHeight)) {
                                DefaultFrametime = entry.DefaultFrametime,
                                DefaultLooping = entry.DefaultLooping,
                                DefaultPositionOffset = new(entry.DefaultOffset.X, entry.DefaultOffset.Y),
                                DefaultStartDelay = entry.DefaultStartDelay
                            });
                        }
                        else {
                            Animations.Add(entry.Name, new([.. entry.Frames.Select(e => new Point2(e.X, e.Y))], new(entry.FrameWidth, entry.FrameHeight)) {
                                DefaultFrametime = entry.DefaultFrametime,
                                DefaultLooping = entry.DefaultLooping,
                                DefaultPositionOffset = new(entry.DefaultOffset.X, entry.DefaultOffset.Y),
                                DefaultStartDelay = entry.DefaultStartDelay
                            });
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Remove extra information from a file path to create a unique asset identifier
    /// </summary>
    /// <param name="relativePath">The path elements to remove from the start of the path</param>
    /// <param name="assetPath">Full path to the asset</param>
    /// <returns></returns>
    static string GetAssetName(string relativePath, string assetPath) {
        string name = Path.Join(Path.GetDirectoryName(assetPath), Path.GetFileNameWithoutExtension(assetPath));
        name = Path.GetRelativePath(relativePath, name);
        name = name.Replace("\\", "/");
        name = name.Replace("/", "/");

        return name;
    }
}