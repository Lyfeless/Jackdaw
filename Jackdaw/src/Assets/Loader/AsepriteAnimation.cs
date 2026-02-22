using System.Text.Json;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Asset loader for creating animations from aseprite files, needs to bne run after <see cref="PackerLoader" /> runs so the textures exist.
/// </summary>
public class AsepriteAnimationLoader : AssetLoaderStage {
    record struct Animation(string Name, Aseprite Data);
    readonly List<Animation> animations = [];

    static readonly AsepriteConfig DefaultConfig = new() {
        Looping = false,
        PositionOffsetX = 0,
        PositionOffsetY = 0,
        FrameData = []
    };

    public AsepriteAnimationLoader() : base() {
        SetAfter<PackerLoader>();
    }

    public override void Run(Assets assets) {
        foreach (Animation animation in animations) {
            AnimationData? anim = GetAnimationData(assets, animation.Name, animation.Data, GetConfig(assets, animation.Name));
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

    static AnimationData? GetAnimationData(Assets assets, string name, Aseprite aseprite, AsepriteConfig config) {
        Subtexture[] textures = new Subtexture[aseprite.Frames.Length];
        AnimationFrame[] frames = new AnimationFrame[aseprite.Frames.Length];
        for (int i = 0; i < aseprite.Frames.Length; ++i) {
            bool flipX = false;
            bool flipY = false;
            string embeddedData = string.Empty;
            AsepriteFrameConfig? frameConfig = config.FrameData.FirstOrDefault(e => e.Frame == i);
            if (frameConfig != null) {
                flipX = frameConfig.FlipX;
                flipY = frameConfig.FlipY;
                embeddedData = frameConfig.EmbeddedData;
            }

            textures[i] = assets.GetSubtexture(AsepriteLoader.GetFrameName(name, i));
            frames[i] = new AnimationFrame(
                texture: i,
                duration: TimeSpan.FromMilliseconds(aseprite.Frames[i].Duration),
                flipX: flipX,
                flipY: flipY,
                embeddedData: embeddedData
            );
        }

        return new(
            textures: textures,
            frames: frames,
            looping: config.Looping,
            positionOffset: new(config.PositionOffsetX, config.PositionOffsetY)
        );
    }

    static AsepriteConfig GetConfig(Assets assets, string name) {
        AssetProviderItem item = new(assets.Config.TextureGroup, name, assets.Config.AsepriteConfigExtension);
        if (!assets.Provider.HasItem(item)) { return DefaultConfig; }
        try {
            using Stream stream = assets.Provider.GetItemStream(item);
            AsepriteConfig? data = JsonSerializer.Deserialize(stream, SourceGenerationContext.Default.AsepriteConfig);
            if (data == null) { return DefaultConfig; }
            return data;
        } catch { }
        return DefaultConfig;
    }
}