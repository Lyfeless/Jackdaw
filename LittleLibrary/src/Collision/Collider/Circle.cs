using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class CircleCollider(Circle circle) : Collider {
    Circle circle = circle;
    public Circle Circle {
        get => circle;
        set {
            bounds = GetBounds(value);
            circle = value;
        }
    }

    Rect bounds = GetBounds(circle);
    public override Rect Bounds => bounds;

    public override Vector2 Center => circle.Position;

    public override bool Multi => false;
    public override Collider[] GetSubColliders(Rect bounds) => [this];

    static Rect GetBounds(Circle circle) {
        return new Rect(-circle.Radius, -circle.Radius, circle.Radius + circle.Radius, circle.Radius + circle.Radius);
    }

    public override Vector2 Support(Vector2 position, Vector2 direction) {
        return position + Circle.Position + (Circle.Radius * direction.Normalized());
    }
}