using System.Text.Json.Serialization;

namespace Jackdaw.Audio.FosterAudio;

/// <summary>
/// All configuration data the manager uses to load sound data.
/// </summary>
public struct AudioConfig() {
    /// <summary>
    /// All sound bus data to be loaded by default.
    /// </summary>
    [JsonPropertyName("buses")]
    public AudioBusConfig[] Buses { get; set; } = [];

    /// <summary>
    /// The sound bus to default to in edge cases.
    /// </summary>
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
    /// <summary>
    /// The sound bus name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The parented sound bus the new bus relative to. Leave empty if the bus has no parent.
    /// </summary>
    [JsonPropertyName("parent")]
    public string Parent { get; set; } = string.Empty;

    /// <summary>
    /// The default volume to set the bus to.
    /// </summary>
    [JsonPropertyName("defaultVolume")]
    public float DefaultVolume { get; set; } = 0.5f;
}