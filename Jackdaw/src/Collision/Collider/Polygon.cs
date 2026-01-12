using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A polygon collider that isn't restricted by convexity.
/// </summary>
/// <param name="polygon">The polygon shape.</param>
// public class PolygonCollider(Polygon polygon) : MultiCollider([.. polygon.Triangles.Select(e => new ConvexPolygonCollider(e))]) { }
public class PolygonCollider(Polygon polygon) : MultiCollider(GetTriangles(polygon)) {
    static Collider[] GetTriangles(Polygon polygon) {
        TriangulationEnumerator tris = polygon.Triangles.GetEnumerator();
        List<Collider> colliders = [];
        do { colliders.Add(new ConvexPolygonCollider(tris.Current)); } while (tris.MoveNext());
        return [.. colliders];
    }
}