using Foster.Framework;

namespace LittleLib;

public class SpriteStack(params Sprite[] sprites) : Sprite() {
    readonly Sprite[] Sprites = sprites;

    public SpriteStack(params Subtexture[] sprites) : this([.. sprites.Select(e => new SpriteSingle(e))]) { }

    public override void Render(Batcher batcher) {
        foreach (Sprite sprite in Sprites) {
            sprite.Render(batcher);
        }
    }
}