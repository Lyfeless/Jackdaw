using Foster.Audio;

namespace Jackdaw.Audio.FosterAudio;

/// <summary>
/// A component for playing a random sound from a pool of multiple sounds.
/// </summary>
/// <param name="manager">The audio manager.</param>
/// <param name="sounds">All sound options that can be played.</param>
/// <param name="bus">The bus to play the sound on.</param>
public class SoundPoolPlayerComponent(AudioManager manager, Sound[] sounds, string? bus = null) : SoundPlayerComponent(manager, manager.GetSound("error"), bus) {
    public Sound[] Sounds = sounds;

    /// <summary>
    /// A component for playing a random sound from a pool of multiple sounds.
    /// </summary>
    /// <param name="manager">The audio manager.</param>
    /// <param name="sounds">All sound options that can be played.</param>
    /// <param name="bus">The bus to play the sound on.</param>
    public SoundPoolPlayerComponent(AudioManager manager, string[] sounds, string? bus = null) : this(manager, [.. sounds.Select(manager.GetSound)], bus) { }

    /// <summary>
    /// A component for playing a random sound from a pool of multiple sounds.
    /// Uses multiple sounds all names with the same naming scheme of baseName#
    /// </summary>
    /// <param name="manager">The audio manager.</param>
    /// <param name="baseName">The base name all sound files have.</param>
    /// <param name="count">The number of sounds in the pool</param>
    /// <param name="startIndex">The number to start the naming count at.</param>
    /// <param name="bus">The bus to play the sound on.</param>
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

/// <summary>
/// A component for playing a random sound from a pool of multiple sounds, with some sounds being more likely than others.
/// </summary>
/// <param name="manager">The audio manager.</param>
/// <param name="sounds">All sound options that can be played.</param>
/// <param name="bus">The bus to play the sound on.</param>
public class WeightedSoundPoolPlayerComponent(AudioManager manager, WeightedRandom<Sound> sounds, string? bus = null) : SoundPlayerComponent(manager, manager.GetSound("error"), bus) {
    /// <summary>
    /// The weighted sound container.
    /// </summary>
    public WeightedRandom<Sound> Sounds = sounds;

    /// <summary>
    /// A component for playing a random sound from a pool of multiple sounds, with some sounds being more likely than others.
    /// Leaves an empty sound container to add to later.
    /// </summary>
    /// <param name="manager">The audio manager.</param>
    /// <param name="bus">The bus to play the sound on.</param>
    public WeightedSoundPoolPlayerComponent(AudioManager manager, string? bus = null) : this(manager, new WeightedRandom<Sound>(manager.Game), bus) { }

    public override void Play() {
        Sound = Sounds.Get();
        base.Play();
    }

    /// <summary>
    /// Add a sound to the weighted pool.
    /// </summary>
    /// <param name="sound">The sound to add.</param>
    /// <param name="weight">The weight chance the sound plays.</param>
    /// <returns>The weighted sound pool.</returns>
    public WeightedSoundPoolPlayerComponent Add(string sound, int weight) => Add(Manager.GetSound(sound), weight);

    /// <summary>
    /// Add a sound to the weighted pool.
    /// </summary>
    /// <param name="sound">The sound to add.</param>
    /// <param name="weight">The weight chance the sound plays.</param>
    /// <returns>The weighted sound pool.</returns>
    public WeightedSoundPoolPlayerComponent Add(Sound sound, int weight) {
        Sounds.Add(sound, weight);
        return this;
    }
}