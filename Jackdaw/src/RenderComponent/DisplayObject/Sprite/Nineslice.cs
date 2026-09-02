using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A tiling sprite with borders.
/// </summary>
public class SpriteNineslice : Sprite {
    /// <summary>
    /// The options available for extending the size of the texture.
    /// </summary>
    public enum ExtendBehavior {
        /// <summary>
        /// Repeat the texture without any distortion.
        /// </summary>
        TILE,
        /// <summary>
        /// Stretch a single texture to the desired size.
        /// </summary>
        STRETCH
    }

    /// <summary>
    /// The options available for limiting the minimum size.
    /// </summary>
    public enum ConstrainBehavior {
        /// <summary>
        /// Don't constrain the size.
        /// </summary>
        NONE,
        /// <summary>
        /// Restrict the object from getting any smaller than the original texture size.
        /// </summary>
        TEXTURE_SIZE,
        /// <summary>
        /// Restrict the object from getting any smaller than the border size.
        /// </summary>
        BORDER_SIZE
    }

    readonly Subtexture Texture;

    readonly SpriteSingle TopLeft;
    readonly SpriteSingle TopRight;
    readonly SpriteSingle BottomLeft;
    readonly SpriteSingle BottomRight;

    readonly SpriteTilingHorizontal TopTiling;
    readonly SpriteTilingHorizontal BottomTiling;
    readonly SpriteTilingVertical LeftTiling;
    readonly SpriteTilingVertical RightTiling;
    readonly SpriteTiling CenterTiling;

    readonly SpriteStretch TopStretch;
    readonly SpriteStretch BottomStretch;
    readonly SpriteStretch LeftStretch;
    readonly SpriteStretch RightStretch;
    readonly SpriteStretch CenterStretch;

    ExtendBehavior extend = ExtendBehavior.TILE;
    ConstrainBehavior constrain = ConstrainBehavior.TEXTURE_SIZE;
    Rect constrainedBounds;

    Rect oldRect;
    SpriteFlip oldFlip;
    Point2 oldOffset;

    /// <summary>
    /// The method used to extend the texture past its regular size.
    /// NOTE: Currently not implemented, defaults to tile.
    /// </summary>
    public ExtendBehavior Extend {
        get => extend;
        set {
            extend = value;
            PrecalculateValues();
        }
    }

    /// <summary>
    /// The method used to limit the minimum size.
    /// </summary>
    public ConstrainBehavior Constrain {
        get => constrain;
        set {
            constrain = value;
            PrecalculateValues();
        }
    }

    /// <summary>
    /// The bounds the nineslice should extend to.
    /// </summary>
    public BoundsComponent BoundsComponent;

    public override Rect GetObjectBounds() => constrainedBounds;

    /// <summary>
    /// Create a new nineslice object.
    /// </summary>
    /// <param name="texture">The texture to extend.</param>
    /// <param name="center">The position middle of the texture that doesn't include any of the edges.</param>
    /// <param name="bounds">The bounds object the nineslice should fill.</param>
    public SpriteNineslice(Subtexture texture, RectInt center, BoundsComponent bounds) : base() {
        Texture = texture;
        BoundsComponent = bounds;

        int leftWidth = center.Left;
        int rightWidth = (int)texture.Width - center.Right;
        int topHeight = center.Top;
        int bottomHeight = (int)texture.Height - center.Bottom;

        TopLeft = new(texture.GetClipSubtexture(new(0, 0, leftWidth, topHeight)));
        TopRight = new(texture.GetClipSubtexture(new(center.Right, 0, rightWidth, topHeight)));
        BottomLeft = new(texture.GetClipSubtexture(new(0, center.Bottom, leftWidth, bottomHeight)));
        BottomRight = new(texture.GetClipSubtexture(new(center.Right, center.Bottom, rightWidth, bottomHeight)));

        Subtexture top = texture.GetClipSubtexture(new(center.Left, 0, center.Width, topHeight));
        Subtexture bottom = texture.GetClipSubtexture(new(center.Left, center.Bottom, center.Width, bottomHeight));
        Subtexture left = texture.GetClipSubtexture(new(0, center.Top, leftWidth, center.Height));
        Subtexture right = texture.GetClipSubtexture(new(center.Right, center.Top, rightWidth, center.Height));
        Subtexture middle = texture.GetClipSubtexture(center);

        TopTiling = new(top, 0);
        BottomTiling = new(bottom, 0);
        LeftTiling = new(left, 0);
        RightTiling = new(right, 0);
        CenterTiling = new(middle, 0, 0);

        TopStretch = new(top, new(bounds.Game, new(top.Size)));
        BottomStretch = new(bottom, new(bounds.Game, new(bottom.Size)));
        LeftStretch = new(left, new(bounds.Game, new(left.Size)));
        RightStretch = new(right, new(bounds.Game, new(right.Size)));
        CenterStretch = new(middle, new(bounds.Game, new(middle.Size)));

        PrecalculateValues();
        CacheBounds();
    }

    /// <summary>
    /// Create a new nineslice object.
    /// </summary>
    /// <param name="assets">The game instance asset container.</param>
    /// <param name="texture">The texture name id.</param>
    /// <param name="center">The position middle of the texture that doesn't include any of the edges.</param>
    /// <param name="bounds">The bounds object the nineslice should fill.</param>
    public SpriteNineslice(Assets assets, string texture, RectInt center, BoundsComponent bounds)
        : this(assets.GetSubtexture(texture), center, bounds) { }

