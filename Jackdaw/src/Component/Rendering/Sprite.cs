using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A component that renders a <seealso cref="Sprite"> object.
/// </summary>
/// <param name="game">The current game instance.</param>
/// <param name="sprite">The sprite to render.</param>
/// <param name="offset">The position offset.</param>
public class SpriteComponent(Game game, Sprite sprite, Point2? offset = null) : Component(game) {
    /// <summary>
    /// The current rendering sprite.
    /// </summary>
    public Sprite Sprite = sprite;

    /// <summary>
    /// The position offset;
    /// </summary>
    public Point2 Offset = offset ?? Point2.Zero;

    /// <summary>
    /// A component that renders a <seealso cref="Sprite"> object.
    /// </summary>
    /// <param name="game">The current game instance.</param>
    /// <param name="sprite">The sprite to render.</param>
    /// <param name="color">the sprite's color tint.</param>
    /// <param name="offset">The position offset.</param>
    public SpriteComponent(Game game, Subtexture sprite, Color color, Point2? offset = null)
        : this(game, new SpriteSingle(sprite) { Color = color }, offset) { }

    /// <summary>
    /// A component that renders a <seealso cref="Sprite"> object.
    /// </summary>
    /// <param name="game">The current game instance.</param>
    /// <param name="sprite">The sprite to render.</param>
    /// <param name="offset">The position offset.</param>
    public SpriteComponent(Game game, Subtexture sprite, Point2? offset = null)
        : this(game, new SpriteSingle(sprite), offset) { }

    /// <summary>
    /// A component that renders a <seealso cref="Sprite"> object.
    /// </summary>
    /// <param name="game">The current game instance.</param>
    /// <param name="sprite">The sprite asset name.</param>
    /// <param name="color">The sprite's color tint.</param>
    /// <param name="offset">The position offset.</param>
    public SpriteComponent(Game game, string sprite, Color color, Point2? offset = null)
        : this(game, game.Assets.GetTexture(sprite), color, offset) { }

    /// <summary>
    /// A component that renders a <seealso cref="Sprite"> object.
    /// </summary>
    /// <param name="game">The current game instance.</param>
    /// <param name="sprite">The sprite asset name.</param>
    /// <param name="offset">The position offset.</param>
    public SpriteComponent(Game game, string sprite, Point2? offset = null)
        : this(game, game.Assets.GetTexture(sprite), offset) { }

    protected override void Render(Batcher batcher) {
        //! FIXME (Alex): This clips tiling sprites for some reason
        //! FIXME (Alex): Disabled because of viewport changes
        // if (!Game.Viewspace.Bounds.Overlaps(Sprite.Bounds.Translate(Actor.GlobalPositionRounded + Offset))) { return; }

        batcher.PushMatrix(Offset);
        Sprite.Render(batcher);
        batcher.PopMatrix();
    }
}