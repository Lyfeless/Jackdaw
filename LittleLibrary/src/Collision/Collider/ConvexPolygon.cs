using Foster.Framework;

namespace LittleLib;

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
        //! FIXME (Alex): Hacky way to convert a triangle to a polygon, consider cleaning up
        Vertices = [
            triangle.GetPoint(0),
            triangle.GetPoint(1),
            triangle.GetPoint(2),
        ]
    }) { }
}