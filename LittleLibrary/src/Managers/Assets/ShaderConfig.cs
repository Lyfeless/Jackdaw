using System.Text.Json.Serialization;

namespace LittleLib;

public class ShaderConfig {
    [JsonPropertyName("entries")]
    public ShaderConfigEntry[] ShaderConfigs { get; set; } = [];
}

public class ShaderConfigEntry {
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("vertex")]
    public string Vertex { get; set; } = string.Empty;

    [JsonPropertyName("fragment")]
    public string Fragment { get; set; } = string.Empty;
}