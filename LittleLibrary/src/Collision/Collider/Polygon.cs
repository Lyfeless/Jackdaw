using Foster.Framework;

namespace LittleLib;

//! FIXME (Alex): Doesn't work with per-collider tags
public class PolygonCollider(Polygon polygon) : MultiCollider([.. polygon.Triangles.Select(e => new ConvexPolygonCollider(e))]) { }