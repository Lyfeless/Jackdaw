using System.Reflection;
using System.Text.Json;
using Foster.Audio;
using Foster.Framework;

namespace LittleLib;

/// <summary>
/// Manager class used for storing and loading game assets including textures, sounds, fonts, etc
/// </summary>
public class Assets {
    #region File/path definitions
    public LittleGameContentConfig Config;

    public readonly string[] TextureExtensions = [".png", ".jpg"];
    public readonly string[] FontExtensions = [".ttf", ".otf", ".fnt"];
    public readonly string[] SoundExtensions = [".wav", ".mp3", ".ogg"];

    #endregion

    #region Assets storage and accessors
    readonly Dictionary<string, Subtexture> Textures = [];
    public Subtexture GetTexture(string name) {
        if (Textures.TryGetValue(name, out Subtexture output)) { return output; }
        Console.WriteLine($"ASSETS: Failed to find texture {name}, returning default");
        return Textures["error"];
    }
    const string TextureFallbackName = "Fallback.texture.png";

    readonly Dictionary<string, SpriteFont> Fonts = [];
    public SpriteFont GetFont(string name) {
        if (Fonts.TryGetValue(name, out SpriteFont? output)) { return output; }
        Console.WriteLine($"ASSETS: Failed to find font {name}, returning default");
        return Fonts["error"];
    }
    const string FontFallbackName = "Fallback.font.ttf";

    readonly Dictionary<string, Shader> Shaders = [];
    public Shader GetShader(string name) {
        if (Shaders.TryGetValue(name, out Shader? output)) { return output; }
        Console.WriteLine($"ASSETS: Failed to find shader {name}, returning default");
        return Shaders["error"];
    }

    //! FIXME (Alex): Animations not re-implemented
    // readonly Dictionary<string, AnimationData> Animations = [];
    // public AnimationData GetAnimation(string name) => Animations.TryGetValue(name, out AnimationData? output) ? output : Animations["error"];


    readonly Dictionary<string, Sound> Sounds = [];
    public Sound GetSound(string name) {
        if (Sounds.TryGetValue(name, out Sound? output)) { return output; }
        Console.WriteLine($"ASSETS: Failed to find sound {name}, returning default");
        return Sounds["error"];
    }
    const string SoundFallbackName = "Fallback.sound.ogg";

    #endregion

    /// <summary>
    /// Load and initialize all asset types
    /// </summary>
    public Assets(GraphicsDevice graphicsDevice, LittleGameContentConfig config) {
        Config = config;

        // Assembly data for fallback
        Assembly assembly = Assembly.GetExecutingAssembly();
        string? assemblyName = assembly.GetName().Name;

        // Textures
        {
            // Create asset packer
            Packer packer = new() {
                Trim = false,
                CombineDuplicates = false,
                Padding = 1
            };

            // Load fallback texture
            using Stream stream = assembly.GetManifestResourceStream($"{assemblyName}.{TextureFallbackName}")!;
            packer.Add("error", new Image(stream));

            // Load all textures in asset directory
            string texturePath = Path.Join(Config.RootFolder, Config.TextureFolder);
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
                pages.Add(new Texture(graphicsDevice, page));
            }

            foreach (var entry in output.Entries) {
                Textures.Add(entry.Name, new Subtexture(pages[entry.Page], entry.Source, entry.Frame));
            }
        }

        // Fonts
        {
            // Load fallback font
            using Stream stream = assembly.GetManifestResourceStream($"{assemblyName}.{FontFallbackName}")!;
            Fonts.Add("error", new SpriteFont(graphicsDevice, stream, 16));

            // Load all fonts in asset directory
            string fontPath = Path.Join(Config.RootFolder, Config.FontFolder);
            if (Directory.Exists(fontPath)) {
                string configPath = Path.Join(Config.RootFolder, Config.RootFolder);
                FontConfig? fontConfig = Path.Exists(configPath) ? JsonSerializer.Deserialize(File.ReadAllText(configPath), SourceGenerationContext.Default.FontConfig) : null;

                foreach (string file in Directory.EnumerateFiles(fontPath, "*.*", SearchOption.AllDirectories).Where(e => FontExtensions.Any(e.EndsWith))) {
                    string name = GetAssetName(fontPath, file);
                    FontConfigEntry? configEntry = fontConfig?.FontConfigs.FirstOrDefault(e => e.Name == name);
                    Fonts.Add(name, new SpriteFont(graphicsDevice, file, configEntry?.Size ?? FontConfig.DefaultFontSize));
                }
            }
        }

        // Sounds
        {
            //! FIXME (Alex): Load config for sound settings, mostly for sounds that should be streamed

            // Load fallback sound
            using Stream stream = assembly.GetManifestResourceStream($"{assemblyName}.{SoundFallbackName}")!;
            Sounds.Add("error", new Sound(stream));

            string soundPath = Path.Join(Config.RootFolder, Config.SoundFolder);
            if (Directory.Exists(soundPath)) {
                string configPath = Path.Join(Config.RootFolder, Config.SoundConfig);
                SoundConfig? soundConfig = Path.Exists(configPath) ? JsonSerializer.Deserialize(File.ReadAllText(configPath), SourceGenerationContext.Default.SoundConfig) : null;

                foreach (string file in Directory.EnumerateFiles(soundPath, "*.*", SearchOption.AllDirectories).Where(e => SoundExtensions.Any(e.EndsWith))) {
                    string name = GetAssetName(soundPath, file);
                    SoundConfigEntry? configEntry = soundConfig?.SoundConfigs.FirstOrDefault(e => e.Name == name);
                    Sounds.Add(name, new Sound(file, configEntry?.LoadingMethod ?? SoundConfig.DefaultLoadingMethod));
                }
            }
        }

