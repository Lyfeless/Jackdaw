using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class AnimationInstance(AnimationData data, Subtexture texture, float? frameTime = null, float? startDelay = null, Vector2? positionOffset = null, bool? looping = null) {
    readonly AnimationData Data = data;
    Subtexture texture = texture;

    readonly float FrameTime = frameTime ?? data.DefaultFrametime;

    readonly double StartTime = (Time.Now.TotalMilliseconds + startDelay) ?? data.DefaultStartDelay;

    public bool Looping = looping ?? data.DefaultLooping;

    public readonly Vector2 PositionOffset = positionOffset ?? data.DefaultPositionOffset;

    public int FrameIndex => (int)Math.Floor((Time.Now.TotalMilliseconds - StartTime) / FrameTime);
    public Point2 FramePosition {
        get {
            Point2 frame = Looping ? Data.Frames[Math.Max(0, FrameIndex % Data.Frames.Length)] : Data.Frames[Math.Clamp(FrameIndex, 0, Data.Frames.Length - 1)];
            return frame * Data.FrameSize;
        }
    }
    public Subtexture FrameTexture {
        get {
            Vector2 position = texture.Source.Position;
            Point2 offset = FramePosition;

            return new Subtexture(texture.Texture, new Rect(
                position + offset,
                position + offset + Data.FrameSize
            ));
        }
    }
    public bool Done => !Looping && FrameIndex >= Data.Frames.Length;
}