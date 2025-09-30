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
    public AllCollisionInfo GetAllCollisions(CollisionComponent collider, Matrix3x2 position, Matrix3x2 positionInv) {
        List<CollisionComponentInfo> collisions = [];
        foreach (CollisionComponent other in Colliders) {
            if (!CheckComponent(collider, other)) { continue; }
            SimplexColliderPair[] pairs = ColliderOverlapCheck(
                collider.Collider,
                position,
                positionInv,
                other.Collider,
                other.Actor.Position.GlobalMatrix,
                other.Actor.Position.GlobalMatrixInverse
            );
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
    public AllCollisionInfo GetAllCollisions(Collider collider, Matrix3x2 position, Matrix3x2 positionInv) {
        List<CollisionComponentInfo> collisions = [];
        foreach (CollisionComponent other in Colliders) {
            if (!CheckComponent(other)) { continue; }
            SimplexColliderPair[] pairs = ColliderOverlapCheck(
                collider,
                position,
                positionInv,
                other.Collider,
                other.Actor.Position.GlobalMatrix,
                other.Actor.Position.GlobalMatrixInverse
            );
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
    public SingleCollisionInfo GetFirstCollision(CollisionComponent collider, Matrix3x2 position, Matrix3x2 positionInv) {
        foreach (CollisionComponent other in Colliders) {
            if (!CheckComponent(collider, other)) { continue; }
            SimplexColliderPair[] pairs = ColliderOverlapCheck(
                collider.Collider,
                position,
                positionInv,
                other.Collider,
                other.Actor.Position.GlobalMatrix,
                other.Actor.Position.GlobalMatrixInverse
            );
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
    public SingleCollisionInfo GetFirstCollision(Collider collider, Matrix3x2 position, Matrix3x2 positionInv) {
        foreach (CollisionComponent other in Colliders) {
            if (!CheckComponent(other)) { continue; }
            SimplexColliderPair[] pairs = ColliderOverlapCheck(
                collider,
                position,
                positionInv,
                other.Collider,
                other.Actor.Position.GlobalMatrix,
                other.Actor.Position.GlobalMatrixInverse
            );
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
    /// <param name="allowNegative">If the final cast position is allowed to be behind the starting point.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetSweptCollision(CollisionComponent collider, Vector2 velocity, bool allowNegative = false)
        => GetSweptCollision(collider, collider.Actor.Position.GlobalMatrix, collider.Actor.Position.GlobalMatrixInverse, velocity, allowNegative);

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The position offset for the collider by.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <param name="allowNegative">If the final cast position is allowed to be behind the starting point.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetSweptCollision(CollisionComponent collider, ActorPosition position, Vector2 velocity, bool allowNegative = false)
        => GetSweptCollision(collider, position.GlobalMatrix, position.GlobalMatrixInverse, velocity, allowNegative);

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The position offset for the collider by.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <param name="allowNegative">If the final cast position is allowed to be behind the starting point.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetSweptCollision(CollisionComponent collider, Transform position, Vector2 velocity, bool allowNegative = false)
        => GetSweptCollision(collider, position.Matrix, position.MatrixInverse, velocity, allowNegative);

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The position offset for the collider by.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <param name="allowNegative">If the final cast position is allowed to be behind the starting point.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetSweptCollision(CollisionComponent collider, Matrix3x2 position, Vector2 velocity, bool allowNegative = false) {
        Matrix3x2.Invert(position, out Matrix3x2 positionInv);
        return GetSweptCollision(collider, position, positionInv, velocity, allowNegative);
    }

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The position offset for the collider by.</param>
    /// <param name="positionInv">A pre-calculated inverted matrix for the position.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <param name="allowNegative">If the final cast position is allowed to be behind the starting point.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetSweptCollision(CollisionComponent collider, Matrix3x2 position, Matrix3x2 positionInv, Vector2 velocity, bool allowNegative = false) {
        // If object isn't moving just get the first collided object to avoid extra calculations
        if (velocity == Vector2.Zero) {
            Log.Info("COLLISION: Trying to get swept collision with no velocity could result in errors, returning basic collision check");
            SingleCollisionInfo collision = GetFirstCollision(collider, position);
            return new(collision.Collided, Vector2.Zero, Vector2.Zero, []);
        }

        Vector2 minFraction = Vector2.One;
        CollisionComponent? minCollider = null;
        List<SweptCollisionComponentInfo> componentInfo = [];

        foreach (CollisionComponent other in Colliders) {
            if (!CheckComponent(collider, other)) { continue; }
            SweptCollisionComponentInfo? result = GetSweptCollisionComponentInfo(collider.Collider, position, positionInv, velocity, other);
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

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The position offset for the collider by.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <param name="allowNegative">If the final cast position is allowed to be behind the starting point.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetSweptCollision(Collider collider, ActorPosition position, Vector2 velocity, bool allowNegative = false)
        => GetSweptCollision(collider, position.GlobalMatrix, position.GlobalMatrixInverse, velocity, allowNegative);

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The position offset for the collider by.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <param name="allowNegative">If the final cast position is allowed to be behind the starting point.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetSweptCollision(Collider collider, Transform position, Vector2 velocity, bool allowNegative = false)
        => GetSweptCollision(collider, position.Matrix, position.MatrixInverse, velocity, allowNegative);

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The position offset for the collider by.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <param name="allowNegative">If the final cast position is allowed to be behind the starting point.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetSweptCollision(Collider collider, Matrix3x2 position, Vector2 velocity, bool allowNegative = false) {
        Matrix3x2.Invert(position, out Matrix3x2 positionInv);
        return GetSweptCollision(collider, position, positionInv, velocity, allowNegative);
    }

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The position offset for the collider by.</param>
    /// <param name="positionInv">A pre-calculated inverted matrix for the position.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <param name="allowNegative">If the final cast position is allowed to be behind the starting point.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetSweptCollision(Collider collider, Matrix3x2 position, Matrix3x2 positionInv, Vector2 velocity, bool allowNegative = false) {
        // If object isn't moving just get the first collided object to avoid extra calculations
        if (velocity == Vector2.Zero) {
            Log.Info("COLLISION: Trying to get swept collision with no velocity could result in errors, returning basic collision check");
            SingleCollisionInfo collision = GetFirstCollision(collider, position);
            return new(collision.Collided, Vector2.Zero, Vector2.Zero, []);
        }

        Vector2 minFraction = Vector2.One;
        CollisionComponent? minCollider = null;
        List<SweptCollisionComponentInfo> componentInfo = [];

        foreach (CollisionComponent other in Colliders) {
            if (!CheckComponent(other)) { continue; }
            SweptCollisionComponentInfo? result = GetSweptCollisionComponentInfo(collider, position, positionInv, velocity, other);
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

    static SweptCollisionComponentInfo? GetSweptCollisionComponentInfo(
        Collider collider,
        Matrix3x2 position,
        Matrix3x2 positionInv,
        Vector2 velocity,
        CollisionComponent other
    ) {
        SweepColliderPair[] collisionPairs = ColliderIntersectionFraction(
            collider,
            position,
            positionInv,
            velocity,
            other.Collider,
            other.Actor.Position.GlobalMatrix,
            other.Actor.Position.GlobalMatrixInverse,
            Vector2.Zero
        );
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

        return new(other, colliders[closest].Fraction, closest, colliders);
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

        return new(collided, velocity * fraction, fraction, [.. colliderInfo.OrderBy(e => e.Fraction.LengthSquared())]);
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
    public PushoutCollisionInfo GetCollisionPushout(CollisionComponent collider, Matrix3x2 position, Matrix3x2 positionInv) {
        Vector2 smallestPushout = Vector2.Zero;
        float smallestLength = float.PositiveInfinity;

        List<PushoutCollisionComponentInfo> componentInfo = [];
        foreach (CollisionComponent other in Colliders) {
            if (!CheckComponent(collider, other)) { continue; }
            PushoutCollisionComponentInfo? result = GetPushoutCollisionComponentInfo(collider.Collider, position, positionInv, other);
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
    public PushoutCollisionInfo GetCollisionPushout(Collider collider, Matrix3x2 position, Matrix3x2 positionInv) {
        Vector2 smallestPushout = Vector2.Zero;
        float smallestLength = float.PositiveInfinity;

        List<PushoutCollisionComponentInfo> componentInfo = [];
        foreach (CollisionComponent other in Colliders) {
            if (!CheckComponent(other)) { continue; }
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
        return SweptToPushoutInfo(GetSweptCollision(collider, position, -direction, true), direction);
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
        return SweptToPushoutInfo(GetSweptCollision(collider, position, -direction, true), direction);
    }

    static PushoutCollisionComponentInfo? GetPushoutCollisionComponentInfo(
        Collider collider,
        Matrix3x2 position,
        Matrix3x2 positionInv,
        CollisionComponent other
    ) {
        PushoutColliderPair[] collisionPairs = ColliderPushout(
            collider,
            position,
            positionInv,
            other.Collider,
            other.Actor.Position.GlobalMatrix,
            other.Actor.Position.GlobalMatrixInverse
        );
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
    static SimplexColliderPair[] ColliderOverlapCheck(Collider colliderA, Matrix3x2 positionA, Matrix3x2 positionAInv, Collider colliderB, Matrix3x2 positionB, Matrix3x2 positionBInv, bool reversed = false) {
        if (!TagMatch(colliderA, colliderB, reversed)) { return []; }

        Rect boundsAOffset = CalcExtra.TransformRect(colliderA.Bounds, positionA);
        Rect boundsBOffset = CalcExtra.TransformRect(colliderB.Bounds, positionB);
        if (!boundsAOffset.Overlaps(boundsBOffset)) {
            return [];
        }

        if (colliderA.Multi) {
            Collider[] subs = colliderA.GetSubColliders(CalcExtra.TransformRect(colliderB.Bounds, positionB * positionAInv));
            List<SimplexColliderPair> pairs = [];
            foreach (Collider collider in subs) {
                SimplexColliderPair[] subCollisions = ColliderOverlapCheck(collider, positionA, positionAInv, colliderB, positionB, positionBInv, reversed);
                if (subCollisions.Length > 0) { pairs.AddRange(subCollisions); }
            }
            return [.. pairs];
        }

        if (colliderB.Multi) {
            return ColliderOverlapCheck(colliderB, positionB, positionBInv, colliderA, positionA, positionAInv, !reversed);
        }

        if (GetCollisionSimplex(colliderA, positionA, positionAInv, colliderB, positionB, positionBInv).Collided) {
            return [reversed ? new(colliderB, colliderA) : new(colliderA, colliderB)];
        }
        return [];
    }

    static PushoutColliderPair[] ColliderPushout(Collider colliderA, Matrix3x2 positionA, Matrix3x2 positionAInv, Collider colliderB, Matrix3x2 positionB, Matrix3x2 positionBInv, bool reversed = false) {
        if (!TagMatch(colliderA, colliderB, reversed)) { return []; }

        Rect boundsAOffset = CalcExtra.TransformRect(colliderA.Bounds, positionA);
        Rect boundsBOffset = CalcExtra.TransformRect(colliderB.Bounds, positionB);
        if (!boundsAOffset.Overlaps(boundsBOffset)) {
            return [];
        }

        if (colliderA.Multi) {
            Collider[] subs = colliderA.GetSubColliders(CalcExtra.TransformRect(colliderB.Bounds, positionB * positionAInv));
            List<PushoutColliderPair> pairs = [];
            foreach (Collider collider in subs) {
                PushoutColliderPair[] subCollisions = ColliderPushout(collider, positionA, positionAInv, colliderB, positionB, positionBInv, reversed);
                if (subCollisions.Length > 0) { pairs.AddRange(subCollisions); }
            }
            return [.. pairs];
        }

        if (colliderB.Multi) {
            return ColliderPushout(colliderB, positionB, positionBInv, colliderA, positionA, positionAInv, !reversed);
        }

        CollisionSimplexInfo simplex = GetCollisionSimplex(colliderA, positionA, positionAInv, colliderB, positionB, positionBInv);
        if (!simplex.Collided) { return []; }

        CollisionPushoutInfo info = GetPushout(colliderA, positionA, positionAInv, colliderB, positionB, positionBInv, simplex);
        if (!info.Collided) { return []; }
        return [reversed ? new(colliderB, colliderA, -info.Pushout) : new(colliderA, colliderB, info.Pushout)];
    }

    static SweepColliderPair[] ColliderIntersectionFraction(
        Collider colliderA,
        Matrix3x2 positionA,
        Matrix3x2 positionAInv,
        Vector2 velocityA,
        Collider colliderB,
        Matrix3x2 positionB,
        Matrix3x2 positionBInv,
        Vector2 velocityB,
        bool reversed = false
    ) {
        if (!TagMatch(colliderA, colliderB, reversed)) { return []; }

        Rect sweptBoundsA = SweptBounds(colliderA, velocityA);
        Rect sweptBoundsB = SweptBounds(colliderB, velocityB);
        Rect sweptBoundsAOffset = CalcExtra.TransformRect(sweptBoundsA, positionA);
        Rect sweptBoundsBOffset = CalcExtra.TransformRect(sweptBoundsB, positionB);
        if (!sweptBoundsAOffset.Overlaps(sweptBoundsBOffset)) {
            return [];
        }

        if (colliderA.Multi) {
            List<SweepColliderPair> pairs = [];
            foreach (Collider collider in colliderA.GetSubColliders(CalcExtra.TransformRect(colliderB.Bounds, positionB * positionAInv))) {
                SweepColliderPair[] subCollisions = ColliderIntersectionFraction(
                    collider,
                    positionA,
                    positionAInv,
                    velocityA,
                    colliderB,
                    positionB,
                    positionBInv,
                    velocityB,
                    reversed
                );
                if (subCollisions.Length > 0) { pairs.AddRange(subCollisions); }
            }
            return [.. pairs];
        }

        if (colliderB.Multi) {
            return ColliderIntersectionFraction(
                colliderB,
                positionB,
                positionBInv,
                velocityB,
                colliderA,
                positionA,
                positionAInv,
                velocityA,
                !reversed
            );
        }

        CollisionSweepInfo collision = GetRayIntersectionFraction(
            colliderA,
            positionA,
            positionAInv,
            velocityA,
            colliderB,
            positionB,
            positionAInv,
            velocityB
        );
        if (FractionNegative(collision.Fraction)) {
            if (ColliderOverlapCheck(colliderA, positionA, positionAInv, colliderB, positionB, positionBInv).Length == 0) {
                return [];
            }
        }
        return [reversed ? new(colliderB, colliderA, collision.Fraction, collision.Normal) : new(colliderA, colliderB, collision.Fraction, collision.Normal)];
    }
    #endregion

    #region Primary Collision Functions
    static CollisionSimplexInfo GetCollisionSimplex(
        Collider colliderA,
        Matrix3x2 positionA,
        Matrix3x2 positionAInv,
        Collider colliderB,
        Matrix3x2 positionB,
        Matrix3x2 positionBInv
    ) {
        // Point A
        Vector2 direction = Vector2.Transform(colliderB.Center, positionB) - Vector2.Transform(colliderA.Center, positionA);
        Vector2 pointA = Support(colliderA, positionA, positionAInv, colliderB, positionB, positionBInv, direction);
        if (!PointCrossesOrigin(pointA, direction)) { return new(pointA, Vector2.Zero, Vector2.Zero, false); }

        // Point B
        direction = -direction;
        Vector2 pointB = Support(colliderA, positionA, positionAInv, colliderB, positionB, positionBInv, direction);
        if (!PointCrossesOrigin(pointB, direction)) { return new(pointA, pointB, Vector2.Zero, false); }

        // Point C
        Vector2 AInv = -pointA;
        Vector2 AB = pointB + AInv;
        direction = CalcExtra.TripleProduct(AB, AInv, AB);
        Vector2 pointC = Support(colliderA, positionA, positionAInv, colliderB, positionB, positionBInv, direction);
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
                pointC = Support(colliderA, positionA, positionAInv, colliderB, positionB, positionBInv, direction);
                if (!PointCrossesOrigin(pointC, direction)) { return new(pointA, pointB, pointC, false); }
            }
            else if (Vector2.Dot(perpCA, CInv) > 0) {
                pointB = pointC;

                direction = perpCA;
                pointC = Support(colliderA, positionA, positionAInv, colliderB, positionB, positionBInv, direction);
                if (!PointCrossesOrigin(pointC, direction)) { return new(pointA, pointB, pointC, false); }
            }
            else {
                return new(pointA, pointB, pointC, true);
            }
        }
    }

    static CollisionPushoutInfo GetPushout(
        Collider colliderA,
        Matrix3x2 positionA,
        Matrix3x2 positionAInv,
        Collider colliderB,
        Matrix3x2 positionB,
        Matrix3x2 positionBInv,
        CollisionSimplexInfo simplex
    ) {
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
            Vector2 newSupport = Support(colliderA, positionA, positionAInv, colliderB, positionB, positionBInv, direction);
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

    static CollisionSweepInfo GetRayIntersectionFraction(
        Collider colliderA,
        Matrix3x2 positionA,
        Matrix3x2 positionAInv,
        Vector2 velocityA,
        Collider colliderB,
        Matrix3x2 positionB,
        Matrix3x2 positionBInv,
        Vector2 velocityB
    ) {
        // Get velocity difference
        Vector2 velocityDifference = velocityB - velocityA;

        // Cancel if objects are moving the same direction or stationary
        if (velocityDifference == Vector2.Zero) { return new(Vector2.One, Vector2.UnitY, false); }

        float velocityLength = velocityDifference.LengthSquared();

        // Point A
        Vector2 direction = -velocityDifference;
        Vector2 pointA = Support(colliderA, positionA, positionAInv, colliderB, positionB, positionBInv, direction);

        // Point B
        direction = CalcExtra.TripleProduct(direction, -pointA, direction);
        Vector2 pointB = Support(colliderA, positionA, positionAInv, colliderB, positionB, positionBInv, direction);

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
            Vector2 pointC = Support(colliderA, positionA, positionAInv, colliderB, positionB, positionBInv, direction);

            // New support point is the same as one of the current points
            if (pointC == pointA || pointC == pointB) {
                return SweepLineIntersection(pointA, pointB, velocityDifference, velocityLength);
            }

            float crossC = Calc.Cross(velocityDifference, pointC);

            // pointC lies exactly on the line
            if (crossC == 0) {
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
        Matrix3x2 positionA,
        Matrix3x2 positionAInv,
        Collider colliderB,
        Matrix3x2 positionB,
        Matrix3x2 positionBInv,
        Vector2 direction
    ) {
        Vector2 directionAdjustedA = CalcExtra.TransformDirection(direction, positionAInv);
        Vector2 directionAdjustedB = CalcExtra.TransformDirection(-direction, positionBInv);
        Vector2 supportA = Vector2.Transform(colliderA.Support(directionAdjustedA), positionA);
        Vector2 supportB = Vector2.Transform(colliderB.Support(directionAdjustedB), positionB);
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
            component.Actor.GlobalTicking &&
            component.Ticking;
    }
    #endregion
}