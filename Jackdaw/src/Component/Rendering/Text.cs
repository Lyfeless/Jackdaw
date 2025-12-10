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
public class TextComponent(Game game, string text, SpriteFont font, Color color) : Component(game) {
    /// <summary>
    /// How to position text relative to the position.
    /// </summary>
    public enum Alignment {
        /// <summary>
        /// Align text to furthest negative side of the text bounds. <br/>
        /// Equivelant to TOP.
        /// </summary>
        LEFT,

        /// <summary>
        /// Align text to furthest negative side of the text bounds. <br/>
        /// Equivelant to LEFT.
        /// </summary>
        TOP,

        /// <summary>
        /// Align text to center of the text bounds.
        /// </summary>
        CENTER,

        /// <summary>
        /// Align text to furthest positive side of the text bounds. <br/>
        /// Equivelant to BOTTOM.
        /// </summary>
        RIGHT,

        /// <summary>
        /// Align text to furthest positive side of the text bounds. <br/>
        /// Equivelant to RIGHT.
        /// </summary>
        BOTTOM
    }

    string text = text;
    SpriteFont font = font;
    Vector2 offset = Vector2.Zero;
    Alignment alignHorizontal = Alignment.LEFT;
    Alignment alignVertical = Alignment.TOP;

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

    /// <summary>
    /// How to align the text horizontally relative to its position.
    /// </summary>
    public Alignment AlignHorizontal {
        get => alignHorizontal;
        set {
            alignHorizontal = value;
            SetBounds();
        }
    }

    /// <summary>
    /// How to algin the text vertically relative to its position.
    /// </summary>
    public Alignment AlignVertical {
        get => alignVertical;
        set {
            alignVertical = value;
            SetBounds();
        }
    }

    /// <summary>
    /// The rectangular region the text occupies.
    /// </summary>
    public Rect Bounds { get; private set; } = GetBounds(Vector2.Zero, text, font, Alignment.LEFT, Alignment.TOP);

    /// <summary>
    /// A simple text rendering component.
    /// </summary>
    /// <param name="game">The current game instance.</param>
    /// <param name="text">The text to render.</param>
    /// <param name="font">The font asset name.</param>
    /// <param name="color">The text color.</param>
    public TextComponent(Game game, string text, string font, Color color) : this(game, text, game.Assets.GetFont(font), color) { }

    protected override void Render(Batcher batcher) {
        if (!Game.Window.BoundsInPixels().Overlaps(CalcExtra.TransformRect(Bounds, Actor.Transform.GlobalDisplayMatrix))) { return; }
        batcher.Text(Font, Text, Bounds.Position, Color);
    }

    void SetBounds() => Bounds = GetBounds(Offset, text, font, alignHorizontal, alignVertical);
    static Rect GetBounds(Vector2 offset, string text, SpriteFont font, Alignment alignX, Alignment alignY) {
        Vector2 textSize = font.SizeOf(text);
        Vector2 alignOffset = GetAlignmentOffset(textSize, alignX, alignY);
        return new(offset + alignOffset, textSize);
    }
    static Vector2 GetAlignmentOffset(Vector2 size, Alignment alignX, Alignment alignY) => new(
        GetAlignmentOffset(size.X, alignX),
        GetAlignmentOffset(size.Y, alignY)
    );
    static float GetAlignmentOffset(float size, Alignment align) => align switch {
        Alignment.LEFT => 0,
        Alignment.TOP => 0,
        Alignment.CENTER => -size / 2,
        Alignment.RIGHT => -size,
        Alignment.BOTTOM => -size,
        _ => 0
    };
}