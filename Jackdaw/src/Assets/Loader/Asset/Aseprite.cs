using System.Transactions;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Asset loader for importing textures and animations from aseprite files.
/// </summary>
public class AsepriteLoader : AssetLoaderStage {
    static readonly string[] AsepriteExtensions = [".aseprite", ".ase"];

    public AsepriteLoader() : base() {
        SetBefore<PackerLoader>();
        SetBefore<AsepriteAnimationLoader>();
    }

    public override AssetProviderItem[] GetLoadOptions(Assets assets)
        => assets.Provider.GetItemsInGroup(assets.Config.TextureGroup, AsepriteExtensions);

    public override void RunLoad(Assets assets, AssetCollection collection) {
        PackerLoader? packer = Find<PackerLoader>(assets);
        AsepriteAnimationLoader? animationLoader = Find<AsepriteAnimationLoader>(assets);
        if (packer == null || animationLoader == null) { return; }

        foreach (AssetProviderItem item in Filter(assets, collection)) {
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

    public override void RunUnload(Assets assets, AssetCollection collection) {
        foreach (AssetProviderItem item in Filter(assets, collection)) {
            // Aseprite file isn't guarenteed to have an animation but this is faster than checking
            RemoveAsset<AnimationData>(assets, item.Name);
        }
    }

    /// <summary>
    /// Get the name of a single frame in an aseprite animation.
    /// </summary>
    /// <param name="name">The original aseprite image's name</param>
    /// <param name="frame">The animation frame number.</param>
    /// <returns>A unique id for the given frame of animation.</returns>
    public static string GetFrameName(string name, int frame) => $"{name}{frame}";

    static AssetProviderItem[] Filter(Assets assets, AssetCollection collection)
        => collection.Filter(assets.Config.TextureGroup, AsepriteExtensions);
}