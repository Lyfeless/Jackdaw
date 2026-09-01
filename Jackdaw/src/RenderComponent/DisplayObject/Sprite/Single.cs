using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A single-image sprite.
/// </summary>
public class SpriteSingle : Sprite {
    readonly Subtexture Texture;
    readonly RectInt textureBounds;
    readonly Point2 halfSize;

    /// <param name="texture">The texture to display.</param>
    public SpriteSingle(Subtexture texture) : base() {
        Texture = texture;
        textureBounds = new(texture.Size.FloorToPoint2());
        halfSize = (texture.Size / 2).FloorToPoint2();

        CacheBounds();
    }

    /// <summary>
    /// A single-image sprite.
    /// </summary>
    /// <param name="assets">The current game's asset manager.</param>
    /// <param name="texture">The texture name.</param>
    public SpriteSingle(Assets assets, string texture) : this(assets.GetSubtexture(texture)) { }

    public override void Render(Batcher batcher) {
        batcher.Image(Texture, Bounds.Center, halfSize, Flip.GetScale(), 0, Color);
    }

    public override Rect GetObjectBounds() => textureBounds;
}