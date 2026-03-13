using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Controller for all collision interactions in the game.
/// </summary>
public class CollisionManager {
    #region Definitions
    const int ITERATION_LIMIT = 32;
    const float SWEEP_TOLERANCE = 0.0001f;

    record struct CollisionSweepInfo(Vector2 Fraction, Vector2 Normal, bool Collided);

    record struct SimplexColliderPair(Collider A, Collider B);
    record struct PushoutColliderPair(Collider A, Collider B, Vector2 Pushout);
    record struct SweepColliderPair(Collider A, Collider B, Vector2 Fraction, Vector2 Normal);
    #endregion

    #region Data
    readonly List<CollisionComponent> Colliders = [];

    CollisionComponent? ActiveComponent = null;
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
        => GetAllCollisions(collider, collider.Actor.Transform.GlobalMatrix, collider.Actor.Transform.GlobalMatrixInverse);

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
    public AllCollisionInfo GetAllCollisions(CollisionComponent collider, Vector2 position)
        => GetAllCollisions(collider, Matrix3x2.CreateTranslation(position));

    /// <summary>
    /// Get all collisions between given collider component and all registered collision components at a given location.
    /// </summary>
    /// <param name="collider">The collider component to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public AllCollisionInfo GetAllCollisions(CollisionComponent collider, Matrix3x2 position)
        => GetAllCollisions(collider, position, position.Invert());

    /// <summary>
    /// Get all collisions between given collider component and all registered collision components at a given location.
    /// </summary>
    /// <param name="collider">The collider component to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <param name="positionInv">A pre-calculated inverted matrix for the position.</param>
    /// <returns>Information about collision check results.</returns>
    public AllCollisionInfo GetAllCollisions(CollisionComponent collider, Matrix3x2 position, Matrix3x2 positionInv) {
        ActiveComponent = collider;
        AllCollisionInfo info = GetAllCollisions(collider.Collider, position, positionInv);
        ActiveComponent = null;
        return info;
    }

    /// <summary>
    /// Get all collisions between given collider and all registered collision components at a given location.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The actor position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public AllCollisionInfo GetAllCollisions(Collider collider, ActorPosition position)
        => GetAllCollisions(collider, position.GlobalMatrix, position.GlobalMatrixInverse);

    /// <summary>
    /// Get all collisions between given collider and all registered collision components at a given location.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public AllCollisionInfo GetAllCollisions(Collider collider, Transform position)
        => GetAllCollisions(collider, position.Matrix, position.MatrixInverse);

    /// <summary>
    /// Get all collisions between given collider and all registered collision components at a given location.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public AllCollisionInfo GetAllCollisions(Collider collider, Vector2 position)
        => GetAllCollisions(collider, Matrix3x2.CreateTranslation(position));

