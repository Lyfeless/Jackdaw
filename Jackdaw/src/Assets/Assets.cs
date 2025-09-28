using System.Reflection;
using System.Text.Json;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Manager class used for storing and loading game assets including textures, sounds, fonts, etc
/// </summary>
public class Assets {
    #region File/path definitions
    /// <summary>
    /// The game's asset-related config data.
    /// </summary>
    public GameContentConfig Config;

    /// <summary>
    /// All file extensions the texture loader will search for.
    /// </summary>
    public readonly string[] TextureExtensions = [".png", ".jpg"];

    /// <summary>
    /// All file extensions the aseprite loader will search for.
    /// </summary>
    public readonly string[] AsepriteExtensions = [".aseprite", ".ase"];

    /// <summary>
    /// All file extensions the font loader will search for.
    /// </summary>
    public readonly string[] FontExtensions = [".ttf", ".otf", ".fnt"];

    string TexturePath;
    string FontPath;
    string ShaderPath;
    string ShaderConfigPath;
    string AnimationPath;

    #endregion

    #region Assets storage and accessors
    readonly Dictionary<string, Subtexture> Textures = [];
    /// <summary>
    /// Find a texture from the loaded texture data.
    /// </summary>
    /// <param name="name">The asset name.</param>
    /// <returns>The requested texture, or the default texture if nothing was found.</returns>
    public Subtexture GetTexture(string name) {
        if (Textures.TryGetValue(name, out Subtexture output)) { return output; }
        Log.Warning($"ASSETS: Failed to find texture {name}, returning default");
        return Textures["error"];
    }
    const string TextureFallbackName = "Fallback.texture.png";
    const string ManFallbackName = "Fallback.man.png";

    readonly Dictionary<string, SpriteFont> Fonts = [];
    /// <summary>
    /// Find a font from the loaded font data.
    /// </summary>
    /// <param name="name">The asset name.</param>
    /// <returns>The requested font, or the default font if nothing was found.</returns>
    public SpriteFont GetFont(string name) {
        if (Fonts.TryGetValue(name, out SpriteFont? output)) { return output; }
        Log.Warning($"ASSETS: Failed to find font {name}, returning default");
        return Fonts["error"];
    }
    const string FontFallbackName = "Fallback.font.ttf";

    readonly Dictionary<string, Shader> Shaders = [];
    /// <summary>
    /// Find a shader from the loaded shader data.
    /// </summary>
    /// <param name="name">The asset name.</param>
    /// <returns>The requested shader, or the default shader if nothing was found.</returns>
    public Shader GetShader(string name) {
        if (Shaders.TryGetValue(name, out Shader? output)) { return output; }
        Log.Warning($"ASSETS: Failed to find shader {name}, returning default");
        return Shaders["error"];
    }

    readonly Dictionary<string, AnimationData> Animations = [];
    /// <summary>
    /// Find a animation from the loaded animation data.
    /// </summary>
    /// <param name="name">The asset name.</param>
    /// <returns>The requested animation, or the default animation if nothing was found.</returns>
    public AnimationData GetAnimation(string name) {
        if (Animations.TryGetValue(name, out AnimationData? output)) { return output; }
        Log.Warning($"ASSETS: Failed to find animation {name}, returning default");
        return Animations["error"];
    }

    #endregion

