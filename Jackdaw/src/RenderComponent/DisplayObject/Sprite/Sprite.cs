using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A renderable sprite for use in the <see cref="DisplayObjectRenderComponent" />.
/// Can be recolored, offset, or flipped.
/// </summary>
public abstract class Sprite : DisplayObject {
    /// <summary>
    /// The sprite's local offset, stacks with the render component offset.
    /// </summary>
    public Point2 Offset = Point2.Zero;

    /// <summary>
    /// How the sprite should be flipped both horizontally and vertically. <br/>
    /// Sprite flipping is applied in-place and does not move the sprite's rendered position.
    /// </summary>
    public SpriteFlip Flip = new();
}