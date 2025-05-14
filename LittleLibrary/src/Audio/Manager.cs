using Foster.Audio;

namespace LittleLib;

public class AudioManager {
    LittleGame Game;

    public readonly Dictionary<string, SoundGroup> Groups = [];

    readonly List<SoundInstance> loopingSounds = [];

    public AudioManager(LittleGame game) {
        Game = game;
        Audio.Startup();
    }

    public void Shutdown() => Audio.Shutdown();

    public void Update() => Audio.Update();

    public void AddGroup(string name, string? parent = null, float volume = 0.5f) {
        if (Groups.ContainsKey(name)) { return; }

        SoundGroup group = new(
            name,
            (parent != null && Groups.TryGetValue(parent, out SoundGroup? value)) ? value : null
        );
        Groups.Add(name, group);
        group.Volume = volume;
    }

    public SoundInstance PlayLooping(string name, string group) {
        SoundInstance instance = Play(name, group);
        instance.Looping = true;
        loopingSounds.Add(instance);
        return instance;
    }

    public SoundInstance PlayRandom(string[] names, string group, float pitchRange = 0) {
        int index = Game.Random.Int(names.Length);
        string name = names[index];
        return Play(name, group, pitchRange);
    }

    public SoundInstance PlayRandom(string namePrefix, int itemCount, string group, float pitchRange = 0) {
        int index = Game.Random.Int(itemCount);
        string name = namePrefix + index;
        return Play(name, group, pitchRange);
    }

    public SoundInstance Play(string name, string group, float pitchRange = 0) {
        if (!Groups.TryGetValue(group, out SoundGroup? groupValue)) { return new SoundInstance(); }

        SoundInstance instance = Game.Assets.GetSound(name).Play(groupValue);
        if (pitchRange != 0) {
            instance.Pitch += Game.Random.Float(pitchRange * 2) - pitchRange;
        }
        return instance;
    }

    public void StopAllLooping(string? group = null) {
        for (int i = loopingSounds.Count - 1; i >= 0; --i) {
            if (group == null || loopingSounds[i].Group?.Name == group) {
                loopingSounds[i].Stop();
                loopingSounds.RemoveAt(i);
            }
        }
    }

    public void StopAll(string? group = null) {
        if (group == null) {
            Audio.StopAll();
            loopingSounds.Clear();
        }
        else {
            Groups[group].StopAll();
            loopingSounds.RemoveAll(i => i.Group?.Name == group);
        }
    }
}