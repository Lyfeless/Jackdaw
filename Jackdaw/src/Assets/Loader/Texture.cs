using System.Reflection;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Asset loader for importing textures from external files.
/// </summary>
public class TextureLoader : AssetLoaderStage {
    readonly string[] TextureExtensions = [".png", ".jpg"];

    const string TextureFallbackName = "Fallback.texture.png";
    const string ManFallbackName = "Fallback.man.png";

    public TextureLoader() : base() {
        SetBefore<PackerLoader>();
    }

    public override void Run(Assets assets) {
        PackerLoader? packer = assets.FindLoaderStage<PackerLoader>();
        if (packer == null) { return; }

        packer.Add("fallback", FallbackTexture(assets.Assembly, assets.AssemblyName, TextureFallbackName));
        packer.Add("fallback-man", FallbackTexture(assets.Assembly, assets.AssemblyName, ManFallbackName));

        string TexturePath = Path.Join(assets.Config.RootFolder, assets.Config.TextureFolder);
        if (!Directory.Exists(TexturePath)) { return; }

        foreach (string file in Assets.GetEnumeratedFiles(TexturePath, TextureExtensions)) {
            string name = Assets.GetAssetName(TexturePath, file);
            packer.Add(name, file);
        }
    }

    static Image FallbackTexture(Assembly assembly, string assemblyName, string name) {
        using Stream streamError = assembly.GetManifestResourceStream($"{assemblyName}.{name}")!;
        return new Image(streamError);
    }
}