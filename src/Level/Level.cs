using System.Numerics;
using Foster.Framework;

namespace LittleLib;

/// <summary>
/// Level information and rendering
/// </summary>
/// <param name="size">Size of the level bounds in pixels</param>
/// <param name="position">Position of the level in world space</param>
/// <param name="layers">A list of all tile layers</param>
public class Level(string id, Point2 size, Point2 position, FieldSaveData[] fields) {
    public static readonly Level Empty = new(string.Empty, Point2.Zero, Point2.Zero, []);

    public readonly string ID = id;

    public readonly Point2 Size = size;
    public readonly Point2 Position = position;
    public Rect Bounds => new(Position, Position + Size);
    //! FIXME (Alex): Not a huge fan of assigning the layers after the fact but its my compromise for now
    Layer[] Layers = [];

    public readonly FieldSaveData[] Fields = fields;

    public void SetLayers(Layer[] layers) { Layers = layers; }

    public Layer? GetLayerByName(string NameID) {
        return Layers.FirstOrDefault(e => e.NameID == NameID);
    }

    public Layer? GetLayerByInstanceID(string InstanceID) {
        return Layers.FirstOrDefault(e => e.InstanceID == InstanceID);
    }

    public TileCollisionInfo CheckEntityCollision(EntityReference entity, string[]? collisionTags = null) {
        if (entity.Entity == null) { return TileCollisionInfo.Empty; }
        return CheckEntityCollision(entity.Entity.Position.Precise, entity.Entity.Hitboxes, collisionTags);
    }

    public TileCollisionInfo CheckEntityCollision(Vector2 position, HitboxContainer hitboxContainer, string[]? collisionTags = null) {
        Rect bounds = hitboxContainer.Bounds;
        if (!CheckHitboxCollision(position, new Hitbox(bounds.Position, bounds.Size, 0, 0, true), collisionTags).Collision) { return TileCollisionInfo.Empty; }

        List<TileHitboxCollisionInfo> collisions = [];
        foreach (Hitbox hitbox in hitboxContainer.Hitboxes) {
            if (!hitbox.CollideWithLevel) { continue; }
            TileHitboxCollisionInfo info = CheckHitboxCollision(position, hitbox, collisionTags);
            if (info.Collision) { collisions.Add(info); }
        }

        return new([.. collisions]);
    }

    //! FIXME (Alex): Add support for point and raycast collision
    public TileHitboxCollisionInfo CheckHitboxCollision(Vector2 position, Hitbox hitbox, string[]? collisionTags = null) {
        bool collided = false;
        List<TileLayerCollisionInfo> collisions = [];
        foreach (Layer layer in Layers) {
            if (layer is TileLayer tileLayer) {
                TileLayerCollisionInfo layerCollision = tileLayer.CheckCollision(new Rect(hitbox.Bounds.TopLeft + position, hitbox.Bounds.BottomRight + position), collisionTags);
                if (layerCollision.CollidedTiles.Length > 0) {
                    collisions.Add(layerCollision);
                    collided = true;
                }
            }
        }

        return new(hitbox, [.. collisions]);
    }

    public TilePointCollisionInfo CheckPointCollision(Vector2 position, string[]? collisionTags = null) {
        List<TileLayerCollisionInfo> collisions = [];
        foreach (Layer layer in Layers) {
            if (layer is TileLayer tileLayer) {
                TileLayerCollisionInfo layerCollision = tileLayer.CheckCollision(position, collisionTags);
                if (layerCollision.CollidedTiles.Length > 0) {
                    collisions.Add(layerCollision);
                }
            }
        }

        return new([.. collisions]);
    }

    //! FIXME (Alex): Raycast collisions not implemented, hope I dont need it
    // public RaycastCollisionInfo CheckRaycastCollision(Vector2 startPos, float angle, string[]? collisionTags = null) {
    //     List<EntityReference> collidedEntites = [];

    //     Vector2 posDelta = Calc.AngleToVector(angle);

    //     Vector2 deltaDistance = new() {
    //         X = Math.Abs(1.0f / posDelta.X),
    //         Y = Math.Abs(1.0f / posDelta.Y),
    //     };

    //     Point2 step = new(
    //         Math.Sign(posDelta.X),
    //         Math.Sign(posDelta.Y)
    //     );


    //     foreach (Layer layer in Layers) {
    //         if (layer is TileLayer tileLayer) {
    //             Point2 tile = (Point2)tileLayer.WorldCoordsToTileCoords(startPos);

    //             Vector2 sideDistance = new() {
    //                 X = (step.X < 0 ? startPos.X - tile.X : tile.X + 1 - startPos.X) * deltaDistance.X,
    //                 Y = (step.Y < 0 ? startPos.Y - tile.Y : tile.Y + 1 - startPos.Y) * deltaDistance.Y,
    //             };

    //             while (
    //                 tileLayer.PositionValid(tile) &&
    //                 collisionTags == null
    //                     ? tileLayer.GetTile(tile)?.IsSolid ?? false
    //                     : tileLayer.GetTile(tile)?.HasAnyTag(collisionTags) ?? false
    //             ) {

    //             }
    //         }
    //     }

    //     for (int i = 0; i < size.X + size.Y; ++i) {
    //         if (isPositionValid(tile.X, tile.Y)) {
    //             trySetTile(tile.X, tile.Y, colorPicker.activeColor);
    //         }

    //         if ((tile.X == (int)endPos.X && tile.Y == (int)endPos.Y) || step == Vector2i.Zero) {
    //             break;
    //         }

    //         if (sideDistance.X < sideDistance.Y) {
    //             sideDistance.X += deltaDistance.X;
    //             tile.X += step.X;
    //         }
    //         else {
    //             sideDistance.Y += deltaDistance.Y;
    //             tile.Y += step.Y;
    //         }

    //         if (tile.X < 0 || tile.Y < 0) {
    //             break;
    //         }
    //     }
    // }
    // }

    public void Activate() {
        foreach (Layer layer in Layers) {
            if (layer is EntityLayer entityLayer) {
                entityLayer.Activate();
            }
        }
    }

    public void Deactivate() {
        foreach (Layer layer in Layers) {
            if (layer is EntityLayer entityLayer) {
                entityLayer.Deactivate();
            }
        }
    }

    /// <summary>
    /// Draw all level elements, expects applied maxtrix to account for all camera movement
    /// </summary>
    /// <param name="batcher">Current rendering batcher</param>
    public void Render(Batcher batcher) {
        batcher.PushMatrix(-Camera.ActiveCamera.TopLeft);

        // Reverse rendering order because ldtk loads front-to-back
        for (int i = Layers.Length - 1; i >= 0; --i) {
            bool applyPosition = Layers[i] is not EntityLayer;
            if (applyPosition) { batcher.PushMatrix(Position); }
            Layers[i].Render(batcher);
            if (applyPosition) { batcher.PopMatrix(); }

        }

        batcher.PopMatrix();
    }
}