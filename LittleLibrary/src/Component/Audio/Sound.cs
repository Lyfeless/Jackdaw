using Foster.Audio;

namespace LittleLib;

public class SoundPlayerComponent(LittleGame game, Sound sound, SoundGroup? group = null) : Component(game) {
    protected readonly Sound Sound = sound;
    protected readonly SoundGroup? Group = group;
    protected SoundInstance Player;

    public bool Autostart = false;

    public virtual void Play() {
        Stop();
        Player = Sound.Play(Group);
    }

    public void Stop() {
        Player.Stop();
    }

    public override void EnterTree() {
        if (Autostart) { Play(); }
    }

    public override void ExitTree() {
        Stop();
    }
}