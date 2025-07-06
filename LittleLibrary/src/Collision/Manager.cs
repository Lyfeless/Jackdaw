using System.Linq.Expressions;
using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class CollisionManager {
    const int ITERATION_LIMIT = 32;
    const float PUSHOUT_TOLERANCE = 0.0001f;

    record struct ParsedCollider(CollisionComponent Component, Collider Collider);
    record struct CollisionSimplexInfo(Vector2 A, Vector2 B, Vector2 C, bool Collided);
    record struct Edge(int Index, Vector2 Normal, float Distance);

    //! FIXME (Alex): TEMP
    public static LittleGame Game;

    public enum BroadPhaseState {
        NONE,
        COLLISION,
        COLLISION_AND_RESOLUTION
    }
    public BroadPhaseState BroadPhase = BroadPhaseState.COLLISION_AND_RESOLUTION;

    readonly List<CollisionComponent> Colliders = [];

    public void Add(CollisionComponent collider) {
        Colliders.Add(collider);
    }

    public void Remove(CollisionComponent collider) {
        Colliders.Remove(collider);
    }

    public void Update() {
        //! FIXME (Alex): Disabling broadphase entirely for now
        return;


        if (Colliders.Count < 2) { return; }

        // ParsedCollider[] parsedColliders = [.. Colliders.Select(e => new ParsedCollider(e, e.Collider.Offset(e.Actor.GlobalPosition)))];

        //! FIXME (Alex): Store collisions from last tick to only clear ones that are needed
        foreach (CollisionComponent collider in Colliders) {
            collider.Collisions.Clear();
        }

        //! FIXME (Alex): Very temp, implement a spatial collision optimization system
        //! FIXME (Alex): Move this to a subfunction to control what entities are tested
        for (int a = 0; a < Colliders.Count - 1; ++a) {
            for (int b = a + 1; b < Colliders.Count; ++b) {
                CollisionComponent colliderA = Colliders[a];
                CollisionComponent colliderB = Colliders[b];

                bool tagMatchA = colliderA.Mask.Any(colliderB.Tags);
                bool tagMatchB = colliderB.Mask.Any(colliderA.Tags);

                if (
                    // Ensure either collider has matching tags
                    (tagMatchA || tagMatchB) &&
                    // Check collider bounds
                    colliderA.Collider.Bounds.Overlaps(colliderB.Collider.Bounds)
                ) {
                    //! FIXME (Alex): Should likely check swept collision first, since objects will be at their final position first and need to be offset by their velocity first
                    // CollisionSimplexInfo simplex = GetCollisionSimplex(colliderA, colliderB);
                    // if (simplex.Collided) {
                    //     //! FIXME (Alex): Store pushout intersection for colliders
                    // }
                    // else {
                    //     Vector2 intersectionFraction = GetRayIntersectionFraction(colliderA, colliderB);
                    //     if (intersectionFraction != Vector2.One) {

                    //     }
                    // }
                }
            }
        }

        //! FIXME (Alex): Collision resolution
        /*
            Not as easy as it may initially seem, there's some edge-cases to worry about
            1. Should resolution only happen between collision resolvers? I think probably, a non-resolver
                components should just be treated as a trigger volume
            2. How should this be passed around?
                Possibly insteadof handling the resolver as a seperate component, it could be an optional
                member of the collision detector. when a collision occurs, store both locally if a
                resolver exists for both and handle all stored resolves afterwards?
        */
        //! FIXME (Alex): How should resolution between object with one-directional matching tags work?
        //      Does the ground need to have player tags? Or should every physics resolve be handled individually?
        //      Or should it only resolve collisions with a tag match in both directions
    }

    public record struct AllCollisionsResult(bool Collided, CollisionComponent[] Collisions);
    public AllCollisionsResult GetAllCollisions(CollisionComponent collider)
        => GetAllCollisions(collider.Collider, collider.Actor.GlobalPosition, collider.Mask);
    public AllCollisionsResult GetAllCollisions(CollisionComponent collider, Vector2 Position)
        => GetAllCollisions(collider.Collider, Position, collider.Mask);
    public AllCollisionsResult GetAllCollisions(Collider collider, Vector2 position, TagContainer tagMask) {
        List<CollisionComponent> collisions = [];
        foreach (CollisionComponent other in Colliders) {
            //! FIXME (Alex): Collider match check might be unreliable? Collider components may share a collider
            if (
                collider != other.Collider &&
                tagMask.Any(other.Tags) &&
                ColliderOverlapCheck(collider, position, other.Collider, other.Actor.GlobalPosition)
            ) {
                collisions.Add(other);
            }
        }

        return new(collisions.Count > 0, [.. collisions]);
    }

    public record struct SingleCollisionsResult(bool Collided, CollisionComponent? FirstCollision);
    public SingleCollisionsResult GetFirstCollision(CollisionComponent collider)
        => GetFirstCollision(collider.Collider, collider.Actor.GlobalPosition, collider.Mask);
    public SingleCollisionsResult GetFirstCollision(CollisionComponent collider, Vector2 position)
        => GetFirstCollision(collider.Collider, position, collider.Mask);
    public SingleCollisionsResult GetFirstCollision(Collider collider, Vector2 position, TagContainer tagMask) {
        //! FIXME (Alex): Redundant with above
        foreach (CollisionComponent other in Colliders) {
            //! FIXME (Alex): Collider match check might be unreliable? Collider components may share a collider
            if (
                collider != other.Collider &&
                tagMask.Any(other.Tags) &&
                ColliderOverlapCheck(collider, position, other.Collider, other.Actor.GlobalPosition)
            ) {
                return new(true, other);
            }
        }

        return new(false, null);
    }

    public record struct RayCollisionResult();
    //! FIXME (Alex): Need raycollider object?
    public RayCollisionResult GetRayCollision(Ray ray, TagContainer tagMask) {
        throw new NotImplementedException();
    }

    public record struct SweptCollisionResult(Vector2 AdjustedVelocity, Vector2 SpeedFraction, CollisionComponent? Collision, bool Collided);
    public SweptCollisionResult GetSweptCollision(CollisionComponent collider, Vector2 velocity, bool allowNegative = false)
        => GetSweptCollision(collider.Collider, collider.Actor.GlobalPosition, velocity, collider.Mask, allowNegative);
    public SweptCollisionResult GetSweptCollision(CollisionComponent collider, Vector2 position, Vector2 velocity, bool allowNegative = false)
        => GetSweptCollision(collider.Collider, position, velocity, collider.Mask, allowNegative);
    public SweptCollisionResult GetSweptCollision(Collider collider, Vector2 position, Vector2 velocity, TagContainer tagMask, bool allowNegative = false) {
        Vector2 minFraction = Vector2.One;
        CollisionComponent? minCollider = null;

        foreach (CollisionComponent other in Colliders) {
            //! FIXME (Alex): Collider match check might be unreliable? Collider components may share a collider
            if (collider == other.Collider || !tagMask.Any(other.Tags)) { continue; }
            Vector2 fraction = ColliderIntersectionFraction(collider, position, velocity, other.Collider, other.Actor.GlobalPosition, Vector2.Zero);
            if (IsRayFractionSmaller(fraction, minFraction)) {
                minFraction = fraction;
                minCollider = other;
            }
        }

        if (!allowNegative && (minFraction.X < 0 || minFraction.Y < 0)) { minFraction = Vector2.Zero; }
        return new(velocity * minFraction, minFraction, minCollider, minCollider == null);
    }

    //! FIXME (Alex): Need manual pushout functions

    static bool ColliderOverlapCheck(Collider colliderA, Vector2 positionA, Collider colliderB, Vector2 positionB) {
        //! FIXME (Alex): Messy?
        if (colliderA.Multi) {
            foreach (Collider collider in colliderA.GetSubColliders(colliderB, positionB - positionA)) {
                if (ColliderOverlapCheck(collider, positionA, colliderB, positionB)) { return true; }
            }
            return false;
        }

        if (colliderB.Multi) {
            //! FIXME (Alex): Does this work?
            return ColliderOverlapCheck(colliderB, positionB, colliderB, positionA);
        }

        return GetCollisionSimplex(colliderA, positionA, colliderB, positionB).Collided;
    }

    static Vector2 ColliderIntersectionFraction(Collider colliderA, Vector2 positionA, Vector2 velocityA, Collider colliderB, Vector2 positionB, Vector2 velocityB) {
        //! FIXME (Alex): Messy and redundant
        if (colliderA.Multi) {
            Vector2 fraction = Vector2.One;
            foreach (Collider collider in colliderA.GetSubColliders(colliderB, positionB - positionA)) {
                Vector2 newFraction = ColliderIntersectionFraction(collider, positionA, velocityA, colliderB, positionB, velocityB);
                if (IsRayFractionSmaller(newFraction, fraction)) { fraction = newFraction; }
            }
            return fraction;
        }

        if (colliderB.Multi) {
            //! FIXME (Alex): Does this work?
            return ColliderIntersectionFraction(colliderB, positionB, velocityB, colliderA, positionA, velocityA);
        }

        Console.WriteLine($"{positionA} {velocityA} {positionB} {velocityB}");
        return GetRayIntersectionFraction(colliderA, positionA, velocityA, colliderB, positionB, velocityB);
    }

    static CollisionSimplexInfo GetCollisionSimplex(
        Collider colliderA,
        Vector2 positionA,
        Collider colliderB,
        Vector2 positionB
    ) {
        // Point A
        Vector2 direction = colliderB.Center + positionB - colliderA.Center + positionA;
        Vector2 pointA = Support(colliderA, positionA, colliderB, positionB, direction);
        if (!PointCrossesOrigin(pointA, direction)) { return new(pointA, Vector2.Zero, Vector2.Zero, false); }

        // Point B
        direction = -direction;
        Vector2 pointB = Support(colliderA, positionA, colliderB, positionB, direction);
        if (!PointCrossesOrigin(pointB, direction)) { return new(pointA, pointB, Vector2.Zero, false); }

        // Point C
        Vector2 AInv = -pointA;
        Vector2 AB = pointB + AInv;
        direction = CalcExtra.TripleProduct(AB, AInv, AB);
        Vector2 pointC = Support(colliderA, positionA, colliderB, positionB, direction);
        if (!PointCrossesOrigin(pointC, direction)) { return new(pointA, pointB, pointC, false); }

        // Adjust points until they cover the origin
        while (true) {
            Vector2 CInv = -pointC;
            Vector2 CB = pointB + CInv;
            Vector2 CA = pointA + CInv;
            Vector2 perpCB = CalcExtra.TripleProduct(CA, CB, CB);
            Vector2 perpCA = CalcExtra.TripleProduct(CB, CA, CA);
            if (Vector2.Dot(perpCB, CInv) > 0) {
                pointA = pointB;
                pointB = pointC;

                direction = perpCB;
                pointC = Support(colliderA, positionA, colliderB, positionB, direction);
                if (!PointCrossesOrigin(pointC, direction)) { return new(pointA, pointB, pointC, false); }
            }
            else if (Vector2.Dot(perpCA, CInv) > 0) {
                pointB = pointC;

                direction = perpCA;
                pointC = Support(colliderA, positionA, colliderB, positionB, direction);
                if (!PointCrossesOrigin(pointC, direction)) { return new(pointA, pointB, pointC, false); }
            }
            else {
                return new(pointA, pointB, pointC, true);
            }
        }
    }

    static Vector2 GetPushout(
        Collider colliderA,
        Vector2 positionA,
        Collider colliderB,
        Vector2 positionB,
        CollisionSimplexInfo simplex
    ) {
        if (!simplex.Collided) { return Vector2.Zero; }

        List<Vector2> points = [
            simplex.A,
            simplex.B,
            simplex.C
        ];

        bool clockwiseWinding = IsClockwiseWinding(simplex.A, simplex.B, simplex.C);
        Edge closestEdge = new(0, Vector2.Zero, float.PositiveInfinity);

        Vector2 pushout = Vector2.Zero;

        for (int i = 0; i < ITERATION_LIMIT; ++i) {
            // Find edge closest to the origin
            for (int indexA = 0; indexA < points.Count; ++indexA) {
                int indexB = (indexA + 1) % points.Count;

                Vector2 pointA = points[indexA];
                Vector2 pointB = points[indexB];
                Vector2 line = pointB - pointA;
                Vector2 normal = clockwiseWinding ? new Vector2(line.Y, -line.X) : new Vector2(-line.Y, line.X);
                normal = normal.Normalized();

                float compareDistance = Vector2.Dot(normal, pointA);
                if (compareDistance < closestEdge.Distance) {
                    closestEdge = new(indexB, normal, compareDistance);
                }
            }

            // Find new support in direction of closest edge
            Vector2 direction = closestEdge.Normal;
            Vector2 newSupport = Support(colliderA, positionA, colliderB, positionB, direction);
            float supportDistance = Vector2.Dot(newSupport, direction);

            // Determine new pushout approximation
            pushout = supportDistance * direction;

            // Return immediately if pushout is as close as possible
            supportDistance = MathF.Abs(supportDistance - closestEdge.Distance);
            if (supportDistance <= PUSHOUT_TOLERANCE) { return pushout; }

            points.Insert(closestEdge.Index, newSupport);
        }

        // Return closest approximate pushout if nothing better can be found in time
        return pushout;
    }

    static Vector2 GetRayIntersectionFraction(
        Collider colliderA,
        Vector2 positionA,
        Vector2 velocityA,
        Collider colliderB,
        Vector2 positionB,
        Vector2 velocityB
    ) {
        // Get velocity difference
        Vector2 velocityDifference = velocityB - velocityA;

        // Cancel if objects are moving the same direction or stationary
        if (velocityDifference == Vector2.Zero) {
            Console.WriteLine("Fail, no velocity");
            return Vector2.One;
        }

        // Point A
        Vector2 direction = -velocityDifference;
        Vector2 pointA = Support(colliderA, positionA, colliderB, positionB, direction);

        // Point B
        direction = CalcExtra.TripleProduct(direction, -pointA, direction);
        Vector2 pointB = Support(colliderA, positionA, colliderB, positionB, direction);

        // Fail intersection if the first 2 supports won't intersect with the velocity
        Console.WriteLine($"{pointA} {Calc.SignCross(velocityDifference, pointA)} {pointB} {Calc.SignCross(velocityDifference, pointB)}");
        if (Calc.SignCross(velocityDifference, pointA) == Calc.SignCross(velocityDifference, pointB)) {
            Console.WriteLine("Fail, no cross");
            return Vector2.One;
        }

        // Extend velocity to cover entire difference bounds
        //! FIXME (Alex): There's likely a better way to do this
        Vector2 farSupport = Support(colliderA, positionA, colliderB, positionB, velocityDifference);
        Vector2 scaledVelocity = velocityDifference.Normalized() * farSupport.Length();
        float velocityLength = velocityDifference.LengthSquared();
        CalcExtra.LineSegmentIntersection(Vector2.Zero, scaledVelocity, pointA, pointB, out Vector2 closestIntersection);
        float closestDistance = closestIntersection.LengthSquared();

        for (int i = 0; i < ITERATION_LIMIT; ++i) {
            // Get new point C
            Vector2 line = pointB - pointA;
            direction = -CalcExtra.TripleProduct(line, pointA, line);
            Vector2 pointC = Support(colliderA, positionA, colliderB, positionB, direction);

            // Find line segment intersections
            bool intersectedAC = CalcExtra.LineSegmentIntersection(pointA, pointC, Vector2.Zero, scaledVelocity, out Vector2 intersectionAC);
            bool intersectedBC = CalcExtra.LineSegmentIntersection(pointB, pointC, Vector2.Zero, scaledVelocity, out Vector2 intersectionBC);
            // Fail intersection if neither line collides
            //! FIXME (Alex): probably unneeded assuming the first fail check works correctly
            if (!intersectedAC && !intersectedBC) {
                Console.WriteLine("Fail, no intersection");
                return Vector2.One;
            }
            float distanceAC = intersectedAC ? intersectionAC.LengthSquared() : float.PositiveInfinity;
            float distanceBC = intersectedBC ? intersectionBC.LengthSquared() : float.PositiveInfinity;

            // Find closest new intersection point
            float compareDistance = closestDistance;
            if (distanceAC < distanceBC) {
                pointB = pointC;
                closestIntersection = intersectionAC;
                closestDistance = distanceAC;
            }
            else {
                pointA = pointC;
                closestIntersection = intersectionBC;
                closestDistance = distanceBC;
            }

            // Finish loop if new intersection didn't get closer
            if (closestDistance >= compareDistance) {
                // Fail if closest intersection still doesn't touch the velocity line
                if (velocityLength < closestDistance) {
                    Console.WriteLine($"Fail, too far {velocityLength} {closestDistance} {distanceAC} {distanceBC}");
                    Game.Events.Dispatch(new SweepDebug(pointA, pointB, pointC, intersectionAC, intersectionBC, velocityDifference));
                    return Vector2.One;
                }
                // Otherwise return percent of velocity length
                Console.WriteLine($"Pass, value {VectorFraction(closestIntersection, velocityDifference)}");
                return VectorFraction(closestIntersection, velocityDifference);
            }
        }

        Console.WriteLine("Fail, iteration max");
        return VectorFraction(closestIntersection, velocityDifference);
    }
    //! FIXME (Alex): TEMP
    public record struct SweepDebug(
        Vector2 PointA,
        Vector2 PointB,
        Vector2 PointC,
        Vector2 PointAC,
        Vector2 PointBC,
        Vector2 Velocity
    );

    static bool PointCrossesOrigin(Vector2 point, Vector2 direction) => Vector2.Dot(direction, point) >= 0;

    static Vector2 Support(
        Collider colliderA,
        Vector2 positionA,
        Collider colliderB,
        Vector2 positionB,
        Vector2 direction
    ) {
        return colliderA.Support(positionA, direction) - colliderB.Support(positionB, -direction);
    }

    static bool IsClockwiseWinding(Vector2 a, Vector2 b, Vector2 c) {
        float wind0 = (b.X - a.X) * (b.Y + a.Y);
        float wind1 = (c.X - b.X) * (c.Y + b.Y);
        float wind2 = (a.X - c.X) * (a.Y + c.Y);
        return wind0 + wind1 + wind2 <= 0;
    }

    static bool IsRayFractionSmaller(Vector2 newFraction, Vector2 originalFraction) {
        // We can assume both axes will be smaller because the ray will always be along the original line
        return newFraction.X < originalFraction.X && newFraction.Y < originalFraction.Y;
    }

    static Vector2 VectorFraction(Vector2 value, Vector2 max) {
        return new(
            (max.X == 0) ? 0 : (value.X / max.X),
            (max.Y == 0) ? 0 : (value.Y / max.Y)
        );
    }
}