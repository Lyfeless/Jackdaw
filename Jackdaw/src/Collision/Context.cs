using System.Numerics;

namespace Jackdaw;

internal readonly struct ColliderContextPair(ColliderContext a, ColliderContext b) {
    public readonly ColliderContext A = a;
    public readonly ColliderContext B = b;

    public ColliderContextPair(
        Collider colliderA,
        Matrix3x2 positionA,
        Matrix3x2 positionInvA,
        Vector2 velocityA,
        Collider colliderB,
        Matrix3x2 positionB,
        Matrix3x2 positionInvB,
        Vector2 velocityB
    ) : this(
        new(colliderA, positionA, positionInvA, velocityA),
        new(colliderB, positionB, positionInvB, velocityB)
    ) { }

    public ColliderContextPair(
        Collider colliderA,
        Matrix3x2 positionA,
        Matrix3x2 positionInvA,
        Collider colliderB,
        Matrix3x2 positionB,
        Matrix3x2 positionInvB
    ) : this(
        new(colliderA, positionA, positionInvA),
        new(colliderB, positionB, positionInvB)
    ) { }

    public ColliderContextPair(
        Collider colliderA,
        Matrix3x2 positionA,
        Vector2 velocityA,
        Collider colliderB,
        Matrix3x2 positionB,
        Vector2 velocityB
    ) : this(
        new(colliderA, positionA, velocityA),
        new(colliderB, positionB, velocityB)
    ) { }

    public ColliderContextPair(
        Collider colliderA,
        Matrix3x2 positionA,
        Collider colliderB,
        Matrix3x2 positionB
    ) : this(
        new(colliderA, positionA),
        new(colliderB, positionB)
    ) { }

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
}

internal readonly struct ColliderContext(Collider collider, Matrix3x2 position, Matrix3x2 positionInv, Vector2 velocity) {
    public readonly Collider Collider = collider;
    public readonly Matrix3x2 Position = position;
    public readonly Matrix3x2 PositionInv = positionInv;
    public readonly Vector2 Velocity = velocity;

    public ColliderContext(Collider collider, Matrix3x2 position, Matrix3x2 positionInv)
        : this(collider, position, positionInv, Vector2.Zero) { }

    public ColliderContext(Collider collider, Matrix3x2 position)
        : this(collider, position, position.Invert(), Vector2.Zero) { }

    public ColliderContext(Collider collider, Matrix3x2 position, Vector2 velocity)
        : this(collider, position, position.Invert(), velocity) { }

    public ColliderContext(Collider collider)
        : this(collider, Matrix3x2.Identity, Matrix3x2.Identity, Vector2.Zero) { }

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