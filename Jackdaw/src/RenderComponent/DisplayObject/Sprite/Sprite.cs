using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A renderable sprite for use in the <see cref="RenderComponent" />.
/// Can be recolored, aligned, offset, or flipped.
/// </summary>
public abstract class Sprite : DisplayObject {
    /// <summary>
    /// How the sprite should be flipped both horizontally and vertically. <br/>
    /// Sprite flipping is applied in-place and does not move the sprite's rendered position.
    /// </summary>
    public SpriteFlip Flip = new();
}