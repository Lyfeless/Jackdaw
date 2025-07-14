using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class CircleCollider(Circle circle) : ICollider {
    Circle circle = circle;
    public Circle Circle {
        get => circle;
        set {
            bounds = GetBounds(value);
            circle = value;
        }
    }

    Rect bounds = GetBounds(circle);
    public Rect Bounds => bounds;

    public Vector2 Center => circle.Position;

    public bool Multi => false;
    public ICollider[] GetSubColliders(Rect bounds) => [this];

    static Rect GetBounds(Circle circle) {
        return new Rect(-circle.Radius, -circle.Radius, circle.Radius + circle.Radius, circle.Radius + circle.Radius);
    }

    public Vector2 Support(Vector2 position, Vector2 direction) {
        return position + Circle.Position + (Circle.Radius * direction.Normalized());
    }
}