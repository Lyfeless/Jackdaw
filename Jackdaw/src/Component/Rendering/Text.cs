using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A simple text rendering component.
/// </summary>
/// <param name="game">The current game instance.</param>
/// <param name="text">The text to render.</param>
/// <param name="font">The font data.</param>
/// <param name="color">The text color.</param>
/// <param name="offset">The position offset.</param>
public class TextComponent(Game game, string text, SpriteFont font, Color color, Vector2? offset = null) : Component(game) {
    /// <summary>
    /// The text to render.
    /// </summary>
    public string Text = text;

    /// <summary>
    /// The font data.
    /// </summary>
    public SpriteFont Font = font;

    /// <summary>
    /// The text color.
    /// </summary>
    public Color Color = color;

    /// <summary>
    /// The position offset.
    /// </summary>
    public Vector2 Offset = offset ?? Vector2.Zero;

    /// <summary>
    /// A simple text rendering component.
    /// </summary>
    /// <param name="game">The current game instance.</param>
    /// <param name="text">The text to render.</param>
    /// <param name="font">The font asset name.</param>
    /// <param name="color">The text color.</param>
    /// <param name="offset">The position offset.</param>
    public TextComponent(Game game, string text, string font, Color color, Vector2? offset = null) : this(game, text, game.Assets.GetFont(font), color, offset) { }

    protected override void Render(Batcher batcher) {
        //! FIXME (Alex): Cull
        batcher.Text(Font, Text, Offset, Color);
    }
}