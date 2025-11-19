using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Controller for all collision interactions in the game.
/// </summary>
public class CollisionManager {
    #region Definitions
    const int ITERATION_LIMIT = 32;
    const float PUSHOUT_TOLERANCE = 0.0001f;
    const float SWEEP_TOLERANCE = 0.0001f;

    record struct ColliderContext(
        Collider ColliderA,
        Matrix3x2 PositionA,
        Matrix3x2 PositionAInv,
        Vector2 VelocityA,
        Collider ColliderB,
        Matrix3x2 PositionB,
        Matrix3x2 PositionBInv,
        Vector2 VelocityB
    );

    record struct Edge(int Index, Vector2 Normal, float Distance);
    record struct CollisionSimplexInfo(Vector2 A, Vector2 B, Vector2 C, bool Collided);
    record struct CollisionPushoutInfo(Vector2 Pushout, bool Collided);
    record struct CollisionSweepInfo(Vector2 Fraction, Vector2 Normal, bool Collided);

    record struct SimplexColliderPair(Collider A, Collider B);
    record struct PushoutColliderPair(Collider A, Collider B, Vector2 Pushout);
    record struct SweepColliderPair(Collider A, Collider B, Vector2 Fraction, Vector2 Normal);
    #endregion

    #region Data
    readonly List<CollisionComponent> Colliders = [];
    #endregion

    #region Object Registration
    /// <summary>
    /// Register a collider component to be used in broadphase and manual collision checks.
    /// </summary>
    /// <param name="collider">The collider to register.</param>
    public void Add(CollisionComponent collider) {
        Colliders.Add(collider);
    }

    /// <summary>
    /// Remove a collider component from collision registrations.
    /// </summary>
    /// <param name="collider">The collider to remove.</param>
    public void Remove(CollisionComponent collider) {
        Colliders.Remove(collider);
    }
    #endregion

    #region Broadphase
    public void Update() { }
    #endregion

    #region Manual All Collisions Checks
    /// <summary>
    /// Get all collisions between given collider component and all registered collision components.
    /// Assumes default location relative to actor.
    /// </summary>
    /// <param name="collider">The collider component to check against.</param>
    /// <returns>Information about collision check results.</returns>
    public AllCollisionInfo GetAllCollisions(CollisionComponent collider)
        => GetAllCollisions(collider, collider.Actor.Position.GlobalMatrix, collider.Actor.Position.GlobalMatrixInverse);

    /// <summary>
    /// Get all collisions between given collider component and all registered collision components at a given location.
    /// </summary>
    /// <param name="collider">The collider component to check against.</param>
    /// <param name="position">The actor position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public AllCollisionInfo GetAllCollisions(CollisionComponent collider, ActorPosition position)
        => GetAllCollisions(collider, position.GlobalMatrix, position.GlobalMatrixInverse);

    /// <summary>
    /// Get all collisions between given collider component and all registered collision components at a given location.
    /// </summary>
    /// <param name="collider">The collider component to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public AllCollisionInfo GetAllCollisions(CollisionComponent collider, Transform position)
        => GetAllCollisions(collider, position.Matrix, position.MatrixInverse);

