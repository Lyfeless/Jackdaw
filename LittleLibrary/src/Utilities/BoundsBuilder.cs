using System.Numerics;
using Foster.Framework;

class BoundsBuilder {
    Vector2 min = Vector2.Zero;
    Vector2 max = Vector2.Zero;

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
            min = point;
            max = point;
        }
        else {
            min = Vector2.Min(min, point);
            max = Vector2.Max(max, point);
        }

        PointCount++;
        SetRect();
        return this;
    }

    public BoundsBuilder Add(Rect rect) {
        if (PointCount == 0) {
            min = rect.TopLeft;
            max = rect.BottomRight;
        }
        else {
            min = Vector2.Min(min, rect.TopLeft);
            max = Vector2.Max(max, rect.BottomRight);
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
        min = Vector2.NegativeInfinity;
        max = Vector2.PositiveInfinity;
        PointCount = 0;

        SetRect();
        return this;
    }

    void SetRect() {
        Rect = PointCount > 0 ? new(min, max - min) : new(0, 0, 0, 0);
    }
}