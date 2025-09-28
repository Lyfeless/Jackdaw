using Foster.Framework;

namespace Jackdaw;

//! FIXME (Alex): Doc comments

public class SpriteTilingHorizontal(Subtexture texture, int width = 0) : Sprite {
    readonly Subtexture Texture = texture;
    public int Width = width;
    public bool PadOrigin = false;

    public override Point2 Size => new(Math.Abs(Width), (int)Texture.Height);

    public override RectInt Bounds => new(new(Math.Min(Width, 0), 0), Size);
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
        Point2 textureSize = (Point2)Texture.Size;

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

public class SpriteTilingVertical(Subtexture texture, int height = 0) : Sprite {
    readonly Subtexture Texture = texture;
    public int Height = height;
    public bool PadOrigin = false;

    public override Point2 Size => new((int)Texture.Width, Math.Abs(Height));

    public override RectInt Bounds => new(new(0, Math.Min(Height, 0)), Size);
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
        Point2 textureSize = (Point2)Texture.Size;

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

public class SpriteTiling(Subtexture texture, Point2 size) : Sprite {
    readonly Subtexture Texture = texture;
    public int Width = size.X;
    public int Height = size.Y;

    public bool PadOriginX = false;
    public bool PadOriginY = false;

    public override Point2 Size { get => new(Math.Abs(Width), Math.Abs(Height)); }

    public override RectInt Bounds => new(new(Math.Min(Width, 0), Math.Min(Height, 0)), Size);

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
        Point2 textureSize = (Point2)Texture.Size;

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

        SpriteTilingHorizontal horizontal = new(texture.GetClipSubtexture(new(0, clipY, textureSize.X, clipHeight)), Math.Abs(Width) - remainder.X) {
            Offset = Offset + offset + new Point2(offsetChangeX, cornerY),
            PadOrigin = padLeft
        };

        SpriteTilingVertical vertical = new(texture.GetClipSubtexture(new(clipX, 0, clipWidth, textureSize.Y)), Math.Abs(Height) - remainder.Y) {
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