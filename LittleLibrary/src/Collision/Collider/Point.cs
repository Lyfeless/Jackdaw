using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class PointCollider(Vector2 point) : Collider {
    Vector2 Point = point;

    public override Rect Bounds => new(Point, Vector2.Zero);

    public override Vector2 Center => Point;

    public override bool Multi => false;

    public override Collider[] GetSubColliders(Rect bounds) => [this];

    public override Vector2 Support(Vector2 position, Vector2 direction) {
        return Point + position;
    }
}