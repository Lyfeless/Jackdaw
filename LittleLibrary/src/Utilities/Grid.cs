using Foster.Framework;

namespace LittleLib;

public class Grid<T>(Point2 gridSize) {
    public readonly Point2 Size = gridSize;
    readonly T?[,] Tiles = new T?[gridSize.X, gridSize.Y];

    public Grid(int width, int height) : this(new(width, height)) { }

    public T? Get(int tileX, int tileY) => Get(new(tileX, tileY));
    public T? Get(Point2 tile) {
        if (Contains(tile)) { return Tiles[tile.X, tile.Y]; }
        //! FIXME (Alex): Verify if this returns null for int? values
        return default;
    }

    public Grid<T> Set(T? value, int tileX, int tileY) => Set(value, new(tileX, tileY));
    public Grid<T> Set(T? value, Point2 tile) {
        if (Contains(tile)) { Tiles[tile.X, tile.Y] = value; }
        return this;
    }

    bool Contains(Point2 tile) {
        return tile.X >= 0 && tile.Y >= 0 && tile.X < Size.X && tile.Y < Size.Y;
    }

    //! FIXME (Alex): Needs extra utility functions
    //      Get start and end tiles contained within rect
}