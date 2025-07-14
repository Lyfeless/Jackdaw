using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class PointCollider(Vector2 point) : ICollider {
    Vector2 Point = point;

    public Rect Bounds => new(Point, Vector2.Zero);

    public Vector2 Center => Point;

    public bool Multi => false;

    public ICollider[] GetSubColliders(Rect bounds) => [this];

    public Vector2 Support(Vector2 position, Vector2 direction) {
        return Point + position;
    }
}