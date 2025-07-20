using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class TextComponent(LittleGame game, string text, SpriteFont font, Color color, Vector2? offset = null) : Component(game) {
    string Text = text;
    SpriteFont Font = font;
    Color Color = color;
    Vector2 Offset = offset ?? Vector2.Zero;

    public TextComponent(LittleGame game, string text, string font, Color color, Vector2? offset = null) : this(game, text, game.Assets.GetFont(font), color, offset) { }

    public override void Render(Batcher batcher) {
        //! FIXME (Alex): Cull
        batcher.Text(Font, Text, Offset, Color);
    }
}