using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A grid with tiles that can be stacked ontop of one another.
/// </summary>
/// <typeparam name="T">The type of object tiles are stored as.</typeparam>
public interface IStackableGrid<T> : IGrid<T> {
    /// <summary>
    /// Add an element to the start of a tile's stack.
    /// </summary>
    /// <param name="element">The element to add to the stack.</param>
    /// <param name="tileX">The tile x coordinate.</param>
    /// <param name="tileY">The tile y coordinate.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<T> AddTileStackStart(T element, int tileX, int tileY);

    /// <summary>
    /// Add an element to the start of a tile's stack.
    /// </summary>
    /// <param name="element">The element to add to the stack.</param>
    /// <param name="tile">The tile to add to.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<T> AddTileStackStart(T element, Point2 tile);

    /// <summary>
    /// Add an element to the end of a tile's stack.
    /// </summary>
    /// <param name="element">The element to add to the stack.</param>
    /// <param name="tileX">The tile x coordinate.</param>
    /// <param name="tileY">The tile y coordinate.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<T> AddTileStackEnd(T element, int tileX, int tileY);

    /// <summary>
    /// Add an element to the end of a tile's stack.
    /// </summary>
    /// <param name="element">The element to add to the stack.</param>
    /// <param name="tile">The tile to add to.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<T> AddTileStackEnd(T element, Point2 tile);

    /// <summary>
    /// Add an element at the given position in a tile's stack.
    /// </summary>
    /// <param name="element">The element to add to the stack.</param>
    /// <param name="tileX">The tile x coordinate.</param>
    /// <param name="tileY">The tile y coordinate.</param>
    /// <param name="index">The index in the tile's stack to add to.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<T> AddTileStackAt(T element, int tileX, int tileY, int index);

    /// <summary>
    /// Add an element at the given position in a tile's stack.
    /// </summary>
    /// <param name="element">The element to add to the stack.</param>
    /// <param name="tile">The tile to add to.</param>
    /// <param name="index">The index in the tile's stack to add to.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<T> AddTileStackAt(T element, Point2 tile, int index);

    /// <summary>
    /// Remove the last element from a tile's stack.
    /// </summary>
    /// <param name="tileX">The tile x coordinate.</param>
    /// <param name="tileY">The tile y coordinate.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<T> RemoveTileStackEnd(int tileX, int tileY);

    /// <summary>
    /// Remove the last element from a tile's stack.
    /// </summary>
    /// <param name="tile">The tile to remove from.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<T> RemoveTileStackEnd(Point2 tile);

    /// <summary>
    /// Remove the first element from a tile's stack.
    /// </summary>
    /// <param name="tileX">The tile x coordinate.</param>
    /// <param name="tileY">The tile y coordinate.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<T> RemoveTileStackStart(int tileX, int tileY);

    /// <summary>
    /// Remove the first element from a tile's stack.
    /// </summary>
    /// <param name="tile">The tile to remove from.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<T> RemoveTileStackStart(Point2 tile);

    /// <summary>
    /// Remove an element at the given position in a tile's stack.
    /// </summary>
    /// <param name="tileX">The tile x coordinate.</param>
    /// <param name="tileY">The tile y coordinate.</param>
    /// <param name="index">The index in the tile's stack to remove.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<T> RemoveTileStackAt(int tileX, int tileY, int index);

    /// <summary>
    /// Remove an element at the given position in a tile's stack.
    /// </summary>
    /// <param name="tile">The tile to remove from.</param>
    /// <param name="index">The index in the tile's stack to remove.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<T> RemoveTileStackAt(Point2 tile, int index);
}

/// <summary>
/// A grid with tiles that can be stacked ontop of one another.
/// </summary>
/// <typeparam name="TIn">The type of object used to assign tiles.</typeparam>
/// <typeparam name="TOut">The type of object the tiles are stored as.</typeparam>
public interface IStackableGrid<TIn, TOut> : IGrid<TIn, TOut> {
    /// <summary>
    /// Add an element to the start of a tile's stack.
    /// </summary>
    /// <param name="element">The element to add to the stack.</param>
    /// <param name="tileX">The tile x coordinate.</param>
    /// <param name="tileY">The tile y coordinate.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<TIn, TOut> AddTileStackStart(TIn element, int tileX, int tileY);

    /// <summary>
    /// Add an element to the start of a tile's stack.
    /// </summary>
    /// <param name="element">The element to add to the stack.</param>
    /// <param name="tile">The tile to add to.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<TIn, TOut> AddTileStackStart(TIn element, Point2 tile);

    /// <summary>
    /// Add an element to the end of a tile's stack.
    /// </summary>
    /// <param name="element">The element to add to the stack.</param>
    /// <param name="tileX">The tile x coordinate.</param>
    /// <param name="tileY">The tile y coordinate.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<TIn, TOut> AddTileStackEnd(TIn element, int tileX, int tileY);

    /// <summary>
    /// Add an element to the end of a tile's stack.
    /// </summary>
    /// <param name="element">The element to add to the stack.</param>
    /// <param name="tile">The tile to add to.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<TIn, TOut> AddTileStackEnd(TIn element, Point2 tile);

    /// <summary>
    /// Add an element at the given position in a tile's stack.
    /// </summary>
    /// <param name="element">The element to add to the stack.</param>
    /// <param name="tileX">The tile x coordinate.</param>
    /// <param name="tileY">The tile y coordinate.</param>
    /// <param name="index">The index in the tile's stack to add to.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<TIn, TOut> AddTileStackAt(TIn element, int tileX, int tileY, int index);

    /// <summary>
    /// Add an element at the given position in a tile's stack.
    /// </summary>
    /// <param name="element">The element to add to the stack.</param>
    /// <param name="tile">The tile to add to.</param>
    /// <param name="index">The index in the tile's stack to add to.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<TIn, TOut> AddTileStackAt(TIn element, Point2 tile, int index);

    /// <summary>
    /// Remove the last element from a tile's stack.
    /// </summary>
    /// <param name="tileX">The tile x coordinate.</param>
    /// <param name="tileY">The tile y coordinate.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<TIn, TOut> RemoveTileStackEnd(int tileX, int tileY);

    /// <summary>
    /// Remove the last element from a tile's stack.
    /// </summary>
    /// <param name="tile">The tile to remove from.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<TIn, TOut> RemoveTileStackEnd(Point2 tile);

    /// <summary>
    /// Remove the first element from a tile's stack.
    /// </summary>
    /// <param name="tileX">The tile x coordinate.</param>
    /// <param name="tileY">The tile y coordinate.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<TIn, TOut> RemoveTileStackStart(int tileX, int tileY);

    /// <summary>
    /// Remove the first element from a tile's stack.
    /// </summary>
    /// <param name="tile">The tile to remove from.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<TIn, TOut> RemoveTileStackStart(Point2 tile);

    /// <summary>
    /// Remove an element at the given position in a tile's stack.
    /// </summary>
    /// <param name="tileX">The tile x coordinate.</param>
    /// <param name="tileY">The tile y coordinate.</param>
    /// <param name="index">The index in the tile's stack to remove.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<TIn, TOut> RemoveTileStackAt(int tileX, int tileY, int index);

    /// <summary>
    /// Remove an element at the given position in a tile's stack.
    /// </summary>
    /// <param name="tile">The tile to remove from.</param>
    /// <param name="index">The index in the tile's stack to remove.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<TIn, TOut> RemoveTileStackAt(Point2 tile, int index);
}