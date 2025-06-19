using System.Text.Json.Serialization;

namespace LittleLib;

internal class FontConfig {
    public const int DefaultFontSize = 16;

    [JsonPropertyName("entries")]
    public FontConfigEntry[] FontConfigs { get; set; } = [];
}

internal class FontConfigEntry {
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("size")]
    public int Size { get; set; }
}