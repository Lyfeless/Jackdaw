using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A renderable sprite for use in the <seealso cref="DisplayObjectRenderComponent" />.
/// Can be recolored, offset, or flipped.
/// </summary>
public abstract class Sprite : DisplayObject {
    /// <summary>
    /// The sprite's local offset, stacks with the render component offset.
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
    /// Get the scale value required to render the flip the sprite by the current flipX and flipY values.
    /// </summary>
    /// <returns>A scale value to flip the sprite by.</returns>
    protected Point2 FlipScale() => FlipScale(FlipX, FlipY);

    /// <summary>
    /// Get the scale value required to render the flip the sprite by a flipX and flipY.
    /// </summary>
    /// <param name="x">If the sprite should be flipped horizontally.</param>
    /// <param name="y">If the sprite should be flipped vertically.</param>
    /// <returns>A scale value to flip the sprite by.</returns>
    protected static Point2 FlipScale(bool x, bool y) {
        return new(x ? -1 : 1, y ? -1 : 1);
    }
}