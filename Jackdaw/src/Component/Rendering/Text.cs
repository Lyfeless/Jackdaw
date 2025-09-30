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
    string text = text;
    SpriteFont font = font;
    public Vector2 offset = offset ?? Vector2.Zero;

    /// <summary>
    /// The text to render.
    /// </summary>
    public string Text {
        get => text;
        set {
            text = value;
            SetBounds();
        }
    }

    /// <summary>
    /// The font data.
    /// </summary>
    public SpriteFont Font {
        get => font;
        set {
            font = value;
            SetBounds();
        }
    }

    /// <summary>
    /// The text color.
    /// </summary>
    public Color Color = color;

    /// <summary>
    /// The position offset.
    /// </summary>
    public Vector2 Offset {
        get => offset;
        set {
            offset = value;
            SetBounds();
        }
    }

    public Rect Bounds { get; private set; } = GetBounds(offset ?? Vector2.Zero, text, font);

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
        if (!Game.Window.BoundsInPixels().Overlaps(CalcExtra.TransformRect(Bounds, Actor.Position.GlobalDisplayMatrix))) { return; }
        batcher.Text(Font, Text, Offset, Color);
    }

    void SetBounds() { Bounds = GetBounds(Offset, text, font); }
    static Rect GetBounds(Vector2 offset, string text, SpriteFont font) => new(offset, font.SizeOf(text));
}