using System.Text.Json.Serialization;

namespace Jackdaw.Audio.FosterAudio;

public struct AudioConfig() {
    [JsonPropertyName("buses")]
    public AudioBusConfig[] Buses { get; set; } = [];

    [JsonPropertyName("defaultBus")]
    public string DefaultBus { get; set; } = string.Empty;

    /// <summary>
    /// The folder to search for sound files. </br>
    /// Defaults to "Sounds".
    /// Relative to asset root folder.
    /// </summary>
    [JsonPropertyName("soundFolder")]
    public string SoundFolder { get; set; } = "Sounds";

    /// <summary>
    /// The location of the sound config data.
    /// Defaults to "Sounds/config.json".
    /// Relative to asset root folder.
    /// </summary>
    [JsonPropertyName("soundConfig")]
    public string SoundConfig { get; set; } = "Sounds/config.json";
}

public struct AudioBusConfig() {
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("parent")]
    public string Parent { get; set; } = string.Empty;

    [JsonPropertyName("defaultVolume")]
    public float DefaultVolume { get; set; } = 0.5f;
}