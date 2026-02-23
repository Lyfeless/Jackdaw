using System.Text.Json;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Asset loader for importing fonts from external files.
/// </summary>
public class FontLoader() : AssetLoaderStage() {
    readonly string[] FontExtensions = [".ttf", ".otf", ".fnt"];

    public override void Run(Assets assets) {
        using Stream fallbackStream = assets.FallbackProvider.GetItemStream(new("", "font", ".ttf"));
        assets.SetFallback(new SpriteFont(assets.GraphicsDevice, fallbackStream, 16));

        FontConfig? config = null;
        string configName = Path.GetFileNameWithoutExtension(assets.Config.FontConfig);
        string configExtension = Path.GetExtension(assets.Config.FontConfig);
        AssetProviderItem configItem = new(assets.Config.FontGroup, configName, configExtension);
        if (assets.Provider.HasItem(configItem)) {
            try {
                using Stream stream = assets.Provider.GetItemStream(configItem);
                config = JsonSerializer.Deserialize(stream, SourceGenerationContext.Default.FontConfig);
            } catch { }
        }

        foreach (AssetProviderItem item in assets.Provider.GetItemsInGroup(assets.Config.FontGroup, FontExtensions)) {
            FontConfigEntry? configEntry = config?.FontConfigs.FirstOrDefault(e => e.Name == item.Name);
            using Stream stream = assets.Provider.GetItemStream(item);
            assets.Add(item.Name, new SpriteFont(assets.GraphicsDevice, stream, configEntry?.Size ?? FontConfig.DefaultFontSize));
        }
    }
}