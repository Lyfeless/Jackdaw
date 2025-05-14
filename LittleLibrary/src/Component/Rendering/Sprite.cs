using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class SpriteComponent(LittleGame game, Sprite sprite, Vector2? offset = null) : Component(game) {
    Sprite Sprite = sprite;
    Vector2 Offset = offset ?? Vector2.Zero;

    public SpriteComponent(LittleGame game, Subtexture sprite, Color color, Vector2? offset = null)
        : this(game, new SpriteSingle(sprite, color), offset) { }

    public SpriteComponent(LittleGame game, Subtexture sprite, Vector2? offset = null)
        : this(game, new SpriteSingle(sprite), offset) { }

    public SpriteComponent(LittleGame game, string sprite, Color color, Vector2? offset = null)
        : this(game, game.Assets.GetTexture(sprite), color, offset) { }

    public SpriteComponent(LittleGame game, string sprite, Vector2? offset = null)
        : this(game, game.Assets.GetTexture(sprite), offset) { }

    public override void Render(Batcher batcher) {
        //! FIXME (Alex): Need sprite bounds function to cull
        // if(Game.Viewspace.Bounds.Contains(new Rect(Actor.GlobalPosition + Offset, Sprite.S)))

        batcher.PushMatrix(Offset);
        Sprite.Render(batcher);
        batcher.PopMatrix();
    }
}