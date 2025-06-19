using System.Text.Json.Serialization;
using Foster.Audio;

namespace LittleLib;

internal class SoundConfig {
    public const SoundLoadingMethod DefaultLoadingMethod = SoundLoadingMethod.Preload;

    [JsonPropertyName("entries")]
    public SoundConfigEntry[] SoundConfigs { get; set; } = [];
}

internal class SoundConfigEntry {
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("loadingMethod")]
    [JsonConverter(typeof(JsonStringEnumConverter<SoundLoadingMethod>))]
    public SoundLoadingMethod LoadingMethod { get; set; } = SoundConfig.DefaultLoadingMethod;
}