using System.Collections.ObjectModel;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Asset loader for importing textures from external files.
/// </summary>
public class TextureLoader : AssetLoaderStage {
    static readonly string[] TextureExtensions = [".png", ".jpg", ".qoi"];

    public TextureLoader() : base() {
        SetBefore<PackerLoader>();
    }

    public override AssetProviderItem[] GetLoadOptions(Assets assets)
        => assets.Provider.GetItemsInGroup(assets.Config.TextureGroup, TextureExtensions);

    public override void RunLoad(Assets assets, AssetCollection collection) {
        PackerLoader? packer = Find<PackerLoader>(assets);
        if (packer == null) { return; }

        foreach (AssetProviderItem item in Filter(assets, collection)) {
            using Stream imageStream = assets.Provider.GetItemStream(item);
            packer.Add(item.Name, new Image(imageStream));
        }
    }

    public override void RunUnload(Assets assets, AssetCollection collection) { }

    static AssetProviderItem[] Filter(Assets assets, AssetCollection collection)
        => collection.Filter(assets.Config.TextureGroup, TextureExtensions);
}