    public override void Render(Batcher batcher) {
        if (oldRect != BoundsComponent.Rect || oldFlip != Flip || oldOffset != Offset) { PrecalculateValues(); }

        switch (Extend) {
            case ExtendBehavior.TILE:
                TopTiling.Render(batcher);
                BottomTiling.Render(batcher);
                LeftTiling.Render(batcher);
                RightTiling.Render(batcher);
                CenterTiling.Render(batcher);
                break;
            case ExtendBehavior.STRETCH:
                TopStretch.Render(batcher);
                BottomStretch.Render(batcher);
                LeftStretch.Render(batcher);
                RightStretch.Render(batcher);
                CenterStretch.Render(batcher);
                break;
        }

        TopLeft.Render(batcher);
        TopRight.Render(batcher);
        BottomLeft.Render(batcher);
        BottomRight.Render(batcher);
    }

    static Point2 LimitSize(Point2 size, Point2 limit) {
        return new(
            Math.Max(size.X, limit.X),
            Math.Max(size.Y, limit.Y)
        );
    }

    void PrecalculateValues() {
        oldRect = BoundsComponent.Rect;
        oldFlip = Flip;
        oldOffset = Offset;

        Point2 boundsSize = BoundsComponent.Size.CeilingToPoint2();
        Point2 topLeftSize = TopLeft.Size.CeilingToPoint2();
        Point2 bottomRightSize = BottomRight.Size.CeilingToPoint2();

        if (Constrain != ConstrainBehavior.NONE) {
            boundsSize = Constrain switch {
                ConstrainBehavior.TEXTURE_SIZE => LimitSize(boundsSize, Texture.Size.FloorToPoint2()),
                ConstrainBehavior.BORDER_SIZE => LimitSize(boundsSize, topLeftSize + bottomRightSize),
                _ => boundsSize
            };
        }

        constrainedBounds = new(BoundsComponent.Position, boundsSize);

        Point2 middleSize = boundsSize - topLeftSize - bottomRightSize;

        int leftPosition = 0;
        int rightPosition = boundsSize.X - bottomRightSize.X;
        int topPosition = 0;
        int bottomPosition = boundsSize.Y - bottomRightSize.Y;
        int centerX = topLeftSize.X;
        int centerY = topLeftSize.Y;

        if (Flip.X) {
            rightPosition = 0;
            leftPosition = boundsSize.X - topLeftSize.X;
            centerX = bottomRightSize.X;
        }
        if (Flip.Y) {
            bottomPosition = 0;
            topPosition = boundsSize.Y - topLeftSize.Y;
            centerY = bottomRightSize.Y;
        }

        TopLeft.Offset = Offset + new Point2(leftPosition, topPosition);
        TopRight.Offset = Offset + new Point2(rightPosition, topPosition);
        BottomLeft.Offset = Offset + new Point2(leftPosition, bottomPosition);
        BottomRight.Offset = Offset + new Point2(rightPosition, bottomPosition);

        TopLeft.Flip = Flip;
        TopRight.Flip = Flip;
        BottomLeft.Flip = Flip;
        BottomRight.Flip = Flip;

        switch (Extend) {
            case ExtendBehavior.STRETCH:
                TopStretch.BoundsComponent.Size = new(middleSize.X, TopStretch.BoundsComponent.Size.Y);
                BottomStretch.BoundsComponent.Size = new(middleSize.X, BottomStretch.BoundsComponent.Size.Y);
                LeftStretch.BoundsComponent.Size = new(LeftStretch.BoundsComponent.Size.X, middleSize.Y);
                RightStretch.BoundsComponent.Size = new(RightStretch.BoundsComponent.Size.X, middleSize.Y);
                CenterStretch.BoundsComponent.Size = middleSize;

                TopStretch.Offset = Offset + new Point2(centerX, topPosition);
                BottomStretch.Offset = Offset + new Point2(centerX, bottomPosition);
                LeftStretch.Offset = Offset + new Point2(leftPosition, centerY);
                RightStretch.Offset = Offset + new Point2(rightPosition, centerY);
                CenterStretch.Offset = Offset + new Point2(centerX, centerY);

                TopStretch.Flip = Flip;
                BottomStretch.Flip = Flip;
                LeftStretch.Flip = Flip;
                RightStretch.Flip = Flip;
                CenterStretch.Flip = Flip;
                break;
            case ExtendBehavior.TILE:
                TopTiling.Width = middleSize.X;
                BottomTiling.Width = middleSize.X;
                LeftTiling.Height = middleSize.Y;
                RightTiling.Height = middleSize.Y;
                CenterTiling.Width = middleSize.X;
                CenterTiling.Height = middleSize.Y;

                TopTiling.Offset = Offset + new Point2(centerX, topPosition);
                BottomTiling.Offset = Offset + new Point2(centerX, bottomPosition);
                LeftTiling.Offset = Offset + new Point2(leftPosition, centerY);
                RightTiling.Offset = Offset + new Point2(rightPosition, centerY);
                CenterTiling.Offset = Offset + new Point2(centerX, centerY);

                TopTiling.Flip = Flip;
                BottomTiling.Flip = Flip;
                LeftTiling.Flip = Flip;
                RightTiling.Flip = Flip;
                CenterTiling.Flip = Flip;

                TopTiling.PadOrigin = Flip.X;
                BottomTiling.PadOrigin = Flip.X;
                LeftTiling.PadOrigin = Flip.Y;
                RightTiling.PadOrigin = Flip.Y;
                CenterTiling.PadOriginX = Flip.X;
                CenterTiling.PadOriginY = Flip.Y;
                break;
        }

        CacheBounds();
    }
}