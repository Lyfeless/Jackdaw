using System.Text.Json;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Asset loader for creating animations from aseprite files, needs to bne run after <see cref="PackerLoader" /> runs so the textures exist.
/// </summary>
public class AsepriteAnimationLoader() : AssetLoaderStage() {
    record struct Animation(string Name, Aseprite Data);
    readonly List<Animation> animations = [];

    public override void Run(Assets assets) {
        string texturePath = Path.Join(assets.Config.RootFolder, assets.Config.TextureFolder);
        foreach (Animation animation in animations) {
            AnimationData? anim = GetAnimationData(assets, animation.Name, animation.Data, texturePath);
            if (anim != null) { assets.Add(animation.Name, anim); }
        }
    }

    /// <summary>
    /// Add a new aseprite animation to be created in the loader run.
    /// </summary>
    /// <param name="name">The animation's name id.</param>
    /// <param name="data">The aseprite file with the animation data.</param>
    public void AddAnimation(string name, Aseprite data)
        => animations.Add(new(name, data));

    static AnimationData? GetAnimationData(Assets assets, string name, Aseprite aseprite, string texturePath) {
        bool looping = true;
        Point2 positionOffset = Point2.Zero;
        AsepriteFrameConfig[] frameConfigs = [];
        string path = Path.Join(texturePath, $"{name}{assets.Config.AsepriteConfigExtension}");
        if (File.Exists(path)) {
            AsepriteConfig? config = JsonSerializer.Deserialize(File.ReadAllText(path), SourceGenerationContext.Default.AsepriteConfig);
            if (config != null) {
                looping = config.Looping;
                positionOffset = new(config.PositionOffsetX, config.PositionOffsetY);
                frameConfigs = config.FrameData;
            }
        }

        Subtexture[] textures = new Subtexture[aseprite.Frames.Length];
        AnimationFrame[] frames = new AnimationFrame[aseprite.Frames.Length];
        for (int i = 0; i < aseprite.Frames.Length; ++i) {
            bool flipX = false;
            bool flipY = false;
            string embeddedData = string.Empty;
            AsepriteFrameConfig? frameConfig = frameConfigs.FirstOrDefault(e => e.Frame == i);
            if (frameConfig != null) {
                flipX = frameConfig.FlipX;
                flipY = frameConfig.FlipY;
                embeddedData = frameConfig.EmbeddedData;
            }

            textures[i] = assets.GetSubtexture(AsepriteLoader.GetFrameName(name, i));
            frames[i] = new AnimationFrame(
                texture: i,
                duration: aseprite.Frames[i].Duration,
                flipX: flipX,
                flipY: flipY,
                embeddedData: embeddedData
            );
        }

        return new(
            textures: textures,
            frames: frames,
            looping: looping,
            positionOffset: positionOffset
        );
    }
}