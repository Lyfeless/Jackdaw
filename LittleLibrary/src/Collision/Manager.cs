using System.Collections.Specialized;
using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class CollisionManager {
    #region Definitions
    const int ITERATION_LIMIT = 32;
    const float PUSHOUT_TOLERANCE = 0.0001f;

    record struct Edge(int Index, Vector2 Normal, float Distance);
    record struct CollisionSimplexInfo(Vector2 A, Vector2 B, Vector2 C, bool Collided);
    record struct CollisionSweepInfo(Vector2 Fraction, Vector2 Normal, bool Collided);

    record struct SimplexColliderPair(Collider A, Collider B);
    record struct SweepColliderPair(Collider A, Collider B, Vector2 Fraction, Vector2 Normal);

    public enum BroadPhaseState {
        NONE,
        COLLISION,
        COLLISION_AND_RESOLUTION
    }
    #endregion

    #region Data
    public BroadPhaseState BroadPhase = BroadPhaseState.COLLISION_AND_RESOLUTION;

    readonly List<CollisionComponent> Colliders = [];
    #endregion

    #region Object Registration
    /// <summary>
    /// Register a collider component to be used in broadphase and manual collision checks.
    /// </summary>
    /// <param name="collider">The collider to register.</param>
    public void Add(CollisionComponent collider) {
        Colliders.Add(collider);
        // Console.WriteLine($"{Colliders.Count} Add {collider}");
    }

    /// <summary>
    /// Remove a collider component from collision registrations.
    /// </summary>
    /// <param name="collider">The collider to remove.</param>
    public void Remove(CollisionComponent collider) {
        Colliders.Remove(collider);
        // Console.WriteLine($"{Colliders.Count} Remove {collider}");
    }
    #endregion

    #region Broadphase
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

                // bool tagMatchA = colliderA.Mask.Any(colliderB.Tags);
                // bool tagMatchB = colliderB.Mask.Any(colliderA.Tags);

                if (
                    // Ensure either collider has matching tags
                    // (tagMatchA || tagMatchB) &&
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
    #endregion

    #region Manual All Collisions Checks
    /// <summary>
    /// Get all collisions between given collider component and all registered collision components.
    /// Assumes default location relative to actor.
    /// </summary>
    /// <param name="collider">The collider component to check against.</param>
    /// <returns>Information about collision check results.</returns>
    public AllCollisionInfo GetAllCollisions(CollisionComponent collider)
        => GetAllCollisions(collider, collider.Actor.GlobalPosition);

    /// <summary>
    /// Get all collisions between given collider component and all registered collision components at a given location.
    /// </summary>
    /// <param name="collider">The collider component to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public AllCollisionInfo GetAllCollisions(CollisionComponent collider, Vector2 position) {
        List<CollisionComponentInfo> collisions = [];
        foreach (CollisionComponent other in Colliders) {
            if (!CheckComponent(collider, other)) { continue; }
            SimplexColliderPair[] pairs = ColliderOverlapCheck(collider.Collider, position, other.Collider, other.Actor.GlobalPosition);
            if (pairs.Length == 0) { continue; }
            collisions.Add(new(other, [.. pairs.Select(e => e.B)]));
        }

        return new(collisions.Count > 0, [.. collisions]);
    }

    /// <summary>
    /// Get all collisions between given collider and all registered collision components at a given location.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public AllCollisionInfo GetAllCollisions(Collider collider, Vector2 position) {
        List<CollisionComponentInfo> collisions = [];
        foreach (CollisionComponent other in Colliders) {
            if (!CheckComponent(other)) { continue; }
            SimplexColliderPair[] pairs = ColliderOverlapCheck(collider, position, other.Collider, other.Actor.GlobalPosition);
            if (pairs.Length == 0) { continue; }
            collisions.Add(new(other, [.. pairs.Select(e => e.B)]));
        }

        return new(collisions.Count > 0, [.. collisions]);
    }
    #endregion

    #region Manual Single Collisions Checks
    /// <summary>
    /// Get the first object the given collider component collides with.
    /// Assumes default location relative to actor.
    /// Not guarenteed to be the closest, used mostly for performance when full collision information isn't needed.
    /// </summary>
    /// <param name="collider">The collider component to check against.</param>
    /// <returns>Information about collision check results.</returns>
    public SingleCollisionInfo GetFirstCollision(CollisionComponent collider)
        => GetFirstCollision(collider, collider.Actor.GlobalPosition);

    /// <summary>
    /// Get the first object the given collider component collides with at a given location.
    /// Not guarenteed to be the closest, used mostly for performance when full collision information isn't needed.
    /// </summary>
    /// <param name="collider">The collider component to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public SingleCollisionInfo GetFirstCollision(CollisionComponent collider, Vector2 position) {
        foreach (CollisionComponent other in Colliders) {
            if (!CheckComponent(collider, other)) { continue; }
            SimplexColliderPair[] pairs = ColliderOverlapCheck(collider.Collider, position, other.Collider, other.Actor.GlobalPosition);
            if (pairs.Length > 0) {
                return new(true, new(other, [.. pairs.Select(e => e.B)]));
            }
        }

        return new(false, null);
    }

    /// <summary>
    /// Get the first object the given collider collides with at a given location.
    /// Not guarenteed to be the closest, used mostly for performance when full collision information isn't needed.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public SingleCollisionInfo GetFirstCollision(Collider collider, Vector2 position) {
        foreach (CollisionComponent other in Colliders) {
            if (!CheckComponent(other)) { continue; }
            SimplexColliderPair[] pairs = ColliderOverlapCheck(collider, position, other.Collider, other.Actor.GlobalPosition);
            if (pairs.Length > 0) {
                return new(true, new(other, [.. pairs.Select(e => e.B)]));
            }
        }

        return new(false, null);
    }
    #endregion

    #region Manual Raycast Collisions Checks
    //! FIXME (Alex): DOC COMMENTS
    public SweptCollisionInfo GetRayCollision(RaycastComponent component)
        => GetRayCollision(component, component.Actor.GlobalPosition);
    public SweptCollisionInfo GetRayCollision(RaycastComponent component, Vector2 position)
        => GetRayCollision(component.Ray.Position + position, component.Ray.Direction, component.Mask);
    public SweptCollisionInfo GetRayCollision(Ray ray, TagContainer tagMask)
        => GetRayCollision(ray.Position, ray.Direction, tagMask);
    public SweptCollisionInfo GetRayCollision(Vector2 position, Vector2 direction, TagContainer tagMask)
        => GetSweptCollision(new PointCollider(position) { Mask = tagMask }, Vector2.Zero, direction);
    #endregion

    #region Manual Swept Collisions Checks
    //! FIXME (Alex): DOC COMMENTS
    public SweptCollisionInfo GetSweptCollision(CollisionComponent collider, Vector2 velocity, bool allowNegative = false)
        => GetSweptCollision(collider, collider.Actor.GlobalPosition, velocity, allowNegative);
    public SweptCollisionInfo GetSweptCollision(CollisionComponent collider, Vector2 position, Vector2 velocity, bool allowNegative = false) {
        // If object isn't moving just get the first collided object to avoid extra calculations
        //! FIXME (Alex): Unsure of how this should be handled, throwing a warning to just avoid running this function with no velocity
        if (velocity == Vector2.Zero) {
            // Console.WriteLine("COLLISION: Trying to get swept collision with no velocity could result in errors, returning basic collision check");
            SingleCollisionInfo collision = GetFirstCollision(collider, position);
            return new(collision.Collided, Vector2.Zero, Vector2.Zero, []);
        }

        Vector2 minFraction = Vector2.One;
        CollisionComponent? minCollider = null;
        List<SweptCollisionComponentInfo> componentInfo = [];

        foreach (CollisionComponent other in Colliders) {
            if (!CheckComponent(collider, other)) { continue; }
            SweptCollisionComponentInfo? result = GetSweptCollisionComponentInfo(collider.Collider, position, velocity, other);
            if (result == null) { continue; }
            SweptCollisionComponentInfo resultCast = (SweptCollisionComponentInfo)result;
            if (IsRayFractionSmaller(resultCast.Colliders[resultCast.ClosestCollider].Fraction, minFraction)) {
                minFraction = resultCast.Colliders[resultCast.ClosestCollider].Fraction;
                minCollider = other;
            }
            componentInfo.Add(resultCast);
        }

        return GetSweptCollisionResult([.. componentInfo], velocity, minFraction, minCollider, allowNegative);
    }
    public SweptCollisionInfo GetSweptCollision(Collider collider, Vector2 position, Vector2 velocity, bool allowNegative = false) {
        // If object isn't moving just get the first collided object to avoid extra calculations
        //! FIXME (Alex): Unsure of how this should be handled, throwing a warning to just avoid running this function with no velocity
        //! FIXME (Alex): THis looks identical to the compnent version of the functions, but they call different overloads so no subfunction :(
        if (velocity == Vector2.Zero) {
            // Console.WriteLine("COLLISION: Trying to get swept collision with no velocity could result in errors, returning basic collision check");
            SingleCollisionInfo collision = GetFirstCollision(collider, position);
            return new(collision.Collided, Vector2.Zero, Vector2.Zero, []);
        }

        Vector2 minFraction = Vector2.One;
        CollisionComponent? minCollider = null;
        List<SweptCollisionComponentInfo> componentInfo = [];

        foreach (CollisionComponent other in Colliders) {
            if (!CheckComponent(other)) { continue; }
            SweptCollisionComponentInfo? result = GetSweptCollisionComponentInfo(collider, position, velocity, other);
            if (result == null) { continue; }
            SweptCollisionComponentInfo resultCast = (SweptCollisionComponentInfo)result;
            if (IsRayFractionSmaller(resultCast.Colliders[resultCast.ClosestCollider].Fraction, minFraction)) {
                minFraction = resultCast.Colliders[resultCast.ClosestCollider].Fraction;
                minCollider = other;
            }
            componentInfo.Add(resultCast);
        }

        return GetSweptCollisionResult([.. componentInfo], velocity, minFraction, minCollider, allowNegative);
    }

    SweptCollisionComponentInfo? GetSweptCollisionComponentInfo(Collider collider, Vector2 position, Vector2 velocity, CollisionComponent other) {
        SweepColliderPair[] collisionPairs = ColliderIntersectionFraction(collider, position, velocity, other.Collider, other.Actor.GlobalPosition, Vector2.Zero);
        if (collisionPairs.Length == 0) { return null; }
        SweptColliderInfo[] colliders = new SweptColliderInfo[collisionPairs.Length];
        int closest = 0;
        for (int i = 0; i < colliders.Length; ++i) {
            SweepColliderPair pair = collisionPairs[i];
            colliders[i] = new(pair.B, pair.Fraction, pair.Normal);
            if (IsRayFractionSmaller(pair.Fraction, collisionPairs[closest].Fraction)) {
                closest = i;
            }
        }

        return new(other, closest, colliders);
    }

    SweptCollisionInfo GetSweptCollisionResult(SweptCollisionComponentInfo[] colliderInfo, Vector2 velocity, Vector2 fraction, CollisionComponent? minCollider, bool allowNegative = false) {
        bool collided = colliderInfo.Length > 0;

        // Apply small pushback to stop objects getting stuck inside one another
        if (collided) {
            fraction -= fraction.Normalized() * 0.001f;
        }

        if (!allowNegative && FractionNegative(fraction)) {
            fraction = Vector2.Zero;
        }

        return new(collided, velocity * fraction, fraction, colliderInfo);
    }
    #endregion

    #region Manual Pushout Checks
    //! FIXME (Alex): Need manual pushout functions
    #endregion

    #region 2 Collider Middleman Functions
    //! FIXME (Alex): Probably has some wires crossed somewhere due to all the list reversals, make sure to test
    static SimplexColliderPair[] ColliderOverlapCheck(Collider colliderA, Vector2 positionA, Collider colliderB, Vector2 positionB, bool reversed = false) {
        if (!TagMatch(colliderA, colliderB, reversed)) { return []; }

        if (!colliderA.Bounds.Translate(positionA).Overlaps(colliderB.Bounds.Translate(positionB))) {
            return [];
        }

        if (colliderA.Multi) {
            Collider[] subs = colliderA.GetSubColliders(new Rect(positionB - positionA + colliderB.Bounds.Position - colliderA.Bounds.Position, colliderB.Bounds.Size));
            List<SimplexColliderPair> pairs = [];
            foreach (Collider collider in subs) {
                SimplexColliderPair[] subCollisions = ColliderOverlapCheck(collider, positionA, colliderB, positionB, reversed);
                if (subCollisions.Length > 0) { pairs.AddRange(subCollisions); }
            }
            return [.. pairs];
        }

        if (colliderB.Multi) {
            return ColliderOverlapCheck(colliderB, positionB, colliderA, positionA, !reversed);
        }

        if (GetCollisionSimplex(colliderA, positionA, colliderB, positionB).Collided) {
            return [reversed ? new(colliderB, colliderA) : new(colliderA, colliderB)];
        }
        return [];
    }

    static SweepColliderPair[] ColliderIntersectionFraction(Collider colliderA, Vector2 positionA, Vector2 velocityA, Collider colliderB, Vector2 positionB, Vector2 velocityB, bool reversed = false) {
        if (!TagMatch(colliderA, colliderB, reversed)) { return []; }

        Rect sweptBoundsA = SweptBounds(colliderA, velocityA);
        Rect sweptBoundsB = SweptBounds(colliderB, velocityB);
        Rect sweptBoundsAOffset = sweptBoundsA.Translate(positionA);
        Rect sweptBoundsBOffset = sweptBoundsB.Translate(positionB);
        if (!sweptBoundsAOffset.Overlaps(sweptBoundsBOffset)) {
            return [];
        }

        if (colliderA.Multi) {
            List<SweepColliderPair> pairs = [];
            foreach (Collider collider in colliderA.GetSubColliders(new Rect(positionB - positionA + sweptBoundsB.Position - sweptBoundsA.Position, sweptBoundsB.Size))) {
                SweepColliderPair[] subCollisions = ColliderIntersectionFraction(collider, positionA, velocityA, colliderB, positionB, velocityB, reversed);
                if (subCollisions.Length > 0) { pairs.AddRange(subCollisions); }
            }
            return [.. pairs];
        }

        if (colliderB.Multi) {
            return ColliderIntersectionFraction(colliderB, positionB, velocityB, colliderA, positionA, velocityA, !reversed);
        }

        CollisionSweepInfo collision = GetRayIntersectionFraction(colliderA, positionA, velocityA, colliderB, positionB, velocityB);
        if (FractionNegative(collision.Fraction)) {
            if (ColliderOverlapCheck(colliderA, positionA, colliderB, positionB).Length == 0) {
                return [];
            }
        }
        return [reversed ? new(colliderB, colliderA, collision.Fraction, collision.Normal) : new(colliderA, colliderB, collision.Fraction, collision.Normal)];
    }
    #endregion

    #region Primary Collision Functions
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

    //! FIXME (Alex): Give back more info than just the pushout
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

    //! FIXME (Alex): Give back more info than just the fraction
    static CollisionSweepInfo GetRayIntersectionFraction(
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
        if (velocityDifference == Vector2.Zero) { return new(Vector2.One, Vector2.UnitY, false); }

        float velocityLength = velocityDifference.LengthSquared();

        // Point A
        Vector2 direction = -velocityDifference;
        Vector2 pointA = Support(colliderA, positionA, colliderB, positionB, direction);

        // Point B
        direction = CalcExtra.TripleProduct(direction, -pointA, direction);
        Vector2 pointB = Support(colliderA, positionA, colliderB, positionB, direction);

        float crossA = Calc.Cross(velocityDifference, pointA);
        float crossB = Calc.Cross(velocityDifference, pointB);
        if (MathF.Sign(crossA) == MathF.Sign(crossB)) {
            // First line lies exactly on velocity vector
            if (crossA == 0 && crossB == 0) {
                if (SweepInfiniteCollisionResolve(pointA, pointB, velocityLength, out Vector2 intersection)) {
                    return new(VectorFraction(intersection, velocityDifference), -velocityDifference.Normalized(), true);
                }
            }

            return new(Vector2.One, Vector2.UnitY, false);
        }

        for (int i = 0; i < ITERATION_LIMIT; ++i) {
            // Get new point C
            direction = PerpDirection(pointA, pointB, -velocityDifference);
            // Exit immediately if the direction has no magnitude (lies exactly on the line AB)
            if (direction == Vector2.Zero) { return SweepLineIntersection(pointA, pointB, velocityDifference, velocityLength); }
            Vector2 pointC = Support(colliderA, positionA, colliderB, positionB, direction);

            // New support point is the same as one of the current points
            if (pointC == pointA || pointC == pointB) {
                return SweepLineIntersection(pointA, pointB, velocityDifference, velocityLength);
            }

            float crossC = Calc.Cross(velocityDifference, pointC);

            // pointC lies exactly on the line
            if (crossC == 0) {
                //! FIXME (Alex): NOT A FRACTION
                return new(VectorFraction(pointC, velocityDifference), PerpDirection(pointA, pointB, -velocityDifference), true);
            }

            if (MathF.Sign(crossC) == MathF.Sign(crossA)) {
                pointA = pointC;
                crossA = crossC;
            }
            else {
                pointB = pointC;
                crossB = crossC;
            }
        }

        return SweepLineIntersection(pointA, pointB, velocityDifference, velocityLength);
    }
    #endregion

    #region Utilities
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
        return newFraction.X <= originalFraction.X && newFraction.Y <= originalFraction.Y;
    }

    static Vector2 VectorFraction(Vector2 value, Vector2 max) {
        return new(
            (max.X == 0) ? 0 : (value.X / max.X),
            (max.Y == 0) ? 0 : (value.Y / max.Y)
        );
    }

    static bool FractionNegative(Vector2 fraction) {
        return MathF.Min(fraction.X, fraction.Y) < 0;
    }

    // Returns squared length for optimization
    // Expects direction to be normalized
    static float SweepIntersectionLength(Vector2 intersection, Vector2 direction) {
        float distance = intersection.LengthSquared();
        return Vector2.Dot(intersection.Normalized(), direction) > 0 ? distance : -distance;
    }

    // Expects velocityLength to be a squared distance
    static bool SweepInfiniteCollisionResolve(Vector2 pointA, Vector2 pointB, float velocityLength, out Vector2 intersection) {
        float distA = pointA.LengthSquared();
        float distB = pointB.LengthSquared();
        float dist;
        if (distA < distB) {
            dist = distA;
            intersection = pointA;
        }
        else {
            dist = distB;
            intersection = pointB;
        }

        return dist < velocityLength;
    }

    // Expects velocityLength to be a squared distance
    static CollisionSweepInfo SweepLineIntersection(Vector2 pointA, Vector2 pointB, Vector2 velocity, float velocityLength) {
        if (!CalcExtra.LineAndLineSegmentIntersection(Vector2.Zero, velocity, pointA, pointB, out Vector2 intersection)) {
            return new(Vector2.One, Vector2.UnitY, false);
        }

        Vector2 normal = PerpDirection(pointA, pointB, velocity).Normalized();

        // NaN is returned if the lines are the same
        if (intersection == Vector2.NaN) {
            if (!SweepInfiniteCollisionResolve(pointA, pointB, velocityLength, out intersection)) { return new(Vector2.One, -velocity, false); }
        }
        else {
            if (SweepIntersectionLength(intersection, velocity) > velocityLength) { return new(Vector2.One, normal, false); }
        }

        return new(VectorFraction(intersection, velocity), normal, true);
    }

    static Vector2 PerpDirection(Vector2 pointA, Vector2 pointB, Vector2 facingDirection) {
        Vector2 line = pointB - pointA;
        Vector2 direction = CalcExtra.TripleProduct(line, pointA, line);
        //! FIXME (Alex): Forcing the vector to always face in the opposite direction of velocity can probably be done with pure math
        if (Vector2.Dot(direction, facingDirection) < 0) { direction = -direction; }
        return direction;
    }

    static Rect SweptBounds(Collider collider, Vector2 velocity) {
        Vector2 min = Vector2.Min(collider.Bounds.TopLeft, collider.Bounds.TopLeft + velocity);
        Vector2 max = Vector2.Max(collider.Bounds.BottomRight, collider.Bounds.BottomRight + velocity);
        return new(min, max - min);
    }

    static bool TagMatch(Collider a, Collider b, bool reversed = false) {
        if (reversed) { return TagMatch(b, a); }
        return a.Mask.Empty || b.Tags.Empty || a.Mask.Any(b.Tags);
    }

    static bool ActorMatch(CollisionComponent a, CollisionComponent b) {
        return a.ActorValid && b.ActorValid && a.Actor == b.Actor;
    }

    static bool CheckComponent(CollisionComponent a, CollisionComponent b) {
        return
            a != b &&
            !ActorMatch(a, b) &&
            CheckComponent(b);
    }

    static bool CheckComponent(CollisionComponent component) {
        return
            component.Actor.GlobalTicking &&
            component.Ticking;
    }
    #endregion
}