using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class GridCollider(Vector2 position, Point2 gridSize, Vector2 tileSize) : Collider {
    //! FIXME (Alex): Recalculate bounds if either of these change
    Vector2 Position = position;
    //! FIXME (Alex): Should this just be float to make things easier?
    Vector2 TileSize = tileSize;

    readonly int[,] Tiles = new int[gridSize.X, gridSize.Y];

    List<Collider> colliders = [new ConvexCollider(new Rect(Vector2.Zero, tileSize))];

    Rect bounds = GetBounds(position, gridSize, tileSize);
    public override Rect Bounds => bounds;

    public GridCollider AddCollider(Collider collider) {
        if (!colliders.Any(e => e == collider)) { colliders.Add(collider); }
        return this;
    }

    public int GetColliderIndex(Collider collider) {
        return colliders.IndexOf(collider);
    }

    public override bool Overlaps(Collider with, out Vector2 pushout) {
        //! FIXME (Alex): Implement
        // Process should be:
        //      - use opposing collider bounds to find overlaps
        //      - Ignore any overlap where collider is unset
        //      - check each overlapped tile aabb
        //      - If passed, check overlapped tile's custom collider, if set
        //      - Collect all pushout positions, ??? to chose one
        throw new NotImplementedException();
    }

    static Rect GetBounds(Vector2 position, Point2 gridSize, Vector2 tileSize) {
        return new Rect(position, gridSize * tileSize);
    }
}