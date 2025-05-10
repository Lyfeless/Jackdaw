using Foster.Framework;

namespace LittleLib;

public class SpriteSingle(Subtexture texture, Color color) : Sprite() {
    readonly Subtexture Texture = texture;
    readonly Color Color = color;

    public SpriteSingle(Subtexture texture) : this(texture, Color.White) { }

    public override void Render(Batcher batcher) {
        batcher.Image(Texture, Color);
    }
}