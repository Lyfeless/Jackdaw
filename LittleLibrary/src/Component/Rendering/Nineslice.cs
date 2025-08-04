using System.Numerics;
using Foster.Framework;

namespace LittleLib;

//! FIXME (Alex): This should be a sprite?
public class NinesliceComponent : Component {
    public enum ExtendBehavior {
        TILE,
        STRETCH
    }

    Subtexture TopLeft;
    Subtexture TopRight;
    Subtexture BottomLeft;
    Subtexture BottomRight;
    Subtexture Top;
    Subtexture Bottom;
    Subtexture Left;
    Subtexture Right;
    Subtexture Center;

    public ExtendBehavior Extend;
    public bool ConstrainSize;

    public BoundsComponent Bounds;

    public NinesliceComponent(LittleGame game, Subtexture texture, RectInt center, ExtendBehavior extendBehavior = ExtendBehavior.TILE, bool constrainSize = true) : base(game) {
        Game = game;
        Bounds = new(game, new(0, 0));

        int leftWidth = center.Left;
        int rightWidth = (int)(texture.Width - center.Right);
        int topHeight = center.Top;
        int bottomHeight = (int)(texture.Height - center.Bottom);

        TopLeft = new(texture.Texture, new(texture.Source.Left, texture.Source.Top, leftWidth, topHeight));
        TopRight = new(texture.Texture, new(texture.Source.Right - rightWidth, texture.Source.Top, rightWidth, topHeight));
        BottomLeft = new(texture.Texture, new(texture.Source.Left, texture.Source.Bottom - bottomHeight, leftWidth, bottomHeight));
        BottomRight = new(texture.Texture, new(texture.Source.Right - rightWidth, texture.Source.Bottom - bottomHeight, rightWidth, bottomHeight));

        Top = new(texture.Texture, new(TopLeft.Source.Right, TopLeft.Source.Top, center.Width, topHeight));
        Bottom = new(texture.Texture, new(BottomLeft.Source.Right, BottomLeft.Source.Top, center.Width, bottomHeight));
        Left = new(texture.Texture, new(TopLeft.Source.Left, TopLeft.Source.Bottom, leftWidth, center.Height));
        Right = new(texture.Texture, new(TopRight.Source.Left, TopRight.Source.Bottom, rightWidth, center.Height));

        Center = new(texture.Texture, new(texture.Source.X + center.X, texture.Source.Y + center.Y, center.Width, center.Height));

        Extend = extendBehavior;
        ConstrainSize = constrainSize;
    }

    public NinesliceComponent(LittleGame game, string texture, RectInt center, ExtendBehavior extendBehavior = ExtendBehavior.TILE, bool ConstrainSize = true)
        : this(game, game.Assets.GetTexture(texture), center, extendBehavior, ConstrainSize) { }

