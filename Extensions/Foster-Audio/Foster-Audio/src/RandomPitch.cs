using Foster.Audio;

namespace LittleLib.Audio.FosterAudio;

public class RandomPitchSoundPlayerComponent(AudioManager manager, Sound sound, float pitchRange, string? bus = null) : SoundPlayerComponent(manager, sound, bus) {
    public float PitchRange = pitchRange;

    public override void Play() {
        base.Play();
        Player.Pitch += Game.Random.Float(PitchRange * 2) - PitchRange;
    }
}