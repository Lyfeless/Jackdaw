using System.Text.Json;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Asset loader for importing fonts from external files.
/// </summary>
public class FontLoader() : AssetLoaderStage() {
    readonly string[] FontExtensions = [".ttf", ".otf", ".fnt"];

    const string FontFallbackName = "Fallback.font.ttf";


    public override void Run(Assets assets) {
        using Stream stream = assets.Assembly.GetManifestResourceStream($"{assets.AssemblyName}.{FontFallbackName}")!;
        assets.SetFallback(new SpriteFont(assets.GraphicsDevice, stream, 16));

        string fontPath = Path.Join(assets.Config.RootFolder, assets.Config.FontFolder);
        if (!Directory.Exists(fontPath)) { return; }

        string configPath = Path.Join(assets.Config.RootFolder, assets.Config.FontConfig);
        FontConfig? fontConfig = Path.Exists(configPath) ? JsonSerializer.Deserialize(File.ReadAllText(configPath), SourceGenerationContext.Default.FontConfig) : null;

        foreach (string file in Assets.GetEnumeratedFiles(fontPath, FontExtensions)) {
            string name = Assets.GetAssetName(fontPath, file);
            FontConfigEntry? configEntry = fontConfig?.FontConfigs.FirstOrDefault(e => e.Name == name);
            assets.Add(name, new SpriteFont(assets.GraphicsDevice, file, configEntry?.Size ?? FontConfig.DefaultFontSize));
        }
    }
}