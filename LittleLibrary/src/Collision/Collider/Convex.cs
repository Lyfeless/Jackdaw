using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public abstract class ConvexCollider(IConvexShape shape) : ICollider {
    IConvexShape shape = shape;
    public IConvexShape Shape {
        get => shape;
        set {
            bounds = GetBounds(value);
            shape = value;
        }
    }

    Rect bounds = GetBounds(shape);
    public Rect Bounds => bounds;
    public Vector2 Center => bounds.Center;

    public bool Multi => false;
    public ICollider[] GetSubColliders(Rect bounds) => [this];

    static Rect GetBounds(IConvexShape shape) {
        BoundsBuilder builder = new();
        for (int i = 0; i < shape.Points; ++i) {
            builder.Add(shape.GetPoint(i));
        }
        return builder.Rect;
    }

    public Vector2 Support(Vector2 position, Vector2 direction) {
        float furthestDistance = float.NegativeInfinity;
        Vector2 furthestPoint = Vector2.Zero;

        for (int i = 0; i < Shape.Points; ++i) {
            Vector2 comparePoint = Shape.GetPoint(i) + position;
            float compareDistance = Vector2.Dot(comparePoint, direction);
            if (compareDistance > furthestDistance) {
                furthestDistance = compareDistance;
                furthestPoint = comparePoint;
            }
        }

        return furthestPoint;
    }
}