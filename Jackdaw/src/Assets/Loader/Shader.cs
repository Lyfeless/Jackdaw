using System.Text.Json;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Asset loader for importing shaders using compiled shader files and configuration data.
/// </summary>
public class ShaderLoader() : AssetLoaderStage() {
    public override void Run(Assets assets) {
        // With shader rework, a fallback really doesn't make sense. Crash if shaders are misconfigured.
        // assets.SetFallback();

        string configPath = Path.Join(assets.Config.RootFolder, assets.Config.ShaderConfig);
        if (!Path.Exists(configPath)) { return; }

        // Shaders are built from more data than just files, so read directly off the config
        ShaderConfig? shaderConfig = JsonSerializer.Deserialize(File.ReadAllText(configPath), SourceGenerationContext.Default.ShaderConfig);
        if (shaderConfig == null) { return; }

        string shaderExtension = assets.GraphicsDevice.Driver.GetShaderExtension(); assets.GraphicsDevice.Driver.GetShaderExtension();
        string shaderPath = Path.Join(assets.Config.RootFolder, assets.Config.ShaderFolder);

        foreach (ShaderConfigEntry entry in shaderConfig.ShaderConfigs) {
            string path = Path.Join(shaderPath, $"{entry.Path}.{shaderExtension}");
            if (!Path.Exists(path)) { continue; }

            byte[] bytes;
            bytes = File.ReadAllBytes(path);

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