using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A generic utility for storing 2D rectangular grids of elements.
/// </summary>
/// <typeparam name="T">The type of tile to store in the grid.</typeparam>
/// <param name="gridSize">The grid dimensions.</param>
public class Grid<T>(Point2 gridSize) : IGrid<T> {
    readonly T?[,] Tiles = new T?[gridSize.X, gridSize.Y];

    readonly Point2 size = gridSize;
    public Point2 TileCount => size;

    public Grid(int width, int height) : this(new(width, height)) { }

    public T? Get(int tileX, int tileY) => Get(new(tileX, tileY));
    public T? Get(Point2 tile) {
        if (Contains(tile)) { return Tiles[tile.X, tile.Y]; }
        return default;
    }

    public IGrid<T> Set(T? value, int tileX, int tileY) => Set(value, new(tileX, tileY));
    public IGrid<T> Set(T? value, Point2 tile) {
        if (Contains(tile)) { Tiles[tile.X, tile.Y] = value; }
        return this;
    }

    public bool Contains(int tileX, int tileY) => Contains(new(tileX, tileY));
    public bool Contains(Point2 tile) {
        return tile.X >= 0 && tile.Y >= 0 && tile.X < TileCount.X && tile.Y < TileCount.Y;
    }
}