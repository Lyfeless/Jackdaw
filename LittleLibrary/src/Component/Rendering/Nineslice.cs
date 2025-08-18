using System.Numerics;
using Foster.Framework;

namespace LittleLib;

//! FIXME (Alex): This should be a sprite?
//! FIXME (Alex): And make a sprite for tiling texture so I can make this less terrible
public class NinesliceComponent : Component {
    public enum ExtendBehavior {
        TILE,
        STRETCH
    }

    readonly Subtexture TopLeft;
    readonly Subtexture TopRight;
    readonly Subtexture BottomLeft;
    readonly Subtexture BottomRight;
    readonly SpriteTilingHorizontal Top;
    readonly SpriteTilingHorizontal Bottom;
    readonly SpriteTilingVertical Left;
    readonly SpriteTilingVertical Right;
    readonly SpriteTiling Center;

    // public ExtendBehavior Extend;
    public bool ConstrainSize;

    public BoundsComponent Bounds;

    public NinesliceComponent(LittleGame game, Subtexture texture, RectInt center) : base(game) {
        Game = game;
        Bounds = new(game, new(Vector2.Zero, texture.Size));

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

        //! FIXME (Alex): Need to make these work
        // Extend = extendBehavior;
        // ConstrainSize = constrainSize;
    }

    public NinesliceComponent(LittleGame game, string texture, RectInt center)
        : this(game, game.Assets.GetTexture(texture), center) { }

    protected override void Render(Batcher batcher) {
        if (!Game.Viewspace.Bounds.Overlaps(new Rect(Actor.GlobalPosition + Bounds.Position, Bounds.Size))) { return; }

        Point2 boundsSize = (Point2)Bounds.Size;
        Point2 topLeftSize = (Point2)TopLeft.Size;
        Point2 bottomRightSize = (Point2)BottomRight.Size;

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
    }
}