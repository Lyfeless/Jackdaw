using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A single frame of a sprite animation.
/// </summary>
public readonly struct AnimationFrame {
    /// <summary>
    /// The texture the frame should display.
    /// </summary>
    public readonly Subtexture Texture;

    /// <summary>
    /// The length of the frame, in milliseconds.
    /// </summary>
    public readonly float Duration = 1;

    /// <summary>
    /// The amount the frame should be offset from the origin position.
    /// Stacks with full animation position offset.
    /// </summary>
    public readonly Point2 PositionOffset = Point2.Zero;

    /// <summary>
    /// If the frame should be flipped horizontally.
    /// </summary>
    public readonly bool FlipX = false;

    /// <summary>
    /// If the frame should be flipped vertically.
    /// </summary>
    public readonly bool FlipY = false;

    /// <summary>
    /// Additional custom data attached to the frame.
    /// </summary>
    public readonly string EmbeddedData = string.Empty;

    /// <summary>
    /// Create a frame for a sprite animation.
    /// </summary>
    /// <param name="texture">The texture to use.</param>
    /// <param name="duration">The length of the frame, in milliseconds.</param>
    /// <param name="positionOffset">The amount to offset this frame's position from the rest of the animation.</param>
    /// <param name="clip">The region of the texture to use for the frame. Defaults to the full texture.</param>
    /// <param name="flipX">If the frame texture should be mirrored horizontally.</param>
    /// <param name="flipY">If the frame texture should be mirrored vertically.</param>
    /// <param name="embeddedData">Metadata to include in the frame.</param>
    public AnimationFrame(
        Subtexture texture,
        float duration,
        Point2? positionOffset = null,
        RectInt? clip = null,
        bool flipX = false,
        bool flipY = false,
        string? embeddedData = null
    ) {
        Duration = duration;
        PositionOffset = positionOffset ?? Point2.Zero;
        FlipX = flipX;
        FlipY = flipY;
        EmbeddedData = embeddedData ?? string.Empty;
        if (clip != null) {
            RectInt clipRect = (RectInt)clip;
            Texture = texture.GetClipSubtexture(clipRect);
        }
        else {
            Texture = texture;
        }
    }

    /// <summary>
    /// Create a frame for a sprite animation by clipping a spritesheet texture.
    /// </summary>
    /// <param name="texture">The base texture.</param>
    /// <param name="clip">The region of the texture to use for the frame.</param>
    /// <param name="duration">The length of the frame, in milliseconds.</param>
    public AnimationFrame(
        Subtexture texture,
        float duration,
        Rect clip
    ) {
        Duration = duration;
        Texture = texture.GetClipSubtexture(clip);
    }
}