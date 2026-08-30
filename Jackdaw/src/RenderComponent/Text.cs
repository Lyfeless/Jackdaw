using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A simple text rendering component. <br/>
/// Not ticking by default.
/// </summary>
public class TextRenderComponent : Component {
    string text;
    SpriteFont font;
    Vector2 offset = Vector2.Zero;
    AlignmentBound alignment = AlignmentBound.TopLeft();

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
    public Color Color;

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
    /// How the text should be aligned relative to its position.
    /// </summary>
    public AlignmentBound Alignment {
        get => alignment;
        set {
            alignment = value;
            SetBounds();
        }
    }

    /// <summary>
    /// The rectangular region the text occupies.
    /// </summary>
    public Rect Bounds { get; private set; }

    /// <param name="game">The current game instance.</param>
    /// <param name="text">The text to render.</param>
    /// <param name="font">The font data.</param>
    /// <param name="color">The text color.</param>
    public TextRenderComponent(Game game, string text, SpriteFont font, Color color) : base(game) {
        this.text = text;
        this.font = font;
        Color = color;
        Bounds = GetBounds(Vector2.Zero, text, font, AlignmentBound.TopLeft());
        Ticking = false;
    }

    /// <summary>
    /// A simple text rendering component.
    /// </summary>
    /// <param name="game">The current game instance.</param>
    /// <param name="text">The text to render.</param>
    /// <param name="font">The font asset name.</param>
    /// <param name="color">The text color.</param>
    public TextRenderComponent(Game game, string text, string font, Color color) : this(game, text, game.Assets.GetSpriteFont(font), color) { }

    protected override void Render(Batcher batcher) {
        if (!Game.Window.BoundsInPixels().Overlaps(Bounds.TransformAABB(Actor.Transform.GlobalDisplayMatrix))) { return; }
        batcher.Text(Font, Text, Bounds.Position, Color);
    }

    void SetBounds() => Bounds = GetBounds(Offset, text, font, alignment);
    static Rect GetBounds(Vector2 offset, string text, SpriteFont font, AlignmentBound alignment) {
        Vector2 textSize = font.SizeOf(text);
        Vector2 alignOffset = alignment.Get(textSize);
        return new(offset + alignOffset, textSize);
    }
}