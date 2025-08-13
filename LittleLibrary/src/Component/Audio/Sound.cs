using Foster.Audio;

namespace LittleLib;

public class SoundPlayerComponent(LittleGame game, Sound sound, string? bus = null) : Component(game) {
    protected readonly Sound Sound = sound;
    protected readonly string? Bus = bus;
    protected SoundInstance Player;

    public bool Autostart = false;

    public virtual void Play() {
        Stop();
        Player = Game.Audio.Play(Sound, Bus);
    }

    public void Stop() {
        Player.Stop();
    }

    protected override void EnterTree() {
        if (Autostart) { Play(); }
    }

    protected override void ExitTree() {
        Stop();
    }
}