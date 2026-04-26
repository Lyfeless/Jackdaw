using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Controller for all collision interactions in the game.
/// </summary>
public class CollisionManager {
    #region Definitions
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
    public CollisionResult GetAllCollisions(CollisionComponent collider)
        => GetAllCollisions(collider, collider.Actor.Transform.GlobalMatrix, collider.Actor.Transform.GlobalMatrixInverse);

    /// <summary>
    /// Get all collisions between given collider component and all registered collision components at a given location.
    /// </summary>
    /// <param name="collider">The collider component to check against.</param>
    /// <param name="position">The actor position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetAllCollisions(CollisionComponent collider, ActorPosition position)
        => GetAllCollisions(collider, position.GlobalMatrix, position.GlobalMatrixInverse);

    /// <summary>
    /// Get all collisions between given collider component and all registered collision components at a given location.
    /// </summary>
    /// <param name="collider">The collider component to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetAllCollisions(CollisionComponent collider, Transform position)
        => GetAllCollisions(collider, position.Matrix, position.MatrixInverse);

    /// <summary>
    /// Get all collisions between given collider component and all registered collision components at a given location.
    /// </summary>
    /// <param name="collider">The collider component to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetAllCollisions(CollisionComponent collider, Vector2 position)
        => GetAllCollisions(collider, Matrix3x2.CreateTranslation(position));

    /// <summary>
    /// Get all collisions between given collider component and all registered collision components at a given location.
    /// </summary>
    /// <param name="collider">The collider component to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetAllCollisions(CollisionComponent collider, Matrix3x2 position)
        => GetAllCollisions(collider, new InvertableMatrix(position));

    /// <summary>
    /// Get all collisions between given collider component and all registered collision components at a given location.
    /// </summary>
    /// <param name="collider">The collider component to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <param name="positionInv">A pre-calculated inverted matrix for the position.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetAllCollisions(CollisionComponent collider, Matrix3x2 position, Matrix3x2 positionInv)
        => GetAllCollisions(collider, new InvertableMatrix(position, positionInv));

