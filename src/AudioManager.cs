using Foster.Audio;
using Foster.Framework;

namespace LittleLib;

//! FIXME (Alex): audio setup crashes on startup unless this is registered ahead of time, look into a better method for the future
public class AudioModule : Module {
    public override void Startup() => Audio.Startup();
    public override void Update() => Audio.Update();
    public override void Shutdown() => Audio.Shutdown();
}

public static class AudioManager {
    public static readonly Dictionary<string, SoundGroup> Groups = [];

    static readonly List<SoundInstance> loopingSounds = [];

    public static void AddGroup(string name, string? parent = null, float volume = 0.5f) {
        if (Groups.ContainsKey(name)) { return; }

        SoundGroup group = new(
            name,
            (parent != null && Groups.TryGetValue(parent, out SoundGroup? value)) ? value : null
        );
        Groups.Add(name, group);
        group.Volume = volume;
    }

    public static SoundInstance PlayLooping(string name, string group) {
        SoundInstance instance = Play(name, group);
        instance.Looping = true;
        loopingSounds.Add(instance);
        return instance;
    }

    public static SoundInstance PlayRandom(string[] names, string group, float pitchRange = 0) {
        int index = Util.random.Int(names.Length);
        string name = names[index];
        return Play(name, group, pitchRange);
    }

    public static SoundInstance PlayRandom(string namePrefix, int itemCount, string group, float pitchRange = 0) {
        int index = Util.random.Int(itemCount);
        string name = namePrefix + index;
        return Play(name, group, pitchRange);
    }

    public static SoundInstance Play(string name, string group, float pitchRange = 0) {
        if (!Groups.TryGetValue(group, out SoundGroup? groupValue)) { return new SoundInstance(); }

        SoundInstance instance = Assets.GetSound(name).Play(groupValue);
        if (pitchRange != 0) {
            instance.Pitch += Util.random.Float(pitchRange * 2) - pitchRange;
        }
        return instance;
    }

    public static void StopAllLooping(string? group = null) {
        for (int i = loopingSounds.Count - 1; i >= 0; --i) {
            if (group == null || loopingSounds[i].Group?.Name == group) {
                loopingSounds[i].Stop();
                loopingSounds.RemoveAt(i);
            }
        }
    }

    public static void StopAll(string? group = null) {
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