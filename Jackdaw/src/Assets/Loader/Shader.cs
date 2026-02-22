using System.Text.Json;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Asset loader for importing shaders using compiled shader files and configuration data.
/// </summary>
public class ShaderLoader() : AssetLoaderStage() {
    public override void Run(Assets assets) {
        string configName = Path.GetFileNameWithoutExtension(Path.GetRelativePath(assets.Config.ShaderGroup, assets.Config.ShaderConfig));
        string configExtension = Path.GetExtension(assets.Config.ShaderConfig);
        AssetProviderItem configItem = new(assets.Config.ShaderGroup, configName, configExtension);
        if (!assets.Provider.HasItem(configItem)) { return; }

        using Stream configStream = assets.Provider.GetItemStream(configItem);
        ShaderConfig? shaderConfig;
        try {
            shaderConfig = JsonSerializer.Deserialize(configStream, SourceGenerationContext.Default.ShaderConfig);
        } catch { return; }
        if (shaderConfig == null) { return; }

        string shaderExtension = $".{assets.GraphicsDevice.Driver.GetShaderExtension()}";

        foreach (ShaderConfigEntry entry in shaderConfig.ShaderConfigs) {
            AssetProviderItem item = new(assets.Config.ShaderGroup, entry.Name, shaderExtension);
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

            assets.Add(entry.Name, new Shader(assets.GraphicsDevice, createInfo));
        }
    }
}