    /// <summary>
    /// Get all collisions between given collider component and all registered collision components at a given location.
    /// </summary>
    /// <param name="collider">The collider component to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetAllCollisions(CollisionComponent collider, InvertableMatrix position) {
        ActiveComponent = collider;
        CollisionResult info = GetAllCollisions(collider.Collider, position);
        ActiveComponent = null;
        return info;
    }

    /// <summary>
    /// Get all collisions between given collider and all registered collision components at a given location.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The actor position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetAllCollisions(Collider collider, ActorPosition position)
        => GetAllCollisions(collider, position.GlobalMatrix, position.GlobalMatrixInverse);

    /// <summary>
    /// Get all collisions between given collider and all registered collision components at a given location.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetAllCollisions(Collider collider, Transform position)
        => GetAllCollisions(collider, position.Matrix, position.MatrixInverse);

    /// <summary>
    /// Get all collisions between given collider and all registered collision components at a given location.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetAllCollisions(Collider collider, Vector2 position)
        => GetAllCollisions(collider, Matrix3x2.CreateTranslation(position));

    /// <summary>
    /// Get all collisions between given collider and all registered collision components at a given location.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetAllCollisions(Collider collider, Matrix3x2 position)
        => GetAllCollisions(collider, new InvertableMatrix(position));

    /// <summary>
    /// Get all collisions between given collider and all registered collision components at a given location.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <param name="positionInv">A pre-calculated inverted matrix for the position.</param>
    /// <returns></returns>
    public CollisionResult GetAllCollisions(Collider collider, Matrix3x2 position, Matrix3x2 positionInv)
        => GetAllCollisions(collider, new InvertableMatrix(position, positionInv));

    /// <summary>
    /// Get all collisions between given collider and all registered collision components at a given location.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetAllCollisions(Collider collider, InvertableMatrix position) {
        List<CollisionResult.ComponentResult> components = [];

        foreach (CollisionComponent other in Colliders) {
            if (ShouldSkipCollider(other)) { continue; }

            ColliderContextPair ctx = new(
                new(collider, position),
                new(other)
            );

            List<ColliderContextPair> pairs = ctx.GetOverlappingPairs();
            if (pairs.Count == 0) { continue; }

            CollisionResult.ColliderResult[] results = [..pairs
                .Where(e => new GJKSimplex(e).Collided)
                .Select(e => new CollisionResult.ColliderResult(e.B))];

            if (results.Length == 0) { continue; }

            components.Add(new(other, results));
        }

        return new([.. components]);
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
    public CollisionResult GetFirstCollision(CollisionComponent collider)
        => GetFirstCollision(collider, collider.Actor.Transform.GlobalMatrix, collider.Actor.Transform.GlobalMatrixInverse);

    /// <summary>
    /// Get the first object the given collider component collides with at a given location.
    /// Not guarenteed to be the closest, used mostly for performance when full collision information isn't needed.
    /// </summary>
    /// <param name="collider">The collider component to check against.</param>
    /// <param name="position">The actor position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetFirstCollision(CollisionComponent collider, ActorPosition position)
        => GetFirstCollision(collider, position.GlobalMatrix, position.GlobalMatrixInverse);

    /// <summary>
    /// Get the first object the given collider component collides with at a given location.
    /// Not guarenteed to be the closest, used mostly for performance when full collision information isn't needed.
    /// </summary>
    /// <param name="collider">The collider component to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetFirstCollision(CollisionComponent collider, Transform position)
        => GetFirstCollision(collider, position.Matrix, position.MatrixInverse);

    /// <summary>
    /// Get the first object the given collider component collides with at a given location.
    /// Not guarenteed to be the closest, used mostly for performance when full collision information isn't needed.
    /// </summary>
    /// <param name="collider">The collider component to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetFirstCollision(CollisionComponent collider, Vector2 position)
        => GetFirstCollision(collider, Matrix3x2.CreateTranslation(position));

    /// <summary>
    /// Get the first object the given collider component collides with at a given location.
    /// Not guarenteed to be the closest, used mostly for performance when full collision information isn't needed.
    /// </summary>
    /// <param name="collider">The collider component to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetFirstCollision(CollisionComponent collider, Matrix3x2 position)
        => GetFirstCollision(collider, new InvertableMatrix(position));

    /// <summary>
    /// Get the first object the given collider component collides with at a given location.
    /// Not guarenteed to be the closest, used mostly for performance when full collision information isn't needed.
    /// </summary>
    /// <param name="collider">The collider component to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <param name="positionInv">A pre-calculated inverted matrix for the position.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetFirstCollision(CollisionComponent collider, Matrix3x2 position, Matrix3x2 positionInv)
        => GetFirstCollision(collider, new InvertableMatrix(position, positionInv));

    /// <summary>
    /// Get the first object the given collider component collides with at a given location.
    /// Not guarenteed to be the closest, used mostly for performance when full collision information isn't needed.
    /// </summary>
    /// <param name="collider">The collider component to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetFirstCollision(CollisionComponent collider, InvertableMatrix position) {
        ActiveComponent = collider;
        CollisionResult info = GetFirstCollision(collider.Collider, position);
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
    public CollisionResult GetFirstCollision(Collider collider, ActorPosition position)
        => GetFirstCollision(collider, position.GlobalMatrix, position.GlobalMatrixInverse);

    /// <summary>
    /// Get the first object the given collider collides with at a given location.
    /// Not guarenteed to be the closest, used mostly for performance when full collision information isn't needed.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetFirstCollision(Collider collider, Transform position)
        => GetFirstCollision(collider, position.Matrix, position.MatrixInverse);

    /// <summary>
    /// Get the first object the given collider collides with at a given location.
    /// Not guarenteed to be the closest, used mostly for performance when full collision information isn't needed.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetFirstCollision(Collider collider, Vector2 position)
        => GetFirstCollision(collider, Matrix3x2.CreateTranslation(position));

    /// <summary>
    /// Get the first object the given collider collides with at a given location.
    /// Not guarenteed to be the closest, used mostly for performance when full collision information isn't needed.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetFirstCollision(Collider collider, Matrix3x2 position)
        => GetFirstCollision(collider, new InvertableMatrix(position));

    /// <summary>
    /// Get the first object the given collider collides with at a given location.
    /// Not guarenteed to be the closest, used mostly for performance when full collision information isn't needed.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <param name="positionInv">A pre-calculated inverted matrix for the position.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetFirstCollision(Collider collider, Matrix3x2 position, Matrix3x2 positionInv)
        => GetFirstCollision(collider, new InvertableMatrix(position, positionInv));

    /// <summary>
    /// Get the first object the given collider collides with at a given location.
    /// Not guarenteed to be the closest, used mostly for performance when full collision information isn't needed.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The global position the collisions should be checked from.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetFirstCollision(Collider collider, InvertableMatrix position) {
        foreach (CollisionComponent other in Colliders) {
            if (ShouldSkipCollider(other)) { continue; }

            ColliderContextPair ctx = new(
                new(collider, position),
                new(other)
            );

            List<ColliderContextPair> pairs = ctx.GetOverlappingPairs();
            if (pairs.Count == 0) { continue; }

            CollisionResult.ColliderResult[] results = [.. pairs
                .Where(e => new GJKSimplex(e).Collided)
                .Select(e => new CollisionResult.ColliderResult(e.B))
            ];
            if (results.Length == 0) { continue; }

            return new([new(other, results)]);
        }

        return new();
    }
    #endregion

    #region Manual Raycast Collisions Checks
    /// <summary>
    /// Get all collision data from a ray.
    /// </summary>
    /// <param name="component">The raycast component to check against.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetRayCollision(RaycastComponent component)
        => GetRayCollision(component, component.Actor.Transform);

    /// <summary>
    /// Get all collision data from a ray.
    /// </summary>
    /// <param name="component">The raycast component to check against.</param>
    /// <param name="position">The transform to apply to the ray.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetRayCollision(RaycastComponent component, ActorPosition position)
        => GetRayCollision(component, position.LocalPosition);

    /// <summary>
    /// Get all collision data from a ray.
    /// </summary>
    /// <param name="component">The raycast component to check against.</param>
    /// <param name="position">The transform to apply to the ray.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetRayCollision(RaycastComponent component, Transform position)
        => GetRayCollision(component, position.Position);

    /// <summary>
    /// Get all collision data from a ray.
    /// </summary>
    /// <param name="component">The raycast component to check against.</param>
    /// <param name="position">The transform to apply to the ray.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetRayCollision(RaycastComponent component, Vector2 position)
        => GetRayCollision(component.Ray.Position + position, component.Ray.Direction, component.Mask);

    /// <summary>
    /// Get all collision data from a ray.
    /// </summary>
    /// <param name="ray">The ray to check against.</param>
    /// <param name="tagMask">The collider tags the ray should collide with.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetRayCollision(Ray ray, TagContainer tagMask)
        => GetRayCollision(ray.Position, ray.Direction, tagMask);

    /// <summary>
    /// Get all collision data from a ray.
    /// </summary>
    /// <param name="position">The position to start the ray from</param>
    /// <param name="direction">The direction of the ray, with the length of the direction determining the length of the ray.</param>
    /// <param name="tagMask">The collider tags the ray should collide with.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetRayCollision(Vector2 position, Vector2 direction, TagContainer tagMask)
        => GetSweptCollision(new PointCollider(position) { Mask = tagMask }, Matrix3x2.Identity, direction);
    #endregion

    #region Manual Swept Collisions Checks
    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetSweptCollision(CollisionComponent collider, Vector2 velocity)
        => GetSweptCollision(collider, collider.Actor.Transform.GlobalMatrix, collider.Actor.Transform.GlobalMatrixInverse, velocity);

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetSweptCollision(CollisionComponent collider, ActorPosition position, Vector2 velocity)
        => GetSweptCollision(collider, position.GlobalMatrix, position.GlobalMatrixInverse, velocity);

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetSweptCollision(CollisionComponent collider, Transform position, Vector2 velocity)
        => GetSweptCollision(collider, position.Matrix, position.MatrixInverse, velocity);

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetSweptCollision(CollisionComponent collider, Vector2 position, Vector2 velocity)
        => GetSweptCollision(collider, Matrix3x2.CreateTranslation(position), velocity);

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetSweptCollision(CollisionComponent collider, Matrix3x2 position, Vector2 velocity)
        => GetSweptCollision(collider, new InvertableMatrix(position), velocity);

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="positionInv">A pre-calculated inverted matrix for the position.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetSweptCollision(CollisionComponent collider, Matrix3x2 position, Matrix3x2 positionInv, Vector2 velocity)
        => GetSweptCollision(collider, new InvertableMatrix(position, positionInv), velocity);

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetSweptCollision(CollisionComponent collider, InvertableMatrix position, Vector2 velocity) {
        ActiveComponent = collider;
        CollisionResult info = GetSweptCollision(collider.Collider, position, velocity);
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
    public CollisionResult GetSweptCollision(Collider collider, ActorPosition position, Vector2 velocity)
        => GetSweptCollision(collider, position.GlobalMatrix, position.GlobalMatrixInverse, velocity);

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetSweptCollision(Collider collider, Transform position, Vector2 velocity)
        => GetSweptCollision(collider, position.Matrix, position.MatrixInverse, velocity);

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetSweptCollision(Collider collider, Vector2 position, Vector2 velocity)
        => GetSweptCollision(collider, Matrix3x2.CreateTranslation(position), velocity);

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetSweptCollision(Collider collider, Matrix3x2 position, Vector2 velocity)
        => GetSweptCollision(collider, new InvertableMatrix(position), velocity);

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="positionInv">A pre-calculated inverted matrix for the position.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetSweptCollision(Collider collider, Matrix3x2 position, Matrix3x2 positionInv, Vector2 velocity)
        => GetSweptCollision(collider, new InvertableMatrix(position, positionInv), velocity);

    /// <summary>
    /// Get collision results for a shapecast.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="velocity">The collider's velocity.</param>
    /// <returns>Information about collision check results.</returns>
    public CollisionResult GetSweptCollision(Collider collider, InvertableMatrix position, Vector2 velocity) {
        // If object isn't moving run basic collision check to avoid extra calculations
        if (velocity == Vector2.Zero) { return GetAllCollisions(collider, position); }

        List<CollisionResult.ComponentResult> components = [];

        foreach (CollisionComponent other in Colliders) {
            if (ShouldSkipCollider(other)) { continue; }

            ColliderContextPair ctx = new(
                new(collider, position, velocity),
                new(other)
            );

            List<ColliderContextPair> pairs = ctx.GetOverlappingPairs();
            if (pairs.Count == 0) { continue; }

            List<CollisionResult.ColliderResult> colliders = [];

            foreach (ColliderContextPair pair in pairs) {
                JDASweep sweep = new(pair);
                if (!sweep.Collided) { continue; }
                colliders.Add(new(pair.B, sweep));
            }

            if (colliders.Count == 0) { continue; }

            components.Add(new(other, [.. colliders.OrderBy(e => e.Sweep.Fraction)]));
        }

        if (components.Count == 0) { return new(new CollisionResult.SweepResult(false, 1, Vector2.UnitY, velocity)); }
        return new([.. components.OrderBy(e => e.Sweep.Fraction)]);
    }
    #endregion

    #region Manual Pushout Checks
    /// <summary>
    /// Get the distance required to push the collision component out of geometry. <br/>
    /// NOTE: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider. <br/>
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <returns>Information about collision pushout results.</returns>
    public CollisionResult GetCollisionPushout(CollisionComponent collider)
        => GetCollisionPushout(collider, collider.Actor.Transform.GlobalMatrix, collider.Actor.Transform.GlobalMatrixInverse);

    /// <summary>
    /// Get the distance required to push the collision component out of geometry. <br/>
    /// NOTE: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider. <br/>
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <returns>Information about collision pushout results.</returns>
    public CollisionResult GetCollisionPushout(CollisionComponent collider, ActorPosition position)
        => GetCollisionPushout(collider, position.GlobalMatrix, position.GlobalMatrixInverse);

    /// <summary>
    /// Get the distance required to push the collision component out of geometry. <br/>
    /// NOTE: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider. <br/>
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <returns>Information about collision pushout results.</returns>
    public CollisionResult GetCollisionPushout(CollisionComponent collider, Transform position)
        => GetCollisionPushout(collider, position.Matrix, position.MatrixInverse);

    /// <summary>
    /// Get the distance required to push the collision component out of geometry. <br/>
    /// NOTE: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider. <br/>
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <returns>Information about collision pushout results.</returns>
    public CollisionResult GetCollisionPushout(CollisionComponent collider, Vector2 position)
        => GetCollisionPushout(collider, Matrix3x2.CreateTranslation(position));

    /// <summary>
    /// Get the distance required to push the collision component out of geometry. <br/>
    /// NOTE: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider. <br/>
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <returns>Information about collision pushout results.</returns>
    public CollisionResult GetCollisionPushout(CollisionComponent collider, Matrix3x2 position)
        => GetCollisionPushout(collider, new InvertableMatrix(position));

    /// <summary>
    /// Get the distance required to push the collision component out of geometry. <br/>
    /// NOTE: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider. <br/>
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="positionInv">A pre-calculated inverted matrix for the position.</param>
    /// <returns>Information about collision pushout results.</returns>
    public CollisionResult GetCollisionPushout(CollisionComponent collider, Matrix3x2 position, Matrix3x2 positionInv)
        => GetCollisionPushout(collider, new InvertableMatrix(position, positionInv));

    /// <summary>
    /// Get the distance required to push the collision component out of geometry. <br/>
    /// NOTE: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider. <br/>
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <returns>Information about collision pushout results.</returns>
    public CollisionResult GetCollisionPushout(CollisionComponent collider, InvertableMatrix position) {
        ActiveComponent = collider;
        CollisionResult info = GetCollisionPushout(collider.Collider, position);
        ActiveComponent = null;
        return info;
    }

    /// <summary>
    /// Get the distance required to push the collider out of geometry. <br/>
    /// NOTE: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider. <br/>
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <returns>Information about collision pushout results.</returns>
    public CollisionResult GetCollisionPushout(Collider collider, ActorPosition position)
        => GetCollisionPushout(collider, position.GlobalMatrix, position.GlobalMatrixInverse);

    /// <summary>
    /// Get the distance required to push the collider out of geometry. <br/>
    /// NOTE: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider. <br/>
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <returns>Information about collision pushout results.</returns>
    public CollisionResult GetCollisionPushout(Collider collider, Transform position)
        => GetCollisionPushout(collider, position.Matrix, position.MatrixInverse);

    /// <summary>
    /// Get the distance required to push the collider out of geometry. <br/>
    /// NOTE: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider. <br/>
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <returns>Information about collision pushout results.</returns>
    public CollisionResult GetCollisionPushout(Collider collider, Vector2 position)
        => GetCollisionPushout(collider, Matrix3x2.CreateTranslation(position));

    /// <summary>
    /// Get the distance required to push the collider out of geometry. <br/>
    /// NOTE: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider. <br/>
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <returns>Information about collision pushout results.</returns>
    public CollisionResult GetCollisionPushout(Collider collider, Matrix3x2 position)
        => GetCollisionPushout(collider, new InvertableMatrix(position));

    /// <summary>
    /// Get the distance required to push the collider out of geometry. <br/>
    /// NOTE: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider. <br/>
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="positionInv">A pre-calculated inverted matrix for the position.</param>
    /// <returns>Information about collision pushout results.</returns>
    public CollisionResult GetCollisionPushout(Collider collider, Matrix3x2 position, Matrix3x2 positionInv)
        => GetCollisionPushout(collider, new InvertableMatrix(position, positionInv));

    /// <summary>
    /// Get the distance required to push the collider out of geometry. <br/>
    /// NOTE: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider. <br/>
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collider to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <returns>Information about collision pushout results.</returns>
    public CollisionResult GetCollisionPushout(Collider collider, InvertableMatrix position) {
        List<CollisionResult.ComponentResult> components = [];

        foreach (CollisionComponent other in Colliders) {
            if (ShouldSkipCollider(other)) { continue; }

            ColliderContextPair ctx = new(
                new(collider, position),
                new(other)
            );

            List<ColliderContextPair> pairs = ctx.GetOverlappingPairs();
            if (pairs.Count == 0) { continue; }

            List<CollisionResult.ColliderResult> colliders = [];

            foreach (ColliderContextPair pair in pairs) {
                EPAPushout pushout = new(pair);
                if (!pushout.Collided) { continue; }
                colliders.Add(new(pair.B, pushout));
            }

            if (Colliders.Count == 0) { continue; }

            components.Add(new(other, [.. colliders.OrderByDescending(e => e.Pushout.LengthSquared())]));
        }

        return new([.. components.OrderByDescending(e => e.Pushout.LengthSquared())]);
    }

    /// <summary>
    /// Get the distance required to push the collision component out of geometry in a given direction. <br/>
    /// NOTE: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider. <br/>
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="direction">The direction to push the collider in. The resulting vector will have the same angle with an adjusted length.</param>
    /// <returns>Information about collision pushout results.</returns>
    public CollisionResult GetCollisionPushoutInDirection(CollisionComponent collider, Vector2 direction)
        => GetSweptCollision(collider, collider.Actor.Transform.GlobalMatrix, -direction);

    /// <summary>
    /// Get the distance required to push the collision component out of geometry in a given direction. <br/>
    /// NOTE: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider. <br/>
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="direction">The direction to push the collider in. The resulting vector will have the same angle with an adjusted length.</param>
    /// <returns>Information about collision pushout results.</returns>
    public CollisionResult GetCollisionPushoutInDirection(CollisionComponent collider, ActorPosition position, Vector2 direction)
        => GetSweptCollision(collider, position.GlobalMatrix, position.GlobalMatrixInverse, -direction);

    /// <summary>
    /// Get the distance required to push the collision component out of geometry in a given direction. <br/>
    /// NOTE: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider. <br/>
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="direction">The direction to push the collider in. The resulting vector will have the same angle with an adjusted length.</param>
    /// <returns>Information about collision pushout results.</returns>
    public CollisionResult GetCollisionPushoutInDirection(CollisionComponent collider, Transform position, Vector2 direction)
        => GetSweptCollision(collider, position.Matrix, position.MatrixInverse, -direction);

    /// <summary>
    /// Get the distance required to push the collision component out of geometry in a given direction. <br/>
    /// NOTE: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider. <br/>
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="direction">The direction to push the collider in. The resulting vector will have the same angle with an adjusted length.</param>
    /// <returns>Information about collision pushout results.</returns>
    public CollisionResult GetCollisionPushoutInDirection(CollisionComponent collider, Vector2 position, Vector2 direction)
        => GetSweptCollision(collider, Matrix3x2.CreateTranslation(position), -direction);

    /// <summary>
    /// Get the distance required to push the collision component out of geometry in a given direction. <br/>
    /// NOTE: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider. <br/>
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="direction">The direction to push the collider in. The resulting vector will have the same angle with an adjusted length.</param>
    /// <returns>Information about collision pushout results.</returns>
    public CollisionResult GetCollisionPushoutInDirection(CollisionComponent collider, Matrix3x2 position, Vector2 direction)
        => GetSweptCollision(collider, new InvertableMatrix(position), -direction);

    /// <summary>
    /// Get the distance required to push the collision component out of geometry in a given direction. <br/>
    /// NOTE: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider. <br/>
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="positionInv">A pre-calculated inverted matrix for the position.</param>
    /// <param name="direction">The direction to push the collider in. The resulting vector will have the same angle with an adjusted length.</param>
    /// <returns>Information about collision pushout results.</returns>
    public CollisionResult GetCollisionPushoutInDirection(CollisionComponent collider, Matrix3x2 position, Matrix3x2 positionInv, Vector2 direction)
        => GetSweptCollision(collider, position, positionInv, -direction);

    /// <summary>
    /// Get the distance required to push the collision component out of geometry in a given direction. <br/>
    /// NOTE: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider. <br/>
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="direction">The direction to push the collider in. The resulting vector will have the same angle with an adjusted length.</param>
    /// <returns>Information about collision pushout results.</returns>
    public CollisionResult GetCollisionPushoutInDirection(Collider collider, ActorPosition position, Vector2 direction)
        => GetSweptCollision(collider, position.GlobalMatrix, position.GlobalMatrixInverse, -direction);

    /// <summary>
    /// Get the distance required to push the collision component out of geometry in a given direction. <br/>
    /// NOTE: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider. <br/>
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="direction">The direction to push the collider in. The resulting vector will have the same angle with an adjusted length.</param>
    /// <returns>Information about collision pushout results.</returns>
    public CollisionResult GetCollisionPushoutInDirection(Collider collider, Transform position, Vector2 direction)
        => GetSweptCollision(collider, position.Matrix, position.MatrixInverse, -direction);

    /// <summary>
    /// Get the distance required to push the collision component out of geometry in a given direction. <br/>
    /// NOTE: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider. <br/>
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="direction">The direction to push the collider in. The resulting vector will have the same angle with an adjusted length.</param>
    /// <returns>Information about collision pushout results.</returns>
    public CollisionResult GetCollisionPushoutInDirection(Collider collider, Vector2 position, Vector2 direction)
        => GetSweptCollision(collider, Matrix3x2.CreateTranslation(position), -direction);

    /// <summary>
    /// Get the distance required to push the collision component out of geometry in a given direction. <br/>
    /// NOTE: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider. <br/>
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="direction">The direction to push the collider in. The resulting vector will have the same angle with an adjusted length.</param>
    /// <returns>Information about collision pushout results.</returns>
    public CollisionResult GetCollisionPushoutInDirection(Collider collider, Matrix3x2 position, Vector2 direction)
        => GetSweptCollision(collider, new InvertableMatrix(position), -direction);

    /// <summary>
    /// Get the distance required to push the collision component out of geometry in a given direction. <br/>
    /// NOTE: The shortest pushout distance isn't guarenteed to not move the collider into more geometry,
    /// as pushout is calculated per-collider. <br/>
    /// The returned data is sorted by distance, so if the pushout isn't valid the rest of the data can be tested sequentially.
    /// </summary>
    /// <param name="collider">The collision component to check against.</param>
    /// <param name="position">The transform to apply to the collider.</param>
    /// <param name="positionInv">A pre-calculated inverted matrix for the position.</param>
    /// <param name="direction">The direction to push the collider in. The resulting vector will have the same angle with an adjusted length.</param>
    /// <returns>Information about collision pushout results.</returns>
    public CollisionResult GetCollisionPushoutInDirection(Collider collider, Matrix3x2 position, Matrix3x2 positionInv, Vector2 direction)
        => GetSweptCollision(collider, position, positionInv, -direction);

    #endregion

    #region Utilities

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