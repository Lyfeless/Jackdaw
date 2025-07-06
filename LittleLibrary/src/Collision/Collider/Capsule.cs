using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class CapsuleCollider(Vector2 point1, Vector2 point2, float radius) : Collider {
    Vector2 point1 = point1;
    public Vector2 Point1 {
        get => point1;
        set {
            bounds = GetBounds(value, point2, radius);
            point1 = value;
        }
    }

    Vector2 point2 = point2;
    public Vector2 Point2 {
        get => point2;
        set {
            bounds = GetBounds(point1, value, radius);
            point2 = value;
        }
    }

    float radius = radius;
    public float Radius {
        get => radius;
        set {
            bounds = GetBounds(point1, point2, value);
            radius = value;
        }
    }

    Rect bounds = GetBounds(point1, point2, radius);
    public override Rect Bounds => bounds;

    public override Vector2 Center => throw new NotImplementedException();

    public override bool Multi => false;
    public override Collider[] GetSubColliders(Collider collider, Vector2 position) => [this];

    public override Vector2 Support(Vector2 position, Vector2 direction) {
        return (Vector2.Dot(point2 - point1, direction) < 0 ? point1 : point2) + position + (radius * direction.Normalized());
    }

    static Rect GetBounds(Vector2 point1, Vector2 point2, float radius) {
        float minX = MathF.Min(point1.X, point2.X) - radius;
        float maxX = MathF.Max(point1.X, point2.X) + radius;
        float minY = MathF.Min(point1.Y, point2.Y) - radius;
        float maxY = MathF.Max(point1.Y, point2.Y) + radius;
        return new(minX, minY, maxX - minX, maxY - minY);
    }
}