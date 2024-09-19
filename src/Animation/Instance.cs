using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class AnimationInstance {
    public readonly string TimeTracker;

    public readonly AnimationData Data;
    Subtexture Texture;

    public readonly float FrameTime;

    public readonly double StartTime;

    public bool Looping;

    public readonly Vector2 PositionOffset;

    public AnimationInstance(AnimationData data, Subtexture texture, float? frameTime = null, float? startDelay = null, Vector2? positionOffset = null, bool? looping = null, string? timeTracker = null) {
        TimeTracker = timeTracker ?? string.Empty;
        Data = data;
        Texture = texture;
        FrameTime = frameTime ?? data.DefaultFrametime;
        StartTime = (Milliseconds + startDelay) ?? data.DefaultStartDelay;
        Looping = looping ?? data.DefaultLooping;
        PositionOffset = positionOffset ?? data.DefaultPositionOffset;
    }

    double Milliseconds => TimeManager.GetTrackedTime(TimeTracker).TotalMilliseconds;

    public int FrameIndex => (int)Math.Floor((Milliseconds - StartTime) / FrameTime);
    public Point2 FramePosition {
        get {
            Point2 frame = Looping ? Data.Frames[Math.Max(0, FrameIndex % Data.Frames.Length)] : Data.Frames[Math.Clamp(FrameIndex, 0, Data.Frames.Length - 1)];
            return frame * Data.FrameSize;
        }
    }
    public Subtexture FrameTexture {
        get {
            Vector2 position = Texture.Source.Position;
            Point2 offset = FramePosition;

            return new Subtexture(Texture.Texture, new Rect(
                position + offset,
                position + offset + Data.FrameSize
            ));
        }
    }
    public bool Done => !Looping && FrameIndex >= Data.Frames.Length;
}