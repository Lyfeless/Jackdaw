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

    public override bool Overlaps(Collider with, out Vector2 pushout) {
        if (with is ConvexCollider withConvex) {
            return withConvex.Shape.Overlaps(Circle, out pushout);
        }

        if (with is CircleCollider withCircle) {
            return Circle.Overlaps(withCircle.Circle, out pushout);
        }

        if (with is GridCollider withGrid) {
            return withGrid.Overlaps(this, out pushout);
        }

        Console.WriteLine($"Collider: Undefined collision interaction between colliders {GetType()} and {with.GetType()}");
        pushout = Vector2.Zero;
        return false;
    }

    static Rect GetBounds(Circle circle) {
        return new BoundsBuilder(new Rect(-circle.Radius, -circle.Radius, circle.Radius * 2, circle.Radius * 2)).Rect;
    }

    public override Collider Offset(Vector2 amount) {
        return new CircleCollider(new(Circle.Position + amount, Circle.Radius));
    }
}