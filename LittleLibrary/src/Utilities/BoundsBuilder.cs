using System.Numerics;
using Foster.Framework;

class BoundsBuilder {
    float Top = float.PositiveInfinity;
    float Bottom = float.NegativeInfinity;
    float Left = float.PositiveInfinity;
    float Right = float.NegativeInfinity;
    int PointCount = 0;
    public Rect Rect;

    public BoundsBuilder() {
        SetRect();
    }

    public BoundsBuilder(Rect rect) {
        Add(rect);
    }

    public BoundsBuilder(Vector2 point) {
        Add(point);
    }

    public BoundsBuilder(params Vector2[] points) {
        Add(points);
    }

    public BoundsBuilder(params Rect[] rects) {
        Add(rects);
    }

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
        SetRect();
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
        SetRect();
        return this;
    }

    public BoundsBuilder Add(params Vector2[] points) {
        foreach (Vector2 point in points) {
            Add(point);
        }
        return this;
    }

    public BoundsBuilder Add(params Rect[] rects) {
        foreach (Rect rect in rects) {
            Add(rect);
        }
        return this;
    }

    public BoundsBuilder Clear() {
        Top = float.PositiveInfinity;
        Bottom = float.NegativeInfinity;
        Left = float.PositiveInfinity;
        Right = float.NegativeInfinity;
        PointCount = 0;

        SetRect();
        return this;
    }

    void SetRect() {
        Rect = PointCount > 0 ? new(Left, Top, Right - Left, Bottom - Top) : new(0, 0, 0, 0);
    }
}