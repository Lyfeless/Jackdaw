using System.Text.Json;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Asset loader for importing textures and animations from aseprite files.
/// </summary>
public class AsepriteLoader() : AssetLoaderStage() {
    readonly string[] AsepriteExtensions = [".aseprite", ".ase"];

    string texturePath;

    public override void Run(Assets assets) {
        PackerLoader? packer = assets.FindLoaderStage<PackerLoader>();
        if (packer == null) { return; }

        texturePath = Path.Join(assets.Config.RootFolder, assets.Config.TextureFolder);
        if (!Directory.Exists(texturePath)) { return; }

        foreach (string file in Assets.GetEnumeratedFiles(texturePath, AsepriteExtensions)) {
            string name = Assets.GetAssetName(texturePath, file);
            Aseprite aseprite = new(file);
            if (aseprite.Frames.Length == 0) { continue; }
            if (aseprite.Frames.Length == 1) { packer.Add(name, aseprite.RenderFrame(0)); continue; }
            Image[] frames = aseprite.RenderAllFrames();
            for (int i = 0; i < frames.Length; ++i) {
                packer.Add(GetFrameName(name, i), frames[i]);
            }

            AnimationData? anim = GetAnimationData(assets, name, aseprite);
            if (anim != null) { assets.Add(name, anim); }
        }
    }

    AnimationData? GetAnimationData(Assets assets, string name, Aseprite aseprite) {
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

            textures[i] = assets.GetSubtexture(GetFrameName(name, i));
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

    static string GetFrameName(string name, int frame) => $"{name}{frame}";
}