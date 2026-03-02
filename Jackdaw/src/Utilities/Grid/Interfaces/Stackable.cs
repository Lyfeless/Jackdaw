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
    /// <param name="gridCoord">The grid coordinate to add to.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<T> AddTileStackStart(T element, Point2 gridCoord);

    /// <summary>
    /// Add an element to the end of a tile's stack.
    /// </summary>
    /// <param name="element">The element to add to the stack.</param>
    /// <param name="gridCoord">The grid coordinate to add to.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<T> AddTileStackEnd(T element, Point2 gridCoord);

    /// <summary>
    /// Add an element at the given position in a tile's stack.
    /// </summary>
    /// <param name="element">The element to add to the stack.</param>
    /// <param name="gridCoord">The grid coordinate to add to.</param>
    /// <param name="index">The index in the tile's stack to add to.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<T> AddTileStackAt(T element, Point2 gridCoord, int index);

    /// <summary>
    /// Remove the last element from a tile's stack.
    /// </summary>
    /// <param name="gridCoord">The grid coordinate to remove from.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<T> RemoveTileStackEnd(Point2 gridCoord);

    /// <summary>
    /// Remove the first element from a tile's stack.
    /// </summary>
    /// <param name="gridCoord">The grid coordinate to remove from.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<T> RemoveTileStackStart(Point2 gridCoord);

    /// <summary>
    /// Remove an element at the given position in a tile's stack.
    /// </summary>
    /// <param name="gridCoord">The grid coordinate to remove from.</param>
    /// <param name="index">The index in the tile's stack to remove.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<T> RemoveTileStackAt(Point2 gridCoord, int index);
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
    /// <param name="gridCoord">The grid coordinate to add to.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<TIn, TOut> AddTileStackStart(TIn element, Point2 gridCoord);

    /// <summary>
    /// Add an element to the end of a tile's stack.
    /// </summary>
    /// <param name="element">The element to add to the stack.</param>
    /// <param name="gridCoord">The grid coordinate to add to.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<TIn, TOut> AddTileStackEnd(TIn element, Point2 gridCoord);

    /// <summary>
    /// Add an element at the given position in a tile's stack.
    /// </summary>
    /// <param name="element">The element to add to the stack.</param>
    /// <param name="gridCoord">The grid coordinate to add to.</param>
    /// <param name="index">The index in the tile's stack to add to.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<TIn, TOut> AddTileStackAt(TIn element, Point2 gridCoord, int index);

    /// <summary>
    /// Remove the last element from a tile's stack.
    /// </summary>
    /// <param name="gridCoord">The grid coordinate to remove from.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<TIn, TOut> RemoveTileStackEnd(Point2 gridCoord);

    /// <summary>
    /// Remove the first element from a tile's stack.
    /// </summary>
    /// <param name="gridCoord">The grid coordinate to remove from.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<TIn, TOut> RemoveTileStackStart(Point2 gridCoord);

    /// <summary>
    /// Remove an element at the given position in a tile's stack.
    /// </summary>
    /// <param name="gridCoord">The grid coordinate to remove from.</param>
    /// <param name="index">The index in the tile's stack to remove.</param>
    /// <returns>The containing grid.</returns>
    public IStackableGrid<TIn, TOut> RemoveTileStackAt(Point2 gridCoord, int index);
}