using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A vertically resizable sprite that tiles the texture to fill space.
/// </summary>
public class SpriteTilingVertical : Sprite {
    readonly Subtexture Texture;
    readonly Vector2 halfSize;
    readonly ScalingAxis height;

    SpriteFlip storedFlip;
    int tileStart;
    int remainderStart;
    int count;
    bool hasRemainder;
    Subtexture clipTexture;
    Vector2 halfClipSize;

    /// <summary>
    /// The sprite's resized height.
    /// </summary>
    public int Height {
        get => height.Value;
        set {
            height.Value = value;
            CacheBounds();
            PrecalculateValues();
        }
    }

    /// <summary>
    /// If tiling should grow from the texture's origin or outer edge.
    /// </summary>
    public bool PadOrigin = false;

    /// <param name="texture">The texture to tile.</param>
    /// <param name="height">The sprite's resized height.</param>
    public SpriteTilingVertical(Subtexture texture, int height = 0) {
        Texture = texture;
        halfSize = texture.Size / 2;
        this.height = new(height);

        CacheBounds();
        PrecalculateValues();
    }

    public override Rect GetObjectBounds() => new(0, height.Position, Texture.Width, height.Size);

    public override void Render(Batcher batcher) {
        if (Height == 0 || Texture.Width == 0 || Texture.Height == 0) { return; }

        if (storedFlip != Flip) { PrecalculateValues(); }
        Vector2 flipScale = Flip.GetScale();

        if (hasRemainder) {
            batcher.Image(clipTexture, Offset + new Point2(0, remainderStart) + halfClipSize, halfClipSize, flipScale, 0, Color);
        }

        for (int i = 0; i < count; ++i) {
            Vector2 iterPos = new(0, tileStart + (i * Texture.Height));
            batcher.Image(Texture, Offset + iterPos + halfSize, halfSize, flipScale, 0, Color);
        }
    }

    void PrecalculateValues() {
        storedFlip = Flip;

        Point2 textureSize = Texture.Size.FloorToPoint2();

        int absHeight = Math.Abs(Height);
        int minHeight = Math.Min(Height, 0);
        count = absHeight / textureSize.Y;
        int remainder = absHeight - (count * textureSize.Y);

        bool isNegative = Height < 0;
        bool stateMatch = isNegative == PadOrigin;

        tileStart = remainder;
        remainderStart = 0;

        if (stateMatch) {
            tileStart = 0;
            remainderStart = absHeight - remainder;
        }

        tileStart += minHeight;
        remainderStart += minHeight;

        hasRemainder = remainder > 0;
        if (hasRemainder) {
            if (Flip.Y) { stateMatch = !stateMatch; }
            int remainderClipHeight = stateMatch ? 0 : textureSize.Y - remainder;

            Rect clipBounds = new(0, remainderClipHeight, textureSize.X, remainder);
            halfClipSize = clipBounds.Size / 2;
            clipTexture = Texture.GetClipSubtexture(clipBounds);
        }
    }
}