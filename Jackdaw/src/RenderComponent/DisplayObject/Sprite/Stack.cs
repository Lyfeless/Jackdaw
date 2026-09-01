using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A sprite made up of multiple sprites displayed on top of one another.
/// </summary>
public class SpriteStack : Sprite {
    public readonly Sprite[] Sprites;
    /// <param name="sprites">All the sprites to display in the stack.</param>
    public SpriteStack(params Sprite[] sprites) : base() {
        Sprites = sprites;

        CacheBounds();
    }

    /// <summary>
    /// Create a stack of sprites.
    /// </summary>
    /// <param name="sprites">All the sprites to display in the stack.</param>
    public SpriteStack(params Subtexture[] sprites) : this([.. sprites.Select(e => new SpriteSingle(e))]) { }

    /// <summary>
    /// Create a stack of sprites.
    /// </summary>
    /// <param name="assets">The game asset storage.</param>
    /// <param name="sprites">All the sprites name ids to display in the stack.</param>
    public SpriteStack(Assets assets, params string[] sprites) : this([.. sprites.Select(e => new SpriteSingle(assets, e))]) { }

    /// <summary>
    /// Create a stack of sprites.
    /// </summary>
    /// <param name="assets">The game asset storage.</param>
    /// <param name="color">The color to tint all sprites in the stack by.</param>
    /// <param name="sprites">All the sprites name ids to display in the stack.</param>
    public SpriteStack(Assets assets, Color color, params string[] sprites) : this([.. sprites.Select(e => new SpriteSingle(assets, e) { Color = color })]) { }

    public override void Render(Batcher batcher) {
        RectInt bounds = Bounds;
        batcher.PushMatrix(Transform.CreateMatrix(bounds.Center, bounds.Center, Flip.GetScale(), 0));

        foreach (Sprite sprite in Sprites) {
            sprite.Render(batcher);
        }

        batcher.PopMatrix();
    }

    public override Rect GetObjectBounds() => new BoundsBuilder([.. Sprites.Select(e => e.Bounds)]).Rect.Translate(Offset).Int();
}