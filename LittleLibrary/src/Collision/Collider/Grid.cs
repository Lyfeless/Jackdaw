using System.Numerics;
using Foster.Framework;

namespace LittleLib;

//! FIXME (Alex): Implment spatial grid
public class GridCollider(Vector2 position, Grid<Collider?> grid, Vector2 tileSize) : Collider, ISpatialGrid<Collider?, Collider?> {
    readonly Grid<Collider?> Grid = grid;
    public readonly RectangleCollider FullCollider = new(tileSize);

    Vector2 position = position;
    public Vector2 Position {
        get => position;
        set => position = value;
    }

    readonly Vector2 tileSize = tileSize;
    public Vector2 TileSize {
        get => tileSize;
        set { }
    }

    public GridCollider(Vector2 position, Point2 gridSize, Vector2 tileSize) : this(position, new Grid<Collider?>(gridSize), tileSize) { }
    public GridCollider(Point2 gridSize, Vector2 tileSize) : this(Vector2.Zero, new Grid<Collider?>(gridSize), tileSize) { }
    public GridCollider(Grid<Collider?> grid, Vector2 tileSize) : this(Vector2.Zero, grid, tileSize) { }

    public Point2 GridSize = grid.Size;

    public override Rect Bounds => new(Position, Grid.Size * TileSize);
    public override Vector2 Center => Bounds.Center;

    public override bool Multi => true;
    public override Collider[] GetSubColliders(Rect bounds) {
        Vector2 min = Vector2.Max(bounds.TopLeft, Bounds.TopLeft);
        Vector2 max = Vector2.Min(bounds.BottomRight, Bounds.BottomRight);
        Vector2 diff = max - min;
        if (diff.X <= 0 || diff.Y <= 0) { return []; }

        //! FIXME (Alex): Would ideally like to use a fixed size array but that would leave empty space if any tiles are empty
        //! FIXME (Alex): I BROKE THIS SOMEHOW
        List<Collider> overlaps = [];
        int tileCountX = (int)MathF.Ceiling(diff.X / TileSize.X) + 1;
        int tileCountY = (int)MathF.Ceiling(diff.Y / TileSize.Y) + 1;
        Point2 startingCoord = this.LocalToTileCoord(min);
        for (int x = 0; x < tileCountX; ++x) {
            for (int y = 0; y < tileCountY; ++y) {
                Point2 coord = startingCoord + new Point2(x, y);
                Collider? tileCollider = Grid.Get(coord);
                if (tileCollider != null) { overlaps.Add(new OffsetCollider(tileCollider, this.TileCoordToLocal(coord))); }
            }
        }
        return [.. overlaps];
    }

    public override Vector2 Support(Vector2 position, Vector2 direction) {
        throw new NotImplementedException();
    }

    public GridCollider Set(Collider? collider, Point2 position) {
        Grid.Set(collider, position);
        return this;
    }

    public void SetTile(Collider? collider, Point2 gridCoord) {
        Grid.Set(collider, gridCoord);
    }

    public void AddTileStack(Collider? element, Point2 gridCoord) {
        if (element == null) { return; }
        Collider? current = Grid.Get(gridCoord);
        if (current == null) { Grid.Set(element, gridCoord); return; }
        if (current is MultiCollider currentMulti) { Grid.Set(new MultiCollider([.. currentMulti.Colliders, element]), gridCoord); return; }
        Grid.Set(new MultiCollider([current, element]), gridCoord);
    }

    public void RemoveTileStack(Point2 gridCoord) {
        Collider? current = Grid.Get(gridCoord);
        if (current == null) { return; }
        if (current is not MultiCollider currentMulti) { Grid.Set(null, gridCoord); return; }
        Grid.Set(new MultiCollider(currentMulti.Colliders[..^1]), gridCoord);
    }

    public void ClearTile(Point2 gridCoord) {
        Grid.Set(null, gridCoord);
    }

    public Collider? GetTile(Point2 gridCoord) {
        return Grid.Get(gridCoord);
    }
}