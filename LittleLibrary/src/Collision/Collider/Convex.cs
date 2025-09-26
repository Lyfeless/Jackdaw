using System.Numerics;
using Foster.Framework;

namespace LittleLib;

/// <summary>
/// A generic collider structure for any convex shape.
/// </summary>
/// <param name="shape">The collider's shape.</param>
public abstract class ConvexCollider(IConvexShape shape) : Collider {
    IConvexShape shape = shape;

    /// <summary>
    /// The collider shape.
    /// </summary>
    public IConvexShape Shape {
        get => shape;
        set {
            bounds = GetBounds(value);
            shape = value;
        }
    }

    Rect bounds = GetBounds(shape);
    public override Rect Bounds => bounds;
    public override Vector2 Center => bounds.Center;

    public override bool Multi => false;
    public override Collider[] GetSubColliders(Rect bounds) => [this];

    static Rect GetBounds(IConvexShape shape) {
        BoundsBuilder builder = new();
        for (int i = 0; i < shape.Points; ++i) {
            builder.Add(shape.GetPoint(i));
        }
        return builder.Rect;
    }

    public override Vector2 Support(Vector2 direction) {
        float furthestDistance = float.NegativeInfinity;
        Vector2 furthestPoint = Vector2.Zero;

        for (int i = 0; i < Shape.Points; ++i) {
            Vector2 comparePoint = Shape.GetPoint(i);
            float compareDistance = Vector2.Dot(comparePoint, direction);
            if (compareDistance > furthestDistance) {
                furthestDistance = compareDistance;
                furthestPoint = comparePoint;
            }
        }

        return furthestPoint;
    }
}