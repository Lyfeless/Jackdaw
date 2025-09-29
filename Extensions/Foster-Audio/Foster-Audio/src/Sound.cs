using System.Numerics;
using Foster.Audio;

namespace Jackdaw.Audio.FosterAudio;

/// <summary>
/// A basic component for playing sounds.
/// </summary>
/// <param name="manager">The audio manager.</param>
/// <param name="sound">The sound to play.</param>
/// <param name="bus">The bus to play the sound on.</param>
public class SoundPlayerComponent(AudioManager manager, Sound sound, string? bus = null) : Component(manager.Game) {
    protected readonly AudioManager Manager = manager;
    protected Sound Sound = sound;
    protected readonly string? Bus = bus;

    /// <summary>
    /// The current sound instance.
    /// </summary>
    public SoundInstance Player;

    /// <summary>
    /// If the sound should automatically start playing when added to the actor tree.
    /// </summary>
    public bool Autostart = false;

    bool paused = false;
    TimeSpan pauseCursor;

    /// <summary>
    /// The volume the sound should play at.
    /// </summary>
    public float? Volume { get => volume; set => volume = value; }
    float? volume;

    /// <summary>
    /// The pitch to play the sound at.
    /// </summary>
    public float? Pitch { get => pitch; set => pitch = value; }
    float? pitch;

    /// <summary>
    /// If the sound should loop when the <see cref="LoopEnd" /> is reached.
    /// </summary>
    public bool? Looping { get => looping; set => looping = value; }
    bool? looping;

    /// <summary>
    /// The timestamp to start at when the sound loops. Defaults to the start of the sound.
    /// </summary>
    public TimeSpan? LoopBegin { get => loopBegin; set => loopBegin = value; }
    TimeSpan? loopBegin;

    /// <summary>
    /// The timestamp to loop the sound when reached. Defaults to the end of the sound.
    /// </summary>
    public TimeSpan? LoopEnd { get => loopEnd; set => loopEnd = value; }
    TimeSpan? loopEnd;

    /// <summary>
    /// If the sound has 3D Audio enabled.
    /// </summary>
    public bool? Spatialized { get => spatialized; set => spatialized = value; }
    bool? spatialized;

    public float? MinGain { get => minGain; set => minGain = value; }
    float? minGain;

    public float? MaxGain { get => maxGain; set => maxGain = value; }
    float? maxGain;

    /// <summary>
    /// The closest distance the sound can be heard.
    /// </summary>
    public float? MinDistance { get => minDistance; set => minDistance = value; }
    float? minDistance;

    /// <summary>
    /// The furthest distance the sound can be heard.
    /// </summary>
    public float? MaxDistance { get => maxDistance; set => maxDistance = value; }
    float? maxDistance;

    public float? DirectionalAttenuationFactor { get => directionalAttenuationFactor; set => directionalAttenuationFactor = value; }
    float? directionalAttenuationFactor;

    public float? DopplerFactor { get => dopplerFactor; set => dopplerFactor = value; }
    float? dopplerFactor;

    public float? Pan { get => pan; set => pan = value; }
    float? pan;

    public float? Rolloff { get => rolloff; set => rolloff = value; }
    float? rolloff;

    public int? PinnedListenerIndex { get => pinnedListenerIndex; set => pinnedListenerIndex = value; }
    int? pinnedListenerIndex;

    /// <summary>
    /// The 3D spatialized position to play the sound at.
    /// </summary>
    public Vector3? Position { get => position; set => position = value; }
    Vector3? position;

    /// <summary>
    /// The sound's 3D velocity.
    /// </summary>
    public Vector3? Velocity { get => velocity; set => velocity = value; }
    Vector3? velocity;

    public Vector3? Direction { get => direction; set => direction = value; }
    Vector3? direction;

    public SoundCone? Cone { get => cone; set => cone = value; }
    SoundCone? cone;

    public SoundPositioning? Positioning { get => positioning; set => positioning = value; }
    SoundPositioning? positioning;

    public SoundAttenuationModel? AttenuationModel { get => attenuationModel; set => attenuationModel = value; }
    SoundAttenuationModel? attenuationModel;

    public ulong? LoopBeginPcmFrames { get => loopBeginPcmFrames; set => loopBeginPcmFrames = value; }
    ulong? loopBeginPcmFrames;

    public ulong? LoopEndPcmFrames { get => loopEndPcmFrames; set => loopEndPcmFrames = value; }
    ulong? loopEndPcmFrames;

    public SoundPlayerComponent(AudioManager manager, string sound, string? bus = null) : this(manager, manager.GetSound(sound), bus) { }

    /// <summary>
    /// Play the sound.
    /// </summary>
    public virtual void Play() {
        paused = false;
        Stop();
        Player = Manager.Play(Sound, Bus);

        // Assign default values.
        if (volume != null) { Player.Volume = (float)volume; }
        if (pitch != null) { Player.Pitch = (float)pitch; }
        if (pan != null) { Player.Pan = (float)pan; }
        if (looping != null) { Player.Looping = (bool)looping; }
        if (loopBegin != null) { Player.LoopBegin = (TimeSpan)loopBegin; }
        if (loopEnd != null) { Player.LoopEnd = loopEnd; }
        if (loopBeginPcmFrames != null) { Player.LoopBeginPcmFrames = (ulong)loopBeginPcmFrames; }
        if (loopEndPcmFrames != null) { Player.LoopEndPcmFrames = (ulong)loopEndPcmFrames; }
        if (velocity != null) { Player.Velocity = (Vector3)velocity; }
        if (direction != null) { Player.Direction = (Vector3)direction; }
        if (spatialized != null) {
            Player.Spatialized = (bool)spatialized;
            if (positioning != null) { Player.Positioning = (SoundPositioning)positioning; }
            if (position != null) { Player.Position = (Vector3)position; }
            if (pinnedListenerIndex != null) { Player.PinnedListenerIndex = (int)pinnedListenerIndex; }
            if (attenuationModel != null) { Player.AttenuationModel = (SoundAttenuationModel)attenuationModel; }
            if (rolloff != null) { Player.Rolloff = (float)rolloff; }
            if (minGain != null) { Player.MinGain = (float)minGain; }
            if (maxGain != null) { Player.MaxGain = (float)maxGain; }
            if (minDistance != null) { Player.MinDistance = (float)minDistance; }
            if (maxDistance != null) { Player.MaxDistance = (float)maxDistance; }
            if (cone != null) { Player.Cone = (SoundCone)cone; }
            if (directionalAttenuationFactor != null) { Player.DirectionalAttenuationFactor = (float)directionalAttenuationFactor; }
            if (dopplerFactor != null) { Player.DopplerFactor = (float)dopplerFactor; }
        }
    }

    /// <summary>
    /// Stop the sound.
    /// </summary>
    public void Stop() {
        paused = false;
        Player.Stop();
    }

    /// <summary>
    /// Pause the sound with stopping it.
    /// </summary>
    public void Pause() {
        paused = true;
        pauseCursor = Player.Cursor;
        Player.Pause();
    }

    /// <summary>
    /// Resume the sound at the position it was paused at.
    /// </summary>
    public void Unpause() {
        if (!paused) { return; }

        paused = false;
        Player.Play();
        Player.Cursor = pauseCursor;
    }

    protected override void EnterTree() {
        if (Autostart) { Play(); }
    }

    protected override void ExitTree() {
        Stop();
    }
}