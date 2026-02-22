using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Asset loader for importing textures and animations from aseprite files.
/// </summary>
public class AsepriteLoader : AssetLoaderStage {
    readonly string[] AsepriteExtensions = [".aseprite", ".ase"];

    public AsepriteLoader() : base() {
        SetBefore<PackerLoader>();
        SetBefore<AsepriteAnimationLoader>();
    }

    public override void Run(Assets assets) {
        PackerLoader? packer = assets.FindLoaderStage<PackerLoader>();
        AsepriteAnimationLoader? animationLoader = assets.FindLoaderStage<AsepriteAnimationLoader>();
        if (packer == null || animationLoader == null) { return; }


        foreach (AssetProviderItem item in assets.Provider.GetItemsInGroup(assets.Config.TextureGroup, AsepriteExtensions)) {
            using Stream stream = assets.Provider.GetItemStream(item);
            Aseprite aseprite = new(stream);
            if (aseprite.Frames.Length == 0) { continue; }
            if (aseprite.Frames.Length == 1) { packer.Add(item.Name, aseprite.RenderFrame(0)); continue; }
            Image[] frames = aseprite.RenderAllFrames();
            for (int i = 0; i < frames.Length; ++i) {
                packer.Add(GetFrameName(item.Name, i), frames[i]);
            }

            animationLoader.AddAnimation(item.Name, aseprite);
        }
    }

    public static string GetFrameName(string name, int frame) => $"{name}{frame}";
}