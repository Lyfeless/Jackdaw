using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A horizontally resizable sprite that tiles the texture to fill space.
/// </summary>
public class SpriteTilingHorizontal : Sprite {
    readonly Subtexture Texture;
    readonly Vector2 halfSize;
    readonly ScalingAxis width;

    SpriteFlip storedFlip;
    int tileStart;
    int remainderStart;
    int count;
    bool hasRemainder;
    Subtexture clipTexture;
    Vector2 halfClipSize;

    /// <summary>
    /// The sprite's resized width.
    /// </summary>
    public int Width {
        get => width.Value;
        set {
            width.Value = value;
            CacheBounds();
            PrecalculateValues();
        }
    }

    /// <summary>
    /// If tiling should grow from the texture's origin or outer edge.
    /// </summary>
    public bool PadOrigin = false;

    /// <param name="texture">The texture to tile.</param>
    /// <param name="width">The sprite's resized height.</param>
    public SpriteTilingHorizontal(Subtexture texture, int width = 0) {
        Texture = texture;
        halfSize = texture.Size / 2;
        this.width = new(width);

        CacheBounds();
        PrecalculateValues();
    }

    public override Rect GetObjectBounds() => new(width.Position, 0, width.Size, Texture.Height);

    public override void Render(Batcher batcher) {
        if (Width == 0) { return; }

        if (storedFlip != Flip) { PrecalculateValues(); }
        Vector2 flipScale = Flip.GetScale();

        if (hasRemainder) {
            batcher.Image(clipTexture, Offset + new Point2(remainderStart, 0) + halfClipSize, halfClipSize, flipScale, 0, Color);
        }

        for (int i = 0; i < count; ++i) {
            Vector2 iterPos = new(tileStart + (i * Texture.Width), 0);
            batcher.Image(Texture, Offset + iterPos + halfSize, halfSize, flipScale, 0, Color);
        }
    }

    void PrecalculateValues() {
        storedFlip = Flip;

        Point2 textureSize = Texture.Size.FloorToPoint2();

        int absWidth = Math.Abs(Width);
        int minWidth = Math.Min(Width, 0);
        count = absWidth / textureSize.X;
        int remainder = absWidth - (count * textureSize.X);

        bool isNegative = Width < 0;
        bool stateMatch = isNegative == PadOrigin;

        tileStart = remainder;
        remainderStart = 0;

        if (stateMatch) {
            tileStart = 0;
            remainderStart = absWidth - remainder;
        }

        tileStart += minWidth;
        remainderStart += minWidth;

        hasRemainder = remainder > 0;
        if (hasRemainder) {
            if (Flip.X) { stateMatch = !stateMatch; }
            int remainderClipWidth = stateMatch ? 0 : textureSize.X - remainder;

            Rect clipBounds = new(remainderClipWidth, 0, remainder, textureSize.Y);
            halfClipSize = clipBounds.Size / 2;
            clipTexture = Texture.GetClipSubtexture(clipBounds);
        }
    }
}