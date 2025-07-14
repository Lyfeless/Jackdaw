using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class CapsuleCollider(Vector2 point1, Vector2 point2, float radius) : ICollider {
    Vector2 point1 = point1;
    public Vector2 Point1 {
        get => point1;
        set {
            bounds = GetBounds(value, point2, radius);
            center = CalcExtra.LineMidpoint(point1, point2);
            point1 = value;
        }
    }

    Vector2 point2 = point2;
    public Vector2 Point2 {
        get => point2;
        set {
            bounds = GetBounds(point1, value, radius);
            center = CalcExtra.LineMidpoint(point1, point2);
            point2 = value;
        }
    }

    float radius = radius;
    public float Radius {
        get => radius;
        set {
            bounds = GetBounds(point1, point2, value);
            center = CalcExtra.LineMidpoint(point1, point2);
            radius = value;
        }
    }

    public CapsuleCollider(float height, float radius)
        : this(Vector2.Zero, height, radius) { }
    public CapsuleCollider(Vector2 position, float height, float radius)
        : this(position + new Vector2(0, height / 2), position - new Vector2(0, height / 2), radius) { }
    public CapsuleCollider(float height, float rotation, float radius)
        : this(Vector2.Zero, height, rotation, radius) { }
    public CapsuleCollider(Vector2 position, float height, float rotation, float radius)
        : this(position, radius, Calc.AngleToVector(rotation, height / 2)) { }
    //! FIXME (Alex): Kinda jank to avoid constructor overlap like this but it's internal so it probably won't matter
    CapsuleCollider(Vector2 position, float radius, Vector2 delta)
        : this(position + delta, position - delta, radius) { }

    Rect bounds = GetBounds(point1, point2, radius);
    public Rect Bounds => bounds;

    Vector2 center = CalcExtra.LineMidpoint(point1, point2);
    public Vector2 Center => center;

    public bool Multi => false;
    public ICollider[] GetSubColliders(Rect bounds) => [this];

    public Vector2 Support(Vector2 position, Vector2 direction) {
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