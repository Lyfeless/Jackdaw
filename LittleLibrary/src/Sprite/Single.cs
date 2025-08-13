using Foster.Framework;

namespace LittleLib;

/// <summary>
/// A single-image sprite.
/// </summary>
/// <param name="texture">The texture to display.</param>
public class SpriteSingle(Subtexture texture) : Sprite() {
    readonly Subtexture Texture = texture;

    public override Point2 Size => (Point2)Texture.Size;
    public override RectInt Bounds => new(Point2.Zero, Size);

    /// <summary>
    /// A single-image sprite.
    /// </summary>
    /// <param name="assets">The current game's asset manager.</params>
    /// <param name="texture">The texture name.</param>
    public SpriteSingle(Assets assets, string texture) : this(assets.GetTexture(texture)) { }

    public override void Render(Batcher batcher) {
        //! FIXME (Alex): Flip code might still be incorrect, re-check
        batcher.Image(Texture, Offset + (Bounds.Size / 2), Bounds.Center, FlipScale(), 0, Color);
    }
}