using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class Hitbox(Vector2 position, Vector2 size, long tags, long collisionTags, bool collideWithLevel) {
    /// <summary>
    /// Any tags describing the current hitbox.
    /// </summary>
    public long Tags = tags;
    /// <summary>
    /// Any tags the hitbox should be able to collide with.
    /// </summary>
    public long CollisionTags = collisionTags;

    public bool CollideWithLevel = collideWithLevel;

    public Vector2 Position = position;
    public Vector2 Size = size;
    public Rect Bounds => new(Position, Position + Size);

    public float ResolveX(TileHitboxCollisionInfo collisionInfo, float currentPosition, float startPosition, float moveAmount) {
        float resolvePosition = currentPosition;

        foreach (TileLayerCollisionInfo layerCollision in collisionInfo.LayerCollisions) {
            foreach (Point2 tile in layerCollision.CollidedTiles) {
                float tilePosition = layerCollision.Layer.TileCoordsToWorldCoords(tile).X;
                if (moveAmount > 0) {
                    if (tilePosition <= resolvePosition + Bounds.Right && tilePosition + layerCollision.Layer.TileSize >= startPosition + Bounds.Right) {
                        resolvePosition = tilePosition - Bounds.Right;
                    }
                }
                else {
                    if (tilePosition + layerCollision.Layer.TileSize > resolvePosition + Bounds.Left && tilePosition <= startPosition + Bounds.Left) {
                        resolvePosition = tilePosition + layerCollision.Layer.TileSize - Bounds.Left;
                    }
                }
            }
        }

        return resolvePosition;
    }

    public float ResolveY(TileHitboxCollisionInfo collisionInfo, float currentPosition, float startPosition, float moveAmount) {
        float resolvePosition = currentPosition;

        foreach (TileLayerCollisionInfo layerCollision in collisionInfo.LayerCollisions) {
            foreach (Point2 tile in layerCollision.CollidedTiles) {
                float tilePosition = layerCollision.Layer.TileCoordsToWorldCoords(tile).Y;
                if (moveAmount > 0) {
                    if (tilePosition <= resolvePosition + Bounds.Bottom && tilePosition + layerCollision.Layer.TileSize >= startPosition + Bounds.Bottom) {
                        resolvePosition = tilePosition - Bounds.Bottom;
                    }
                }
                else {
                    if (tilePosition + layerCollision.Layer.TileSize > resolvePosition + Bounds.Top && tilePosition <= startPosition + Bounds.Top) {
                        resolvePosition = tilePosition + layerCollision.Layer.TileSize - Bounds.Top;
                    }
                }
            }
        }

        return resolvePosition;
    }

    /// <summary>
    /// Check if entity has all given tags.
    /// </summary>
    /// <param name="tags">Tag objects combined into a single value e.g <c>EntityTags.A | EntityTags.B | EntityTags.C</c></param>
    /// <returns></returns>
    public bool HasTags(long tags) {
        if (tags == 0) { return false; }
        return (Tags & tags) == tags;
    }

    /// <summary>
    /// Check if entity has any given tag.
    /// </summary>
    /// <param name="tags">Tag objects combined into a single value e.g <c>EntityTags.A | EntityTags.B | EntityTags.C</c></param>
    /// <returns></returns>
    public bool HasAnyTag(long tags) {
        if (tags == 0) { return false; }
        return (Tags & tags) != 0;
    }
}