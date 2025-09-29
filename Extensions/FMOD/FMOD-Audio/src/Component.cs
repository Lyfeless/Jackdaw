using System.Numerics;
using FMOD;
using FMOD.Studio;

namespace Jackdaw.Audio.FMODAudio;

/// <summary>
/// A component responsible for playing sounds from a sound event.
/// </summary>
/// <param name="game">The current game instance.</param>
/// <param name="soundEvent">The sound event the play.</param>
/// <param name="autoplay">If the sound should start playing automatically when added to the actor tree.</param>
public class SoundPlayerComponent(Game game, SoundEvent soundEvent, bool autoplay = false) : Component(game) {
    readonly SoundEvent Sound = soundEvent;
    public bool Autoplay = autoplay;

    /// <summary>
    /// A component responsible for playing sounds from a sound event.
    /// </summary>
    /// <param name="audio">The audio manager.</param>
    /// <param name="soundEvent">The sound event to play.</param>
    /// <param name="autoplay">If the sound should start playing automatically when added to the actor tree.</param>
    public SoundPlayerComponent(AudioManager audio, string soundEvent, bool autoplay = false) : this(audio.Game, audio, soundEvent, autoplay) { }

    /// <summary>
    /// A component responsible for playing sounds from a sound event.
    /// </summary>
    /// <param name="game">The current game instance.</param>
    /// <param name="audio">The audio manager.</param>
    /// <param name="soundEvent">The sound event to play.</param>
    /// <param name="autoplay">If the sound should start playing automatically when added to the actor tree.</param>
    public SoundPlayerComponent(Game game, AudioManager audio, string soundEvent, bool autoplay = false) : this(game, audio.Get(soundEvent), autoplay) { }

    /// <summary>
    /// If the sound is paused.
    /// </summary>
    public bool Paused {
        get => Sound.Paused;
        set => Sound.Paused = value;
    }

    /// <summary>
    /// The sound's current pitch.
    /// </summary>
    public float Pitch {
        get => Sound.Pitch;
        set => Sound.Pitch = value;
    }

    /// <summary>
    /// The current playing position in the audio event timeline.
    /// </summary>
    public int TimelinePosition {
        get => Sound.TimelinePosition;
        set => Sound.TimelinePosition = value;
    }

    /// <summary>
    /// The sound's volume.
    /// </summary>
    public float Volume {
        get => Sound.Volume;
        set => Sound.Volume = value;
    }

    /// <summary>
    /// All data relating to the sound's playing position in 3D space.
    /// </summary>
    public ATTRIBUTES_3D PositionAttributes {
        get => Sound.PositionAttributes;
        set => Sound.PositionAttributes = value;
    }

    /// <summary>
    /// The sound's 3D playing position.
    /// </summary>
    public Vector3 Position3D {
        get => Sound.Position3D;
        set => Sound.Position3D = value;
    }

    /// <summary>
    /// The sound's 3D velocity.
    /// </summary>
    public Vector3 Velocity3D {
        get => Sound.Velocity3D;
        set => Sound.Velocity3D = value;
    }

    /// <summary>
    /// The sound's 3D facing direction.
    /// </summary>
    public Vector3 Forward3D {
        get => Sound.Forward3D;
        set => Sound.Forward3D = value;
    }

    /// <summary>
    /// The sound's 3D up vector.
    /// </summary>
    public Vector3 Up3D {
        get => Sound.Up3D;
        set => Sound.Up3D = value;
    }

    protected override void EnterTree() {
        if (Autoplay) { Play(); }
    }

    protected override void ExitTree() {
        Stop(true);
    }

    protected override void Invalidated() {
        Sound.Release();
    }

    /// <summary>
    /// If the sound event has a parameter.
    /// </summary>
    /// <param name="id">The parameter to check for.</param>
    /// <returns>If the sound even has the given parameter.</returns>
    public bool HasParameter(PARAMETER_ID id)
        => Sound.HasParameter(id);


    /// <summary>
    /// If the sound event has a parameter.
    /// </summary>
    /// <param name="name">The parameter to check for.</param>
    /// <returns>If the sound even has the given parameter.</returns>
    public bool HasParameter(string name)
        => Sound.HasParameter(name);

    /// <summary>
    /// Get the current state of an event parameter.
    /// </summary>
    /// <param name="id">The paramter to query.</param>
    /// <returns>The current value of the parameter, or 0 if no parameter is found.</returns>
    public float GetParameter(PARAMETER_ID id)
        => Sound.GetParameter(id);


    /// <summary>
    /// Get the current state of an event parameter.
    /// </summary>
    /// <param name="name">The paramter to query.</param>
    /// <returns>The current value of the parameter, or 0 if no parameter is found.</returns>
    public float GetParameter(string name)
        => Sound.GetParameter(name);

    /// <summary>
    /// Set the state of an event parameter.
    /// </summary>
    /// <param name="id">The parameter to set.</param>
    /// <param name="value">The value to set the parameter to.</param>
    public void SetParameter(PARAMETER_ID id, float value)
        => Sound.SetParameter(id, value);

    /// <summary>
    /// Set the state of an event parameter.
    /// </summary>
    /// <param name="name">The parameter to set.</param>
    /// <param name="value">The value to set the parameter to.</param>
    public void SetParameter(string name, float value)
        => Sound.SetParameter(name, value);

    /// <summary>
    /// Play the sound event.
    /// </summary>
    public void Play() {
        Sound.Play(false);
    }

    /// <summary>
    /// Stop the sound event.
    /// </summary>
    /// <param name="immediate">If the sound should be allowed to fade out.</param>
    /// <param name="release">If the instance should be unloaded when finished.</param>
    public void Stop(bool immediate = false) {
        Sound.Stop(immediate, false);
    }
}