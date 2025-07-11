using Foster.Framework;

namespace LittleLib;

public class ConvexPolygonCollider(ConvexPolygon polygon) : ConvexCollider(polygon) {
    //! FIXME (Alex): Should this just be a child type of TriangleCollider? Might be wiser in the long run
    public ConvexPolygonCollider(Triangle triangle) : this(new ConvexPolygon() {
        //! FIXME (Alex): Hacky way to convert a triangle to a polygon, consider cleaning up
        Vertices = [
            triangle.GetPoint(0),
            triangle.GetPoint(1),
            triangle.GetPoint(2),
        ]
    }) { }
}