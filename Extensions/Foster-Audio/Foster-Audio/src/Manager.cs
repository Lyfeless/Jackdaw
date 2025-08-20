using System.Reflection;
using System.Text.Json;
using Foster.Audio;

namespace LittleLib.Audio.FosterAudio;

public class AudioManager(LittleGame game, AudioConfig config) : Component(game) {
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
        Console.WriteLine($"ASSETS: Failed to find sound {name}, returning default");
        return Sounds["error"];
    }
    const string SoundFallbackName = "Fallback.sound.ogg";

    public void AddBus(string name, string? parent = null, float volume = 0.5f) {
        if (!Buses.ContainsKey(name)) {
            Buses.Add(name, new SoundGroup(name, GetBus(parent)) {
                Volume = volume
            });
        }
    }

    public SoundGroup? GetBus(string? name) {
        if (name == null) { return null; }

        if (Buses.TryGetValue(name, out SoundGroup? value)) {
            return value;
        }

        return DefaultBus;
    }

    public SoundInstance Play(Sound sound, string? bus) => Play(sound, GetBus(bus));

    public static SoundInstance Play(Sound sound, SoundGroup? bus) {
        return sound.Play(bus);
    }

    protected override void EnterTree() {
        Console.WriteLine("startup");
        Foster.Audio.Audio.Startup();

        Console.WriteLine("create buses");
        DefaultBus = new();
        foreach (AudioBusConfig bus in Config.Buses) {
            AddBus(bus.Name, bus.Parent != string.Empty ? bus.Parent : null, bus.DefaultVolume);
        }

        Console.WriteLine("set default");
        if (Config.DefaultBus != string.Empty) {
            SoundGroup? bus = GetBus(Config.DefaultBus);
            if (bus != null) { DefaultBus = bus; }
        }

        Console.WriteLine("assembly");
        // Assembly data for fallback
        Assembly assembly = Assembly.GetExecutingAssembly();
        string? assemblyName = assembly.GetName().Name;
        Console.WriteLine(string.Join(", ", assembly.GetName().Name));
        Console.WriteLine(string.Join(", ", assembly.GetManifestResourceNames()));

        Console.WriteLine("stream and error");
        // Load fallback sound
        using Stream stream = assembly.GetManifestResourceStream($"{assemblyName}.{SoundFallbackName}")!;
        Sounds.Add("error", new Sound(stream));

        Console.WriteLine("sounds");
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
    protected override void ExitTree() => Foster.Audio.Audio.Shutdown();
}