    /// <summary>
    /// Get all collisions between given collider and all registered collision components at a given location.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public AllCollisionInfo GetAllCollisions(Collider collider, Matrix3x2 position)
        => GetAllCollisions(collider, position, position.Invert());

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
            if (ShouldSkipCollider(other)) { continue; }
            SimplexColliderPair[] pairs = ColliderOverlapCheck(new(
                new(collider, position, positionInv),
                new(other.Collider, other.Actor.Transform.GlobalMatrix, other.Actor.Transform.GlobalMatrixInverse)
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
        => GetFirstCollision(collider, collider.Actor.Transform.GlobalMatrix, collider.Actor.Transform.GlobalMatrixInverse);

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
    public SingleCollisionInfo GetFirstCollision(CollisionComponent collider, Vector2 position)
        => GetFirstCollision(collider, Matrix3x2.CreateTranslation(position));

    /// <summary>
    /// Get the first object the given collider component collides with at a given location.
    /// Not guarenteed to be the closest, used mostly for performance when full collision information isn't needed.
    /// </summary>
    /// <param name="collider">The collider component to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public SingleCollisionInfo GetFirstCollision(CollisionComponent collider, Matrix3x2 position)
        => GetFirstCollision(collider, position, position.Invert());

    /// <summary>
    /// Get the first object the given collider component collides with at a given location.
    /// Not guarenteed to be the closest, used mostly for performance when full collision information isn't needed.
    /// </summary>
    /// <param name="collider">The collider component to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <param name="positionInv">A pre-calculated inverted matrix for the position.</param>
    /// <returns>Information about collision check results.</returns>
    public SingleCollisionInfo GetFirstCollision(CollisionComponent collider, Matrix3x2 position, Matrix3x2 positionInv) {
        ActiveComponent = collider;
        SingleCollisionInfo info = GetFirstCollision(collider.Collider, position, positionInv);
        ActiveComponent = null;
        return info;
    }

    /// <summary>
    /// Get the first object the given collider collides with at a given location.
    /// Not guarenteed to be the closest, used mostly for performance when full collision information isn't needed.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The actor position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public SingleCollisionInfo GetFirstCollision(Collider collider, ActorPosition position)
        => GetFirstCollision(collider, position.GlobalMatrix, position.GlobalMatrixInverse);

    /// <summary>
    /// Get the first object the given collider collides with at a given location.
    /// Not guarenteed to be the closest, used mostly for performance when full collision information isn't needed.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public SingleCollisionInfo GetFirstCollision(Collider collider, Transform position)
        => GetFirstCollision(collider, position.Matrix, position.MatrixInverse);

    /// <summary>
    /// Get the first object the given collider collides with at a given location.
    /// Not guarenteed to be the closest, used mostly for performance when full collision information isn't needed.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public SingleCollisionInfo GetFirstCollision(Collider collider, Vector2 position)
        => GetFirstCollision(collider, Matrix3x2.CreateTranslation(position));

    /// <summary>
    /// Get the first object the given collider collides with at a given location.
    /// Not guarenteed to be the closest, used mostly for performance when full collision information isn't needed.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public SingleCollisionInfo GetFirstCollision(Collider collider, Matrix3x2 position)
        => GetFirstCollision(collider, position, position.Invert());

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
            if (ShouldSkipCollider(other)) { continue; }
            SimplexColliderPair[] pairs = ColliderOverlapCheck(new(
                new(collider, position, positionInv),
                new(other.Collider, other.Actor.Transform.GlobalMatrix, other.Actor.Transform.GlobalMatrixInverse)
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
        => GetRayCollision(component, component.Actor.Transform);

    /// <summary>
    /// Get all collision data from a ray.
    /// </summary>
    /// <param name="component">The raycast component to check against.</param>
    /// <param name="position">The transform to apply to the ray.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetRayCollision(RaycastComponent component, ActorPosition position)
        => GetRayCollision(component, position.LocalPosition);

    /// <summary>
    /// Get all collision data from a ray.
    /// </summary>
    /// <param name="component">The raycast component to check against.</param>
    /// <param name="position">The transform to apply to the ray.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetRayCollision(RaycastComponent component, Transform position)
        => GetRayCollision(component, position.Position);

    /// <summary>
    /// Get all collision data from a ray.
    /// </summary>
    /// <param name="component">The raycast component to check against.</param>
    /// <param name="position">The transform to apply to the ray.</param>
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
        => GetSweptCollision(collider, collider.Actor.Transform.GlobalMatrix, collider.Actor.Transform.GlobalMatrixInverse, velocity);

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetSweptCollision(CollisionComponent collider, ActorPosition position, Vector2 velocity)
        => GetSweptCollision(collider, position.GlobalMatrix, position.GlobalMatrixInverse, velocity);

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetSweptCollision(CollisionComponent collider, Transform position, Vector2 velocity)
        => GetSweptCollision(collider, position.Matrix, position.MatrixInverse, velocity);

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetSweptCollision(CollisionComponent collider, Vector2 position, Vector2 velocity)
        => GetSweptCollision(collider, Matrix3x2.CreateTranslation(position), velocity);

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetSweptCollision(CollisionComponent collider, Matrix3x2 position, Vector2 velocity)
        => GetSweptCollision(collider, position, position.Invert(), velocity);

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="positionInv">A pre-calculated inverted matrix for the position.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetSweptCollision(CollisionComponent collider, Matrix3x2 position, Matrix3x2 positionInv, Vector2 velocity) {
        ActiveComponent = collider;
        SweptCollisionInfo info = GetSweptCollision(collider.Collider, position, positionInv, velocity);
        ActiveComponent = null;
        return info;
    }

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetSweptCollision(Collider collider, ActorPosition position, Vector2 velocity)
        => GetSweptCollision(collider, position.GlobalMatrix, position.GlobalMatrixInverse, velocity);

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetSweptCollision(Collider collider, Transform position, Vector2 velocity)
        => GetSweptCollision(collider, position.Matrix, position.MatrixInverse, velocity);

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetSweptCollision(Collider collider, Vector2 position, Vector2 velocity)
        => GetSweptCollision(collider, Matrix3x2.CreateTranslation(position), velocity);

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetSweptCollision(Collider collider, Matrix3x2 position, Vector2 velocity)
        => GetSweptCollision(collider, position, position.Invert(), velocity);

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="positionInv">A pre-calculated inverted matrix for the position.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <returns>Information about collision check results.</returns>
    public SweptCollisionInfo GetSweptCollision(Collider collider, Matrix3x2 position, Matrix3x2 positionInv, Vector2 velocity) {
        // If object isn't moving run basic collision check to avoid extra calculations
        if (velocity == Vector2.Zero) { return SweptInfoFromAllCollisionInfo(GetAllCollisions(collider, position)); }

        int minCollider = -1;
        List<SweptCollisionComponentInfo> componentInfo = [];

        foreach (CollisionComponent other in Colliders) {
            if (ShouldSkipCollider(other)) { continue; }

            SweptCollisionComponentInfo result = GetSweptComponentInfo(new(
                new(collider, position, positionInv, velocity),
                new(other.Collider, other.Actor.Transform.GlobalMatrix, other.Actor.Transform.GlobalMatrixInverse)
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
        if (GetFirstCollision(collider, Matrix3x2.CreateTranslation(velocityScaled) * position).Collided) {
            velocityScaled = Vector2.Zero;
            velocityScaledClamped = Vector2.Zero;
        }

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

    SweptCollisionComponentInfo GetSweptComponentInfo(ColliderContextPair ctx, CollisionComponent component) {
        SweepColliderPair[] collisionPairs = ColliderIntersectionFraction(ctx);

        if (collisionPairs.Length == 0) { return new(component, Vector2.One, Vector2.One, 0, []); }

        SweptColliderInfo[] colliders = new SweptColliderInfo[collisionPairs.Length];
        int closest = 0;
        for (int i = 0; i < colliders.Length; ++i) {
            SweepColliderPair pair = collisionPairs[i];

            Vector2 fraction = pair.Fraction;
            // Apply small pushback to stop objects getting stuck inside one another
            if (ctx.A.Velocity.X != 0) { fraction.X -= SWEEP_TOLERANCE + float.Epsilon; }
            if (ctx.A.Velocity.Y != 0) { fraction.Y -= SWEEP_TOLERANCE + float.Epsilon; }
            Vector2 fractionClamped = IsFractionNegative(fraction) ? Vector2.Zero : fraction;

            SweptColliderInfo info = new(pair.B, fraction, fractionClamped, pair.Normal);
            if (IsRayFractionSmaller(info.Fraction, collisionPairs[closest].Fraction)) {
                closest = i;
            }
            colliders[i] = info;
        }

        return new(component, colliders[closest].Fraction, colliders[closest].FractionClamped, closest, colliders);
    }

    SweptCollisionInfo SweptInfoFromAllCollisionInfo(AllCollisionInfo info) {
        return new(
            info.Collided,
            Vector2.Zero,
            Vector2.Zero,
            Vector2.Zero,
            Vector2.Zero,
            [.. info.ColliderInfo.Select(component => new SweptCollisionComponentInfo(
                component.Component,
                Vector2.Zero,
                Vector2.Zero,
                0,
                [.. component.Colliders.Select(collider => new SweptColliderInfo(collider, Vector2.Zero, Vector2.Zero, Vector2.UnitY))]
            ))]
        );
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
        => GetCollisionPushout(collider, collider.Actor.Transform.GlobalMatrix, collider.Actor.Transform.GlobalMatrixInverse);

    /// <summary>
    /// Get the distance required to push the collision component out of geometry.
    /// Note: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider.
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
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
    /// <param name="position">The transform to apply to the collider.</param>
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
    /// <param name="position">The transform to apply to the collider.</param>
    /// <returns>Information about collision pushout results.</returns>
    public PushoutCollisionInfo GetCollisionPushout(CollisionComponent collider, Vector2 position)
        => GetCollisionPushout(collider, Matrix3x2.CreateTranslation(position));

    /// <summary>
    /// Get the distance required to push the collision component out of geometry.
    /// Note: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider.
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <returns>Information about collision pushout results.</returns>
    public PushoutCollisionInfo GetCollisionPushout(CollisionComponent collider, Matrix3x2 position)
        => GetCollisionPushout(collider, position, position.Invert());

    /// <summary>
    /// Get the distance required to push the collision component out of geometry.
    /// Note: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider.
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="positionInv">A pre-calculated inverted matrix for the position.</param>
    /// <returns>Information about collision pushout results.</returns>
    public PushoutCollisionInfo GetCollisionPushout(CollisionComponent collider, Matrix3x2 position, Matrix3x2 positionInv) {
        ActiveComponent = collider;
        PushoutCollisionInfo info = GetCollisionPushout(collider.Collider, position, positionInv);
        ActiveComponent = null;
        return info;
    }

    /// <summary>
    /// Get the distance required to push the collider out of geometry.
    /// Note: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider.
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <returns>Information about collision pushout results.</returns>
    public PushoutCollisionInfo GetCollisionPushout(Collider collider, ActorPosition position)
        => GetCollisionPushout(collider, position.GlobalMatrix, position.GlobalMatrixInverse);

    /// <summary>
    /// Get the distance required to push the collider out of geometry.
    /// Note: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider.
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <returns>Information about collision pushout results.</returns>
    public PushoutCollisionInfo GetCollisionPushout(Collider collider, Transform position)
        => GetCollisionPushout(collider, position.Matrix, position.MatrixInverse);

    /// <summary>
    /// Get the distance required to push the collider out of geometry.
    /// Note: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider.
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <returns>Information about collision pushout results.</returns>
    public PushoutCollisionInfo GetCollisionPushout(Collider collider, Vector2 position)
        => GetCollisionPushout(collider, Matrix3x2.CreateTranslation(position));

    /// <summary>
    /// Get the distance required to push the collider out of geometry.
    /// Note: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider.
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <returns>Information about collision pushout results.</returns>
    public PushoutCollisionInfo GetCollisionPushout(Collider collider, Matrix3x2 position)
        => GetCollisionPushout(collider, position, position.Invert());

    /// <summary>
    /// Get the distance required to push the collider out of geometry.
    /// Note: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider.
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="positionInv">A pre-calculated inverted matrix for the position.</param>
    /// <returns>Information about collision pushout results.</returns>
    public PushoutCollisionInfo GetCollisionPushout(Collider collider, Matrix3x2 position, Matrix3x2 positionInv) {
        Vector2 smallestPushout = Vector2.Zero;
        float smallestLength = float.PositiveInfinity;

        List<PushoutCollisionComponentInfo> componentInfo = [];
        foreach (CollisionComponent other in Colliders) {
            if (ShouldSkipCollider(other)) { continue; }
            PushoutCollisionComponentInfo result = GetPushoutCollisionComponentInfo(collider, position, positionInv, other);
            if (result.Colliders.Length == 0) { continue; }

            float compareLength = result.Colliders[result.LargestPushout].Pushout.LengthSquared();
            if (compareLength < smallestLength) {
                smallestPushout = result.Colliders[result.LargestPushout].Pushout;
                smallestLength = compareLength;
            }
            componentInfo.Add(result);
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
        => GetCollisionPushoutInDirection(collider, collider.Actor.Transform.GlobalMatrix, direction);

    /// <summary>
    /// Get the distance required to push the collision component out of geometry in a given direction.
    /// Note: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider.
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="direction">The direction to push the collider in. The resulting vector will have the same angle with an adjusted length.</param>
    /// <returns>Information about collision pushout results.</returns>
    public PushoutCollisionInfo GetCollisionPushoutInDirection(CollisionComponent collider, ActorPosition position, Vector2 direction)
        => GetCollisionPushoutInDirection(collider, position.GlobalMatrix, position.GlobalMatrixInverse, direction);

    /// <summary>
    /// Get the distance required to push the collision component out of geometry in a given direction.
    /// Note: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider.
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="direction">The direction to push the collider in. The resulting vector will have the same angle with an adjusted length.</param>
    /// <returns>Information about collision pushout results.</returns>
    public PushoutCollisionInfo GetCollisionPushoutInDirection(CollisionComponent collider, Transform position, Vector2 direction)
        => GetCollisionPushoutInDirection(collider, position.Matrix, position.MatrixInverse, direction);

    /// <summary>
    /// Get the distance required to push the collision component out of geometry in a given direction.
    /// Note: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider.
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="direction">The direction to push the collider in. The resulting vector will have the same angle with an adjusted length.</param>
    /// <returns>Information about collision pushout results.</returns>
    public PushoutCollisionInfo GetCollisionPushoutInDirection(CollisionComponent collider, Vector2 position, Vector2 direction)
        => GetCollisionPushoutInDirection(collider, Matrix3x2.CreateTranslation(position), direction);

    /// <summary>
    /// Get the distance required to push the collision component out of geometry in a given direction.
    /// Note: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider.
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="direction">The direction to push the collider in. The resulting vector will have the same angle with an adjusted length.</param>
    /// <returns>Information about collision pushout results.</returns>
    public PushoutCollisionInfo GetCollisionPushoutInDirection(CollisionComponent collider, Matrix3x2 position, Vector2 direction)
        => GetCollisionPushoutInDirection(collider, position, position.Invert(), direction);

    /// <summary>
    /// Get the distance required to push the collision component out of geometry in a given direction.
    /// Note: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider.
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="positionInv">A pre-calculated inverted matrix for the position.</param>
    /// <param name="direction">The direction to push the collider in. The resulting vector will have the same angle with an adjusted length.</param>
    /// <returns>Information about collision pushout results.</returns>
    public PushoutCollisionInfo GetCollisionPushoutInDirection(CollisionComponent collider, Matrix3x2 position, Matrix3x2 positionInv, Vector2 direction) {
        if (direction == Vector2.Zero) { return GetCollisionPushout(collider, position, positionInv); }
        return SweptToPushoutInfo(GetSweptCollision(collider, position, positionInv, -direction), direction);
    }

    /// <summary>
    /// Get the distance required to push the collision component out of geometry in a given direction.
    /// Note: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider.
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="direction">The direction to push the collider in. The resulting vector will have the same angle with an adjusted length.</param>
    /// <returns>Information about collision pushout results.</returns>
    public PushoutCollisionInfo GetCollisionPushoutInDirection(Collider collider, ActorPosition position, Vector2 direction)
        => GetCollisionPushoutInDirection(collider, position.GlobalMatrix, position.GlobalMatrixInverse, direction);

    /// <summary>
    /// Get the distance required to push the collision component out of geometry in a given direction.
    /// Note: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider.
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="direction">The direction to push the collider in. The resulting vector will have the same angle with an adjusted length.</param>
    /// <returns>Information about collision pushout results.</returns>
    public PushoutCollisionInfo GetCollisionPushoutInDirection(Collider collider, Transform position, Vector2 direction)
        => GetCollisionPushoutInDirection(collider, position.Matrix, position.MatrixInverse, direction);

    /// <summary>
    /// Get the distance required to push the collision component out of geometry in a given direction.
    /// Note: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider.
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="direction">The direction to push the collider in. The resulting vector will have the same angle with an adjusted length.</param>
    /// <returns>Information about collision pushout results.</returns>
    public PushoutCollisionInfo GetCollisionPushoutInDirection(Collider collider, Vector2 position, Vector2 direction)
        => GetCollisionPushoutInDirection(collider, Matrix3x2.CreateTranslation(position), direction);

    /// <summary>
    /// Get the distance required to push the collision component out of geometry in a given direction.
    /// Note: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider.
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="direction">The direction to push the collider in. The resulting vector will have the same angle with an adjusted length.</param>
    /// <returns>Information about collision pushout results.</returns>
    public PushoutCollisionInfo GetCollisionPushoutInDirection(Collider collider, Matrix3x2 position, Vector2 direction)
        => GetCollisionPushoutInDirection(collider, position, position.Invert(), direction);

    /// <summary>
    /// Get the distance required to push the collision component out of geometry in a given direction.
    /// Note: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider.
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="positionInv">A pre-calculated inverted matrix for the position.</param>
    /// <param name="direction">The direction to push the collider in. The resulting vector will have the same angle with an adjusted length.</param>
    /// <returns>Information about collision pushout results.</returns>
    public PushoutCollisionInfo GetCollisionPushoutInDirection(Collider collider, Matrix3x2 position, Matrix3x2 positionInv, Vector2 direction) {
        if (direction == Vector2.Zero) { return GetCollisionPushout(collider, position); }
        return SweptToPushoutInfo(GetSweptCollision(collider, position, positionInv, -direction), direction);
    }

    static PushoutCollisionComponentInfo GetPushoutCollisionComponentInfo(
        Collider collider,
        Matrix3x2 position,
        Matrix3x2 positionInv,
        CollisionComponent other
    ) {
        PushoutColliderPair[] collisionPairs = ColliderPushout(new(
            new(collider, position, positionInv),
            new(other.Collider, other.Actor.Transform.GlobalMatrix, other.Actor.Transform.GlobalMatrixInverse)
        ));
        if (collisionPairs.Length == 0) { return new(other, 0, []); }

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
    static SimplexColliderPair[] ColliderOverlapCheck(ColliderContextPair ctx, bool reversed = false) {
        if (!ctx.TagMatch(reversed)) { return []; }

        Rect boundsAOffset = ctx.A.Collider.Bounds.TransformAABB(ctx.A.Position);
        Rect boundsBOffset = ctx.B.Collider.Bounds.TransformAABB(ctx.B.Position);
        if (!boundsAOffset.Overlaps(boundsBOffset)) {
            return [];
        }

        if (ctx.A.Collider.Multi) {
            Collider[] subs = ctx.A.Collider.GetSubColliders(ctx.B.Collider.Bounds.TransformAABB(ctx.B.Position * ctx.A.PositionInv));
            List<SimplexColliderPair> pairs = [];
            foreach (Collider collider in subs) {
                SimplexColliderPair[] subCollisions = ColliderOverlapCheck(ctx.WithA(collider), reversed);
                if (subCollisions.Length > 0) { pairs.AddRange(subCollisions); }
            }
            return [.. pairs];
        }

        if (ctx.B.Collider.Multi) { return ColliderOverlapCheck(ctx.Flip(), !reversed); }

        if (new GJKSimplex(ctx).Collided) {
            return [reversed ? new(ctx.B.Collider, ctx.A.Collider) : new(ctx.A.Collider, ctx.B.Collider)];
        }
        return [];
    }

    static PushoutColliderPair[] ColliderPushout(ColliderContextPair ctx, bool reversed = false) {
        if (!ctx.TagMatch(reversed)) { return []; }

        Rect boundsAOffset = ctx.A.Collider.Bounds.TransformAABB(ctx.A.Position);
        Rect boundsBOffset = ctx.B.Collider.Bounds.TransformAABB(ctx.B.Position);
        if (!boundsAOffset.Overlaps(boundsBOffset)) { return []; }

        if (ctx.A.Collider.Multi) {
            Collider[] subs = ctx.A.Collider.GetSubColliders(ctx.B.Collider.Bounds.TransformAABB(ctx.B.Position * ctx.A.PositionInv));
            List<PushoutColliderPair> pairs = [];
            foreach (Collider collider in subs) {
                PushoutColliderPair[] subCollisions = ColliderPushout(ctx.WithA(collider), reversed);
                if (subCollisions.Length > 0) { pairs.AddRange(subCollisions); }
            }
            return [.. pairs];
        }

        if (ctx.B.Collider.Multi) { return ColliderPushout(ctx.Flip(), !reversed); }

        EPAPushout info = new(ctx);
        if (!info.Collided) { return []; }
        return [reversed ? new(ctx.B.Collider, ctx.A.Collider, -info.Pushout) : new(ctx.A.Collider, ctx.B.Collider, info.Pushout)];
    }

    static SweepColliderPair[] ColliderIntersectionFraction(ColliderContextPair ctx, bool reversed = false) {
        if (!ctx.TagMatch(reversed)) { return []; }

        Rect boundAOffset = ctx.A.Collider.Bounds.TransformAABB(ctx.A.Position);
        Rect sweptBoundsB = SweptBounds(ctx.B.Collider, ctx.B.Velocity);
        Rect sweptBoundsBCombined = SweptBounds(sweptBoundsB, -ctx.A.Velocity);
        Rect sweptBoundsBCombinedOffset = sweptBoundsBCombined.TransformAABB(ctx.B.Position);
        if (!sweptBoundsBCombinedOffset.Overlaps(boundAOffset)) { return []; }

        if (ctx.A.Collider.Multi) {
            List<SweepColliderPair> pairs = [];
            foreach (Collider collider in ctx.A.Collider.GetSubColliders(sweptBoundsB.TransformAABB(ctx.B.Position * ctx.A.PositionInv))) {
                SweepColliderPair[] subCollisions = ColliderIntersectionFraction(ctx.WithA(collider), reversed);
                if (subCollisions.Length > 0) { pairs.AddRange(subCollisions); }
            }
            return [.. pairs];
        }

        if (ctx.B.Collider.Multi) { return ColliderIntersectionFraction(ctx.Flip(), !reversed); }

        CollisionSweepInfo collision = GetRayIntersectionFraction(ctx);
        // Sweep calculation sometimes gives results for colliders that aren't initially colliding
        if (!collision.Collided || (IsFractionNegative(collision.Fraction) && ColliderOverlapCheck(ctx).Length == 0)) { return []; }
        return [reversed ? new(ctx.B.Collider, ctx.A.Collider, collision.Fraction, collision.Normal) : new(ctx.A.Collider, ctx.B.Collider, collision.Fraction, collision.Normal)];
    }
    #endregion

    #region Primary Collision Functions

    static CollisionSweepInfo GetRayIntersectionFraction(ColliderContextPair ctx) {
        // Get velocity difference
        Vector2 velocityDifference = ctx.B.Velocity - ctx.A.Velocity;

        // Cancel if objects are moving the same direction or stationary
        if (velocityDifference == Vector2.Zero) { return new(Vector2.One, Vector2.UnitY, false); }

        float velocityLength = velocityDifference.LengthSquared();

        // Point A
        Vector2 direction = -velocityDifference;
        Vector2 pointA = ctx.Support(direction);

        // Point B
        direction = Calc.TripleProduct(direction, -pointA, direction);
        Vector2 pointB = ctx.Support(direction);

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
            Vector2 pointC = ctx.Support(direction);

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

    static bool IsFractionNegative(Vector2 fraction) {
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
        if (!new Line(Vector2.Zero, velocity).IntersectionLineSegment(new(pointA, pointB), out Vector2 intersection)) {
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
        Vector2 direction = Calc.TripleProduct(line, pointA, line);
        if (Vector2.Dot(direction, facingDirection) < 0) { direction = -direction; }
        return direction;
    }

    static Rect SweptBounds(Collider collider, Vector2 velocity)
        => SweptBounds(collider.Bounds, velocity);

    static Rect SweptBounds(Rect rect, Vector2 velocity) {
        Vector2 min = Vector2.Min(rect.TopLeft, rect.TopLeft + velocity);
        Vector2 max = Vector2.Max(rect.BottomRight, rect.BottomRight + velocity);
        return new(min, max - min);
    }

    bool ShouldSkipCollider(CollisionComponent component) =>
        !component.Ticking ||
        !component.Actor.GlobalComponentsTicking ||
        (ActiveComponent != null && ComponentMatch(component, ActiveComponent));

    static bool ComponentMatch(CollisionComponent a, CollisionComponent b)
        => a == b || ActorMatch(a, b);

    static bool ActorMatch(CollisionComponent a, CollisionComponent b)
        => a.ActorValid && b.ActorValid && a.Actor == b.Actor;

    #endregion
}