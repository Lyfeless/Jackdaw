using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Sprite-based animation data
/// </summary>
public class AnimationData {
    /// <summary>
    /// All textures used for the animation
    /// </summary>
    public readonly Subtexture[] Textures = [];

    /// <summary>
    /// Individual frame data.
    /// </summary>
    public readonly AnimationFrame[] Frames = [];

    /// <summary>
    /// The length of the full animation, in milliseconds.
    /// </summary>
    public readonly float Duration;

    /// <summary>
    /// If the animation should loop once it completes.
    /// </summary>
    public readonly bool Looping;

    /// <summary>
    /// The amount all frames should be offset from the origin position.
    /// </summary>
    public readonly Point2 PositionOffset;

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
    ) : this([texture], frames, frames.Sum(e => e.Duration), looping, positionOffset) { }

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
    ) : this(textures, frames, frames.Sum(e => e.Duration), looping, positionOffset) { }

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
        float duration,
        bool looping = false,
        Point2? positionOffset = null
    ) : this([texture], frames, duration, looping, positionOffset) { }

    /// <summary>
    /// Create an animation using predefined frame data.
    /// </summary>
    /// <param name="textures">All textures used for the animation.</param>
    /// <param name="frames">Each individual frame to display in order.</param>
    /// <param name="duration">The full length of the animation.</param>
    /// <param name="looping">If the animation should loop back to the start when finished.</param>
    /// <param name="positionOffset">The amount to offset the entire animation's position from the origin.</param>
    public AnimationData(
        Subtexture[] textures,
        AnimationFrame[] frames,
        float duration,
        bool looping = false,
        Point2? positionOffset = null
    ) {
        Textures = textures;
        Frames = frames;
        Duration = duration;
        Looping = looping;
        PositionOffset = positionOffset ?? Point2.Zero;
    }

    /// <summary>
    /// Get a frame from the animation.
    /// </summary>
    /// <param name="time">The elapsed duration of the animation, in milliseconds.</param>
    /// <returns></returns>
    public AnimationFrame GetFrame(float time) {
        float count = 0;
        for (int i = 0; i < Frames.Length; ++i) {
            count += Frames[i].Duration;
            if (time <= count) { return Frames[i]; }
        }
        return Frames[^1];
    }

    public Subtexture FrameTexture(float duration)
        => FrameTexture(GetFrame(duration));

    public Subtexture FrameTexture(AnimationFrame frame) {
        Subtexture texture = Textures[frame.Texture];
        RectInt clip = frame.Clip;
        if (clip.Size == Vector2.Zero) { clip = new((int)texture.Width, (int)texture.Height); }
        return texture.GetClipSubtexture(clip);
    }
}