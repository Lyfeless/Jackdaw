using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class SpriteComponent(LittleGame game, Sprite sprite, Point2? offset = null) : Component(game) {
    public Sprite Sprite = sprite;
    public Point2 Offset = offset ?? Point2.Zero;

    public SpriteComponent(LittleGame game, Subtexture sprite, Color color, Point2? offset = null)
        : this(game, new SpriteSingle(sprite) { Color = color }, offset) { }

    public SpriteComponent(LittleGame game, Subtexture sprite, Point2? offset = null)
        : this(game, new SpriteSingle(sprite), offset) { }

    public SpriteComponent(LittleGame game, string sprite, Color color, Point2? offset = null)
        : this(game, game.Assets.GetTexture(sprite), color, offset) { }

    public SpriteComponent(LittleGame game, string sprite, Point2? offset = null)
        : this(game, game.Assets.GetTexture(sprite), offset) { }

    public override void Render(Batcher batcher) {
        if (!Game.Viewspace.Bounds.Overlaps(Sprite.Bounds.Translate(Actor.GlobalPositionRounded + Offset))) { return; }

        batcher.PushMatrix(Offset);
        batcher.RectLine(Sprite.Bounds, 2, Color.Green);
        Sprite.Render(batcher);
        batcher.PopMatrix();
    }
}