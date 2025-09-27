namespace Jackdaw.Audio.FMODAudio;

public class SoundPlayerComponent(Game game, SoundEvent soundEvent, bool autoplay = false) : Component(game) {
    readonly SoundEvent Sound = soundEvent;
    public bool Autoplay = autoplay;

    public bool Paused {
        get => Sound.Paused;
        set => Sound.Paused = value;
    }

    public float Pitch {
        get => Sound.Pitch;
        set => Sound.Pitch = value;
    }

    public int TimelinePosition {
        get => Sound.TimelinePosition;
        set => Sound.TimelinePosition = value;
    }

    public float Volume {
        get => Sound.Volume;
        set => Sound.Volume = value;
    }

    //! FIXME (Alex): Needs support for 3d position and properties/parameters

    protected override void EnterTree() {
        if (Autoplay) { Play(); }
    }

    protected override void ExitTree() {
        Stop();
    }

    protected override void Invalidated() {
        Sound.Release();
    }

    public void Play() {
        Sound.Play(false);
    }

    public void Stop(bool immediate = false) {
        Sound.Stop(immediate, false);
    }
}