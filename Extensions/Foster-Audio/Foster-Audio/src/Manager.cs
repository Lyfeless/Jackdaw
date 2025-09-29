using System.Reflection;
using System.Text.Json;
using Foster.Audio;
using Foster.Framework;

namespace Jackdaw.Audio.FosterAudio;

/// <summary>
/// A component responsible for loading and playing sound files.
/// Runs setup and update once it's added to the main actor tree.
/// </summary>
/// <param name="game">The current game instance.</param>
/// <param name="config">The audio configuration data to load the sounds with.</param>
public class AudioManager(Game game, AudioConfig config) : Component(game) {
    AudioConfig Config = config;

    readonly Dictionary<string, SoundGroup> Buses = [];
    SoundGroup DefaultBus;

    public readonly string[] SoundExtensions = [".wav", ".mp3", ".ogg"];

    readonly Dictionary<string, Sound> Sounds = [];

    /// <summary>
    /// Find a sound from the loaded sound data.
    /// </summary>
    /// <param name="name">The asset name.</param>
    /// <returns>The requested sound, or the default sound if nothing was found.</returns>
    public Sound GetSound(string name) {
        if (Sounds.TryGetValue(name, out Sound? output)) { return output; }
        Log.Warning($"ASSETS: Failed to find sound {name}, returning default");
        return Sounds["error"];
    }
    const string SoundFallbackName = "Fallback.sound.ogg";

    /// <summary>
    /// Create a new sound bus.
    /// </summary>
    /// <param name="name">The bus name.</param>
    /// <param name="parent">The parented bus the new bus relative to, treated as a master bus if not assigned.</param>
    /// <param name="volume">The default volume to set the bus to.</param>
    public void AddBus(string name, string? parent = null, float volume = 0.5f) {
        if (!Buses.ContainsKey(name)) {
            Buses.Add(name, new SoundGroup(name, GetBus(parent)) {
                Volume = volume
            });
        }
    }

    /// <summary>
    /// Get a sound bus.
    /// </summary>
    /// <param name="name">The name of the sound bus to return.</param>
    /// <returns>The sound bus, null if no matching bus is found.</returns>
    public SoundGroup? GetBus(string? name) {
        if (name == null) { return null; }

        if (Buses.TryGetValue(name, out SoundGroup? value)) {
            return value;
        }

        return DefaultBus;
    }

    /// <summary>
    /// Play a sound.
    /// </summary>
    /// <param name="sound">The sound to play.</param>
    /// <param name="bus">The bus to play the sound on.</param>
    /// <returns>A reference to the playing sound.</returns>
    public SoundInstance Play(Sound sound, string? bus) => Play(sound, GetBus(bus));

    /// <summary>
    /// Play a sound.
    /// </summary>
    /// <param name="sound">The sound to play</param>
    /// <param name="bus">The bus to play the sound on.</param>
    /// <returns>A reference to the playing sound.</returns>
    public static SoundInstance Play(Sound sound, SoundGroup? bus) {
        return sound.Play(bus);
    }

    protected override void EnterTreeFirst() {
        Foster.Audio.Audio.Startup();

        DefaultBus = new();
        foreach (AudioBusConfig bus in Config.Buses) {
            AddBus(bus.Name, bus.Parent != string.Empty ? bus.Parent : null, bus.DefaultVolume);
        }

        if (Config.DefaultBus != string.Empty) {
            SoundGroup? bus = GetBus(Config.DefaultBus);
            if (bus != null) { DefaultBus = bus; }
        }

        // Assembly data for fallback
        Assembly assembly = Assembly.GetExecutingAssembly();
        string? assemblyName = assembly.GetName().Name;

        // Load fallback sound
        using Stream stream = assembly.GetManifestResourceStream($"{assemblyName}.{SoundFallbackName}")!;
        Sounds.Add("error", new Sound(stream));

        string soundPath = Path.Join(Game.Assets.Config.RootFolder, Config.SoundFolder);
        if (Directory.Exists(soundPath)) {
            string configPath = Path.Join(Game.Assets.Config.RootFolder, Config.SoundConfig);
            SoundConfig? soundConfig = Path.Exists(configPath) ? JsonSerializer.Deserialize(File.ReadAllText(configPath), SourceGenerationContext.Default.SoundConfig) : null;

            foreach (string file in Directory.EnumerateFiles(soundPath, "*.*", SearchOption.AllDirectories).Where(e => SoundExtensions.Any(e.EndsWith))) {
                string name = Assets.GetAssetName(soundPath, file);
                SoundConfigEntry? configEntry = soundConfig?.SoundConfigs.FirstOrDefault(e => e.Name == name);
                Sounds.Add(name, new Sound(file, configEntry?.LoadingMethod ?? SoundConfig.DefaultLoadingMethod));
            }
        }
    }
    protected override void Update() => Foster.Audio.Audio.Update();
    protected override void Invalidated() => Foster.Audio.Audio.Shutdown();
}