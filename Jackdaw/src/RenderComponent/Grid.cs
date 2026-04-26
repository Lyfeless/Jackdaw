using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A renderable grid of sprites. <br/>
/// Not ticking by default.
/// </summary>
public class GridRenderComponent : Component, IStackableGrid<Sprite?>, ISpatialGrid {
    protected readonly Grid<Sprite> Grid;

    protected Point2 position;
    protected Point2 tileSize;

    /// <summary>
    /// The grid position.
    /// </summary>
    public Vector2 Position {
        get => position;
        set => position = value.FloorToPoint2();
    }

    /// <summary>
    /// The size of each tile.
    /// </summary>
    public Vector2 TileSize {
        get => tileSize;
        set => tileSize = value.FloorToPoint2();
    }

    public Point2 TileCount => Grid.TileCount;

    public Rect Bounds => new(Position, Grid.TileCount * TileSize);

    /// <param name="game">The current game instance.</param>
    /// <param name="position">The grid position.</param>
    /// <param name="grid">The grid instance.</param>
    /// <param name="tileSize">The size of each tile.</param>
    public GridRenderComponent(Game game, Point2 position, Grid<Sprite> grid, Point2 tileSize) : base(game) {
        Grid = grid;
        this.position = position;
        this.tileSize = tileSize;
        Ticking = false;
    }

    /// <summary>
    /// A renderable grid of sprites.
    /// </summary>
    /// <param name="game">The current game instance.</param>
    /// <param name="position">The grid position.</param>
    /// <param name="gridSize">The grid tile dimensions.</param>
    /// <param name="tileSize">The size of each tile.</param>
    public GridRenderComponent(Game game, Point2 position, Point2 gridSize, Point2 tileSize)
        : this(game, position, new Grid<Sprite>(gridSize), tileSize) { }

    protected override void Render(Batcher batcher) {
        if (TileCount.X == 0 || TileCount.Y == 0) { return; }

        Rect bounds = new Rect(Game.Window.Width, Game.Window.Height).TransformAABB(Actor.Transform.GlobalDisplayMatrixInverse);

        Vector2 topLeft = Game.Convert.LocalToTile(bounds.TopLeft, this);
        Vector2 bottomRight = Game.Convert.LocalToTile(bounds.BottomRight, this);

        Point2 start = new(
            (int)Math.Clamp(topLeft.X, 0, Grid.TileCount.X - 1),
            (int)Math.Clamp(topLeft.Y, 0, Grid.TileCount.Y - 1)
        );
        Point2 end = new(
            (int)Math.Clamp(bottomRight.X, 0, Grid.TileCount.X - 1),
            (int)Math.Clamp(bottomRight.Y, 0, Grid.TileCount.Y - 1)
        );

        for (int x = start.X; x <= end.X; ++x) {
            for (int y = start.Y; y <= end.Y; ++y) {
                Sprite? sprite = Grid.Get(x, y);
                if (sprite == null) { continue; }
                batcher.PushMatrix((TileSize * new Point2(x, y)) + Position);
                sprite.Render(batcher);
                batcher.PopMatrix();
            }
        }
    }

    public IGrid<Sprite?> Set(Sprite? element, int tileX, int tileY) => Set(element, new(tileX, tileY));
    public IGrid<Sprite?> Set(Sprite? element, Point2 tile) { Grid.Set(element, tile); return this; }
    public Sprite? Get(int tileX, int tileY) => Get(new(tileX, tileY));
    public Sprite? Get(Point2 tile) => Grid.Get(tile);
    public bool Contains(int tileX, int tileY) => Contains(new(tileX, tileY));
    public bool Contains(Point2 tile) => Grid.Contains(tile);

    public IStackableGrid<Sprite?> AddTileStackStart(Sprite? element, int tileX, int tileY) => AddTileStackStart(element, new(tileX, tileY));
    public IStackableGrid<Sprite?> AddTileStackStart(Sprite? element, Point2 gridCoord) => AddTileStackAt(element, gridCoord, 0);
    public IStackableGrid<Sprite?> AddTileStackEnd(Sprite? element, int tileX, int tileY) => AddTileStackEnd(element, new(tileX, tileY));
    public IStackableGrid<Sprite?> AddTileStackEnd(Sprite? element, Point2 gridCoord) => AddTileStackAt(element, gridCoord, -1);
    public IStackableGrid<Sprite?> AddTileStackAt(Sprite? element, int tileX, int tileY, int index) => AddTileStackAt(element, new(tileX, tileY), index);
    public IStackableGrid<Sprite?> AddTileStackAt(Sprite? element, Point2 gridCoord, int index) {
        if (element == null) { return this; }

        Sprite? current = Grid.Get(gridCoord);
        if (current == null) { Grid.Set(element, gridCoord); return this; }

        List<Sprite> sprites = current is SpriteStack currentStack ? [.. currentStack.Sprites] : [current];

        if (index == -1) { index = sprites.Count; }
        else { index = Calc.Clamp(index, 0, sprites.Count); }

        sprites.Insert(index, element);
        Grid.Set(new SpriteStack([.. sprites]), gridCoord);

        return this;
    }

    public IStackableGrid<Sprite?> RemoveTileStackStart(int tileX, int tileY) => RemoveTileStackStart(new(tileX, tileY));
    public IStackableGrid<Sprite?> RemoveTileStackStart(Point2 gridCoord) => RemoveTileStackAt(gridCoord, 0);
    public IStackableGrid<Sprite?> RemoveTileStackEnd(int tileX, int tileY) => RemoveTileStackEnd(new(tileX, tileY));
    public IStackableGrid<Sprite?> RemoveTileStackEnd(Point2 gridCoord) => RemoveTileStackAt(gridCoord, -1);
    public IStackableGrid<Sprite?> RemoveTileStackAt(int tileX, int tileY, int index) => RemoveTileStackAt(new(tileX, tileY), index);
    public IStackableGrid<Sprite?> RemoveTileStackAt(Point2 gridCoord, int index) {
        Sprite? current = Grid.Get(gridCoord);
        if (current == null) { return this; }

        if (current is not SpriteStack currentStack) {
            if (index == 0 || index == -1) { Grid.Set(null, gridCoord); }
            return this;
        }

        if (index == -1) { index = currentStack.Sprites.Length - 1; }
        if (index < 0 || index <= currentStack.Sprites.Length) { return this; }

        List<Sprite> sprites = [.. currentStack.Sprites];
        sprites.RemoveAt(index);

        if (sprites.Count == 0) { Grid.Set(null, gridCoord); }
        else if (sprites.Count == 1) { Grid.Set(sprites[0], gridCoord); }
        else { Grid.Set(new SpriteStack([.. sprites]), gridCoord); }

        return this;
    }
}