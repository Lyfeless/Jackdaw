using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class SpriteSingle(Subtexture texture, Color color) : Sprite() {
    readonly Subtexture Texture = texture;
    readonly Color Color = color;

    public override Vector2 Size => Texture.Size;
    public override Rect Bounds => new(Vector2.Zero, Texture.Size);

    public SpriteSingle(Subtexture texture) : this(texture, Color.White) { }
    public SpriteSingle(Assets assets, string texture) : this(assets, texture, Color.White) { }
    public SpriteSingle(Assets assets, string texture, Color color) : this(assets.GetTexture(texture), color) { }

    public override void Render(Batcher batcher) {
        batcher.Image(Texture, Color);
    }
}