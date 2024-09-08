using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class UILabel : UIElement {
    public SpriteFont Font { get; protected set; }
    public string Text { get; protected set; }
    public Color Color { get; protected set; }

    public UILabel(string font, string text, Color color, UICreateArgs args) : this(Assets.GetFont(font), text, color, args) { }
    public UILabel(SpriteFont font, string text, Color color, UICreateArgs args) : base(args) {
        Font = font;
        Text = text;
        Color = color;

        Vector2 size = Font.SizeOf(Text);
        SetSize(UIVector2.Pixel(size.X, size.Y));
    }

    public override void Render(Batcher batcher) {
        batcher.Text(Font, Text, RelativeTopLeft, Color);
        // batcher.RectLine(RelativeBounds, 1, Color.Red);
    }
}