using System.Text.Json;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Asset loader for importing shaders using compiled shader files and configuration data.
/// </summary>
public class ShaderLoader() : AssetLoaderStage() {
    ShaderConfig? Config = null;

    public override AssetProviderItem[] GetLoadOptions(Assets assets) {
        Config = GetConfig(assets);
        if (Config == null) { return []; }
        return [.. Config.ShaderConfigs.Select(e => new AssetProviderItem(assets.Config.ShaderGroup, e.Name, string.Empty))];
    }

    public override void RunLoad(Assets assets, AssetCollection collection) {
        if (Config == null) { return; }

        AssetProviderItem[] items = Filter(assets, collection);
        if (items.Length == 0) { return; }

        string shaderExtension = GetShaderExtension(assets);

        foreach (ShaderConfigEntry entry in Config.ShaderConfigs.Where(e => items.Any(i => i.Name == e.Name))) {
            AssetProviderItem item = new(assets.Config.ShaderGroup, entry.Path, shaderExtension);
            if (!assets.Provider.HasItem(item)) { continue; }

            using Stream shaderStream = assets.Provider.GetItemStream(item);
            byte[] bytes = new byte[shaderStream.Length];
            shaderStream.ReadExactly(bytes);

            ShaderCreateInfo createInfo = new(
                Code: bytes,
                Stage: entry.Stage,
                SamplerCount: entry.Samplers,
                UniformBufferCount: entry.Uniforms,
                StorageBufferCount: entry.StorageBuffers
            );

            Shader shader;
            lock (assets.GraphicsDevice) {
                shader = new(assets.GraphicsDevice, createInfo);
            }
            AddAsset(assets, entry.Name, shader);
        }
    }

    public override void RunUnload(Assets assets, AssetCollection collection) {
        foreach (AssetProviderItem item in Filter(assets, collection)) {
            RemoveAsset<Shader>(assets, item.Name);
        }
    }

    static ShaderConfig? GetConfig(Assets assets) {
        string configName = Path.GetFileNameWithoutExtension(Path.GetRelativePath(assets.Config.ShaderGroup, assets.Config.ShaderConfig));
        string configExtension = Path.GetExtension(assets.Config.ShaderConfig);
        AssetProviderItem configItem = new(assets.Config.ShaderGroup, configName, configExtension);
        if (!assets.Provider.HasItem(configItem)) { return null; }

        using Stream configStream = assets.Provider.GetItemStream(configItem);
        ShaderConfig? shaderConfig = null;
        try {
            shaderConfig = JsonSerializer.Deserialize(configStream, SourceGenerationContext.Default.ShaderConfig);
        } catch { return null; }

        return shaderConfig;
    }

    static string GetShaderExtension(Assets assets) => $".{assets.GraphicsDevice.Driver.GetShaderExtension()}";

    static AssetProviderItem[] Filter(Assets assets, AssetCollection collection)
        => collection.Filter(assets.Config.ShaderGroup, string.Empty);
}