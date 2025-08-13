using Foster.Framework;

namespace LittleLib;

/// <summary>
/// A renderable sprite for use in the <seealso cref="SpriteComponent">.
/// Can be recolored, offset, or flipped.
/// </summary>
public abstract class Sprite {
    /// <summary>
    /// The color to render the sprite at. Defaults to white.
    /// </summary>
    public Color Color = Color.White;

    /// <summary>
    /// The position offset from the sprite origin.
    /// </summary>
    public Point2 Offset = Point2.Zero;

    /// <summary>
    /// If the sprite should be flipped horizontally.
    /// </summary>
    public bool FlipX = false;

    /// <summary>
    /// If the sprite should be flipped vertically.
    /// </summary>
    public bool FlipY = false;

    /// <summary>
    /// The size of the sprite.
    /// </summary>
    public abstract Point2 Size { get; }

    /// <summary>
    /// The space the sprite covers including offsets.
    /// </summary>
    public abstract RectInt Bounds { get; }
    public abstract void Render(Batcher batcher);

    protected Point2 FlipScale() => FlipScale(FlipX, FlipY);
    protected static Point2 FlipScale(bool x, bool y) {
        return new(x ? -1 : 1, y ? -1 : 1);
    }
}