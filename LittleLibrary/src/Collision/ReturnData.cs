using System.Numerics;

namespace LittleLib;

public record struct SingleCollisionInfo(bool Collided, CollisionComponentInfo? ColliderInfo);
public record struct AllCollisionInfo(bool Collided, CollisionComponentInfo[] ColliderInfo);
public record struct PushoutCollisionInfo(bool Collided, Vector2 Pushout, PushoutCollisionComponentInfo[] ColliderInfo);
public record struct SweptCollisionInfo(bool Collided, Vector2 AdjustedVelocity, Vector2 VelocityFraction, SweptCollisionComponentInfo[] ColliderInfo);

public record struct CollisionComponentInfo(CollisionComponent Component, Collider[] Colliders);
public record struct PushoutCollisionComponentInfo(CollisionComponent Component, int LargestPushout, PushoutColliderInfo[] Colliders);
public record struct SweptCollisionComponentInfo(CollisionComponent Component, int ClosestCollider, SweptColliderInfo[] Colliders);

public record struct PushoutColliderInfo(Collider Collider, Vector2 Pushout);
public record struct SweptColliderInfo(Collider Collider, Vector2 Fraction, Vector2 Normal);