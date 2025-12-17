namespace Jackdaw;

/// <summary>
/// Asset loader for storing the texture fallback, as the fallback isn't created until after the <see cref="PackerLoader" /> runs.
/// </summary>
public class TextureFallbackLoader() : AssetLoaderStage() {
    public override void Run(Assets assets) {
        assets.SetFallback(assets.GetSubtexture("fallback"));
    }
}