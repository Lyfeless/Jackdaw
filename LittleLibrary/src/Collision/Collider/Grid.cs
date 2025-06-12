using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class GridCollider(Vector2 position, Grid<int?> grid) : Collider {
    Vector2 Position = position;

    readonly Grid<int?> Grid = grid;

    readonly List<Collider> colliders = [new RectangleCollider(grid.TileSize)];

    public override Rect Bounds => new(Position, Grid.Size);

    public GridCollider(Vector2 position, Point2 gridSize, Vector2 tileSize) : this(position, new Grid<int?>(gridSize, tileSize)) { }

    public GridCollider AddCollider(Collider collider) {
        if (!colliders.Any(e => e == collider)) { colliders.Add(collider); }
        return this;
    }

    //! FIXME (Alex): Adds collider if it doesnt exist, do we want that?
    public int GetColliderIndex(Collider collider) {
        int index = colliders.IndexOf(collider);
        if (index == -1) {
            colliders.Add(collider);
            return colliders.Count - 1;
        }
        return index;
    }

    public GridCollider Set(Collider collider, Point2 position) => Set(GetColliderIndex(collider), position);
    public GridCollider Set(int? collider, Point2 position) {
        Grid.Set(collider, position);
        return this;
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

    public override Collider Offset(Vector2 amount) {
        //! FIXME (Alex): Needs proper offset when implemented
        throw new NotImplementedException();
    }
}