    /// <summary>
    /// Get all collisions between given collider component and all registered collision components at a given location.
    /// </summary>
    /// <param name="collider">The collider component to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public AllCollisionInfo GetAllCollisions(CollisionComponent collider, Matrix3x2 position) {
        Matrix3x2.Invert(position, out Matrix3x2 positionInv);
        return GetAllCollisions(collider, position, positionInv);
    }

    /// <summary>
    /// Get all collisions between given collider component and all registered collision components at a given location.
    /// </summary>
    /// <param name="collider">The collider component to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <param name="positionInv">A pre-calculated inverted matrix for the position.</param>
    /// <returns>Information about collision check results.</returns>
    public AllCollisionInfo GetAllCollisions(CollisionComponent collider, Matrix3x2 position, Matrix3x2 positionInv)
        => GetAllCollisions(e => CheckComponent(collider, e), collider.Collider, position, positionInv);

    /// <summary>
    /// Get all collisions between given collider and all registered collision components at a given location.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public AllCollisionInfo GetAllCollisions(Collider collider, Matrix3x2 position) {
        Matrix3x2.Invert(position, out Matrix3x2 positionInv);
        return GetAllCollisions(collider, position, positionInv);
    }

    /// <summary>
    /// Get all collisions between given collider and all registered collision components at a given location.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <param name="positionInv">A pre-calculated inverted matrix for the position.</param>
    /// <returns>Information about collision check results.</returns>
    public AllCollisionInfo GetAllCollisions(Collider collider, Matrix3x2 position, Matrix3x2 positionInv)
        => GetAllCollisions(CheckComponent, collider, position, positionInv);

    AllCollisionInfo GetAllCollisions(Func<CollisionComponent, bool> componentCheck, Collider collider, Matrix3x2 position, Matrix3x2 positionInv) {
        List<CollisionComponentInfo> collisions = [];
        foreach (CollisionComponent other in Colliders) {
            if (!componentCheck(other)) { continue; }
            SimplexColliderPair[] pairs = ColliderOverlapCheck(new(
                collider,
                position,
                positionInv,
                Vector2.Zero,
                other.Collider,
                other.Actor.Position.GlobalMatrix,
                other.Actor.Position.GlobalMatrixInverse,
                Vector2.Zero
            ));
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
        => GetFirstCollision(collider, collider.Actor.Position.GlobalMatrix, collider.Actor.Position.GlobalMatrixInverse);

    /// <summary>
    /// Get the first object the given collider component collides with at a given location.
    /// Not guarenteed to be the closest, used mostly for performance when full collision information isn't needed.
    /// </summary>
    /// <param name="collider">The collider component to check against.</param>
    /// <param name="position">The actor position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public SingleCollisionInfo GetFirstCollision(CollisionComponent collider, ActorPosition position)
        => GetFirstCollision(collider, position.GlobalMatrix, position.GlobalMatrixInverse);

    /// <summary>
    /// Get the first object the given collider component collides with at a given location.
    /// Not guarenteed to be the closest, used mostly for performance when full collision information isn't needed.
    /// </summary>
    /// <param name="collider">The collider component to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public SingleCollisionInfo GetFirstCollision(CollisionComponent collider, Transform position)
        => GetFirstCollision(collider, position.Matrix, position.MatrixInverse);

    /// <summary>
    /// Get the first object the given collider component collides with at a given location.
    /// Not guarenteed to be the closest, used mostly for performance when full collision information isn't needed.
    /// </summary>
    /// <param name="collider">The collider component to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public SingleCollisionInfo GetFirstCollision(CollisionComponent collider, Matrix3x2 position) {
        Matrix3x2.Invert(position, out Matrix3x2 positionInv);
        return GetFirstCollision(collider, position, positionInv);
    }

    /// <summary>
    /// Get the first object the given collider component collides with at a given location.
    /// Not guarenteed to be the closest, used mostly for performance when full collision information isn't needed.
    /// </summary>
    /// <param name="collider">The collider component to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <param name="positionInv">A pre-calculated inverted matrix for the position.</param>
    /// <returns>Information about collision check results.</returns>
    public SingleCollisionInfo GetFirstCollision(CollisionComponent collider, Matrix3x2 position, Matrix3x2 positionInv)
        => GetFirstCollision(e => CheckComponent(collider, e), collider.Collider, position, positionInv);

    /// <summary>
    /// Get the first object the given collider collides with at a given location.
    /// Not guarenteed to be the closest, used mostly for performance when full collision information isn't needed.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public SingleCollisionInfo GetFirstCollision(Collider collider, Matrix3x2 position) {
        Matrix3x2.Invert(position, out Matrix3x2 positionInv);
        return GetFirstCollision(collider, position, positionInv);
    }

    /// <summary>
    /// Get the first object the given collider collides with at a given location.
    /// Not guarenteed to be the closest, used mostly for performance when full collision information isn't needed.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <param name="positionInv">A pre-calculated inverted matrix for the position.</param>
    /// <returns>Information about collision check results.</returns>
    public SingleCollisionInfo GetFirstCollision(Collider collider, Matrix3x2 position, Matrix3x2 positionInv)
        => GetFirstCollision(CheckComponent, collider, position, positionInv);

    SingleCollisionInfo GetFirstCollision(Func<CollisionComponent, bool> componentCheck, Collider collider, Matrix3x2 position, Matrix3x2 positionInv) {
        foreach (CollisionComponent other in Colliders) {
            if (!componentCheck(other)) { continue; }
            SimplexColliderPair[] pairs = ColliderOverlapCheck(new(
                collider,
                position,
                positionInv,
                Vector2.Zero,
                other.Collider,
                other.Actor.Position.GlobalMatrix,
                other.Actor.Position.GlobalMatrixInverse,
                Vector2.Zero
            ));
            if (pairs.Length > 0) {
                return new(true, new(other, [.. pairs.Select(e => e.B)]));
            }
        }

        return new(false, null);
    }
    #endregion

    #region Manual Raycast Collisions Checks
    /// <summary>
    /// Get all collision data from a ray.
    /// </summary>
    /// <param name="component">The raycast component to check against.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetRayCollision(RaycastComponent component)
        => GetRayCollision(component, component.Actor.Position);

    /// <summary>
    /// Get all collision data from a ray.
    /// </summary>
    /// <param name="component">The raycast component to check against.</param>
    /// <param name="position">The position to offset the raycast by.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetRayCollision(RaycastComponent component, ActorPosition position)
        => GetRayCollision(component, position.LocalPosition);

    /// <summary>
    /// Get all collision data from a ray.
    /// </summary>
    /// <param name="component">The raycast component to check against.</param>
    /// <param name="position">The position to offset the raycast by.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetRayCollision(RaycastComponent component, Transform position)
        => GetRayCollision(component, position.Position);

    /// <summary>
    /// Get all collision data from a ray.
    /// </summary>
    /// <param name="component">The raycast component to check against.</param>
    /// <param name="position">The position to offset the raycast by.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetRayCollision(RaycastComponent component, Vector2 position)
        => GetRayCollision(component.Ray.Position + position, component.Ray.Direction, component.Mask);

    /// <summary>
    /// Get all collision data from a ray.
    /// </summary>
    /// <param name="ray">The ray to check against.</param>
    /// <param name="tagMask">The collider tags the ray should collide with.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetRayCollision(Ray ray, TagContainer tagMask)
        => GetRayCollision(ray.Position, ray.Direction, tagMask);

    /// <summary>
    /// Get all collision data from a ray.
    /// </summary>
    /// <param name="position">The position to start the ray from</param>
    /// <param name="direction">The direction of the ray, with the length of the direction determining the length of the ray.</param>
    /// <param name="tagMask">The collider tags the ray should collide with.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetRayCollision(Vector2 position, Vector2 direction, TagContainer tagMask)
        => GetSweptCollision(new PointCollider(position) { Mask = tagMask }, Matrix3x2.Identity, direction);
    #endregion

    #region Manual Swept Collisions Checks
    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetSweptCollision(CollisionComponent collider, Vector2 velocity)
        => GetSweptCollision(collider, collider.Actor.Position.GlobalMatrix, collider.Actor.Position.GlobalMatrixInverse, velocity);

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The position offset for the collider by.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetSweptCollision(CollisionComponent collider, ActorPosition position, Vector2 velocity)
        => GetSweptCollision(collider, position.GlobalMatrix, position.GlobalMatrixInverse, velocity);

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The position offset for the collider by.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetSweptCollision(CollisionComponent collider, Transform position, Vector2 velocity)
        => GetSweptCollision(collider, position.Matrix, position.MatrixInverse, velocity);

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The position offset for the collider by.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetSweptCollision(CollisionComponent collider, Matrix3x2 position, Vector2 velocity) {
        Matrix3x2.Invert(position, out Matrix3x2 positionInv);
        return GetSweptCollision(collider, position, positionInv, velocity);
    }

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The position offset for the collider by.</param>
    /// <param name="positionInv">A pre-calculated inverted matrix for the position.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetSweptCollision(CollisionComponent collider, Matrix3x2 position, Matrix3x2 positionInv, Vector2 velocity)
        => GetSweptCollision(e => CheckComponent(collider, e), collider.Collider, position, positionInv, velocity);

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The position offset for the collider by.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetSweptCollision(Collider collider, ActorPosition position, Vector2 velocity)
        => GetSweptCollision(collider, position.GlobalMatrix, position.GlobalMatrixInverse, velocity);

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The position offset for the collider by.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetSweptCollision(Collider collider, Transform position, Vector2 velocity)
        => GetSweptCollision(collider, position.Matrix, position.MatrixInverse, velocity);

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The position offset for the collider by.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetSweptCollision(Collider collider, Matrix3x2 position, Vector2 velocity) {
        Matrix3x2.Invert(position, out Matrix3x2 positionInv);
        return GetSweptCollision(collider, position, positionInv, velocity);
    }

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The position offset for the collider by.</param>
    /// <param name="positionInv">A pre-calculated inverted matrix for the position.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetSweptCollision(Collider collider, Matrix3x2 position, Matrix3x2 positionInv, Vector2 velocity)
        => GetSweptCollision(CheckComponent, collider, position, positionInv, velocity);

    SweptCollisionInfo GetSweptCollision(Func<CollisionComponent, bool> componentCheck, Collider collider, Matrix3x2 position, Matrix3x2 positionInv, Vector2 velocity) {
        // If object isn't moving just get the first collided object to avoid extra calculations
        if (velocity == Vector2.Zero) {
            Log.Info("COLLISION: Trying to get swept collision with no velocity could result in errors, returning basic collision check");
            SingleCollisionInfo collision = GetFirstCollision(collider, position);
            return new(collision.Collided, Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero, []);
        }

        int minCollider = -1;
        List<SweptCollisionComponentInfo> componentInfo = [];

        foreach (CollisionComponent other in Colliders) {
            if (!componentCheck(other)) { continue; }

            SweptCollisionComponentInfo result = GetSweptComponentInfo(new(
                collider,
                position,
                positionInv,
                velocity,
                other.Collider,
                other.Actor.Position.GlobalMatrix,
                other.Actor.Position.GlobalMatrixInverse,
                Vector2.Zero
            ), other);

            if (result.Colliders.Length == 0) { continue; }

            if (minCollider == -1 || IsRayFractionSmaller(result.Colliders[result.ClosestCollider].Fraction, componentInfo[minCollider].Fraction)) {
                minCollider = componentInfo.Count;
            }
            componentInfo.Add(result);
        }

        if (minCollider == -1) { return new(false, velocity, velocity, Vector2.One, Vector2.One, []); }

        Vector2 velocityScaled = velocity * componentInfo[minCollider].Fraction;
        Vector2 velocityScaledClamped = velocity * componentInfo[minCollider].FractionClamped;

        // Failsafe: Collision bugs occasionally let object intersect after a sweep, if that happens to move is cancelled.
        //  The hope is this only occurs with precision due to colliders being very close so the cancel shouldn't be too noticable.
        //  Need to find the solution in the algorithm eventually though.
        if (GetFirstCollision(collider, Matrix3x2.CreateTranslation(velocityScaled) * position).Collided) { return new(false, Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero, []); }

        SweptCollisionComponentInfo[] outputComponentInfo = [.. componentInfo.OrderBy(e => e.Fraction.LengthSquared())];

        return new(
            true,
            velocityScaled,
            velocityScaledClamped,
            componentInfo[minCollider].Fraction,
            componentInfo[minCollider].FractionClamped,
            outputComponentInfo
        );
    }

    SweptCollisionComponentInfo GetSweptComponentInfo(ColliderContext ctx, CollisionComponent component) {
        SweepColliderPair[] collisionPairs = ColliderIntersectionFraction(ctx);

        if (collisionPairs.Length == 0) { return new(component, Vector2.One, Vector2.One, 0, []); }

        SweptColliderInfo[] colliders = new SweptColliderInfo[collisionPairs.Length];
        int closest = 0;
        for (int i = 0; i < colliders.Length; ++i) {
            SweepColliderPair pair = collisionPairs[i];

            Vector2 fraction = pair.Fraction;
            // Apply small pushback to stop objects getting stuck inside one another
            fraction -= Vector2.One * (SWEEP_TOLERANCE + float.Epsilon);
            Vector2 fractionClamped = FractionNegative(fraction) ? Vector2.Zero : fraction;

            SweptColliderInfo info = new(pair.B, fraction, fractionClamped, pair.Normal);
            if (IsRayFractionSmaller(info.Fraction, collisionPairs[closest].Fraction)) {
                closest = i;
            }
            colliders[i] = info;
        }

        return new(component, colliders[closest].Fraction, colliders[closest].FractionClamped, closest, colliders);
    }
    #endregion

    #region Manual Pushout Checks
    /// <summary>
    /// Get the distance required to push the collision component out of geometry.
    /// Note: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider.
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <returns>Information about collision pushout results.</returns>
    public PushoutCollisionInfo GetCollisionPushout(CollisionComponent collider)
        => GetCollisionPushout(collider, collider.Actor.Position.GlobalMatrix, collider.Actor.Position.GlobalMatrixInverse);

    /// <summary>
    /// Get the distance required to push the collision component out of geometry.
    /// Note: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider.
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The position offset for the collider by.</param>
    /// <returns>Information about collision pushout results.</returns>
    public PushoutCollisionInfo GetCollisionPushout(CollisionComponent collider, ActorPosition position)
        => GetCollisionPushout(collider, position.GlobalMatrix, position.GlobalMatrixInverse);

    /// <summary>
    /// Get the distance required to push the collision component out of geometry.
    /// Note: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider.
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The position offset for the collider by.</param>
    /// <returns>Information about collision pushout results.</returns>
    public PushoutCollisionInfo GetCollisionPushout(CollisionComponent collider, Transform position)
        => GetCollisionPushout(collider, position.Matrix, position.MatrixInverse);

    /// <summary>
    /// Get the distance required to push the collision component out of geometry.
    /// Note: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider.
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The position offset for the collider by.</param>
    /// <returns>Information about collision pushout results.</returns>
    public PushoutCollisionInfo GetCollisionPushout(CollisionComponent collider, Matrix3x2 position) {
        Matrix3x2.Invert(position, out Matrix3x2 positionInv);
        return GetCollisionPushout(collider, position, positionInv);
    }

    /// <summary>
    /// Get the distance required to push the collision component out of geometry.
    /// Note: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider.
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The position offset for the collider by.</param>
    /// <param name="positionInv">A pre-calculated inverted matrix for the position.</param>
    /// <returns>Information about collision pushout results.</returns>
    public PushoutCollisionInfo GetCollisionPushout(CollisionComponent collider, Matrix3x2 position, Matrix3x2 positionInv)
        => GetCollisionPushout(e => CheckComponent(collider, e), collider.Collider, position, positionInv);

    /// <summary>
    /// Get the distance required to push the collider out of geometry.
    /// Note: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider.
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The position offset for the collider by.</param>
    /// <returns>Information about collision pushout results.</returns>
    public PushoutCollisionInfo GetCollisionPushout(Collider collider, Matrix3x2 position) {
        Matrix3x2.Invert(position, out Matrix3x2 positionInv);
        return GetCollisionPushout(collider, position, positionInv);
    }

    /// <summary>
    /// Get the distance required to push the collider out of geometry.
    /// Note: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider.
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The position offset for the collider by.</param>
    /// <param name="positionInv">A pre-calculated inverted matrix for the position.</param>
    /// <returns>Information about collision pushout results.</returns>
    public PushoutCollisionInfo GetCollisionPushout(Collider collider, Matrix3x2 position, Matrix3x2 positionInv)
        => GetCollisionPushout(CheckComponent, collider, position, positionInv);

    /// <summary>
    /// Get the distance required to push the collision component out of geometry in a given direction.
    /// Note: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider.
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="direction">The direction to push the collider in. The resulting vector will have the same angle with an adjusted length.</param>
    /// <returns>Information about collision pushout results.</returns>
    public PushoutCollisionInfo GetCollisionPushoutInDirection(CollisionComponent collider, Vector2 direction)
        => GetCollisionPushoutInDirection(collider, collider.Actor.Position.GlobalMatrix, direction);

    /// <summary>
    /// Get the distance required to push the collision component out of geometry in a given direction.
    /// Note: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider.
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The position offset for the collider by.</param>
    /// <param name="direction">The direction to push the collider in. The resulting vector will have the same angle with an adjusted length.</param>
    /// <returns>Information about collision pushout results.</returns>
    public PushoutCollisionInfo GetCollisionPushoutInDirection(CollisionComponent collider, ActorPosition position, Vector2 direction)
        => GetCollisionPushoutInDirection(collider, position.GlobalMatrix, direction);

    /// <summary>
    /// Get the distance required to push the collision component out of geometry in a given direction.
    /// Note: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider.
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The position offset for the collider by.</param>
    /// <param name="direction">The direction to push the collider in. The resulting vector will have the same angle with an adjusted length.</param>
    /// <returns>Information about collision pushout results.</returns>
    public PushoutCollisionInfo GetCollisionPushoutInDirection(CollisionComponent collider, Transform position, Vector2 direction)
        => GetCollisionPushoutInDirection(collider, position.Matrix, direction);

    /// <summary>
    /// Get the distance required to push the collision component out of geometry in a given direction.
    /// Note: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider.
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The position offset for the collider by.</param>
    /// <param name="direction">The direction to push the collider in. The resulting vector will have the same angle with an adjusted length.</param>
    /// <returns>Information about collision pushout results.</returns>
    public PushoutCollisionInfo GetCollisionPushoutInDirection(CollisionComponent collider, Matrix3x2 position, Vector2 direction) {
        if (direction == Vector2.Zero) { return GetCollisionPushout(collider, position); }
        return SweptToPushoutInfo(GetSweptCollision(collider, position, -direction), direction);
    }

    /// <summary>
    /// Get the distance required to push the collision component out of geometry in a given direction.
    /// Note: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider.
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The position offset for the collider by.</param>
    /// <param name="direction">The direction to push the collider in. The resulting vector will have the same angle with an adjusted length.</param>
    /// <returns>Information about collision pushout results.</returns>
    public PushoutCollisionInfo GetCollisionPushoutInDirection(Collider collider, Matrix3x2 position, Vector2 direction) {
        if (direction == Vector2.Zero) { return GetCollisionPushout(collider, position); }
        return SweptToPushoutInfo(GetSweptCollision(collider, position, -direction), direction);
    }

    PushoutCollisionInfo GetCollisionPushout(Func<CollisionComponent, bool> componentCheck, Collider collider, Matrix3x2 position, Matrix3x2 positionInv) {
        Vector2 smallestPushout = Vector2.Zero;
        float smallestLength = float.PositiveInfinity;

        List<PushoutCollisionComponentInfo> componentInfo = [];
        foreach (CollisionComponent other in Colliders) {
            if (!componentCheck(other)) { continue; }
            PushoutCollisionComponentInfo? result = GetPushoutCollisionComponentInfo(collider, position, positionInv, other);
            if (result == null) { continue; }
            PushoutCollisionComponentInfo resultCast = (PushoutCollisionComponentInfo)result;

            float compareLength = resultCast.Colliders[resultCast.LargestPushout].Pushout.LengthSquared();
            if (compareLength < smallestLength) {
                smallestPushout = resultCast.Colliders[resultCast.LargestPushout].Pushout;
                smallestLength = compareLength;
            }
            componentInfo.Add(resultCast);
        }

        return new(componentInfo.Count > 0, smallestPushout, [.. componentInfo.OrderByDescending(e => e.Colliders[e.LargestPushout].Pushout.LengthSquared())]);
    }

    static PushoutCollisionComponentInfo? GetPushoutCollisionComponentInfo(
        Collider collider,
        Matrix3x2 position,
        Matrix3x2 positionInv,
        CollisionComponent other
    ) {
        PushoutColliderPair[] collisionPairs = ColliderPushout(new(
            collider,
            position,
            positionInv,
            Vector2.Zero,
            other.Collider,
            other.Actor.Position.GlobalMatrix,
            other.Actor.Position.GlobalMatrixInverse,
            Vector2.Zero
        ));
        if (collisionPairs.Length == 0) { return null; }

        PushoutColliderInfo[] colliders = new PushoutColliderInfo[collisionPairs.Length];
        int closest = 0;
        for (int i = 0; i < colliders.Length; ++i) {
            PushoutColliderPair pair = collisionPairs[i];
            colliders[i] = new(pair.B, pair.Pushout);
            if (pair.Pushout.LengthSquared() < collisionPairs[closest].Pushout.LengthSquared()) {
                closest = i;
            }
        }

        return new(other, closest, colliders);
    }

    static PushoutCollisionInfo SweptToPushoutInfo(SweptCollisionInfo info, Vector2 direction) {
        return new(info.Collided, info.AdjustedVelocity, [.. info.ColliderInfo.Select(e => {
            return new PushoutCollisionComponentInfo(e.Component, e.ClosestCollider, [.. e.Colliders.Select(c => {
                return new PushoutColliderInfo(c.Collider, c.Fraction * direction);
            })]);
        })]);
    }

    #endregion

    #region 2 Collider Middleman Functions
    static SimplexColliderPair[] ColliderOverlapCheck(ColliderContext ctx, bool reversed = false) {
        if (!TagMatch(ctx.ColliderA, ctx.ColliderB, reversed)) { return []; }

        Rect boundsAOffset = CalcExtra.TransformRect(ctx.ColliderA.Bounds, ctx.PositionA);
        Rect boundsBOffset = CalcExtra.TransformRect(ctx.ColliderB.Bounds, ctx.PositionB);
        if (!boundsAOffset.Overlaps(boundsBOffset)) {
            return [];
        }

        if (ctx.ColliderA.Multi) {
            Collider[] subs = ctx.ColliderA.GetSubColliders(CalcExtra.TransformRect(ctx.ColliderB.Bounds, ctx.PositionB * ctx.PositionAInv));
            List<SimplexColliderPair> pairs = [];
            foreach (Collider collider in subs) {
                SimplexColliderPair[] subCollisions = ColliderOverlapCheck(ctx with { ColliderA = collider }, reversed);
                if (subCollisions.Length > 0) { pairs.AddRange(subCollisions); }
            }
            return [.. pairs];
        }

        if (ctx.ColliderB.Multi) { return ColliderOverlapCheck(FlipContext(ctx), !reversed); }

        if (GetCollisionSimplex(ctx).Collided) {
            return [reversed ? new(ctx.ColliderB, ctx.ColliderA) : new(ctx.ColliderA, ctx.ColliderB)];
        }
        return [];
    }

    static PushoutColliderPair[] ColliderPushout(ColliderContext ctx, bool reversed = false) {
        if (!TagMatch(ctx.ColliderA, ctx.ColliderB, reversed)) { return []; }

        Rect boundsAOffset = CalcExtra.TransformRect(ctx.ColliderA.Bounds, ctx.PositionA);
        Rect boundsBOffset = CalcExtra.TransformRect(ctx.ColliderB.Bounds, ctx.PositionB);
        if (!boundsAOffset.Overlaps(boundsBOffset)) {
            return [];
        }

        if (ctx.ColliderA.Multi) {
            Collider[] subs = ctx.ColliderA.GetSubColliders(CalcExtra.TransformRect(ctx.ColliderB.Bounds, ctx.PositionB * ctx.PositionAInv));
            List<PushoutColliderPair> pairs = [];
            foreach (Collider collider in subs) {
                PushoutColliderPair[] subCollisions = ColliderPushout(ctx with { ColliderA = collider }, reversed);
                if (subCollisions.Length > 0) { pairs.AddRange(subCollisions); }
            }
            return [.. pairs];
        }

        if (ctx.ColliderB.Multi) { return ColliderPushout(FlipContext(ctx), !reversed); }

        CollisionSimplexInfo simplex = GetCollisionSimplex(ctx);
        if (!simplex.Collided) { return []; }

        CollisionPushoutInfo info = GetPushout(ctx, simplex);
        if (!info.Collided) { return []; }
        return [reversed ? new(ctx.ColliderB, ctx.ColliderA, -info.Pushout) : new(ctx.ColliderA, ctx.ColliderB, info.Pushout)];
    }

    static SweepColliderPair[] ColliderIntersectionFraction(ColliderContext ctx, bool reversed = false) {
        if (!TagMatch(ctx.ColliderA, ctx.ColliderB, reversed)) { return []; }

        Rect sweptBoundsA = SweptBounds(ctx.ColliderA, ctx.VelocityA);
        Rect sweptBoundsB = SweptBounds(ctx.ColliderB, ctx.VelocityB);
        Rect sweptBoundsAOffset = CalcExtra.TransformRect(sweptBoundsA, ctx.PositionA);
        Rect sweptBoundsBOffset = CalcExtra.TransformRect(sweptBoundsB, ctx.PositionB);
        if (!sweptBoundsAOffset.Overlaps(sweptBoundsBOffset)) {
            return [];
        }

        if (ctx.ColliderA.Multi) {
            List<SweepColliderPair> pairs = [];
            foreach (Collider collider in ctx.ColliderA.GetSubColliders(CalcExtra.TransformRect(ctx.ColliderB.Bounds, ctx.PositionB * ctx.PositionAInv))) {
                SweepColliderPair[] subCollisions = ColliderIntersectionFraction(ctx with { ColliderA = collider }, reversed);
                if (subCollisions.Length > 0) { pairs.AddRange(subCollisions); }
            }
            return [.. pairs];
        }

        if (ctx.ColliderB.Multi) { return ColliderIntersectionFraction(FlipContext(ctx), !reversed); }

        CollisionSweepInfo collision = GetRayIntersectionFraction(ctx);
        if (FractionNegative(collision.Fraction)) {
            // Sweep calculation sometimes gives results for colliders that aren't initially colliding
            if (ColliderOverlapCheck(ctx).Length == 0) {
                return [];
            }
        }
        return [reversed ? new(ctx.ColliderB, ctx.ColliderA, collision.Fraction, collision.Normal) : new(ctx.ColliderA, ctx.ColliderB, collision.Fraction, collision.Normal)];
    }
    #endregion

    #region Primary Collision Functions
    static CollisionSimplexInfo GetCollisionSimplex(ColliderContext ctx) {
        // Point A
        Vector2 direction = Vector2.Transform(ctx.ColliderB.Center, ctx.PositionB) - Vector2.Transform(ctx.ColliderA.Center, ctx.PositionA);
        Vector2 pointA = Support(ctx, direction);
        if (!PointCrossesOrigin(pointA, direction)) { return new(pointA, Vector2.Zero, Vector2.Zero, false); }

        // Point B
        direction = -direction;
        Vector2 pointB = Support(ctx, direction);
        if (!PointCrossesOrigin(pointB, direction)) { return new(pointA, pointB, Vector2.Zero, false); }

        // Point C
        Vector2 AInv = -pointA;
        Vector2 AB = pointB + AInv;
        direction = CalcExtra.TripleProduct(AB, AInv, AB);
        Vector2 pointC = Support(ctx, direction);
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
                pointC = Support(ctx, direction);
                if (!PointCrossesOrigin(pointC, direction)) { return new(pointA, pointB, pointC, false); }
            }
            else if (Vector2.Dot(perpCA, CInv) > 0) {
                pointB = pointC;

                direction = perpCA;
                pointC = Support(ctx, direction);
                if (!PointCrossesOrigin(pointC, direction)) { return new(pointA, pointB, pointC, false); }
            }
            else {
                return new(pointA, pointB, pointC, true);
            }
        }
    }

    static CollisionPushoutInfo GetPushout(ColliderContext ctx, CollisionSimplexInfo simplex) {
        if (!simplex.Collided) { return new(Vector2.Zero, false); }

        List<Vector2> points = [
            simplex.A,
            simplex.B,
            simplex.C
        ];

        bool clockwiseWinding = IsClockwiseWinding(simplex.A, simplex.B, simplex.C);
        Vector2 pushout = Vector2.Zero;

        for (int i = 0; i < ITERATION_LIMIT; ++i) {
            Edge closestEdge = new(0, Vector2.Zero, float.PositiveInfinity);

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
            Vector2 newSupport = Support(ctx, direction);
            float supportDistance = Vector2.Dot(newSupport, direction);

            // Determine new pushout approximation
            pushout = supportDistance * direction;

            // Return immediately if pushout is as close as possible
            supportDistance = MathF.Abs(supportDistance - closestEdge.Distance);
            if (supportDistance <= PUSHOUT_TOLERANCE) { return new(pushout, true); }

            points.Insert(closestEdge.Index, newSupport);
        }

        // Return closest approximate pushout if nothing better can be found in time
        return new(pushout, true);
    }

    static CollisionSweepInfo GetRayIntersectionFraction(ColliderContext ctx) {
        // Get velocity difference
        Vector2 velocityDifference = ctx.VelocityB - ctx.VelocityA;

        // Cancel if objects are moving the same direction or stationary
        if (velocityDifference == Vector2.Zero) { return new(Vector2.One, Vector2.UnitY, false); }

        float velocityLength = velocityDifference.LengthSquared();

        // Point A
        Vector2 direction = -velocityDifference;
        Vector2 pointA = Support(ctx, direction);

        // Point B
        direction = CalcExtra.TripleProduct(direction, -pointA, direction);
        Vector2 pointB = Support(ctx, direction);

        float crossA = Calc.Cross(velocityDifference, pointA);
        float crossB = Calc.Cross(velocityDifference, pointB);
        if (MathF.Sign(crossA) == MathF.Sign(crossB)) {
            // First line lies exactly on velocity vector
            if (MathF.Abs(crossA) < float.Epsilon && MathF.Abs(crossB) < float.Epsilon) {
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
            if (direction.LengthSquared() < float.Epsilon) {
                return SweepLineIntersection(pointA, pointB, velocityDifference, velocityLength);
            }
            Vector2 pointC = Support(ctx, direction);

            // New support point is the same as one of the current points
            if (Vector2.DistanceSquared(pointC, pointA) < SWEEP_TOLERANCE || Vector2.DistanceSquared(pointC, pointB) < SWEEP_TOLERANCE) {
                return SweepLineIntersection(pointA, pointB, velocityDifference, velocityLength);
            }

            float crossC = Calc.Cross(velocityDifference, pointC);

            // pointC lies exactly on the line
            if (MathF.Abs(crossC) < float.Epsilon) {
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

    static Vector2 Support(ColliderContext ctx, Vector2 direction) {
        Vector2 directionAdjustedA = CalcExtra.TransformDirection(direction, ctx.PositionAInv);
        Vector2 directionAdjustedB = CalcExtra.TransformDirection(-direction, ctx.PositionBInv);
        Vector2 supportA = Vector2.Transform(ctx.ColliderA.Support(directionAdjustedA), ctx.PositionA);
        Vector2 supportB = Vector2.Transform(ctx.ColliderB.Support(directionAdjustedB), ctx.PositionB);
        return supportA - supportB;
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
            component.Actor.GlobalComponentsTicking &&
            component.Ticking;
    }

    static ColliderContext FlipContext(ColliderContext ctx) {
        return new(ctx.ColliderB, ctx.PositionB, ctx.PositionBInv, ctx.VelocityB, ctx.ColliderA, ctx.PositionA, ctx.PositionAInv, ctx.VelocityA);
    }
    #endregion
}