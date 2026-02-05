using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Simple utility to building a bounding box out of many points and rectangles.
/// </summary>
public struct BoundsBuilder {
    bool hasPoints = false;

    /// <summary>
    /// The total bounds all the added values cover.
    /// </summary>
    public Rect Rect = new(0, 0);

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
    /// <param name="point">The point to add.</param>
    /// <returns>The expanded builder.</returns>
    public BoundsBuilder Add(Vector2 point) {
        if (!hasPoints) { Rect = new(point, Vector2.Zero); }
        else { Rect = Rect.Conflate(point); }

        hasPoints = true;
        return this;
    }

    /// <summary>
    /// Add a single rect to the builder's bounds
    /// </summary>
    /// <param name="rect">The rect to add.</param>
    /// <returns>The expanded builder.</returns>
    public BoundsBuilder Add(Rect rect) {
        if (!hasPoints) { Rect = rect; }
        else { Rect = Rect.Conflate(rect); }

        hasPoints = true;
        return this;
    }

    /// <summary>
    /// Add multiple points to the builder's bounds
    /// </summary>
    /// <param name="points">The points to add.</param>
    /// <returns>The expanded builder.</returns>
    public BoundsBuilder Add(params Vector2[] points) {
        foreach (Vector2 point in points) { Add(point); }
        return this;
    }

    /// <summary>
    /// Add multiple rects to the builder's bounds
    /// </summary>
    /// <param name="rects">The rects to add.</param>
    /// <returns>The expanded builder.</returns>
    public BoundsBuilder Add(params Rect[] rects) {
        foreach (Rect rect in rects) { Add(rect); }
        return this;
    }

    /// <summary>
    /// Reset the builder back to empty
    /// </summary>
    /// <returns>The empty builder.</returns>
    public BoundsBuilder Clear() {
        hasPoints = false;
        Rect = new(0, 0);
        return this;
    }
}