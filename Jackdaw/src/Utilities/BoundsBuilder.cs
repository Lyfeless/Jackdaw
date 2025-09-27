using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Simple utility to building a bounding box out of many points and rectangles.
/// </summary>
public struct BoundsBuilder {
    Vector2 min = Vector2.Zero;
    Vector2 max = Vector2.Zero;

    int PointCount = 0;

    /// <summary>
    /// The total bounds all the added values cover.
    /// </summary>
    public Rect Rect;

    /// <summary>
    /// Create a new empty builder.
    /// </summary>
    public BoundsBuilder() {
        SetRect();
    }

    /// <summary>
    /// Create a new builder the a starting size of a point.
    /// </summary>
    /// <param name="point">The point to add to the builder.</param>
    public BoundsBuilder(Vector2 point) {
        Add(point);
    }

    /// <summary>
    /// Create a new builder the a starting size of a rect.
    /// </summary>
    /// <param name="rect">The rect to add to the builder.</param>
    public BoundsBuilder(Rect rect) {
        Add(rect);
    }

    /// <summary>
    /// Create a new builder the a starting size of multiple points.
    /// </summary>
    /// <param name="points">The points to add to the builder.</param>
    public BoundsBuilder(params Vector2[] points) {
        Add(points);
    }

    /// <summary>
    /// Create a new builder the a starting size of multiple rects.
    /// </summary>
    /// <param name="rects">The rects to add to the builder.</param>
    public BoundsBuilder(params Rect[] rects) {
        Add(rects);
    }

    /// <summary>
    /// Add a single point to the builder's bounds
    /// </summary>
    /// <param name="point">the point to add.</param>
    /// <returns>The expanded builder.</returns>
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

    /// <summary>
    /// Add a single rect to the builder's bounds
    /// </summary>
    /// <param name="rect">the rect to add.</param>
    /// <returns>The expanded builder.</returns>
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

    /// <summary>
    /// Add multiple points to the builder's bounds
    /// </summary>
    /// <param name="points">the points to add.</param>
    /// <returns>The expanded builder.</returns>
    public BoundsBuilder Add(params Vector2[] points) {
        foreach (Vector2 point in points) {
            Add(point);
        }
        return this;
    }

    /// <summary>
    /// Add multiple rects to the builder's bounds
    /// </summary>
    /// <param name="rects">the rects to add.</param>
    /// <returns>The expanded builder.</returns>
    public BoundsBuilder Add(params Rect[] rects) {
        foreach (Rect rect in rects) {
            Add(rect);
        }
        return this;
    }

    /// <summary>
    /// Reset the builder back to empty
    /// </summary>
    /// <returns>The empty builder.</returns>
    public BoundsBuilder Clear() {
        min = Vector2.Zero;
        max = Vector2.Zero;
        PointCount = 0;

        SetRect();
        return this;
    }

    void SetRect() {
        Rect = PointCount > 0 ? new(min, max - min) : new(0, 0, 0, 0);
    }
}