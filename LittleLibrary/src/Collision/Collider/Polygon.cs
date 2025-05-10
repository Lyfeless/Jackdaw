using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class PolygonCollider(Polygon polygon) : Collider {
    public Polygon Polygon = polygon;

    public override Rect Bounds => Polygon.Bounds;

    public override bool Overlaps(Collider with, out Vector2 pushout) {
        //! FIXME (Alex): Unsure how to handle this
        throw new NotImplementedException();
    }
}