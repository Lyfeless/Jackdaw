using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A convex polygon-shaped collider.
/// </summary>
/// <param name="polygon">The polygon shape.</param>
public class ConvexPolygonCollider(ConvexPolygon polygon) : ConvexCollider(polygon) {
    /// <summary>
    /// Create a new convex collider from a triangle.
    /// </summary>
    /// <param name="triangle">The triangle collider.</param>
    public ConvexPolygonCollider(Triangle triangle) : this(new ConvexPolygon() {
        Vertices = [
            triangle.A,
            triangle.B,
            triangle.C,
        ]
    }) { }
}