using Foster.Audio;

namespace LittleLib;

public class LoopingSoundPlayerComponent(LittleGame game, Sound sound, string? bus = null) : SoundPlayerComponent(game, sound, bus) {
    public float? LoopStart;
    public float? LoopEnd;

    public override void Play() {
        base.Play();
        Player.Looping = true;
        if (LoopStart != null) { Player.LoopBegin = TimeSpan.FromMilliseconds((long)LoopStart); }
        if (LoopEnd != null) { Player.LoopEnd = TimeSpan.FromMilliseconds((long)LoopEnd); }
    }
}