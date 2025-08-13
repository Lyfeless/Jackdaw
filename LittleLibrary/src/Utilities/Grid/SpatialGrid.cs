using System.Numerics;
using Foster.Framework;

namespace LittleLib;

//! FIXME (Alex): Doc comments
//! FIXME (Alex): make a few premade spatial grids

public interface ISpatialGrid<TIn, TOut> {
    public Vector2 Position { get; set; }
    public Vector2 TileSize { get; protected set; }

    public void SetTile(TIn element, Point2 gridCoord);
    public void AddTileStack(TIn element, Point2 gridCoord);
    public void RemoveTileStack(Point2 gridCoord);
    public void ClearTile(Point2 gridCoord);
    public TOut? GetTile(Point2 gridCoord);
}

public static class ISpatialGridHelper {
    public static void SetTileLocal<TIn, TOut>(this ISpatialGrid<TIn, TOut> grid, TIn element, Vector2 localCoord)
        => grid.SetTile(element, grid.LocalToTileCoord(localCoord));
    public static void AddTileStackLocal<TIn, TOut>(this ISpatialGrid<TIn, TOut> grid, TIn element, Vector2 localCoord)
        => grid.AddTileStack(element, grid.LocalToTileCoord(localCoord));
    public static void RemoveTileStackLocal<TIn, TOut>(this ISpatialGrid<TIn, TOut> grid, Vector2 localCoord)
        => grid.RemoveTileStack(grid.LocalToTileCoord(localCoord));
    public static void ClearTileLocal<TIn, TOut>(this ISpatialGrid<TIn, TOut> grid, Vector2 localCoord)
        => grid.ClearTile(grid.LocalToTileCoord(localCoord));
    public static TOut? GetTileLocal<TIn, TOut>(this ISpatialGrid<TIn, TOut> grid, Vector2 localCoord)
        => grid.GetTile(grid.LocalToTileCoord(localCoord));

    public static Point2 LocalToTileCoord<TIn, TOut>(this ISpatialGrid<TIn, TOut> grid, Vector2 localPosition)
        => (Point2)((localPosition - grid.Position) / grid.TileSize);

    public static Vector2 TileCoordToLocal<TIn, TOut>(this ISpatialGrid<TIn, TOut> grid, Point2 tileCoords)
        => (tileCoords * grid.TileSize) + grid.Position;
}