using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

internal readonly struct ColliderContextPair(ColliderContext a, ColliderContext b) {
    public readonly ColliderContext A = a;
    public readonly ColliderContext B = b;

    public readonly bool Moving = a.Moving || b.Moving;

    public ColliderContextPair WithA(Collider collider) => WithA(A.WithCollider(collider));
    public ColliderContextPair WithB(Collider collider) => WithB(B.WithCollider(collider));
    public ColliderContextPair WithA(ColliderContext collider) => new(collider, B);
    public ColliderContextPair WithB(ColliderContext collider) => new(A, collider);

    public ColliderContextPair Flip() => new(B, A);

    public Vector2 Support(Vector2 direction)
        => A.Support(direction) - B.Support(-direction);

    public bool TagMatch(bool reversed = false) => reversed
        ? TagMatch(B, A)
        : TagMatch(A, B);

    static bool TagMatch(ColliderContext a, ColliderContext b) =>
        a.Collider.Mask.Empty ||
        b.Collider.Tags.Empty ||
        a.Collider.Mask.Any(b.Collider.Tags);

    public bool BoundsOverlap() => Moving ? BoundsOverlapMoving() : BoundsOverlapStatic();

    bool BoundsOverlapStatic() {
        Rect boundsAOffset = A.Collider.Bounds.TransformAABB(A.Position);
        Rect boundsBOffset = B.Collider.Bounds.TransformAABB(B.Position);
        return boundsAOffset.Overlaps(boundsBOffset);
    }

    bool BoundsOverlapMoving() {
        Rect boundAOffset = A.Collider.Bounds.TransformAABB(A.Position);
        Rect sweptBoundsB = SweptBounds(B.Collider, B.Velocity);
        Rect sweptBoundsBCombined = SweptBounds(sweptBoundsB, -A.Velocity);
        Rect sweptBoundsBCombinedOffset = sweptBoundsBCombined.TransformAABB(B.Position);
        return sweptBoundsBCombinedOffset.Overlaps(boundAOffset);
    }

    public Rect GetSubColliderBounds() => Moving ? GetSubColliderBoundsMoving() : GetSubColliderBoundsStatic();
    Rect GetSubColliderBoundsStatic() => B.Collider.Bounds.TransformAABB(B.Position * A.PositionInv);
    Rect GetSubColliderBoundsMoving() {
        Rect sweptBoundsB = SweptBounds(B.Collider, B.Velocity);
        return sweptBoundsB.TransformAABB(B.Position * A.PositionInv);
    }

    static Rect SweptBounds(Collider collider, Vector2 velocity)
        => SweptBounds(collider.Bounds, velocity);

    static Rect SweptBounds(Rect rect, Vector2 velocity) {
        Vector2 min = Vector2.Min(rect.TopLeft, rect.TopLeft + velocity);
        Vector2 max = Vector2.Max(rect.BottomRight, rect.BottomRight + velocity);
        return Rect.Between(min, max);
    }

    public List<ColliderContextPair> GetOverlappingPairs() {
        List<ColliderContextPair> pairs = [];
        GetOverlappingPairs(this, in pairs);
        return pairs;
    }

    static void GetOverlappingPairs(ColliderContextPair ctx, in List<ColliderContextPair> pairs, bool reversed = false) {
        if (!ctx.TagMatch(reversed) || !ctx.BoundsOverlap()) { return; }

        if (ctx.A.Collider.Multi) {
            Collider[] subs = ctx.A.Collider.GetSubColliders(ctx.GetSubColliderBounds());
            foreach (Collider collider in subs) { GetOverlappingPairs(ctx.WithA(collider), in pairs, reversed); }
            return;
        }

        if (ctx.B.Collider.Multi) { GetOverlappingPairs(ctx.Flip(), in pairs, !reversed); return; }

        pairs.Add(reversed ? ctx.Flip() : ctx);
    }
}

internal readonly struct ColliderContext {
    public readonly Collider Collider;
    public readonly Matrix3x2 Position;
    public readonly Matrix3x2 PositionInv;
    public readonly Vector2 Velocity;

    public readonly bool Moving;

    public ColliderContext(Collider collider, Matrix3x2 position, Matrix3x2 positionInv, Vector2 velocity) {
        Collider = collider;
        Position = position;
        PositionInv = positionInv;
        Velocity = velocity;

        Moving = Velocity != Vector2.Zero;
    }

    public ColliderContext(Collider collider, Matrix3x2 position, Matrix3x2 positionInv)
        : this(collider, position, positionInv, Vector2.Zero) { }
    public ColliderContext(Collider collider, Matrix3x2 position)
        : this(collider, position, position.Invert(), Vector2.Zero) { }
    public ColliderContext(Collider collider, Matrix3x2 position, Vector2 velocity)
        : this(collider, position, position.Invert(), velocity) { }
    public ColliderContext(Collider collider)
        : this(collider, Matrix3x2.Identity, Matrix3x2.Identity, Vector2.Zero) { }

    public ColliderContext(CollisionComponent component, Matrix3x2 position, Matrix3x2 positionInv, Vector2 velocity) {
        Collider = component.Collider;
        Position = position;
        PositionInv = positionInv;
        Velocity = velocity;
    }

    public ColliderContext(CollisionComponent component, Matrix3x2 position, Matrix3x2 positionInv)
        : this(component, position, positionInv, Vector2.Zero) { }
    public ColliderContext(CollisionComponent component, Matrix3x2 position)
        : this(component, position, position.Invert(), Vector2.Zero) { }
    public ColliderContext(CollisionComponent component, Matrix3x2 position, Vector2 velocity)
        : this(component, position, position.Invert(), velocity) { }
    public ColliderContext(CollisionComponent component, Vector2 velocity)
        : this(component, component.Actor.Transform.GlobalMatrix, component.Actor.Transform.GlobalMatrixInverse, velocity) { }
    public ColliderContext(CollisionComponent component)
        : this(component, component.Actor.Transform.GlobalMatrix, component.Actor.Transform.GlobalMatrixInverse, Vector2.Zero) { }

    public ColliderContext WithCollider(Collider collider) => new(
        collider,
        Position,
        PositionInv,
        Velocity
    );

    public Vector2 Support(Vector2 direction) {
        Vector2 adjustedDirection = direction.TransformDirection(PositionInv);
        return Vector2.Transform(Collider.Support(adjustedDirection), Position);
    }
}