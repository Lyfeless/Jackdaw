using Foster.Audio;

namespace Jackdaw.Audio.FosterAudio;

public class SoundPoolPlayerComponent(AudioManager manager, Sound[] sounds, string? bus = null) : SoundPlayerComponent(manager, manager.GetSound("error"), bus) {
    public Sound[] Sounds = sounds;

    public SoundPoolPlayerComponent(AudioManager manager, string[] sounds, string? bus = null) : this(manager, [.. sounds.Select(manager.GetSound)], bus) { }
    public SoundPoolPlayerComponent(AudioManager manager, string baseName, int count, int startIndex = 0, string? bus = null) : this(manager, NamesFromBase(baseName, count, startIndex), bus) { }

    public override void Play() {
        Sound = Sounds[Game.Random.Int(Sounds.Length)];
        base.Play();
    }

    static string[] NamesFromBase(string baseName, int count, int startIndex) {
        string[] names = new string[count];
        for (int i = 0; i < count; ++i) {
            names[i] = $"{baseName}{i + startIndex}";
        }
        return names;
    }
}

public class WeightedSoundPoolPlayerComponent(AudioManager manager, WeightedRandom<Sound> sounds, string? bus = null) : SoundPlayerComponent(manager, manager.GetSound("error"), bus) {
    public WeightedRandom<Sound> Sounds = sounds;

    public WeightedSoundPoolPlayerComponent(AudioManager manager, string? bus = null) : this(manager, new WeightedRandom<Sound>(manager.Game), bus) { }

    public override void Play() {
        Sound = Sounds.Get();
        base.Play();
    }

    public WeightedSoundPoolPlayerComponent Add(string sound, int weight) => Add(Manager.GetSound(sound), weight);
    public WeightedSoundPoolPlayerComponent Add(Sound sound, int weight) {
        Sounds.Add(sound, weight);
        return this;
    }
}