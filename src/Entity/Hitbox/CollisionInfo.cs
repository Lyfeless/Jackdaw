using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public readonly struct EntityCollisionInfo(EntityReference entity, Hitbox hitbox, Hitbox localHitbox) {
    public readonly EntityReference Entity = entity;
    public readonly Hitbox Hitbox = hitbox;
    public readonly Hitbox LocalHitbox = localHitbox;
}

public readonly struct TileCollisionInfo(TileHitboxCollisionInfo[] hitboxCollisions) {
    public static readonly TileCollisionInfo Empty = new([]);

    public readonly bool Collision = hitboxCollisions.Length > 0;
    public readonly TileHitboxCollisionInfo[] HitboxCollisions = hitboxCollisions;
}

public readonly struct TileHitboxCollisionInfo(Hitbox hitbox, TileLayerCollisionInfo[] layerCollisions) {
    public readonly Hitbox Hitbox = hitbox;
    public readonly bool Collision = layerCollisions.Length > 0;
    public readonly TileLayerCollisionInfo[] LayerCollisions = layerCollisions;
}

public readonly struct TileLayerCollisionInfo(TileLayer layer, Point2[] collidedTiles) {
    public readonly TileLayer Layer = layer;
    public readonly bool Collision = collidedTiles.Length > 0;
    public readonly Point2[] CollidedTiles = collidedTiles;
}

public readonly struct TilePointCollisionInfo(TileLayerCollisionInfo[] layerCollisions) {
    public readonly bool Collision = layerCollisions.Length > 0;
    public readonly TileLayerCollisionInfo[] LayerCollisions = layerCollisions;
}

public readonly struct RaycastCollisionInfo(Vector2 hitPoint, Layer hitLayer, Entity[] collidedEntities) {
    public readonly Vector2 HitPoint = hitPoint;
    public readonly Layer HitLayer = hitLayer;
    public readonly Entity[] CollidedEntities = collidedEntities;
}