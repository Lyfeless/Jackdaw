using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class SpriteStack(params Sprite[] sprites) : Sprite() {
    public readonly Sprite[] Sprites = sprites;

    readonly RectInt bounds = (RectInt)new BoundsBuilder([.. sprites.Select(e => e.Bounds)]).Rect;
    public override Point2 Size => bounds.Size;
    public override RectInt Bounds => bounds.Translate(Offset);

    public SpriteStack(params Subtexture[] sprites) : this([.. sprites.Select(e => new SpriteSingle(e))]) { }
    public SpriteStack(Assets assets, params string[] sprites) : this([.. sprites.Select(e => new SpriteSingle(assets, e))]) { }
    public SpriteStack(Assets assets, Color color, params string[] sprites) : this([.. sprites.Select(e => new SpriteSingle(assets, e) { Color = color })]) { }

    public override void Render(Batcher batcher) {
        //! FIXME (Alex): Flip code might still be incorrect, re-check
        batcher.PushMatrix(Transform.CreateMatrix(Offset + (bounds.Size / 2), bounds.Center, FlipScale(), 0));

        foreach (Sprite sprite in Sprites) {
            sprite.Render(batcher);
        }
        batcher.PopMatrix();
    }
}