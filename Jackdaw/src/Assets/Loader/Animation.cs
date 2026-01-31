using System.Text.Json;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Asset loader for importing texture animations from configuration data.
/// </summary>
public class AnimationLoader() : AssetLoaderStage() {
    public override void Run(Assets assets) {
        assets.SetFallback(new AnimationData(assets.GetFallback<Subtexture>(), [new(0, TimeSpan.Zero)], TimeSpan.Zero));

        string AnimationPath = Path.Join(assets.Config.RootFolder, assets.Config.AnimationFolder);
        if (!Directory.Exists(AnimationPath)) { return; }

        // Load single animations
        foreach (string file in Assets.GetEnumeratedFiles(AnimationPath, assets.Config.AnimationExtension)) {
            AnimationConfig? data = JsonSerializer.Deserialize(File.ReadAllText(file), SourceGenerationContext.Default.AnimationConfig);
            if (data == null) { continue; }
            string name = Assets.GetAssetName(AnimationPath, file);
            AddAnimation(assets, name, data);
        }

        // Load animation group files
        foreach (string file in Assets.GetEnumeratedFiles(AnimationPath, assets.Config.AnimationGroupExtension)) {
            AnimationGroupConfig? data = JsonSerializer.Deserialize(File.ReadAllText(file), SourceGenerationContext.Default.AnimationGroupConfig);
            if (data == null) { continue; }
            foreach (AnimationConfigEntry entry in data.Entries) {
                string name = $"{Assets.GetAssetName(AnimationPath, file)}/{entry.Name}";
                AddAnimation(assets, name, entry.Animation);
            }
        }
    }

    static void AddAnimation(Assets assets, string name, AnimationConfig animConfig) {
        AnimationData? anim = animConfig.AnimationType switch {
            AnimationType.SPRITESHEET => GetSpriteSheetAnimation(assets, animConfig),
            AnimationType.MULTI_TEXTURE => GetMultiTextureAnimation(assets, animConfig),
            _ => null
        };

        if (anim != null) { assets.Add(name, anim); }
    }

    static AnimationData? GetSpriteSheetAnimation(Assets assets, AnimationConfig config) {
        Subtexture spritesheet = assets.GetSubtexture(config.Spritesheet);
        Point2 frameSize = new(
            (int)spritesheet.Width / config.HorizontalFrames,
            (int)spritesheet.Height / config.VerticalFrames
        );

        AnimationFrame[] frames;
        if (config.Frames.Length > 0) {
            frames = [.. config.Frames.Select(frame => new AnimationFrame(
                texture: 0,
                duration: TimeSpan.FromMilliseconds(frame.Duration),
                clip: SpriteSheetClip(frameSize, frame.FrameX, frame.FrameY),
                positionOffset: new(frame.PositionOffsetX, frame.PositionOffsetY),
                flipX: frame.FlipX,
                flipY: frame.FlipY,
                embeddedData: frame.EmbeddedData
            ))];
        }
        else {
            frames = new AnimationFrame[config.HorizontalFrames * config.VerticalFrames];
            for (int y = 0; y < config.VerticalFrames; ++y) {
                for (int x = 0; x < config.HorizontalFrames; ++x) {
                    frames[x + (y * config.HorizontalFrames)] = new(0, TimeSpan.FromMilliseconds(config.FrameTime), SpriteSheetClip(frameSize, x, y));
                }
            }
        }

        return new(
            texture: spritesheet,
            frames: frames,
            looping: config.Looping,
            positionOffset: new(config.PositionOffsetX, config.PositionOffsetY)
        );
    }

    static RectInt SpriteSheetClip(Point2 frameSize, int x, int y)
        => new(frameSize * new Point2(x, y), frameSize);

    static AnimationData? GetMultiTextureAnimation(Assets assets, AnimationConfig config) {
        return new(
            GetAllTextures(assets, config.Textures),
            frames: [..config.Frames.Select(frame => new AnimationFrame(
                texture: frame.Texture,
                duration: TimeSpan.FromMilliseconds(frame.Duration),
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

    static Subtexture[] GetAllTextures(Assets assets, string[] names) {
        Subtexture[] textures = new Subtexture[names.Length];
        for (int i = 0; i < names.Length; ++i) {
            textures[i] = assets.GetSubtexture(names[i]);
        }
        return textures;
    }
}