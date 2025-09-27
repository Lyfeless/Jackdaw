using FMOD.Studio;

namespace Jackdaw.Audio.FMODAudio;

public class SoundEvent(EventDescription description) {
    readonly EventDescription Description = description;
    EventInstance Instance;

    public bool Paused {
        get {
            if (!Instance.isValid()) { return false; }
            Instance.getPaused(out bool paused);
            return paused;
        }
        set {
            if (!Instance.isValid()) { return; }
            Instance.setPaused(value);
        }
    }

    public float Pitch {
        get {
            if (!Instance.isValid()) { return 1; }
            Instance.getPitch(out float pitch);
            return pitch;
        }
        set {
            if (!Instance.isValid()) { return; }
            Instance.setPitch(value);
        }
    }

    public int TimelinePosition {
        get {
            if (!Instance.isValid()) { return 0; }
            Instance.getTimelinePosition(out int timelinePosition);
            return timelinePosition;
        }
        set {
            if (!Instance.isValid()) { return; }
            Instance.setTimelinePosition(value);
        }
    }

    public float Volume {
        get {
            if (!Instance.isValid()) { return 0; }
            Instance.getVolume(out float volume);
            return volume;
        }
        set {
            if (!Instance.isValid()) { return; }
            Instance.setVolume(value);
        }
    }

    //! FIXME (Alex): Needs support for 3d position and properties/parameters

    public void Play(bool release = true) {
        if (!Description.isValid() || Description.createInstance(out Instance) != FMOD.RESULT.OK) { return; }
        Instance.start();
        if (release) { Instance.release(); }
    }

    public void Stop(bool immediate = false, bool release = true) {
        if (!Instance.isValid()) { return; }
        Instance.stop(immediate ? STOP_MODE.IMMEDIATE : STOP_MODE.ALLOWFADEOUT);
        if (release) { Instance.release(); }
    }

    public void Release() {
        if (!Instance.isValid()) { return; }
        Instance.release();
    }
}