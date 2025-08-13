using System.Numerics;
using Foster.Framework;

namespace LittleLib;

/// <summary>
/// A grid of colliders, designed to reduce total collision checks for tilemaps.
/// </summary>
/// <param name="position">The position of the grid.</param>
/// <param name="grid">The grid object.</param>
/// <param name="tileSize">The size of each tile.</param>
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
    public override Collider[] GetSubColliders(Rect bounds) {
        Vector2 min = Vector2.Max(bounds.TopLeft, Bounds.TopLeft);
        Vector2 max = Vector2.Min(bounds.BottomRight, Bounds.BottomRight);
        Vector2 diff = max - min;
        if (diff.X <= 0 || diff.Y <= 0) { return []; }

        //! FIXME (Alex): Would ideally like to use a fixed size array but that would leave empty space if any tiles are empty
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

    /// <summary>
    /// Set the collider for a tile.
    /// </summary>
    /// <param name="collider">The collider to set.</param>
    /// <param name="position">The position on the grid in local space.</param>
    /// <returns></returns>
    public GridCollider Set(Collider? collider, Point2 position) {
        Grid.Set(collider, position);
        return this;
    }

    /// <summary>
    /// Set the collider for a tile.
    /// </summary>
    /// <param name="collider">The collider to set.</param>
    /// <param name="gridCoord">The tile location.</param>
    /// <returns></returns>
    public void SetTile(Collider? collider, Point2 gridCoord) {
        Grid.Set(collider, gridCoord);
    }

    /// <summary>
    /// Add a collider onto a tile, combining it with other existing colliders.
    /// </summary>
    /// <param name="collider">The collider to add.</param>
    /// <param name="gridCoord">The tile location.</param>
    /// <returns></returns>
    public void AddTileStack(Collider? collider, Point2 gridCoord) {
        if (collider == null) { return; }
        Collider? current = Grid.Get(gridCoord);
        if (current == null) { Grid.Set(collider, gridCoord); return; }
        if (current is MultiCollider currentMulti) { Grid.Set(new MultiCollider([.. currentMulti.Colliders, collider]), gridCoord); return; }
        Grid.Set(new MultiCollider([current, collider]), gridCoord);
    }

    /// <summary>
    /// Remove the most recent collider from a tile's stack.
    /// </summary>
    /// <param name="gridCoord">The tile location.</param>
    /// <returns></returns>
    public void RemoveTileStack(Point2 gridCoord) {
        Collider? current = Grid.Get(gridCoord);
        if (current == null) { return; }
        if (current is not MultiCollider currentMulti) { Grid.Set(null, gridCoord); return; }
        Grid.Set(new MultiCollider(currentMulti.Colliders[..^1]), gridCoord);
    }

    /// <summary>
    /// Remove all colliders from a tile.
    /// </summary>
    /// <param name="gridCoord">The tile location.</param>
    public void ClearTile(Point2 gridCoord) {
        Grid.Set(null, gridCoord);
    }

    /// <summary>
    /// Get the collider at a grid coordinate.
    /// </summary>
    /// <param name="gridCoord">The tile location.</param>
    /// <returns>The tile at the given location, null if the coord is out of bounds or the tile doesn't exist.</returns>
    public Collider? GetTile(Point2 gridCoord) {
        return Grid.Get(gridCoord);
    }
}