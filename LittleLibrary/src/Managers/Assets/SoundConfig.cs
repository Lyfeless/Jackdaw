using System.Text.Json.Serialization;
using Foster.Audio;

namespace LittleLib;

public class SoundConfig {
    public const SoundLoadingMethod DefaultLoadingMethod = SoundLoadingMethod.Preload;

    [JsonPropertyName("entries")]
    public SoundConfigEntry[] SoundConfigs { get; set; } = [];
}

public class SoundConfigEntry {
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("loadingMethod")]
    [JsonConverter(typeof(JsonStringEnumConverter<SoundLoadingMethod>))]
    public SoundLoadingMethod LoadingMethod { get; set; } = SoundConfig.DefaultLoadingMethod;
}