using System.Text.Json;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Asset loader for importing fonts from external files.
/// </summary>
public class FontLoader() : AssetLoaderStage() {
    static readonly string[] FontExtensions = [".ttf", ".otf", ".fnt"];

    public override AssetProviderItem[] GetLoadOptions(Assets assets)
        => assets.Provider.GetItemsInGroup(assets.Config.FontGroup, FontExtensions);

    public override void RunLoad(Assets assets, AssetCollection collection) {
        FontConfig config = GetConfig(assets);

        foreach (AssetProviderItem item in Filter(assets, collection)) {
            FontConfigEntry? configEntry = config.FontConfigs.FirstOrDefault(e => e.Name == item.Name);
            using Stream stream = assets.Provider.GetItemStream(item);

            SpriteFont font;
            lock (assets.GraphicsDevice) {
                font = new(assets.GraphicsDevice, stream, configEntry?.Size ?? FontConfig.DefaultFontSize);
            }
            AddAsset(assets, item.Name, font);
        }
    }

    public override void RunUnload(Assets assets, AssetCollection collection) {
        foreach (AssetProviderItem item in Filter(assets, collection)) {
            RemoveAsset<SpriteFont>(assets, item.Name);
        }
    }

    static FontConfig GetConfig(Assets assets) {
        string configName = Path.GetFileNameWithoutExtension(assets.Config.FontConfig);
        string configExtension = Path.GetExtension(assets.Config.FontConfig);
        AssetProviderItem configItem = new(assets.Config.FontGroup, configName, configExtension);
        if (assets.Provider.HasItem(configItem)) {
            try {
                using Stream stream = assets.Provider.GetItemStream(configItem);
                FontConfig? config = JsonSerializer.Deserialize(stream, SourceGenerationContext.Default.FontConfig);
                if (config != null) { return config; }
            } catch { }
        }
        return new();
    }

    static AssetProviderItem[] Filter(Assets assets, AssetCollection collection)
        => collection.Filter(assets.Config.FontGroup, FontExtensions);
}