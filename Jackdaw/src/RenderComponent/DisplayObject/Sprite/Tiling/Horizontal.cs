using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A horizontally resizable sprite that tiles the texture to fill space.
/// </summary>
public class SpriteTilingHorizontal : Sprite {
    readonly Subtexture Texture;
    readonly ScalingAxis width;

    /// <summary>
    /// The sprite's resized width.
    /// </summary>
    public int Width { get => width.Value; set => width.Value = value; }

    /// <summary>
    /// If tiling should grow from the texture's origin or outer edge.
    /// </summary>
    public bool PadOrigin = false;

    /// <param name="texture">The texture to tile.</param>
    /// <param name="width">The sprite's resized width.</param>
    public SpriteTilingHorizontal(Subtexture texture, int width = 0) {
        Texture = texture;
        this.width = new(width);

        CacheBounds();
    }

    public override Rect GetObjectBounds() => new(width.Position, 0, width.Size, Texture.Height);

    public override void Render(Batcher batcher) {
        if (Width == 0 || Texture.Width == 0 || Texture.Height == 0) { return; }

        if (Width < 0) {
            Render(batcher, Width, !PadOrigin);
        }
        else {
            Render(batcher, 0, PadOrigin);
        }
    }

    void Render(Batcher batcher, int offset, bool padLeft) {
        Point2 textureSize = Texture.Size.FloorToPoint2();

        int count = Math.Abs(Width) / textureSize.X;
        int remainder = Math.Abs(Width) % textureSize.X;
        if (padLeft) {
            batcher.Image(Texture.GetClipSubtexture(new(Texture.Width - remainder, 0, remainder, Texture.Height)), Offset + new Point2(offset, 0), Color);
            offset += remainder;
        }
        else {
            batcher.Image(Texture.GetClipSubtexture(new(0, 0, remainder, Texture.Height)), Offset + new Point2(offset + (count * textureSize.X), 0), Color);
        }

        for (int i = 0; i < count; ++i) {
            batcher.Image(Texture, Offset + new Point2(offset + (i * textureSize.X), 0), Color);
        }
    }
}