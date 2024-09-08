using System.Numerics;
using Foster.Framework;

class BoundsBuilder {
    float Top = float.PositiveInfinity;
    float Bottom = float.NegativeInfinity;
    float Left = float.PositiveInfinity;
    float Right = float.NegativeInfinity;
    int PointCount = 0;
    public Rect Rect => PointCount > 0 ? new(new Vector2(Left, Top), new Vector2(Right, Bottom)) : new(0, 0, 0, 0);

    public BoundsBuilder Add(Vector2 point) {
        if (PointCount == 0) {
            Top = point.Y;
            Bottom = point.Y;
            Left = point.X;
            Right = point.X;
        }
        else {
            Top = Math.Min(point.Y, Top);
            Bottom = Math.Max(point.Y, Bottom);
            Left = Math.Min(point.X, Left);
            Right = Math.Max(point.X, Right);
        }

        PointCount++;
        return this;
    }

    public BoundsBuilder Add(Rect rect) {
        if (PointCount == 0) {
            Top = rect.Top;
            Bottom = rect.Bottom;
            Left = rect.Left;
            Right = rect.Right;
        }
        else {
            Top = Math.Min(rect.Top, Top);
            Bottom = Math.Max(rect.Bottom, Bottom);
            Left = Math.Min(rect.Left, Left);
            Right = Math.Max(rect.Right, Right);
        }

        PointCount += 4;
        return this;
    }

    public BoundsBuilder Clear() {
        Top = float.PositiveInfinity;
        Bottom = float.NegativeInfinity;
        Left = float.PositiveInfinity;
        Right = float.NegativeInfinity;
        PointCount = 0;

        return this;
    }
}