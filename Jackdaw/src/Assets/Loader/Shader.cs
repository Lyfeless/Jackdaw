using System.Text.Json;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Asset loader for importing shaders using compiled shader files and configuration data.
/// </summary>
public class ShaderLoader() : AssetLoaderStage() {
    public override void Run(Assets assets) {
        assets.SetFallback(new BatcherShader(assets.GraphicsDevice));

        string configPath = Path.Join(assets.Config.RootFolder, assets.Config.ShaderConfig);
        if (!Path.Exists(configPath)) { return; }

        // Shaders are built from more data than just files, so read directly off the config
        ShaderConfig? shaderConfig = JsonSerializer.Deserialize(File.ReadAllText(configPath), SourceGenerationContext.Default.ShaderConfig);
        if (shaderConfig == null) { return; }

        string shaderExtension = assets.GraphicsDevice.Driver.GetShaderExtension(); assets.GraphicsDevice.Driver.GetShaderExtension();
        string shaderPath = Path.Join(assets.Config.RootFolder, assets.Config.ShaderFolder);

        foreach (ShaderConfigEntry entry in shaderConfig.ShaderConfigs) {
            string vertexPath = Path.Join(shaderPath, $"{entry.Vertex.Path}.{shaderExtension}");
            string fragmentPath = Path.Join(shaderPath, $"{entry.Fragment.Path}.{shaderExtension}");
            if (!Path.Exists(vertexPath) || !Path.Exists(fragmentPath)) { continue; }

            byte[] vertexBytes;
            byte[] fragmentBytes;
            vertexBytes = File.ReadAllBytes(vertexPath);
            fragmentBytes = vertexPath == fragmentPath ? vertexBytes : File.ReadAllBytes(fragmentPath);

            ShaderCreateInfo createInfo = new(
                Vertex: new(
                    Code: vertexBytes,
                    SamplerCount: entry.Vertex.Samplers,
                    UniformBufferCount: entry.Vertex.Uniforms,
                    EntryPoint: entry.Vertex.EntryPoint
                ),
                Fragment: new(
                    Code: fragmentBytes,
                    SamplerCount: entry.Fragment.Samplers,
                    UniformBufferCount: entry.Fragment.Uniforms,
                    EntryPoint: entry.Fragment.EntryPoint
                )
            );

            assets.Add(entry.Name, new Shader(assets.GraphicsDevice, createInfo));
        }
    }
}