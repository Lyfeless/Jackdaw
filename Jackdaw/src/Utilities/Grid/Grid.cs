using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A generic utility for storing 2D rectangular grids of elements.
/// </summary>
/// <typeparam name="T">The type of tile to store in the grid.</typeparam>
/// <param name="gridSize">The grid dimensions.</param>
public class Grid<T>(Point2 gridSize) {
    readonly T?[,] Tiles = new T?[gridSize.X, gridSize.Y];

    /// <summary>
    /// The tile count of the grid.
    /// </summary>
    public readonly Point2 Size = gridSize;

    public Grid(int width, int height) : this(new(width, height)) { }

    /// <summary>
    /// Get a tile at the given location.
    /// </summary>
    /// <param name="tileX">The tile x coordinate.</param>
    /// <param name="tileY">The tile y coordinate.</param>
    /// <returns>The tile at the location, null if the tile doesn't exist or the position is out of bounds.</returns>
    public T? Get(int tileX, int tileY) => Get(new(tileX, tileY));

    /// <summary>
    /// Get a tile at the given location.
    /// </summary>
    /// <param name="tile">The tile coordinate.</param>
    /// <returns>The tile at the location, null if the tile doesn't exist or the position is out of bounds.</returns>
    public T? Get(Point2 tile) {
        if (Contains(tile)) { return Tiles[tile.X, tile.Y]; }
        //! FIXME (Alex): Verify if this returns null for int? values
        return default;
    }

    /// <summary>
    /// Set a tile at the given location.
    /// </summary>
    /// <param name="value">The tile data to set.</param>
    /// <param name="tileX">The tile x coordinate.</param>
    /// <param name="tileY">The tile y coordinate.</param>
    /// <returns>The containing grid.</returns>
    public Grid<T> Set(T? value, int tileX, int tileY) => Set(value, new(tileX, tileY));

    /// <summary>
    /// Set a tile at the given location.
    /// </summary>
    /// <param name="value">The tile data to set.</param>
    /// <param name="tile">The tile coordinate.</param>
    /// <returns>The containing grid.</returns>
    public Grid<T> Set(T? value, Point2 tile) {
        if (Contains(tile)) { Tiles[tile.X, tile.Y] = value; }
        return this;
    }

    /// <summary>
    /// Check if a tile coordinate is in the bounds of the grid.
    /// </summary>
    /// <param name="tileX">the tile x coordinate.</param>
    /// <param name="tileY">the tile y coordinate.</param>
    /// <returns>If the tile is inside the grid.</returns>
    public bool Contains(int tileX, int tileY) => Contains(new(tileX, tileY));

    /// <summary>
    /// Check if a tile coordinate is in the bounds of the grid.
    /// </summary>
    /// <param name="tile">the tile coordinate.</param>
    /// <returns>If the tile is inside the grid.</returns>
    public bool Contains(Point2 tile) {
        return tile.X >= 0 && tile.Y >= 0 && tile.X < Size.X && tile.Y < Size.Y;
    }

    //! FIXME (Alex): Needs extra utility functions
    //      Get start and end tiles contained within rect
}