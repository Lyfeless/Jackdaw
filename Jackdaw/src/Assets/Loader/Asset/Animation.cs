using System.Text.Json;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Asset loader for importing texture animations from configuration data.
/// </summary>
public class AnimationLoader : AssetLoaderStage {
    public AnimationLoader() : base() {
        SetAfter<PackerLoader>();
    }

    public override AssetProviderItem[] GetLoadOptions(Assets assets)
        => assets.Provider.GetItemsInGroup(assets.Config.AnimationGroup, [
            assets.Config.AnimationExtension,
            assets.Config.AnimationGroupExtension
        ]);

    public override void RunLoad(Assets assets, AssetCollection collection) {
        foreach (AssetProviderItem item in FilterSingles(assets, collection)) {
            AnimationConfig? data = GetSingleConfig(assets, item);
            if (data == null) { continue; }
            AddAnimation(assets, item.Name, data);
        }

        foreach (AssetProviderItem item in FilterGroups(assets, collection)) {
            AnimationGroupConfig? data = GetGroupConfig(assets, item);
            if (data == null) { continue; }
            foreach (AnimationConfigEntry entry in data.Entries) {
                AddAnimation(assets, GetGroupEntryName(item, entry), entry.Animation);
            }
        }
    }

    public override void RunUnload(Assets assets, AssetCollection collection) {
        foreach (AssetProviderItem item in FilterSingles(assets, collection)) {
            RemoveAsset<AnimationData>(assets, item.Name);
        }

        foreach (AssetProviderItem item in FilterGroups(assets, collection)) {
            AnimationGroupConfig? data = GetGroupConfig(assets, item);
            if (data == null) { continue; }
            foreach (AnimationConfigEntry entry in data.Entries) {
                RemoveAsset<AnimationData>(assets, GetGroupEntryName(item, entry));
            }
        }
    }

    static AssetProviderItem[] FilterSingles(Assets assets, AssetCollection collection)
        => collection.Filter(assets.Config.AnimationGroup, assets.Config.AnimationExtension);

    static AssetProviderItem[] FilterGroups(Assets assets, AssetCollection collection)
        => collection.Filter(assets.Config.AnimationGroup, assets.Config.AnimationGroupExtension);

    static void AddAnimation(Assets assets, string name, AnimationConfig animConfig) {
        AnimationData? anim = animConfig.AnimationType switch {
            AnimationType.SPRITESHEET => GetSpriteSheetAnimation(assets, animConfig),
            AnimationType.MULTI_TEXTURE => GetMultiTextureAnimation(assets, animConfig),
            _ => null
        };

        if (anim != null) { AddAsset(assets, name, anim); }
    }

    static AnimationConfig? GetSingleConfig(Assets assets, AssetProviderItem item) {
        using Stream stream = assets.Provider.GetItemStream(item);
        return JsonSerializer.Deserialize(stream, SourceGenerationContext.Default.AnimationConfig);
    }

    static AnimationGroupConfig? GetGroupConfig(Assets assets, AssetProviderItem item) {
        using Stream stream = assets.Provider.GetItemStream(item);
        return JsonSerializer.Deserialize(stream, SourceGenerationContext.Default.AnimationGroupConfig);
    }

    static string GetGroupEntryName(AssetProviderItem item, AnimationConfigEntry entry) => $"{item.Name}/{entry.Name}";

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