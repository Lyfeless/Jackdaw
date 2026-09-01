using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

public class SpriteStretch : Sprite {
    readonly Subtexture Texture;
    readonly RectInt textureBounds;
    readonly Point2 halfSize;

    readonly BoundsComponent BoundsComponent;
    Rect oldRect;

    public SpriteStretch(Subtexture texture, BoundsComponent bounds) {
        Texture = texture;
        textureBounds = new(texture.Size.FloorToPoint2());
        halfSize = (texture.Size / 2).FloorToPoint2();
        BoundsComponent = bounds;
        oldRect = BoundsComponent.Rect;

        CacheBounds();
    }

    public override Rect GetObjectBounds() => BoundsComponent.Rect;

    public override void Render(Batcher batcher) {
        TryCacheBounds();
        Vector2 scale = BoundsComponent.Rect.Size / textureBounds.Size;
        batcher.Image(Texture, Bounds.Center, halfSize, Flip.GetScale() * scale, 0, Color);
    }

    void TryCacheBounds() {
        if (oldRect == BoundsComponent.Rect) { return; }

        oldRect = BoundsComponent.Rect;
        CacheBounds();
    }
}