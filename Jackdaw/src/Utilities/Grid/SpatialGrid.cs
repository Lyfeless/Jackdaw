using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

//! FIXME (Alex): Doc comments

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
        => grid.SetTile(element, (Point2)grid.LocalToTileCoord(localCoord));
    public static void AddTileStackLocal<TIn, TOut>(this ISpatialGrid<TIn, TOut> grid, TIn element, Vector2 localCoord)
        => grid.AddTileStack(element, (Point2)grid.LocalToTileCoord(localCoord));
    public static void RemoveTileStackLocal<TIn, TOut>(this ISpatialGrid<TIn, TOut> grid, Vector2 localCoord)
        => grid.RemoveTileStack((Point2)grid.LocalToTileCoord(localCoord));
    public static void ClearTileLocal<TIn, TOut>(this ISpatialGrid<TIn, TOut> grid, Vector2 localCoord)
        => grid.ClearTile((Point2)grid.LocalToTileCoord(localCoord));
    public static TOut? GetTileLocal<TIn, TOut>(this ISpatialGrid<TIn, TOut> grid, Vector2 localCoord)
        => grid.GetTile((Point2)grid.LocalToTileCoord(localCoord));

    public static Matrix3x2 LocalToTileCoord<TIn, TOut>(this ISpatialGrid<TIn, TOut> grid, Matrix3x2 localPosition) {
        return localPosition * grid.InMatrix();
    }

    public static Vector2 LocalToTileCoord<TIn, TOut>(this ISpatialGrid<TIn, TOut> grid, Vector2 localPosition) {
        return Vector2.Transform(localPosition, grid.InMatrix());
    }

    public static Matrix3x2 TileCoordToLocal<TIn, TOut>(this ISpatialGrid<TIn, TOut> grid, Matrix3x2 tileCoords) {
        return tileCoords * grid.OutMatrix();
    }

    public static Vector2 TileCoordToLocal<TIn, TOut>(this ISpatialGrid<TIn, TOut> grid, Point2 tileCoords)
        => TileCoordToLocal(grid, (Vector2)tileCoords);

    public static Vector2 TileCoordToLocal<TIn, TOut>(this ISpatialGrid<TIn, TOut> grid, Vector2 tileCoords) {
        return Vector2.Transform(tileCoords, grid.OutMatrix());
    }

    static Matrix3x2 OutMatrix<TIn, TOut>(this ISpatialGrid<TIn, TOut> grid)
        => Transform.CreateMatrix(grid.Position, Vector2.Zero, grid.TileSize, 0);

    static Matrix3x2 InMatrix<TIn, TOut>(this ISpatialGrid<TIn, TOut> grid)
        => Transform.CreateMatrix(-grid.Position, Vector2.Zero, Vector2.One / grid.TileSize, 0);
}