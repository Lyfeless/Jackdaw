using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class GridCollider(Vector2 position, Grid<int?> grid, Vector2 tileSize) : Collider {
    Vector2 Position = position;

    readonly Grid<int?> Grid = grid;
    readonly Vector2 TileSize = tileSize;

    readonly List<Collider> colliders = [new RectangleCollider(tileSize)];

    public GridCollider(Vector2 position, Point2 gridSize, Vector2 tileSize) : this(position, new Grid<int?>(gridSize), tileSize) { }

    public override Rect Bounds => new(Position, Grid.Size);
    public override Vector2 Center => Bounds.Center;

    public override bool Multi => true;
    public override Collider[] GetSubColliders(Collider collider, Vector2 position) {
        Vector2 min = Vector2.Max(collider.Bounds.TopLeft, Bounds.TopLeft);
        Vector2 max = Vector2.Min(collider.Bounds.BottomRight, Bounds.BottomRight);
        Vector2 diff = max - min;
        if (diff.X <= 0 || diff.Y <= 0) { return []; }

        //! FIXME (Alex): Would ideally like to use a fixed size array but that would leave empty space if any tiles are empty
        List<Collider> overlaps = [];
        int tileCountX = (int)MathF.Ceiling(diff.X / TileSize.X);
        int tileCountY = (int)MathF.Ceiling(diff.Y / TileSize.Y);
        Point2 startingCoord = GetTileCoords(min);
        for (int x = 0; x < tileCountX; ++x) {
            for (int y = 0; y < tileCountY; ++y) {
                Point2 coord = startingCoord + new Point2(x, y);
                Collider? tileCollider = GetCollider(coord);
                if (tileCollider != null) { overlaps.Add(new OffsetCollider(tileCollider, GetLocalPosition(coord))); }
            }
        }
        return [.. overlaps];
    }

    public GridCollider AddCollider(Collider collider) {
        if (!colliders.Any(e => e == collider)) { colliders.Add(collider); }
        return this;
    }

    public Point2 GetTileCoords(Vector2 localPosition) {
        return (Point2)((localPosition - Position) / TileSize);
    }

    public Vector2 GetLocalPosition(Point2 tileCoords) {
        return (tileCoords * TileSize) + Position;
    }

    public Collider? GetCollider(Vector2 localPosition) => GetCollider(GetTileCoords(localPosition));
    public Collider? GetCollider(Point2 tileCoord) {
        int? index = Grid.Get(tileCoord);
        if (index == null) { return null; }
        return colliders[(int)index];
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

    public override Vector2 Support(Vector2 position, Vector2 direction) {
        throw new NotImplementedException();
    }
}