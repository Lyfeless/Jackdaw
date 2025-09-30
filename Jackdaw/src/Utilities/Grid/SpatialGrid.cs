using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A grid mean to exist in world space, with a position and tile size.
/// </summary>
/// <typeparam name="TIn">The type of object used to assign tiles.</typeparam>
/// <typeparam name="TOut">The type of object the tiles are stored as.</typeparam>
public interface ISpatialGrid<TIn, TOut> {
    /// <summary>
    /// The position of the entire tile grid.
    /// </summary>
    public Vector2 Position { get; set; }

    /// <summary>
    /// The width and height of an individual tile.
    /// </summary>
    public Vector2 TileSize { get; protected set; }

    /// <summary>
    /// Set a tile at a position in the grid.
    /// </summary>
    /// <param name="element">The element to set the tile to.</param>
    /// <param name="gridCoord">The grid coordinate to set at.</param>
    public void SetTile(TIn element, Point2 gridCoord);

    /// <summary>
    /// If applicable, add an additional element to stack on a tile.
    /// </summary>
    /// <param name="element">The element to add to the stack.</param>
    /// <param name="gridCoord">The grid coordinate to add to.</param>
    public void AddTileStack(TIn element, Point2 gridCoord);

    /// <summary>
    /// If applicable, remove the top element from a tile's element stack.
    /// </summary>
    /// <param name="gridCoord">The grid coordinate to remove from.</param>
    public void RemoveTileStack(Point2 gridCoord);

    /// <summary>
    /// Clear all data from a tile.
    /// </summary>
    /// <param name="gridCoord">The grid coordinate to clear.</param>
    public void ClearTile(Point2 gridCoord);

    /// <summary>
    /// Get the stored value at a tile position.
    /// </summary>
    /// <param name="gridCoord">The grid coordinate to read.</param>
    /// <returns>The tile value at the given grid coordinate.</returns>
    public TOut? GetTile(Point2 gridCoord);
}

public static class ISpatialGridHelper {
    /// <summary>
    /// Set a tile at a position in the grid, in local coordinates.
    /// </summary>
    /// <param name="grid">The grid to set to.</param>
    /// <param name="element">The element to set the tile to.</param>
    /// <param name="localCoord">The local coordinate to add to.</param>
    public static void SetTileLocal<TIn, TOut>(this ISpatialGrid<TIn, TOut> grid, TIn element, Vector2 localCoord)
        => grid.SetTile(element, (Point2)grid.LocalToTileCoord(localCoord));

    /// <summary>
    /// If applicable, add an additional element to stack on a tile, in local coordinates.
    /// </summary>
    /// <param name="grid">The grid to add to.</param>
    /// <param name="element">The element to set the tile to.</param>
    /// <param name="localCoord">The local coordinate to add to.</param>
    public static void AddTileStackLocal<TIn, TOut>(this ISpatialGrid<TIn, TOut> grid, TIn element, Vector2 localCoord)
        => grid.AddTileStack(element, (Point2)grid.LocalToTileCoord(localCoord));

    /// <summary>
    /// If applicable, remove the top element from a tile's element stack, in local coordinates.
    /// </summary>
    /// <param name="grid">The grid to remove from.</param>
    /// <param name="localCoord">The local coordinate to add to.</param>
    public static void RemoveTileStackLocal<TIn, TOut>(this ISpatialGrid<TIn, TOut> grid, Vector2 localCoord)
        => grid.RemoveTileStack((Point2)grid.LocalToTileCoord(localCoord));

    /// <summary>
    /// Clear all data from a tile, in local coordinates.
    /// </summary>
    /// <param name="grid">The grid to set to.</param>
    /// <param name="localCoord">The local coordinate to add to.</param>
    public static void ClearTileLocal<TIn, TOut>(this ISpatialGrid<TIn, TOut> grid, Vector2 localCoord)
        => grid.ClearTile((Point2)grid.LocalToTileCoord(localCoord));

    /// <summary>
    /// Get the stored value at a tile position, in local coordinates.
    /// </summary>
    /// <param name="grid">The grid to set to.</param>
    /// <param name="localCoord">The local coordinate to add to.</param>
    /// <returns>The tile value at the given local position.</returns>
    public static TOut? GetTileLocal<TIn, TOut>(this ISpatialGrid<TIn, TOut> grid, Vector2 localCoord)
        => grid.GetTile((Point2)grid.LocalToTileCoord(localCoord));

    /// <summary>
    /// Convert a position from a spatial position to a tile coordinate.
    /// </summary>
    /// <param name="grid">The spatial grid.</param>
    /// <param name="localPosition">The local coord to be converted.</param>
    /// <returns>The local coordinate in tile space.</returns>
    public static Matrix3x2 LocalToTileCoord<TIn, TOut>(this ISpatialGrid<TIn, TOut> grid, Matrix3x2 localPosition) {
        return localPosition * grid.InMatrix();
    }

    /// <summary>
    /// Convert a position from a spatial position to a tile coordinate.
    /// </summary>
    /// <param name="grid">The spatial grid.</param>
    /// <param name="localPosition">The local coord to be converted.</param>
    /// <returns>The local coordinate in tile space.</returns>
    public static Vector2 LocalToTileCoord<TIn, TOut>(this ISpatialGrid<TIn, TOut> grid, Vector2 localPosition) {
        return Vector2.Transform(localPosition, grid.InMatrix());
    }

    /// <summary>
    /// Convert a position from a tile coordinate to a spatial position.
    /// </summary>
    /// <param name="grid">The spatial grid.</param>
    /// <param name="tileCoords">The tile coord to be converted.</param>
    /// <returns>The tile coordinate in local space.</returns>
    public static Matrix3x2 TileCoordToLocal<TIn, TOut>(this ISpatialGrid<TIn, TOut> grid, Matrix3x2 tileCoords) {
        return tileCoords * grid.OutMatrix();
    }

    /// <summary>
    /// Convert a position from a tile coordinate to a spatial position.
    /// </summary>
    /// <param name="grid">The spatial grid.</param>
    /// <param name="tileCoords">The tile coord to be converted.</param>
    /// <returns>The tile coordinate in local space.</returns>
    public static Vector2 TileCoordToLocal<TIn, TOut>(this ISpatialGrid<TIn, TOut> grid, Point2 tileCoords)
        => TileCoordToLocal(grid, (Vector2)tileCoords);

    /// <summary>
    /// Convert a position from a tile coordinate to a spatial position.
    /// </summary>
    /// <param name="grid">The spatial grid.</param>
    /// <param name="tileCoords">The tile coord to be converted.</param>
    /// <returns>The tile coordinate in local space.</returns>
    public static Vector2 TileCoordToLocal<TIn, TOut>(this ISpatialGrid<TIn, TOut> grid, Vector2 tileCoords) {
        return Vector2.Transform(tileCoords, grid.OutMatrix());
    }

    static Matrix3x2 OutMatrix<TIn, TOut>(this ISpatialGrid<TIn, TOut> grid)
        => Transform.CreateMatrix(grid.Position, Vector2.Zero, grid.TileSize, 0);

    static Matrix3x2 InMatrix<TIn, TOut>(this ISpatialGrid<TIn, TOut> grid)
        => Transform.CreateMatrix(-grid.Position, Vector2.Zero, Vector2.One / grid.TileSize, 0);
}