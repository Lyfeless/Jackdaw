using Foster.Framework;

namespace LittleLib;

/// <summary>
/// A single frame of a sprite animation.
/// </summary>
public readonly struct AnimationFrame {
    public readonly Subtexture Texture;
    public readonly float Duration = 1;
    public readonly Point2 PositionOffset = Point2.Zero;
    public readonly bool FlipX = false;
    public readonly bool FlipY = false;
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