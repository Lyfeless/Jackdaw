using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A display object for rendering basic text.
/// </summary>
public class DisplayText : DisplayObject {
    string text;
    SpriteFont font;

    /// <summary>
    /// The text to render.
    /// </summary>
    public string Text {
        get => text;
        set {
            text = value;
            CacheBounds();
        }
    }

    /// <summary>
    /// The font data.
    /// </summary>
    public SpriteFont Font {
        get => font;
        set {
            font = value;
            CacheBounds();
        }
    }

    public override Rect GetObjectBounds() => new(font.SizeOf(text));

    /// <param name="text">The text to render.</param>
    /// <param name="font">The font data.</param>
    public DisplayText(string text, SpriteFont font) {
        this.text = text;
        this.font = font;

        CacheBounds();
    }

    /// <summary>
    /// A simple text rendering component.
    /// </summary>
    /// <param name="game">The current game instance.</param>
    /// <param name="text">The text to render.</param>
    /// <param name="font">The font asset name.</param>
    public DisplayText(Game game, string text, string font)
        : this(text, game.Assets.GetSpriteFont(font)) { }

    public override void Render(Batcher batcher) {
        batcher.Text(Font, Text, Vector2.Zero, Color);
    }
}