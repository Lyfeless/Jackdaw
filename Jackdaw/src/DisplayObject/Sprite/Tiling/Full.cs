using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A resizable sprite that tiles the texture to fill space.
/// </summary>
/// <param name="texture">The texture to tile.</param>
/// <param name="size">The sprite's resized size.</param>
public class SpriteTiling(Subtexture texture, Point2 size) : Sprite {
    readonly Subtexture Texture = texture;

    readonly ScalingPoint2 size = new(size);

    public int Width { get => size.Width; set => size.Width = value; }
    public int Height { get => size.Height; set => size.Height = value; }

    /// <summary>
    /// If horizontal tiling should grow from the texture's origin or outer edge.
    /// </summary>
    public bool PadOriginX = false;

    /// <summary>
    /// If vertical tiling should grow from the texture's origin or outer edge.
    /// </summary>
    public bool PadOriginY = false;

    public override RectInt Bounds => new(Offset + size.Position, size.Size);

    public override void Render(Batcher batcher) {
        if (Width == 0 || Height == 0 || Texture.Width == 0 || Texture.Height == 0) { return; }

        bool padLeft = PadOriginX;
        bool padTop = PadOriginY;
        int offsetX = 0;
        int offsetY = 0;

        if (Width < 0) {
            padLeft = !padLeft;
            offsetX = Width;
        }

        if (Height < 0) {
            padTop = !padTop;
            offsetY = Height;
        }

        Render(batcher, new Point2(offsetX, offsetY), padLeft, padTop);
    }

    void Render(Batcher batcher, Point2 offset, bool padLeft, bool padTop) {
        Point2 textureSize = Texture.Size.RoundToPoint2();

        Point2 count = new(
            Math.Abs(Width) / textureSize.X,
            Math.Abs(Height) / textureSize.Y
        );

        Point2 remainder = new(
            Math.Abs(Width) % textureSize.X,
            Math.Abs(Height) % textureSize.Y
        );

        int cornerX = count.X * textureSize.X;
        int cornerY = count.Y * textureSize.Y;
        int clipX = 0;
        int clipY = 0;
        int clipWidth = remainder.X;
        int clipHeight = remainder.Y;
        int offsetChangeX = 0;
        int offsetChangeY = 0;

        if (padLeft) {
            cornerX = 0;
            clipX = textureSize.X - remainder.X;
            offsetChangeX = remainder.X;
        }

        if (padTop) {
            cornerY = 0;
            clipY = textureSize.Y - remainder.Y;
            offsetChangeY = remainder.Y;
        }

        batcher.Image(Texture.GetClipSubtexture(new(clipX, clipY, clipWidth, clipHeight)), Offset + offset + new Point2(cornerX, cornerY), Color);

        SpriteTilingHorizontal horizontal = new(Texture.GetClipSubtexture(new(0, clipY, textureSize.X, clipHeight)), Math.Abs(Width) - remainder.X) {
            Offset = Offset + offset + new Point2(offsetChangeX, cornerY),
            PadOrigin = padLeft
        };

        SpriteTilingVertical vertical = new(Texture.GetClipSubtexture(new(clipX, 0, clipWidth, textureSize.Y)), Math.Abs(Height) - remainder.Y) {
            Offset = Offset + offset + new Point2(cornerX, offsetChangeY),
            PadOrigin = padTop
        };

        offset += new Point2(offsetChangeX, offsetChangeY);

        for (int x = 0; x < count.X; ++x) {
            for (int y = 0; y < count.Y; ++y) {
                batcher.Image(Texture, Offset + offset + (new Point2(x, y) * Texture.Size), Color);
            }
        }

        horizontal.Render(batcher);
        vertical.Render(batcher);
    }
}