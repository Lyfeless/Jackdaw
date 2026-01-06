using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A single frame of a sprite animation.
/// </summary>
public readonly struct AnimationFrame {
    /// <summary>
    /// The texture the frame should display.
    /// </summary>
    public readonly int Texture;

    /// <summary>
    /// The length of the frame.
    /// </summary>
    public readonly TimeSpan Duration = TimeSpan.FromMilliseconds(1);

    /// <summary>
    /// The region of the texture to use for the frame. Uses the full frame if the size is 0.
    /// </summary>
    public readonly RectInt Clip = new(0, 0);

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
    /// <param name="texture">The texture index to use.</param>
    /// <param name="duration">The length of the frame.</param>
    /// <param name="clip">The region of the texture to use for the frame. Defaults to the full texture.</param>
    /// <param name="positionOffset">The amount to offset this frame's position from the rest of the animation.</param>
    /// <param name="flipX">If the frame texture should be mirrored horizontally.</param>
    /// <param name="flipY">If the frame texture should be mirrored vertically.</param>
    /// <param name="embeddedData">Metadata to include in the frame.</param>
    public AnimationFrame(
        int texture,
        TimeSpan duration,
        RectInt? clip = null,
        Point2? positionOffset = null,
        bool flipX = false,
        bool flipY = false,
        string? embeddedData = null
    ) {
        Texture = texture;
        Duration = duration;
        FlipX = flipX;
        FlipY = flipY;
        EmbeddedData = embeddedData ?? string.Empty;
        if (positionOffset != null) { PositionOffset = (Point2)positionOffset; }
        if (clip != null) { Clip = (RectInt)clip; }
    }
}