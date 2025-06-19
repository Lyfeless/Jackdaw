using Foster.Audio;

namespace LittleLib;

public class RandomPitchSoundPlayerComponent(LittleGame game, Sound sound, float pitchRange, SoundGroup? group = null) : SoundPlayerComponent(game, sound, group) {
    public float PitchRange = pitchRange;

    public override void Play() {
        base.Play();
        Player.Pitch += Game.Random.Float(PitchRange * 2) - PitchRange;
    }
}