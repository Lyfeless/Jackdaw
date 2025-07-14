using Foster.Audio;

namespace LittleLib;

public class AudioManager {
    readonly Dictionary<string, SoundGroup> Buses = [];
    SoundGroup DefaultBus;

    public AudioManager() {
        Startup();
        DefaultBus = new();
    }

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

    public void SetDefaultBus(string name) {
        DefaultBus = GetBus(name) ?? DefaultBus;
    }

    public SoundInstance Play(Sound sound, string? bus) {
        return sound.Play(GetBus(bus));
    }

    public void Startup() {
        Audio.Startup();
    }
    public void Update() => Audio.Update();
    public void Shutdown() => Audio.Shutdown();
}