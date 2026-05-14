using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A collider that takes up a single point.
/// </summary>
/// <param name="point">The point location.</param>
public class PointCollider(Vector2 point) : Collider {
    static readonly Vector2 BoundsOffset = new(0.0001f);
    static readonly Vector2 BoundsSize = new(0.0002f);

    /// <summary>
    /// The local point the collider should check.
    /// </summary>
    public Vector2 Point = point;

    public override Rect Bounds => new(Point + BoundsOffset, BoundsSize);

    public override Vector2 Center => Point;

    public override bool Multi => false;

    public override Collider[] GetSubColliders(Rect bounds) => [this];

    public override Vector2 Support(Vector2 direction, InvertableMatrix position) => GetGlobalPoint(Point, position);
}