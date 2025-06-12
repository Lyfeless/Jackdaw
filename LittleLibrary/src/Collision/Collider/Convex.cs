using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public abstract class ConvexCollider(IConvexShape shape) : Collider {
    IConvexShape shape = shape;
    public IConvexShape Shape {
        get => shape;
        set {
            bounds = GetBounds(value);
            shape = value;
        }
    }

    Rect bounds = GetBounds(shape);
    public override Rect Bounds => bounds;

    //! FIXME (Alex): Exit will have issues if they make it far enough into an object, closest exit point will be on the other side
    public override bool Overlaps(Collider with, out Vector2 pushout) {
        if (with is ConvexCollider withConvex) {
            return shape.Overlaps(withConvex.Shape, out pushout);
        }

        if (with is CircleCollider withCircle) {
            return shape.Overlaps(withCircle.Circle, out pushout);
        }

        if (with is GridCollider withGrid) {
            return withGrid.Overlaps(this, out pushout);
        }

        Console.WriteLine($"Collider: Undefined collision interaction between colliders {GetType()} and {with.GetType()}");
        pushout = Vector2.Zero;
        return false;
    }

    static Rect GetBounds(IConvexShape shape) {
        BoundsBuilder builder = new();
        for (int i = 0; i < shape.Points; ++i) {
            builder.Add(shape.GetPoint(i));
        }
        return builder.Rect;
    }
}