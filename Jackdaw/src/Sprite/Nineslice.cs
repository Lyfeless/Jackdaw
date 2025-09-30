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

    readonly Subtexture Full;
    readonly Subtexture TopLeft;
    readonly Subtexture TopRight;
    readonly Subtexture BottomLeft;
    readonly Subtexture BottomRight;
    readonly SpriteTilingHorizontal Top;
    readonly SpriteTilingHorizontal Bottom;
    readonly SpriteTilingVertical Left;
    readonly SpriteTilingVertical Right;
    readonly SpriteTiling Center;

    /// <summary>
    /// The method used to extend the texture past its regular size.
    /// NOTE: Currently not implemented, defaults to tile.
    /// </summary>
    public ExtendBehavior Extend;

    /// <summary>
    /// The method used to limit the minimum size.
    /// </summary>
    public ConstrainBehavior Constrain;

    /// <summary>
    /// The bounds the nineslice should extend to.
    /// </summary>
    public BoundsComponent BoundsComponent;

    /// <summary>
    /// The Rectangle bounds of the nineslice.
    /// </summary>
    public override RectInt Bounds => (RectInt)BoundsComponent.Rect;

    /// <summary>
    /// The dimensions of the nineslice.
    /// </summary>
    public override Point2 Size => (Point2)BoundsComponent.Size;


    /// <summary>
    /// Create a new nineslice object.
    /// </summary>
    /// <param name="texture">The texture to extend.</param>
    /// <param name="center">The position middle of the texture that doesn't include any of the edges.</param>
    /// <param name="bounds">The bounds object the nineslice should fill.</param>
    /// <param name="extend">The method used to extend the texture past its regular size.</param>
    /// <param name="constrain">The method used to limit the minimum size.</param>
    public SpriteNineslice(Subtexture texture, RectInt center, BoundsComponent bounds, ExtendBehavior extend = ExtendBehavior.TILE, ConstrainBehavior constrain = ConstrainBehavior.NONE) : base() {
        BoundsComponent = bounds;

        Full = texture;

        int leftWidth = center.Left;
        int rightWidth = (int)texture.Width - center.Right;
        int topHeight = center.Top;
        int bottomHeight = (int)texture.Height - center.Bottom;

        TopLeft = texture.GetClipSubtexture(new(0, 0, leftWidth, topHeight));
        TopRight = texture.GetClipSubtexture(new(center.Right, 0, rightWidth, topHeight));
        BottomLeft = texture.GetClipSubtexture(new(0, center.Bottom, leftWidth, bottomHeight));
        BottomRight = texture.GetClipSubtexture(new(center.Right, center.Bottom, rightWidth, bottomHeight));

        Top = new(texture.GetClipSubtexture(new(center.Left, 0, center.Width, topHeight)), 0) {
            Offset = new(leftWidth, 0)
        };
        Bottom = new(texture.GetClipSubtexture(new(center.Left, center.Bottom, center.Width, bottomHeight)), 0) {
            Offset = new(leftWidth, 0)
        };
        Left = new(texture.GetClipSubtexture(new(0, center.Top, center.Height, leftWidth)), 0) {
            Offset = new(0, topHeight)
        };
        Right = new(texture.GetClipSubtexture(new(center.Right, center.Top, center.Height, rightWidth)), 0) {
            Offset = new(0, topHeight)
        };

        Center = new(texture.GetClipSubtexture(center), Point2.Zero) {
            Offset = new(leftWidth, topHeight)
        };

        Extend = extend;
        Constrain = constrain;
    }

    /// <summary>
    /// Create a new nineslice object.
    /// </summary>
    /// <param name="assets">The game instance asset container.</param>
    /// <param name="texture">The texture name id.</param>
    /// <param name="center">The position middle of the texture that doesn't include any of the edges.</param>
    /// <param name="bounds">The bounds object the nineslice should fill.</param>
    /// <param name="extend">The method used to extend the texture past its regular size.</param>
    /// <param name="constrain">The method used to limit the minimum size.</param>
    public SpriteNineslice(Assets assets, string texture, RectInt center, BoundsComponent bounds, ExtendBehavior extend = ExtendBehavior.TILE, ConstrainBehavior constrain = ConstrainBehavior.NONE)
        : this(assets.GetTexture(texture), center, bounds, extend, constrain) { }

    public override void Render(Batcher batcher) {
        batcher.PushMatrix((Point2)BoundsComponent.Position);

        Point2 boundsSize = (Point2)BoundsComponent.Size;
        Point2 topLeftSize = (Point2)TopLeft.Size;
        Point2 bottomRightSize = (Point2)BottomRight.Size;

        if (Constrain != ConstrainBehavior.NONE) {
            boundsSize = Constrain switch {
                ConstrainBehavior.TEXTURE_SIZE => LimitSize(boundsSize, (Point2)Full.Size),
                ConstrainBehavior.BORDER_SIZE => LimitSize(boundsSize, topLeftSize + bottomRightSize),
                _ => boundsSize
            };
        }

        int middleWidth = boundsSize.X - topLeftSize.X - bottomRightSize.X;
        int middleHeight = boundsSize.Y - topLeftSize.Y - bottomRightSize.Y;

        Top.Width = middleWidth;
        Bottom.Width = middleWidth;
        Left.Height = middleHeight;
        Right.Height = middleHeight;
        Center.Width = middleWidth;
        Center.Height = middleHeight;

        Right.Offset = new Point2(topLeftSize.X + middleWidth, Right.Offset.Y);
        Bottom.Offset = new Point2(Bottom.Offset.X, topLeftSize.Y + middleHeight);

        Top.Render(batcher);
        Bottom.Render(batcher);
        Left.Render(batcher);
        Right.Render(batcher);
        Center.Render(batcher);

        batcher.Image(TopLeft, Point2.Zero, Color.White);
        batcher.Image(TopRight, new(boundsSize.X - bottomRightSize.X, 0), Color.White);
        batcher.Image(BottomLeft, new(0, boundsSize.Y - bottomRightSize.Y), Color.White);
        batcher.Image(BottomRight, new(boundsSize.X - bottomRightSize.X, boundsSize.Y - bottomRightSize.Y), Color.White);

        batcher.PopMatrix();
    }

    static Point2 LimitSize(Point2 size, Point2 limit) {
        return new(
            Math.Min(size.X, limit.X),
            Math.Min(size.Y, limit.Y)
        );
    }
}