    /// <summary>
    /// Load and initialize all asset types
    /// </summary>
    public Assets(GraphicsDevice graphicsDevice, GameContentConfig config) {
        Config = config;

        // Assembly data for fallback
        Assembly assembly = Assembly.GetExecutingAssembly();
        string? assemblyName = assembly.GetName().Name;

        TexturePath = Path.Join(Config.RootFolder, Config.TextureFolder);
        FontPath = Path.Join(Config.RootFolder, Config.FontFolder);
        ShaderPath = Path.Join(Config.RootFolder, Config.ShaderFolder);
        ShaderConfigPath = Path.Join(Config.RootFolder, Config.ShaderConfig);
        AnimationPath = Path.Join(config.RootFolder, config.AnimationFolder);

        Dictionary<string, Aseprite> asepriteAnims = [];

        // Textures
        {
            // Create asset packer
            Packer packer = new() {
                Trim = false,
                CombineDuplicates = false,
                Padding = 1
            };

            // Load fallback texture
            packer.Add("error", FallbackTexture(assembly, assemblyName!, TextureFallbackName));
            packer.Add("fallback-man", FallbackTexture(assembly, assemblyName!, ManFallbackName));

            // Load all textures in asset directory
            if (Directory.Exists(TexturePath)) {
                // Standard files
                foreach (string file in Directory.EnumerateFiles(TexturePath, "*.*", SearchOption.AllDirectories).Where(e => TextureExtensions.Any(e.EndsWith))) {
                    string name = GetAssetName(TexturePath, file);
                    packer.Add(name, file);
                }
                // Aseprite files
                foreach (string file in Directory.EnumerateFiles(TexturePath, "*.*", SearchOption.AllDirectories).Where(e => AsepriteExtensions.Any(e.EndsWith))) {
                    string name = GetAssetName(TexturePath, file);
                    Aseprite aseprite = new(file);
                    if (aseprite.Frames.Length == 0) { continue; }
                    if (aseprite.Frames.Length == 1) { packer.Add(name, aseprite.RenderFrame(0)); continue; }
                    Image[] frames = aseprite.RenderAllFrames();
                    for (int i = 0; i < frames.Length; ++i) {
                        packer.Add(GetFrameName(name, i), frames[i]);
                    }

                    asepriteAnims.Add(name, aseprite);
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
            if (Directory.Exists(FontPath)) {
                string configPath = Path.Join(Config.RootFolder, Config.RootFolder);
                FontConfig? fontConfig = Path.Exists(configPath) ? JsonSerializer.Deserialize(File.ReadAllText(configPath), SourceGenerationContext.Default.FontConfig) : null;

                foreach (string file in Directory.EnumerateFiles(FontPath, "*.*", SearchOption.AllDirectories).Where(e => FontExtensions.Any(e.EndsWith))) {
                    string name = GetAssetName(FontPath, file);
                    FontConfigEntry? configEntry = fontConfig?.FontConfigs.FirstOrDefault(e => e.Name == name);
                    Fonts.Add(name, new SpriteFont(graphicsDevice, file, configEntry?.Size ?? FontConfig.DefaultFontSize));
                }
            }
        }

        // Shaders
        {
            // Load fallback shader
            Shaders.Add("error", new BatcherShader(graphicsDevice));

            // Load shaders from config
            //      Shaders are built from more data than just files, so read directly off the config
            string shaderExtension = graphicsDevice.Driver.GetShaderExtension(); graphicsDevice.Driver.GetShaderExtension();
            if (Path.Exists(ShaderConfigPath)) {
                ShaderConfig? shaderConfig = JsonSerializer.Deserialize(File.ReadAllText(ShaderConfigPath), SourceGenerationContext.Default.ShaderConfig);

                if (shaderConfig != null) {
                    foreach (ShaderConfigEntry entry in shaderConfig.ShaderConfigs) {
                        // Find files from config entry
                        string vertexPath = Path.Join(ShaderPath, $"{entry.Vertex.Path}.{shaderExtension}");
                        string fragmentPath = Path.Join(ShaderPath, $"{entry.Fragment.Path}.{shaderExtension}");
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
            // Create fallback animation
            Animations.Add("error", new(this));

            // Load all animations and groups from directory
            if (Directory.Exists(AnimationPath)) {
                // Load single animations
                foreach (string file in Directory.EnumerateFiles(AnimationPath, "*.*", SearchOption.AllDirectories).Where(e => e.EndsWith(Config.AnimationExtension))) {
                    string name = GetAssetName(AnimationPath, file);
                    AnimationConfig? data = JsonSerializer.Deserialize(File.ReadAllText(file), SourceGenerationContext.Default.AnimationConfig);
                    if (data != null) {
                        AnimationData? anim = GetAnimationData(data);
                        if (anim != null) { Animations.Add(name, anim); }

                    }
                }

                // Load animation group files
                foreach (string file in Directory.EnumerateFiles(AnimationPath, "*.*", SearchOption.AllDirectories).Where(e => e.EndsWith(Config.AnimationGroupExtension))) {
                    AnimationGroupConfig? data = JsonSerializer.Deserialize(File.ReadAllText(file), SourceGenerationContext.Default.AnimationGroupConfig);
                    if (data != null) {
                        foreach (AnimationConfigEntry entry in data.Entries) {
                            AnimationData? anim = GetAnimationData(entry.Animation);
                            if (anim != null) { Animations.Add(entry.Name, anim); }
                        }
                    }
                }
            }

            // Load animations from aseprite files
            foreach ((string name, Aseprite aseprite) in asepriteAnims) {
                AnimationData? anim = GetAnimationData(name, aseprite);
                if (anim != null) { Animations.Add(name, anim); }
            }
        }
    }

    AnimationData? GetAnimationData(AnimationConfig config) {
        if (config.HorizontalFrames != 0 && config.VerticalFrames != 0) {
            return new(
                texture: GetTexture(config.Textures[0]),
                horizontalFrames: config.HorizontalFrames,
                verticalFrames: config.VerticalFrames,
                frameTime: config.FrameTime,
                looping: config.Looping,
                positionOffset: new(config.PositionOffsetX, config.PositionOffsetY)
            );
        }
        else if (config.Frames.Length > 0) {
            return new(
                frames: [..config.Frames.Select(frame => new AnimationFrame(
                    texture: GetTexture(config.Textures[frame.Texture]),
                    duration: frame.Duration,
                    flipX: frame.FlipX,
                    flipY: frame.FlipY,
                    positionOffset: new(frame.PositionOffsetX, frame.PositionOffsetY),
                    clip: (frame.ClipWidth > 0 && frame.ClipHeight > 0) ? new(frame.ClipX, frame.ClipY, frame.ClipWidth, frame.ClipHeight) : null,
                    embeddedData: frame.EmbeddedData
                ))],
                looping: config.Looping,
                positionOffset: new(config.PositionOffsetX, config.PositionOffsetY)
            );
        }

        return null;
    }

    AnimationData? GetAnimationData(string name, Aseprite aseprite) {
        float startDelay = 0;
        bool looping = true;
        Point2 positionOffset = Point2.Zero;
        AsepriteFrameConfig[] frameConfigs = [];
        string path = Path.Join(TexturePath, $"{name}{Config.AsepriteConfigExtension}");
        if (File.Exists(path)) {
            AsepriteConfig? config = JsonSerializer.Deserialize(File.ReadAllText(path), SourceGenerationContext.Default.AsepriteConfig);
            if (config != null) {
                startDelay = config.StartDelay;
                looping = config.Looping;
                positionOffset = new(config.PositionOffsetX, config.PositionOffsetY);
                frameConfigs = config.FrameData;
            }
        }

        return new(
            frames: [.. aseprite.Frames.Select((e, i) => {
                bool flipX = false;
                bool flipY = false;
                string embeddedData = string.Empty;
                AsepriteFrameConfig? frameConfig = frameConfigs.FirstOrDefault(e => e.Frame == i);
                if(frameConfig != null) {
                    flipX = frameConfig.FlipX;
                    flipY = frameConfig.FlipY;
                    embeddedData = frameConfig.EmbeddedData;
                }

                return new AnimationFrame(
                    texture: GetTexture(GetFrameName(name, i)),
                    duration: e.Duration,
                    flipX: flipX,
                    flipY: flipY,
                    embeddedData: embeddedData
                );
            })],
            startDelay: startDelay,
            looping: looping,
            positionOffset: positionOffset
        );
    }

    /// <summary>
    /// Remove extra information from a file path to create a unique asset identifier
    /// </summary>
    /// <param name="relativePath">The path elements to remove from the start of the path</param>
    /// <param name="assetPath">Full path to the asset</param>
    /// <returns></returns>
    public static string GetAssetName(string relativePath, string assetPath) {
        string name = Path.Join(Path.GetDirectoryName(assetPath), Path.GetFileNameWithoutExtension(assetPath));
        name = Path.GetRelativePath(relativePath, name);
        name = name.Replace("\\", "/");
        name = name.Replace("/", "/");

        return name;
    }

    static string GetFrameName(string name, int frame) {
        return $"{name}{frame}";
    }

    static Image FallbackTexture(Assembly assembly, string assemblyName, string name) {
        using Stream streamError = assembly.GetManifestResourceStream($"{assemblyName}.{name}")!;
        return new Image(streamError);
    }
}