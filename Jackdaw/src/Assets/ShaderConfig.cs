using System.Text.Json.Serialization;

namespace Jackdaw;

internal class ShaderConfig {
    [JsonPropertyName("entries")]
    public ShaderConfigEntry[] ShaderConfigs { get; set; } = [];
}

internal class ShaderConfigEntry {
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("vertex")]
    public ShaderConfigEntryStage Vertex { get; set; } = new();

    [JsonPropertyName("fragment")]
    public ShaderConfigEntryStage Fragment { get; set; } = new();
}

internal class ShaderConfigEntryStage {
    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("entryPoint")]
    public string EntryPoint { get; set; } = "main";

    [JsonPropertyName("samplers")]
    public int Samplers { get; set; } = 0;

    [JsonPropertyName("uniforms")]
    public int Uniforms { get; set; } = 0;
}