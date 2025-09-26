using System.Numerics;

namespace LittleLib;

/// <summary>
/// Collision return results for single collision checks.
/// </summary>
/// <param name="Collided">If the collision check found any collisions.</param>
/// <param name="ColliderInfo">Information about collision, null if none were found.</param>
public record struct SingleCollisionInfo(bool Collided, CollisionComponentInfo? ColliderInfo);

/// <summary>
/// Collision return results for multi collision checks.
/// </summary>
/// <param name="Collided">If the collision check found any collisions.</param>
/// <param name="ColliderInfo">Information about all found collisions.</param>
public record struct AllCollisionInfo(bool Collided, CollisionComponentInfo[] ColliderInfo);

/// <summary>
/// Collision return results for pushout collision checks.
/// </summary>
/// <param name="Collided">If the collision check found any collisions.</param>
/// <param name="Pushout">The largest pushout from all the checks.</param>
//! FIXME (Alex): MAKE THIS TRUE
/// <param name="ColliderInfo">Information about all found collisions, sorted by largest pushout.</param>
public record struct PushoutCollisionInfo(bool Collided, Vector2 Pushout, PushoutCollisionComponentInfo[] ColliderInfo);

/// <summary>
/// Collision return results for raycast or shapecast collision checks.
/// </summary>
/// <param name="Collided">If the collision check found any collisions.</param>
/// <param name="AdjustedVelocity">The velocity adjusted by all collisions.</param>
/// <param name="VelocityFraction">The percent of the total velocity the shape was able to move.</param>
/// <param name="ColliderInfo">Information about all found collisions.</param>
public record struct SweptCollisionInfo(bool Collided, Vector2 AdjustedVelocity, Vector2 VelocityFraction, SweptCollisionComponentInfo[] ColliderInfo);


/// <summary>
/// Generic single collision information.
/// </summary>
/// <param name="Component">The collided component.</param>
/// <param name="Colliders">All the subcolliders in the component that were collided.</param>
public record struct CollisionComponentInfo(CollisionComponent Component, Collider[] Colliders);

/// <summary>
/// Single collision information with pushout information.
/// </summary>
/// <param name="Component">The collided component.</param>
/// <param name="LargestPushout">The largest pushout of any of the colliders.</param>
/// <param name="Colliders">All the subcolliders in the component that were collided.</param>
public record struct PushoutCollisionComponentInfo(CollisionComponent Component, int LargestPushout, PushoutColliderInfo[] Colliders);

/// <summary>
/// Raycast and shapecast single collision information.
/// </summary>
/// <param name="Component">The collided component.</param>
/// <param name="ClosestCollider">The collider closest to the start of the cast.</param>
/// <param name="Colliders">All the subcolliders in the component that were collided.</param>
//! FIXME (Alex): Shouls this also include the fraction?
public record struct SweptCollisionComponentInfo(CollisionComponent Component, int ClosestCollider, SweptColliderInfo[] Colliders);

/// <summary>
/// Single collider information with pushout information.
/// </summary>
/// <param name="Collider">The collider.</param>
/// <param name="Pushout">Shortest pushout to move out of the collider.</param>
public record struct PushoutColliderInfo(Collider Collider, Vector2 Pushout);

/// <summary>
/// Raycast and shapecast single collider information.
/// </summary>
/// <param name="Collider">The collider.</param>
/// <param name="Fraction">The fraction of the total velocity the cast was able to complete.</param>
/// <param name="Normal">The normal vector of the collided side of the collider.</param>
public record struct SweptColliderInfo(Collider Collider, Vector2 Fraction, Vector2 Normal);