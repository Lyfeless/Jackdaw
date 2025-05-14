using Foster.Framework;

namespace LittleLib;

public class PolygonCollider(Polygon polygon) : MultiCollider([.. polygon.Triangles.Select(e => new ConvexCollider(e))]) { }