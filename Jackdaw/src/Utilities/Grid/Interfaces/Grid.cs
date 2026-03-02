using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A rectanglular array for storing elements in a grid.
/// </summary>
/// <typeparam name="T">The type of object tiles are stored as.</typeparam>
public interface IGrid<T> {
    /// <summary>
    /// Get a tile at the given location.
    /// </summary>
    /// <param name="tileX">The tile x coordinate.</param>
    /// <param name="tileY">The tile y coordinate.</param>
    /// <returns>The tile at the location, null if the tile doesn't exist or the position is out of bounds.</returns>
    public T? Get(int tileX, int tileY);

    /// <summary>
    /// Get a tile at the given location.
    /// </summary>
    /// <param name="tile">The tile coordinate.</param>
    /// <returns>The tile at the location, null if the tile doesn't exist or the position is out of bounds.</returns>
    public T? Get(Point2 tile);

    /// <summary>
    /// Set a tile at the given location.
    /// </summary>
    /// <param name="value">The tile data to set.</param>
    /// <param name="tileX">The tile x coordinate.</param>
    /// <param name="tileY">The tile y coordinate.</param>
    /// <returns>The containing grid.</returns>
    public IGrid<T> Set(T? value, int tileX, int tileY);

    /// <summary>
    /// Set a tile at the given location.
    /// </summary>
    /// <param name="value">The tile data to set.</param>
    /// <param name="tile">The tile coordinate.</param>
    /// <returns>The containing grid.</returns>
    public IGrid<T> Set(T? value, Point2 tile);

    /// <summary>
    /// Check if a tile coordinate is in the bounds of the grid.
    /// </summary>
    /// <param name="tileX">the tile x coordinate.</param>
    /// <param name="tileY">the tile y coordinate.</param>
    /// <returns>If the tile is inside the grid.</returns>
    public bool Contains(int tileX, int tileY);

    /// <summary>
    /// Check if a tile coordinate is in the bounds of the grid.
    /// </summary>
    /// <param name="tile">the tile coordinate.</param>
    /// <returns>If the tile is inside the grid.</returns>
    public bool Contains(Point2 tile);
}

/// <summary>
/// A rectanglular array for storing elements in a grid.
/// </summary>
/// <typeparam name="TIn">The type of object used to assign tiles.</typeparam>
/// <typeparam name="TOut">The type of object the tiles are stored as.</typeparam>
public interface IGrid<TIn, TOut> {
    /// <summary>
    /// Get a tile at the given location.
    /// </summary>
    /// <param name="tileX">The tile x coordinate.</param>
    /// <param name="tileY">The tile y coordinate.</param>
    /// <returns>The tile at the location, null if the tile doesn't exist or the position is out of bounds.</returns>
    public TOut? Get(int tileX, int tileY);

    /// <summary>
    /// Get a tile at the given location.
    /// </summary>
    /// <param name="tile">The tile coordinate.</param>
    /// <returns>The tile at the location, null if the tile doesn't exist or the position is out of bounds.</returns>
    public TOut? Get(Point2 tile);

    /// <summary>
    /// Set a tile at the given location.
    /// </summary>
    /// <param name="value">The tile data to set.</param>
    /// <param name="tileX">The tile x coordinate.</param>
    /// <param name="tileY">The tile y coordinate.</param>
    /// <returns>The containing grid.</returns>
    public IGrid<TIn, TOut> Set(TIn? value, int tileX, int tileY) => Set(value, new(tileX, tileY));

    /// <summary>
    /// Set a tile at the given location.
    /// </summary>
    /// <param name="value">The tile data to set.</param>
    /// <param name="tile">The tile coordinate.</param>
    /// <returns>The containing grid.</returns>
    public IGrid<TIn, TOut> Set(TIn? value, Point2 tile);

    /// <summary>
    /// Check if a tile coordinate is in the bounds of the grid.
    /// </summary>
    /// <param name="tileX">the tile x coordinate.</param>
    /// <param name="tileY">the tile y coordinate.</param>
    /// <returns>If the tile is inside the grid.</returns>
    public bool Contains(int tileX, int tileY);

    /// <summary>
    /// Check if a tile coordinate is in the bounds of the grid.
    /// </summary>
    /// <param name="tile">the tile coordinate.</param>
    /// <returns>If the tile is inside the grid.</returns>
    public bool Contains(Point2 tile);
}