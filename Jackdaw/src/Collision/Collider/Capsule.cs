using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A capsule-shaped collider, defined as two circles connected by a rectangle.
/// </summary>
/// <param name="point1">The center of the first circle.</param>
/// <param name="point2">The center of the second circle.</param>
/// <param name="radius">The circle radius.</param>
public class CapsuleCollider(Vector2 point1, Vector2 point2, float radius) : Collider {
    Vector2 point1 = point1;

    /// <summary>
    /// The center of the first circle.
    /// </summary>
    public Vector2 Point1 {
        get => point1;
        set {
            bounds = GetBounds(value, point2, radius);
            center = new Line(point1, point2).Midpoint;
            point1 = value;
        }
    }

    Vector2 point2 = point2;

    /// <summary>
    /// The center of the second circle.
    /// </summary>
    public Vector2 Point2 {
        get => point2;
        set {
            bounds = GetBounds(point1, value, radius);
            center = new Line(point1, point2).Midpoint;
            point2 = value;
        }
    }

    float radius = radius;

    /// <summary>
    /// The radius of both circles.
    /// </summary>
    public float Radius {
        get => radius;
        set {
            bounds = GetBounds(point1, point2, value);
            center = new Line(point1, point2).Midpoint;
            radius = value;
        }
    }

    /// <summary>
    /// A capsule-shaped collider, defined as two circles connected by a rectangle.
    /// </summary>
    /// <param name="height">The distance between the two circles.</param>
    /// <param name="radius">The radius of the circles.</param>
    public CapsuleCollider(float height, float radius)
        : this(Vector2.Zero, height, radius) { }

    /// <summary>
    /// A capsule-shaped collider, defined as two circles connected by a rectangle.
    /// </summary>
    /// <param name="position">The position in the middle of the two circles.</param>
    /// <param name="height">The distance between the two circles.</param>
    /// <param name="radius">The radius of the circles.</param>
    public CapsuleCollider(Vector2 position, float height, float radius)
        : this(position + new Vector2(0, height / 2), position - new Vector2(0, height / 2), radius) { }

    /// <summary>
    /// A capsule-shaped collider, defined as two circles connected by a rectangle.
    /// </summary>
    /// <param name="height">The distance between the two circles.</param>
    /// <param name="rotation">The capsule's rotation.</param>
    /// <param name="radius">The radius of the circles.</param>
    public CapsuleCollider(float height, float rotation, float radius)
        : this(Vector2.Zero, height, rotation, radius) { }

    /// <summary>
    /// A capsule-shaped collider, defined as two circles connected by a rectangle.
    /// </summary>
    /// <param name="position">The position in the middle of the two circles.</param>
    /// <param name="height">The distance between the two circles.</param>
    /// <param name="rotation">The capsule's rotation.</param>
    /// <param name="radius">The radius of the circles.</param>
    public CapsuleCollider(Vector2 position, float height, float rotation, float radius)
        : this(position, radius, Calc.AngleToVector(rotation, height / 2)) { }

    CapsuleCollider(Vector2 position, float radius, Vector2 delta)
        : this(position + delta, position - delta, radius) { }

    Rect bounds = GetBounds(point1, point2, radius);
    public override Rect Bounds => bounds;

    Vector2 center = new Line(point1, point2).Midpoint;
    public override Vector2 Center => center;

    public override bool Multi => false;
    public override Collider[] GetSubColliders(Rect bounds) => [this];

    public override Vector2 Support(Vector2 direction) {
        return (Vector2.Dot(point2 - point1, direction) < 0 ? point1 : point2) + (radius * direction.Normalized());
    }

    static Rect GetBounds(Vector2 point1, Vector2 point2, float radius) {
        float minX = MathF.Min(point1.X, point2.X) - radius;
        float maxX = MathF.Max(point1.X, point2.X) + radius;
        float minY = MathF.Min(point1.Y, point2.Y) - radius;
        float maxY = MathF.Max(point1.Y, point2.Y) + radius;
        return new(minX, minY, maxX - minX, maxY - minY);
    }
}