        // Shaders
        {
            // Load fallback shader
            //! FIXME (Alex): I could see an argument for just crashing if a shader doesn't load correctly, might be harder to identify bugs if it doesn't crash on fail
            Shaders.Add("error", new BatcherShader(graphicsDevice));

            // Load shaders from config
            //      Shaders are built from more data than just files, so read directly off the config
            string shaderPath = Path.Join(Config.RootFolder, Config.ShaderFolder);
            string shaderConfigPath = Path.Join(Config.RootFolder, Config.ShaderConfig);
            string shaderExtension = graphicsDevice.Driver.GetShaderExtension(); graphicsDevice.Driver.GetShaderExtension();
            if (Path.Exists(shaderConfigPath)) {
                ShaderConfig? shaderConfig = JsonSerializer.Deserialize(File.ReadAllText(shaderConfigPath), SourceGenerationContext.Default.ShaderConfig);

                if (shaderConfig != null) {
                    foreach (ShaderConfigEntry entry in shaderConfig.ShaderConfigs) {
                        // Find files from config entry
                        //! FIXME (Alex): Should the path extensions be more configurable?
                        string vertexPath = Path.Join(shaderPath, $"{entry.Vertex.Path}.{shaderExtension}");
                        string fragmentPath = Path.Join(shaderPath, $"{entry.Fragment.Path}.{shaderExtension}");
                        if (!Path.Exists(vertexPath) || !Path.Exists(fragmentPath)) { continue; }

                        // Load files into bytes
                        byte[] vertexBytes;
                        byte[] fragmentBytes;
                        vertexBytes = File.ReadAllBytes(vertexPath);
                        fragmentBytes = vertexPath == fragmentPath ? vertexBytes : File.ReadAllBytes(fragmentPath);

                        // Build shader info
                        ShaderCreateInfo createInfo = new(
                            Vertex: new(
                                Code: vertexBytes,
                                SamplerCount: entry.Vertex.Samplers,
                                UniformBufferCount: entry.Vertex.Uniforms,
                                EntryPoint: entry.Vertex.EntryPoint
                            ),
                            Fragment: new(
                                Code: fragmentBytes,
                                SamplerCount: entry.Fragment.Samplers,
                                UniformBufferCount: entry.Fragment.Uniforms,
                                EntryPoint: entry.Fragment.EntryPoint
                            )
                        );

                        // Add shader to dict
                        Shaders.Add(entry.Name, new Shader(graphicsDevice, createInfo));
                    }
                }
            }
        }

        // Animation Data
        {
            //! FIXME (Alex): Rework animations
            //     Animations.Add("error", new([Point2.Zero], Point2.One));
            //     string path = Path.Join(config.RootFolder, config.AnimationConfig);
            //     if (Path.Exists(path)) {
            //         AnimationConfig? animationConfig = JsonSerializer.Deserialize(File.ReadAllText(path), SourceGenerationContext.Default.AnimationConfig);
            //         if (config != null) {
            //             foreach (AnimationConfigEntry entry in config.AnimationConfigs) {
            //                 if (entry.Frames.Length == 0) {
            //                     Animations.Add(entry.Name, new(entry.HorizontalFrames, entry.VerticalFrames, new(entry.FrameWidth, entry.FrameHeight)) {
            //                         DefaultFrametime = entry.DefaultFrameTime,
            //                         DefaultLooping = entry.DefaultLooping,
            //                         DefaultPositionOffset = new(entry.DefaultOffset.X, entry.DefaultOffset.Y),
            //                         DefaultStartDelay = entry.DefaultStartDelay
            //                     });
            //                 }
            //                 else {
            //                     Animations.Add(entry.Name, new([.. entry.Frames.Select(e => new Point2(e.X, e.Y))], new(entry.FrameWidth, entry.FrameHeight)) {
            //                         DefaultFrametime = entry.DefaultFrameTime,
            //                         DefaultLooping = entry.DefaultLooping,
            //                         DefaultPositionOffset = new(entry.DefaultOffset.X, entry.DefaultOffset.Y),
            //                         DefaultStartDelay = entry.DefaultStartDelay
            //                     });
            //                 }
            //             }
            //         }
            //     }
        }
    }

    /// <summary>
    /// Remove extra information from a file path to create a unique asset identifier
    /// </summary>
    /// <param name="relativePath">The path elements to remove from the start of the path</param>
    /// <param name="assetPath">Full path to the asset</param>
    /// <returns></returns>
    string GetAssetName(string relativePath, string assetPath) {
        string name = Path.Join(Path.GetDirectoryName(assetPath), Path.GetFileNameWithoutExtension(assetPath));
        name = Path.GetRelativePath(relativePath, name);
        name = name.Replace("\\", "/");
        name = name.Replace("/", "/");

        return name;
    }
}