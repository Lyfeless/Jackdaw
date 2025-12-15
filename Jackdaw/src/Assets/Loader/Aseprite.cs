using System.Text.Json;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Asset loader for importing textures and animations from aseprite files.
/// </summary>
public class AsepriteLoader() : AssetLoaderStage() {
    readonly string[] AsepriteExtensions = [".aseprite", ".ase"];


    public override void Run(Assets assets) {
        PackerLoader? packer = assets.FindLoaderStage<PackerLoader>();
        AsepriteAnimationLoader? animationLoader = assets.FindLoaderStage<AsepriteAnimationLoader>();
        if (packer == null || animationLoader == null) { return; }

        string texturePath = Path.Join(assets.Config.RootFolder, assets.Config.TextureFolder);
        if (!Directory.Exists(texturePath)) { return; }

        foreach (string file in Assets.GetEnumeratedFiles(texturePath, AsepriteExtensions)) {
            string name = Assets.GetAssetName(texturePath, file);
            Aseprite aseprite = new(file);
            if (aseprite.Frames.Length == 0) { continue; }
            if (aseprite.Frames.Length == 1) { packer.Add(name, aseprite.RenderFrame(0)); continue; }
            Image[] frames = aseprite.RenderAllFrames();
            for (int i = 0; i < frames.Length; ++i) {
                packer.Add(GetFrameName(name, i), frames[i]);
            }

            animationLoader.AddAnimation(name, aseprite);
        }
    }

    public static string GetFrameName(string name, int frame) => $"{name}{frame}";
}