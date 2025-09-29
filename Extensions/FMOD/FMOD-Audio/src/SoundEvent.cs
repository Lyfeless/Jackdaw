using System.Numerics;
using FMOD;
using FMOD.Studio;

namespace Jackdaw.Audio.FMODAudio;

/// <summary>
/// A simple player for FMOD sound events.
/// Create using <see cref="AudioManager.Get" />
/// </summary>
public class SoundEvent {
    EventDescription Description;
    EventInstance Instance = new();

    /// <summary>
    /// A simple player for FMOD sound events.
    /// Create using <see cref="AudioManager.Get" />
    /// </summary>
    /// <param name="description">The sound event data.</param>
    public SoundEvent(EventDescription description) {
        Description = description;
        CreateInstance();
    }

    /// <summary>
    /// If the sound is paused.
    /// </summary>
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

    /// <summary>
    /// The sound's current pitch.
    /// </summary>
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

    /// <summary>
    /// The current playing position in the audio event timeline.
    /// </summary>
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

    /// <summary>
    /// The sound's volume.
    /// </summary>
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

    /// <summary>
    /// All data relating to the sound's playing position in 3D space.
    /// </summary>
    public ATTRIBUTES_3D PositionAttributes {
        get {
            Instance.get3DAttributes(out ATTRIBUTES_3D attributes);
            return attributes;
        }
        set {
            Instance.set3DAttributes(value);
        }
    }

    /// <summary>
    /// The sound's 3D playing position.
    /// </summary>
    public Vector3 Position3D {
        get => VectorConvert(PositionAttributes.position);
        set {
            ATTRIBUTES_3D attributes = PositionAttributes;
            attributes.position = VectorConvert(value);
            Instance.set3DAttributes(attributes);
        }
    }

    /// <summary>
    /// The sound's 3D velocity.
    /// </summary>
    public Vector3 Velocity3D {
        get => VectorConvert(PositionAttributes.velocity);
        set {
            ATTRIBUTES_3D attributes = PositionAttributes;
            attributes.velocity = VectorConvert(value);
            Instance.set3DAttributes(attributes);
        }
    }

    /// <summary>
    /// The sound's 3D facing direction.
    /// </summary>
    public Vector3 Forward3D {
        get => VectorConvert(PositionAttributes.forward);
        set {
            ATTRIBUTES_3D attributes = PositionAttributes;
            attributes.forward = VectorConvert(value);
            Instance.set3DAttributes(attributes);
        }
    }

    /// <summary>
    /// The sound's 3D up vector.
    /// </summary>
    public Vector3 Up3D {
        get => VectorConvert(PositionAttributes.up);
        set {
            ATTRIBUTES_3D attributes = PositionAttributes;
            attributes.up = VectorConvert(value);
            Instance.set3DAttributes(attributes);
        }
    }

    /// <summary>
    /// If the sound event has a parameter.
    /// </summary>
    /// <param name="id">The parameter to check for.</param>
    /// <returns>If the sound even has the given parameter.</returns>
    public bool HasParameter(PARAMETER_ID id)
        => Instance.isValid() && Instance.getParameterByID(id, out _) == RESULT.OK;

    /// <summary>
    /// If the sound event has a parameter.
    /// </summary>
    /// <param name="name">The parameter to check for.</param>
    /// <returns>If the sound even has the given parameter.</returns>
    public bool HasParameter(string name)
        => Instance.isValid() && Instance.getParameterByName(name, out _) == RESULT.OK;

    /// <summary>
    /// Get the current state of an event parameter.
    /// </summary>
    /// <param name="id">The paramter to query.</param>
    /// <returns>The current value of the parameter, or 0 if no parameter is found.</returns>
    public float GetParameter(PARAMETER_ID id) {
        if (!Instance.isValid() || Instance.getParameterByID(id, out float value) != RESULT.OK) { return 0; }
        return value;
    }

    /// <summary>
    /// Get the current state of an event parameter.
    /// </summary>
    /// <param name="name">The paramter to query.</param>
    /// <returns>The current value of the parameter, or 0 if no parameter is found.</returns>
    public float GetParameter(string name) {
        if (!Instance.isValid() || Instance.getParameterByName(name, out float value) != RESULT.OK) { return 0; }
        return value;
    }

    /// <summary>
    /// Set the state of an event parameter.
    /// </summary>
    /// <param name="id">The parameter to set.</param>
    /// <param name="value">The value to set the parameter to.</param>
    public void SetParameter(PARAMETER_ID id, float value) {
        if (!Instance.isValid()) { return; }
        Instance.setParameterByID(id, value);
    }

    /// <summary>
    /// Set the state of an event parameter.
    /// </summary>
    /// <param name="name">The parameter to set.</param>
    /// <param name="value">The value to set the parameter to.</param>
    public void SetParameter(string name, float value) {
        if (!Instance.isValid()) { return; }
        Instance.setParameterByName(name, value);
    }

    /// <summary>
    /// Play the sound event.
    /// </summary>
    /// <param name="release">If the instance should be unloaded when finished.</param>
    public void Play(bool release = true) {
        if (!Instance.isValid() && !CreateInstance()) { return; }
        Paused = false;
        Instance.start();
        if (release) { Instance.release(); }
    }

    /// <summary>
    /// Stop the sound event.
    /// </summary>
    /// <param name="immediate">If the sound should be allowed to fade out.</param>
    /// <param name="release">If the instance should be unloaded when finished.</param>
    public void Stop(bool immediate = false, bool release = true) {
        if (!Instance.isValid()) { return; }
        Instance.stop(immediate ? STOP_MODE.IMMEDIATE : STOP_MODE.ALLOWFADEOUT);
        if (release) { Instance.release(); }
    }

    /// <summary>
    /// Set the instance to unload when finished.
    /// </summary>
    public void Release() {
        if (!Instance.isValid()) { return; }
        Instance.release();
    }

    bool CreateInstance() => Description.isValid() && Description.createInstance(out Instance) == RESULT.OK;
    static Vector3 VectorConvert(VECTOR v) => new(v.x, v.y, v.z);
    static VECTOR VectorConvert(Vector3 v) => new() {
        x = v.X,
        y = v.Y,
        z = v.Z
    };
}