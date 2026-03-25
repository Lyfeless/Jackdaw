using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A circle-shaped collider, defined by a position and radius.
/// </summary>
/// <param name="circle">The collision circle.</param>
public class CircleCollider(Circle circle) : Collider {
    Circle circle = circle;

    /// <summary>
    /// The collider circle.
    /// </summary>
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

    static Rect GetBounds(Circle circle) => circle.Bounds;

    public override Vector2 Support(Vector2 direction, InvertableMatrix position)
        => GetGlobalPoint(Circle.Position + (Circle.Radius * GetLocalDirection(direction, position).Normalized()), position);
}