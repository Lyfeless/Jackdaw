using System.Reflection;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Asset loader for importing textures from external files.
/// </summary>
public class TextureLoader : AssetLoaderStage {
    readonly string[] TextureExtensions = [".png", ".jpg", ".qoi"];

    const string TextureFallbackName = "Fallback.texture.png";
    const string ManFallbackName = "Fallback.man.png";

    public TextureLoader() : base() {
        SetBefore<PackerLoader>();
    }

    public override void Run(Assets assets) {
        PackerLoader? packer = assets.FindLoaderStage<PackerLoader>();
        if (packer == null) { return; }

        packer.Add("fallback", FallbackTexture(assets, "texture", ".png"));
        packer.Add("fallback-man", FallbackTexture(assets, "man", ".png"));

        foreach (AssetProviderItem item in assets.Provider.GetItemsInGroup(assets.Config.TextureGroup, TextureExtensions)) {
            using Stream imageStream = assets.Provider.GetItemStream(item);
            packer.Add(item.Name, new Image(imageStream));
        }
    }

    static Image FallbackTexture(Assets assets, string name, string extension) {
        using Stream stream = assets.FallbackProvider.GetItemStream(new("", name, extension)); ;
        return new Image(stream);
    }
}