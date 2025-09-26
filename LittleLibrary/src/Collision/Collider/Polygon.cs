using Foster.Framework;

namespace LittleLib;

/// <summary>
/// A polygon collider that isn't restricted by convexity.
/// </summary>
/// <param name="polygon">The polygon shape.</param>
public class PolygonCollider(Polygon polygon) : MultiCollider([.. polygon.Triangles.Select(e => new ConvexPolygonCollider(e))]) { }