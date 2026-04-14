using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A vertically resizable sprite that tiles the texture to fill space.
/// </summary>
/// <param name="texture">The texture to tile.</param>
/// <param name="height">The sprite's resized height.</param>
public class SpriteTilingVertical(Subtexture texture, int height = 0) : Sprite {
    readonly Subtexture Texture = texture;
    readonly ScalingAxis height = new(height);

    /// <summary>
    /// The sprite's resized height.
    /// </summary>
    public int Height { get => height.Value; set => height.Value = value; }

    /// <summary>
    /// If tiling should grow from the texture's origin or outer edge.
    /// </summary>
    public bool PadOrigin = false;

    public override RectInt Bounds => new(Offset.X, Offset.Y + height.Position, (int)Texture.Width, height.Size);

    public override void Render(Batcher batcher) {
        if (Height == 0 || Texture.Width == 0 || Texture.Height == 0) { return; }

        if (Height < 0) {
            Render(batcher, Height, !PadOrigin);
        }
        else {
            Render(batcher, 0, PadOrigin);
        }
    }

    void Render(Batcher batcher, int offset, bool padTop) {
        Point2 textureSize = Texture.Size.FloorToPoint2();

        int count = Math.Abs(Height) / textureSize.Y;
        int remainder = Math.Abs(Height) % textureSize.Y;
        if (padTop) {
            batcher.Image(Texture.GetClipSubtexture(new(0, Texture.Height - remainder, Texture.Width, remainder)), Offset + new Point2(0, offset), Color);
            offset += remainder;
        }
        else {
            batcher.Image(Texture.GetClipSubtexture(new(0, 0, Texture.Width, remainder)), Offset + new Point2(0, offset + (count * textureSize.Y)), Color);
        }

        for (int i = 0; i < count; ++i) {
            batcher.Image(Texture, Offset + new Point2(0, offset + (i * textureSize.Y)), Color);
        }
    }
}