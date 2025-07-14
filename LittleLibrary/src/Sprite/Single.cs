using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class SpriteSingle(Subtexture texture) : Sprite() {
    readonly Subtexture Texture = texture;

    public override Vector2 Size => Texture.Size;
    public override Rect Bounds => new(Vector2.Zero, Texture.Size);

    public SpriteSingle(Assets assets, string texture) : this(assets.GetTexture(texture)) { }

    public override void Render(Batcher batcher) {
        batcher.Image(Texture, Color);
    }
}