    public override void Render(Batcher batcher) {
        //! FIXME (Alex): Verify culling is correct
        if (Game.Viewspace.Bounds.Overlaps(new Rect(Actor.GlobalPosition + Bounds.Position, Bounds.Size))) { return; }

        batcher.PushMatrix(Bounds.Position);

        int minWidth = (int)(TopLeft.Width + TopRight.Width);
        int minHeight = (int)(TopLeft.Height + BottomLeft.Height);

        Point2 size = (Point2)Bounds.Size;
        if (ConstrainSize) {
            size = new(Math.Max(minWidth, size.X), Math.Max(minHeight, size.Y));
        }

        int middleWidth = size.X - minWidth;
        float scaleX = middleWidth / Center.Width;
        int flooredTileCountX = (int)scaleX;
        int remainderWidth = (int)Math.Round(Top.Width * (scaleX - flooredTileCountX));

        int middleHeight = size.Y - minHeight;
        float scaleY = middleHeight / Center.Height;
        int flooredTileCountY = (int)scaleY;
        int remainderHeight = (int)Math.Round(Left.Height * (scaleY - flooredTileCountY));

        //! FIXME (Alex): This is so gross looking but I'm not sure moving it to functions would make it any less gross
        if (Extend == ExtendBehavior.TILE) {
            for (int i = 0; i < flooredTileCountX; ++i) {
                // Primary top and bottom fill
                batcher.Image(Top, TopLeft.Frame.TopRight + new Point2(i * (int)Top.Frame.Width, 0), Color.White);
                batcher.Image(Bottom, TopLeft.Frame.BottomRight + new Point2(i * (int)Bottom.Frame.Width, middleHeight), Color.White);

                batcher.Image(Center, new(0, 0, Center.Width, remainderHeight), TopLeft.Frame.BottomRight + new Point2(i * (int)Bottom.Frame.Width, middleHeight - remainderHeight), Vector2.Zero, Vector2.One, 0, Color.White);
            }
            batcher.Image(Top, new(0, 0, remainderWidth, Top.Height), TopLeft.Frame.TopRight + new Point2(flooredTileCountX * (int)Top.Frame.Width, 0), Vector2.Zero, Vector2.One, 0, Color.White);
            batcher.Image(Bottom, new(0, 0, remainderWidth, Bottom.Height), TopLeft.Frame.BottomRight + new Point2(flooredTileCountX * (int)Bottom.Frame.Width, middleHeight), Vector2.Zero, Vector2.One, 0, Color.White);

            for (int i = 0; i < flooredTileCountY; ++i) {
                // Primary left and right fill
                batcher.Image(Left, TopLeft.Frame.BottomLeft + new Point2(0, i * (int)Left.Frame.Height), Color.White);
                batcher.Image(Right, TopLeft.Frame.BottomRight + new Point2(middleWidth, i * (int)Right.Frame.Height), Color.White);

                batcher.Image(Center, new(0, 0, remainderWidth, Center.Height), TopLeft.Frame.BottomRight + new Point2(middleWidth - remainderWidth, i * (int)Right.Frame.Height), Vector2.Zero, Vector2.One, 0, Color.White);
            }
            batcher.Image(Left, new(0, 0, Left.Width, remainderHeight), TopLeft.Frame.BottomLeft + new Point2(0, flooredTileCountY * (int)Left.Frame.Height), Vector2.Zero, Vector2.One, 0, Color.White);
            batcher.Image(Right, new(0, 0, Right.Width, remainderHeight), TopLeft.Frame.BottomRight + new Point2(middleWidth, flooredTileCountY * (int)Right.Frame.Height), Vector2.Zero, Vector2.One, 0, Color.White);

            for (int x = 0; x < flooredTileCountX; ++x) {
                for (int y = 0; y < flooredTileCountY; ++y) {
                    batcher.Image(Center, TopLeft.Frame.BottomRight + new Point2((int)Center.Width * x, (int)Center.Height * y), Color.White);
                }
            }
            batcher.Image(Center, new(0, 0, remainderWidth, remainderHeight), size - BottomRight.Source.Size - new Vector2(remainderWidth, remainderHeight), Vector2.Zero, Vector2.One, 0, Color.White);
        }
        else {
            batcher.Image(Top, TopLeft.Frame.TopRight, Vector2.Zero, new(scaleX, 1), 0, Color.White);
            batcher.Image(Bottom, new(BottomLeft.Width, size.Y - BottomLeft.Height), Vector2.Zero, new(scaleX, 1), 0, Color.White);

            batcher.Image(Left, TopLeft.Frame.BottomLeft, Vector2.Zero, new(1, scaleY), 0, Color.White);
            batcher.Image(Right, new(size.X - TopRight.Width, TopRight.Height), Vector2.Zero, new(1, scaleY), 0, Color.White);

            batcher.Image(Center, TopLeft.Frame.BottomRight, Vector2.Zero, new(scaleX, scaleY), 0, Color.White);
        }

        batcher.Image(TopRight, new(size.X - TopRight.Width, 0), Color.White);
        batcher.Image(BottomLeft, new(0, size.Y - BottomLeft.Height), Color.White);
        batcher.Image(BottomRight, size - BottomRight.Source.Size, Color.White);
        batcher.Image(TopLeft, Vector2.Zero, Color.White);

        batcher.PopMatrix();
    }
}