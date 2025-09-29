using System.Text.Json.Serialization;
using Foster.Audio;

namespace Jackdaw.Audio.FosterAudio;

/// <summary>
/// Per-sound configuration data.
/// </summary>
internal class SoundConfig {
    /// <summary>
    /// The loading method to default to when nothing is assigned.
    /// </summary>
    public const SoundLoadingMethod DefaultLoadingMethod = SoundLoadingMethod.Preload;

    /// <summary>
    /// Individual song file configurations.
    /// </summary>
    [JsonPropertyName("entries")]
    public SoundConfigEntry[] SoundConfigs { get; set; } = [];
}

/// <summary>
/// Configurations for a single sound file.
/// </summary>
internal class SoundConfigEntry {
    /// <summary>
    /// The sound file name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The method used for loading the sound.
    /// </summary>
    [JsonPropertyName("loadingMethod")]
    [JsonConverter(typeof(JsonStringEnumConverter<SoundLoadingMethod>))]
    public SoundLoadingMethod LoadingMethod { get; set; } = SoundConfig.DefaultLoadingMethod;
}