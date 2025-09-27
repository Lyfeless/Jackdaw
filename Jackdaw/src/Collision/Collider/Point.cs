using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A collider that takes up a single point.
/// </summary>
/// <param name="point">The point location.</param>
public class PointCollider(Vector2 point) : Collider {
    Vector2 Point = point;

    public override Rect Bounds => new(Point, Vector2.Zero);

    public override Vector2 Center => Point;

    public override bool Multi => false;

    public override Collider[] GetSubColliders(Rect bounds) => [this];

    public override Vector2 Support(Vector2 direction) => Point;
}