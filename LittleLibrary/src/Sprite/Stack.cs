using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class SpriteStack(params Sprite[] sprites) : Sprite() {
    readonly Sprite[] Sprites = sprites;

    readonly Rect bounds = new BoundsBuilder([.. sprites.Select(e => e.Bounds)]).Rect;
    public override Vector2 Size => bounds.Size;
    public override Rect Bounds => bounds;

    public SpriteStack(params Subtexture[] sprites) : this([.. sprites.Select(e => new SpriteSingle(e))]) { }
    public SpriteStack(Assets assets, params string[] sprites) : this([.. sprites.Select(e => new SpriteSingle(assets, e))]) { }
    public SpriteStack(Assets assets, Color color, params string[] sprites) : this([.. sprites.Select(e => new SpriteSingle(assets, e, color))]) { }

    public override void Render(Batcher batcher) {
        foreach (Sprite sprite in Sprites) {
            sprite.Render(batcher);
        }
    }
}