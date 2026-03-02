using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A grid of colliders, designed to reduce total collision checks for tilemaps.
/// </summary>
/// <param name="position">The position of the grid.</param>
/// <param name="grid">The grid object.</param>
/// <param name="tileSize">The size of each tile.</param>
public class GridCollider(Vector2 position, Grid<Collider?> grid, Vector2 tileSize) : Collider, IStackableGrid<Collider?>, ISpatialGrid {
    readonly Grid<Collider?> Grid = grid;

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

    /// <summary>
    /// A grid of colliders, designed to reduce total collision checks for tilemaps.
    /// </summary>
    /// <param name="position">The position of the grid.</param>
    /// <param name="gridSize">The grid tile dimensions.</param>
    /// <param name="tileSize">The size of each tile.</param>
    public GridCollider(Vector2 position, Point2 gridSize, Vector2 tileSize) : this(position, new Grid<Collider?>(gridSize), tileSize) { }

    /// <summary>
    /// A grid of colliders, designed to reduce total collision checks for tilemaps.
    /// </summary>
    /// <param name="gridSize">The grid tile dimensions.</param>
    /// <param name="tileSize">The size of each tile.</param>
    public GridCollider(Point2 gridSize, Vector2 tileSize) : this(Vector2.Zero, new Grid<Collider?>(gridSize), tileSize) { }

    /// <summary>
    /// A grid of colliders, designed to reduce total collision checks for tilemaps.
    /// </summary>
    /// <param name="grid">The grid object.</param>
    /// <param name="tileSize">The size of each tile.</param>
    public GridCollider(Grid<Collider?> grid, Vector2 tileSize) : this(Vector2.Zero, grid, tileSize) { }

    /// <summary>
    /// The grid tile dimensions.
    /// </summary>
    public Point2 GridSize = grid.Size;

    public override Rect Bounds => new(Position, Grid.Size * TileSize);
    public override Vector2 Center => Bounds.Center;

    public override bool Multi => true;

    // Support function will never be called by collision functions because it has subcolliders
    public override Vector2 Support(Vector2 direction) => throw new NotImplementedException();

    public override Collider[] GetSubColliders(Rect bounds) {
        Vector2 min = Vector2.Max(bounds.TopLeft, Bounds.TopLeft);
        Vector2 max = Vector2.Min(bounds.BottomRight, Bounds.BottomRight);
        Vector2 diff = max - min;
        if (diff.X <= 0 || diff.Y <= 0) { return []; }

        List<Collider> overlaps = [];
        int tileCountX = (int)MathF.Ceiling(diff.X / TileSize.X) + 1;
        int tileCountY = (int)MathF.Ceiling(diff.Y / TileSize.Y) + 1;
        Point2 startingCoord = this.LocalToTileCoord(min).FloorToPoint2();
        for (int x = 0; x < tileCountX; ++x) {
            for (int y = 0; y < tileCountY; ++y) {
                Point2 coord = startingCoord + new Point2(x, y);
                Collider? tileCollider = Grid.Get(coord);
                if (tileCollider != null) { overlaps.Add(new OffsetCollider(tileCollider, this.TileCoordToLocal(coord))); }
            }
        }
        return [.. overlaps];
    }


    /// <summary>
    /// Get a new rectangle collider that fills the entire space of a single tile.
    /// </summary>
    public RectangleCollider FullCollider => new(tileSize);

    public IGrid<Collider?> Set(Collider? value, int tileX, int tileY) { Grid.Set(value, tileX, tileY); return this; }
    public IGrid<Collider?> Set(Collider? value, Point2 tile) { Grid.Set(value, tile); return this; }
    public Collider? Get(int tileX, int tileY) => Grid.Get(tileX, tileY);
    public Collider? Get(Point2 tile) => Grid.Get(tile);
    public bool Contains(int tileX, int tileY) => Grid.Contains(tileX, tileY);
    public bool Contains(Point2 tile) => Grid.Contains(tile);

    public IStackableGrid<Collider?> AddTileStackStart(Collider? element, Point2 gridCoord) => AddTileStackAt(element, gridCoord, 0);
    public IStackableGrid<Collider?> AddTileStackEnd(Collider? element, Point2 gridCoord) => AddTileStackAt(element, gridCoord, -1);
    public IStackableGrid<Collider?> AddTileStackAt(Collider? element, Point2 gridCoord, int index) {
        if (element == null) { return this; }

        Collider? current = Grid.Get(gridCoord);
        if (current == null) {
            Grid.Set(element, gridCoord);
            return this;
        }

        List<Collider> colliders = current is MultiCollider currentMulti ? [.. currentMulti.Colliders] : [current];

        if (index == -1) { index = colliders.Count; }
        else { index = Calc.Clamp(index, 0, colliders.Count); }

        colliders.Insert(index, element);

        Grid.Set(new MultiCollider([.. colliders]), gridCoord);

        return this;
    }

    public IStackableGrid<Collider?> RemoveTileStackStart(Point2 gridCoord) => RemoveTileStackAt(gridCoord, 0);
    public IStackableGrid<Collider?> RemoveTileStackEnd(Point2 gridCoord) => RemoveTileStackAt(gridCoord, -1);
    public IStackableGrid<Collider?> RemoveTileStackAt(Point2 gridCoord, int index) {
        Collider? current = Grid.Get(gridCoord);
        if (current == null) { return this; }

        if (current is not MultiCollider currentMulti) {
            if (index == 0 || index == -1) { Grid.Set(null, gridCoord); }
            return this;
        }

        if (index == -1) { index = currentMulti.Colliders.Length - 1; }
        if (index < 0 || index <= currentMulti.Colliders.Length) { return this; }

        List<Collider> colliders = [.. currentMulti.Colliders];
        colliders.RemoveAt(index);

        if (colliders.Count == 0) { Grid.Set(null, gridCoord); }
        else if (colliders.Count == 1) { Grid.Set(colliders[0], gridCoord); }
        else { Grid.Set(new MultiCollider([.. colliders]), gridCoord); }

        return this;
    }
}