using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class SpriteSingle(Subtexture texture) : Sprite() {
    readonly Subtexture Texture = texture;

    public override Point2 Size => (Point2)Texture.Size;
    public override RectInt Bounds => new(Point2.Zero, Size);

    public SpriteSingle(Assets assets, string texture) : this(assets.GetTexture(texture)) { }

    public override void Render(Batcher batcher) {
        //! FIXME (Alex): Flip code might still be incorrect, re-check
        batcher.Image(Texture, Offset + (Bounds.Size / 2), Bounds.Center, FlipScale(), 0, Color);
    }
}