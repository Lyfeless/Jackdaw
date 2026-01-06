using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Sprite-based animation data
/// </summary>
/// <remarks>
/// Create an animation using predefined frame data.
/// </remarks>
/// <param name="textures">All textures used for the animation.</param>
/// <param name="frames">Each individual frame to display in order.</param>
/// <param name="duration">The full length of the animation.</param>
/// <param name="looping">If the animation should loop back to the start when finished.</param>
/// <param name="positionOffset">The amount to offset the entire animation's position from the origin.</param>
public class AnimationData(
    Subtexture[] textures,
    AnimationFrame[] frames,
    TimeSpan duration,
    bool looping = false,
    Point2? positionOffset = null
) {
    /// <summary>
    /// All textures used for the animation
    /// </summary>
    public readonly Subtexture[] Textures = textures;

    /// <summary>
    /// Individual frame data.
    /// </summary>
    public readonly AnimationFrame[] Frames = frames;

    /// <summary>
    /// The length of the full animation.
    /// </summary>
    public readonly TimeSpan Duration = duration;

    /// <summary>
    /// If the animation should loop once it completes.
    /// </summary>
    public readonly bool Looping = looping;

    /// <summary>
    /// The amount all frames should be offset from the origin position.
    /// </summary>
    public readonly Point2 PositionOffset = positionOffset ?? Point2.Zero;

    /// <summary>
    /// Create an animation using predefined frame data.
    /// </summary>
    /// <param name="texture">The texture used for all frames of the animation.</param>
    /// <param name="frames">Each individual frame to display in order.</param>
    /// <param name="looping">If the animation should loop back to the start when finished.</param>
    /// <param name="positionOffset">The amount to offset the entire animation's position from the origin.</param>
    public AnimationData(
        Subtexture texture,
        AnimationFrame[] frames,
        bool looping = false,
        Point2? positionOffset = null
    ) : this([texture], frames, new(frames.Sum(e => e.Duration.Ticks)), looping, positionOffset) { }

    /// <summary>
    /// Create an animation using predefined frame data.
    /// </summary>
    /// <param name="textures">All textures used for the animation.</param>
    /// <param name="frames">Each individual frame to display in order.</param>
    /// <param name="looping">If the animation should loop back to the start when finished.</param>
    /// <param name="positionOffset">The amount to offset the entire animation's position from the origin.</param>
    public AnimationData(
        Subtexture[] textures,
        AnimationFrame[] frames,
        bool looping = false,
        Point2? positionOffset = null
    ) : this(textures, frames, new(frames.Sum(e => e.Duration.Ticks)), looping, positionOffset) { }

    /// <summary>
    /// Create an animation using predefined frame data.
    /// </summary>
    /// <param name="texture">All textures used for the animation.</param>
    /// <param name="frames">Each individual frame to display in order.</param>
    /// <param name="duration">The full length of the animation.</param>
    /// <param name="looping">If the animation should loop back to the start when finished.</param>
    /// <param name="positionOffset">The amount to offset the entire animation's position from the origin.</param>
    public AnimationData(
        Subtexture texture,
        AnimationFrame[] frames,
        TimeSpan duration,
        bool looping = false,
        Point2? positionOffset = null
    ) : this([texture], frames, duration, looping, positionOffset) { }

    /// <summary>
    /// Get a frame from the animation.
    /// </summary>
    /// <param name="time">The elapsed duration of the animation.</param>
    /// <returns></returns>
    public AnimationFrame GetFrame(TimeSpan time) {
        TimeSpan count = TimeSpan.Zero;
        for (int i = 0; i < Frames.Length; ++i) {
            count += Frames[i].Duration;
            if (time <= count) { return Frames[i]; }
        }
        return Frames[^1];
    }

    public Subtexture FrameTexture(TimeSpan duration)
        => FrameTexture(GetFrame(duration));

    public Subtexture FrameTexture(AnimationFrame frame) {
        Subtexture texture = Textures[frame.Texture];
        RectInt clip = frame.Clip;
        if (clip.Size == Vector2.Zero) { clip = new((int)texture.Width, (int)texture.Height); }
        return texture.GetClipSubtexture(clip);
    }
}