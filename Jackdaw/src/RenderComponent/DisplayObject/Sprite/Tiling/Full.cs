using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A resizable sprite that tiles the texture to fill space.
/// </summary>
public class SpriteTiling : Sprite {
    readonly Subtexture Texture;
    readonly Vector2 halfSize;
    readonly ScalingPoint2 size;

    SpriteFlip storedFlip;
    Point2 tileStart;
    Point2 remainderStart;
    Point2 count;
    bool hasRemainderX;
    bool hasRemainderY;
    bool hasRemainderBoth;
    Subtexture clipTextureX;
    Subtexture clipTextureY;
    Subtexture clipTextureCorner;
    Vector2 halfClipSizeX;
    Vector2 halfClipSizeY;
    Vector2 halfClipSizeCorner;

    public int Width {
        get => size.Width;
        set {
            size.Width = value;
            CacheBounds();
            PrecalculateValues();
        }
    }
    public int Height {
        get => size.Height;
        set {
            size.Height = value;
            CacheBounds();
            PrecalculateValues();
        }
    }

    /// <summary>
    /// If horizontal tiling should grow from the texture's origin or outer edge.
    /// </summary>
    public bool PadOriginX = false;

    /// <summary>
    /// If vertical tiling should grow from the texture's origin or outer edge.
    /// </summary>
    public bool PadOriginY = false;

    /// <param name="texture">The texture to tile.</param>
    /// <param name="size">The sprite's resized size.</param>
    public SpriteTiling(Subtexture texture, Point2 size) {
        Texture = texture;
        halfSize = texture.Size / 2;
        this.size = new(size);

        CacheBounds();
        PrecalculateValues();
    }

    /// <param name="texture">The texture to tile.</param>
    /// <param name="width">The sprite's resized width.</param>
    /// <param name="height">The sprite's resized height.</param>
    public SpriteTiling(Subtexture texture, int width, int height) : this(texture, new(width, height)) { }

    public override Rect GetObjectBounds() => size.Bounds;

    public override void Render(Batcher batcher) {
        if (Width == 0 || Height == 0) { return; }

        if (storedFlip != Flip) { PrecalculateValues(); }
        Vector2 flipScale = Flip.GetScale();

        if (hasRemainderBoth) {
            batcher.Image(clipTextureCorner, Offset + remainderStart + halfClipSizeCorner, halfClipSizeCorner, flipScale, 0, Color);
        }

        if (hasRemainderX) {
            for (int i = 0; i < count.Y; ++i) {
                Vector2 iterPos = new(
                    remainderStart.X,
                    tileStart.Y + (i * Texture.Height)
                );
                batcher.Image(clipTextureX, Offset + iterPos + halfClipSizeX, halfClipSizeX, flipScale, 0, Color);
            }
        }

        if (hasRemainderY) {
            for (int i = 0; i < count.X; ++i) {
                Vector2 iterPos = new(
                    tileStart.X + (i * Texture.Width),
                    remainderStart.Y
                );
                batcher.Image(clipTextureY, Offset + iterPos + halfClipSizeY, halfClipSizeY, flipScale, 0, Color);
            }
        }

        for (int x = 0; x < count.X; ++x) {
            for (int y = 0; y < count.Y; ++y) {
                Vector2 iterPos = tileStart + new Vector2(x * Texture.Width, y * Texture.Height);
                batcher.Image(Texture, Offset + iterPos + halfSize, halfSize, flipScale, 0, Color);
            }
        }
    }

    void PrecalculateValues() {
        storedFlip = Flip;

        Point2 textureSize = Texture.Size.FloorToPoint2();

        Point2 absSize = new(Math.Abs(Width), Math.Abs(Height));
        Point2 minSize = new(Math.Min(Width, 0), Math.Min(Height, 0));
        count = absSize / textureSize;
        Point2 remainder = absSize - (count * textureSize);

        bool isNegativeX = Width < 0;
        bool isNegativeY = Height < 0;
        bool stateMatchX = isNegativeX == PadOriginX;
        bool stateMatchY = isNegativeY == PadOriginY;

        tileStart = remainder;
        remainderStart = Point2.Zero;

        if (stateMatchX) {
            tileStart.X = 0;
            remainderStart.X = absSize.X - remainder.X;
        }

        if (stateMatchY) {
            tileStart.Y = 0;
            remainderStart.Y = absSize.Y - remainder.Y;
        }

        tileStart += minSize;
        remainderStart += minSize;

        hasRemainderX = remainder.X > 0;
        hasRemainderY = remainder.Y > 0;
        hasRemainderBoth = hasRemainderX && hasRemainderY;

        Point2 remainderClipPosition = Point2.Zero;
        if (hasRemainderX) {
            if (Flip.X) { stateMatchX = !stateMatchX; }
            if (!stateMatchX) { remainderClipPosition.X = textureSize.X - remainder.X; }

            Rect clipBounds = new(remainderClipPosition.X, 0, remainder.X, textureSize.Y);
            halfClipSizeX = clipBounds.Size / 2;
            clipTextureX = Texture.GetClipSubtexture(clipBounds);
        }

        if (hasRemainderY) {
            if (Flip.Y) { stateMatchY = !stateMatchY; }
            if (!stateMatchY) { remainderClipPosition.Y = textureSize.Y - remainder.Y; }

            Rect clipBounds = new(0, remainderClipPosition.Y, textureSize.X, remainder.Y);
            halfClipSizeY = clipBounds.Size / 2;
            clipTextureY = Texture.GetClipSubtexture(clipBounds);
        }

        if (hasRemainderBoth) {
            Rect clipBounds = new(remainderClipPosition, remainder);
            halfClipSizeCorner = clipBounds.Size / 2;
            clipTextureCorner = Texture.GetClipSubtexture(clipBounds);
        }
    }
}