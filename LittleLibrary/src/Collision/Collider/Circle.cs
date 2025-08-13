using System.Numerics;
using Foster.Framework;

namespace LittleLib;

/// <summary>
/// A circle-shaped collider, defined by a position and radius.
/// </summary>
/// <param name="circle">The collision circle.</param>
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
        return new Rect(circle.Position.X - circle.Radius, circle.Position.Y - circle.Radius, circle.Radius + circle.Radius, circle.Radius + circle.Radius);
    }

    public override Vector2 Support(Vector2 position, Vector2 direction) {
        return position + Circle.Position + (Circle.Radius * direction.Normalized());
    }
}