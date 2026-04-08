using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Asset loader for packing textures from other loaders into a single sprite atlas.
/// </summary>
public class PackerLoader() : AssetLoaderStage() {
    readonly Packer Packer = new() {
        Trim = false,
        CombineDuplicates = false,
        Padding = 1
    };

    public override AssetProviderItem[] GetLoadOptions(Assets assets) => [];

    public override void RunLoad(Assets assets, AssetCollection collection) {
        var output = Packer.Pack();
        List<Texture> pages = [];

        foreach (var page in output.Pages) {
            pages.Add(new Texture(assets.GraphicsDevice, page));
        }

        foreach (var entry in output.Entries) {
            Subtexture texture = new(pages[entry.Page], entry.Source, entry.Frame);
            AddAsset(assets, entry.Name, texture);
        }

        Packer.Clear();
    }

    public override void RunUnload(Assets assets, AssetCollection collection) { }

    public void Add(string name, string file) => Packer.Add(name, file);
    public void Add(string name, Image image) => Packer.Add(name, image);
    public void Add(string name, Image image, RectInt clip) => Packer.Add(name, image, clip);
    public void Add(string name, int width, int height, ReadOnlySpan<Color> pixels) => Packer.Add(name, width, height, pixels);
}