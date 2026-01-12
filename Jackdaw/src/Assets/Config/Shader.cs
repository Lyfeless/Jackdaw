using System.Text.Json.Serialization;
using Foster.Framework;

namespace Jackdaw;

internal class ShaderConfig {
    [JsonPropertyName("entries")]
    public ShaderConfigEntry[] ShaderConfigs { get; set; } = [];
}

internal class ShaderConfigEntry {
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("stage")]
    [JsonConverter(typeof(JsonStringEnumConverter<ShaderStage>))]
    public ShaderStage Stage { get; set; } = ShaderStage.Fragment;

    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("entryPoint")]
    public string EntryPoint { get; set; } = "main";

    [JsonPropertyName("samplers")]
    public int Samplers { get; set; } = 0;

    [JsonPropertyName("uniforms")]
    public int Uniforms { get; set; } = 0;

    [JsonPropertyName("storageBuffers")]
    public int StorageBuffers { get; set; } = 0;
}