using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A single-image sprite.
/// </summary>
/// <param name="texture">The texture to display.</param>
public class SpriteSingle(Subtexture texture) : Sprite() {
    readonly Subtexture Texture = texture;
    readonly RectInt textureBounds = new(texture.Size.RoundToPoint2());
    readonly Point2 halfSize = (texture.Size / 2).RoundToPoint2();

    public override RectInt Bounds => textureBounds.Translate(Offset);

    /// <summary>
    /// A single-image sprite.
    /// </summary>
    /// <param name="assets">The current game's asset manager.</param>
    /// <param name="texture">The texture name.</param>
    public SpriteSingle(Assets assets, string texture) : this(assets.GetSubtexture(texture)) { }

    public override void Render(Batcher batcher) {
        batcher.Image(Texture, Bounds.Center, halfSize, FlipScale(), 0, Color);
    }
}