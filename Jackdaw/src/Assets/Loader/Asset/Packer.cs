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

    readonly Dictionary<string, string[]> collectionNameCache = [];

    public override AssetProviderItem[] GetLoadOptions(Assets assets) => [];

    public override void RunLoad(Assets assets, AssetCollection collection) {
        Packer.Output output = Packer.Pack();
        List<Texture> pages = [];

        foreach (Image? page in output.Pages) {
            Thread.Sleep(1000);
            lock (assets.GraphicsDevice) {
                pages.Add(new Texture(assets.GraphicsDevice, page));
            }
        }

        foreach (Packer.Entry entry in output.Entries) {
            Subtexture texture = new(pages[entry.Page], entry.Source, entry.Frame);
            AddAsset(assets, entry.Name, texture);
        }

        if (!collectionNameCache.ContainsKey(collection.Name)) {
            collectionNameCache.Add(collection.Name, [.. output.Entries.Select(e => e.Name)]);
        }

        Packer.Clear();
    }

    public override void RunUnload(Assets assets, AssetCollection collection) {
        if (!collectionNameCache.TryGetValue(collection.Name, out string[]? names) || names == null) { return; }
        foreach (string name in names) {
            RemoveAsset<Subtexture>(assets, name);
        }
    }

    /// <summary>
    /// Add an image to the packer.
    /// </summary>
    /// <param name="name">The asset's name.</param>
    /// <param name="file">The file path to load.</param>
    public void Add(string name, string file) => Packer.Add(name, file);

    /// <summary>
    /// Add an image to the packer.
    /// </summary>
    /// <param name="name">The asset's name.</param>
    /// <param name="image">The image to add.</param>
    public void Add(string name, Image image) => Packer.Add(name, image);

    /// <summary>
    /// Add an image to the packer.
    /// </summary>
    /// <param name="name">The asset's name.</param>
    /// <param name="image">The image to add.</param>
    /// <param name="clip">The bounds to clip the image to.</param>
    public void Add(string name, Image image, RectInt clip) => Packer.Add(name, image, clip);

    /// <summary>
    /// Add an image to the packer.
    /// </summary>
    /// <param name="name">The asset's name.</param>
    /// <param name="width">The image's width.</param>
    /// <param name="height">The image's height.</param>
    /// <param name="pixels">The image's pixel data.</param>
    public void Add(string name, int width, int height, ReadOnlySpan<Color> pixels) => Packer.Add(name, width, height, pixels);
}