using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class UILabel : UIElement {
    public SpriteFont Font;
    public string Text;
    public Color Color;

    public UILabel(string font, string text, Color color, UICreateArgs args) : this(Assets.GetFont(font), text, color, args) { }
    public UILabel(SpriteFont font, string text, Color color, UICreateArgs args) : base(args) {
        Font = font;
        Text = text;
        Color = color;

        Vector2 size = Font.SizeOf(Text);
        SetSize(UIVector2.Pixel(size.X, size.Y));
    }

    public override void Render(Batcher batcher) {
        batcher.Text(Font, Text, Vector2.Zero, Color);
    }
}