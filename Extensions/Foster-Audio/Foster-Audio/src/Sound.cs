using System.Numerics;
using Foster.Audio;

namespace Jackdaw.Audio.FosterAudio;

public class SoundPlayerComponent(AudioManager manager, Sound sound, string? bus = null) : Component(manager.Game) {
    protected readonly AudioManager Manager = manager;
    protected Sound Sound = sound;
    protected readonly string? Bus = bus;
    public SoundInstance Player;

    public bool Autostart = false;

    bool paused = false;
    TimeSpan pauseCursor;

    float? volume;
    public float? Volume { get => volume; set => volume = value; }

    float? pitch;
    public float? Pitch { get => pitch; set => pitch = value; }

    bool? looping;
    public bool? Looping { get => looping; set => looping = value; }

    TimeSpan? loopBegin;
    public TimeSpan? LoopBegin { get => loopBegin; set => loopBegin = value; }

    TimeSpan? loopEnd;
    public TimeSpan? LoopEnd { get => loopEnd; set => loopEnd = value; }

    bool? spatialized;
    public bool? Spatialized { get => spatialized; set => spatialized = value; }

    float? minGain;
    public float? MinGain { get => minGain; set => minGain = value; }

    float? minDistance;
    public float? MinDistance { get => minDistance; set => minDistance = value; }

    float? maxDistance;
    public float? MaxDistance { get => maxDistance; set => maxDistance = value; }

    float? directionalAttenuationFactor;
    public float? DirectionalAttenuationFactor { get => directionalAttenuationFactor; set => directionalAttenuationFactor = value; }

    float? dopplerFactor;
    public float? DopplerFactor { get => dopplerFactor; set => dopplerFactor = value; }

    float? maxGain;
    public float? MaxGain { get => maxGain; set => maxGain = value; }

    float? pan;
    public float? Pan { get => pan; set => pan = value; }

    float? rolloff;
    public float? Rolloff { get => rolloff; set => rolloff = value; }

    int? pinnedListenerIndex;
    public int? PinnedListenerIndex { get => pinnedListenerIndex; set => pinnedListenerIndex = value; }

    Vector3? position;
    public Vector3? Position { get => position; set => position = value; }

    Vector3? velocity;
    public Vector3? Velocity { get => velocity; set => velocity = value; }

    Vector3? direction;
    public Vector3? Direction { get => direction; set => direction = value; }

    SoundCone? cone;
    public SoundCone? Cone { get => cone; set => cone = value; }

    SoundPositioning? positioning;
    public SoundPositioning? Positioning { get => positioning; set => positioning = value; }

    SoundAttenuationModel? attenuationModel;
    public SoundAttenuationModel? AttenuationModel { get => attenuationModel; set => attenuationModel = value; }

    ulong? loopBeginPcmFrames;
    public ulong? LoopBeginPcmFrames { get => loopBeginPcmFrames; set => loopBeginPcmFrames = value; }

    ulong? loopEndPcmFrames;
    public ulong? LoopEndPcmFrames { get => loopEndPcmFrames; set => loopEndPcmFrames = value; }

    public SoundPlayerComponent(AudioManager manager, string sound, string? bus = null) : this(manager, manager.GetSound(sound), bus) { }

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

    public void Stop() {
        paused = false;
        Player.Stop();
    }

    public void Pause() {
        paused = true;
        pauseCursor = Player.Cursor;
        Player.Pause();
    }

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