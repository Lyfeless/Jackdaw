using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A renderable grid of sprites.
/// </summary>
/// <param name="game">The current game instance.</param>
/// <param name="position">The grid position.</param>
/// <param name="grid">The grid instance.</param>
/// <param name="tileSize">The size of each tile.</param>
public class GridRendererComponent(Game game, Point2 position, Grid<Sprite> grid, Point2 tileSize) : Component(game), ISpatialGrid<Sprite?, Sprite?> {
    protected readonly Grid<Sprite> Grid = grid;

    protected Point2 position = position;

    /// <summary>
    /// The grid position.
    /// </summary>
    public Vector2 Position {
        get => position;
        set => position = (Point2)value;
    }

    protected Point2 tileSize = tileSize;

    /// <summary>
    /// The size of each tile.
    /// </summary>
    public Vector2 TileSize {
        get => tileSize;
        set => tileSize = (Point2)value;
    }

    /// <summary>
    /// A renderable grid of sprites.
    /// </summary>
    /// <param name="game">The current game instance.</param>
    /// <param name="position">The grid position.</param>
    /// <param name="gridSize">The grid tile dimensions.</param>
    /// <param name="tileSize">The size of each tile.</param>
    public GridRendererComponent(Game game, Point2 position, Point2 gridSize, Point2 tileSize)
        : this(game, position, new Grid<Sprite>(gridSize), tileSize) { }

    protected override void Render(Batcher batcher) {
        Rect bounds = new Rect(Game.Window.Width, Game.Window.Height).TransformAABB(Actor.Transform.GlobalDisplayMatrixInverse);

        Vector2 topLeft = Game.Convert.LocalToTile(bounds.TopLeft, this);
        Vector2 bottomRight = Game.Convert.LocalToTile(bounds.BottomRight, this);

        Point2 start = new(
            (int)Math.Clamp(topLeft.X, 0, Grid.Size.X - 1),
            (int)Math.Clamp(topLeft.Y, 0, Grid.Size.Y - 1)
        );
        Point2 end = new(
            (int)Math.Clamp(bottomRight.X, 0, Grid.Size.X - 1),
            (int)Math.Clamp(bottomRight.Y, 0, Grid.Size.Y - 1)
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

    /// <summary>
    /// Set the sprite for a tile.
    /// </summary>
    /// <param name="sprite">The sprite to set.</param>
    /// <param name="gridCoord">The tile location.</param>
    /// <returns></returns>
    public void SetTile(Sprite? sprite, Point2 gridCoord) {
        Grid.Set(sprite, gridCoord);
    }

    /// <summary>
    /// Add a sprite onto a tile, combining it with other existing colliders.
    /// </summary>
    /// <param name="sprite">The collider to add.</param>
    /// <param name="gridCoord">The tile location.</param>
    /// <returns></returns>
    public void AddTileStack(Sprite? sprite, Point2 gridCoord) {
        if (sprite == null) { return; }
        Sprite? current = Grid.Get(gridCoord);
        if (current == null) { Grid.Set(sprite, gridCoord); return; }
        if (current is SpriteStack currentStack) { Grid.Set(new SpriteStack([.. currentStack.Sprites, sprite]), gridCoord); return; }
        Grid.Set(new SpriteStack(current, sprite), gridCoord);
    }

    /// <summary>
    /// Remove the most recent sprite from a tile's stack.
    /// </summary>
    /// <param name="gridCoord">The tile location.</param>
    /// <returns></returns>
    public void RemoveTileStack(Point2 gridCoord) {
        Sprite? current = Grid.Get(gridCoord);
        if (current == null) { return; }
        if (current is not SpriteStack currentStack) { Grid.Set(null, gridCoord); return; }
        Grid.Set(new SpriteStack(currentStack.Sprites[..^1]), gridCoord);
    }

    /// <summary>
    /// Remove all sprites from a tile.
    /// </summary>
    /// <param name="gridCoord">The tile location.</param>
    public void ClearTile(Point2 gridCoord) {
        Grid.Set(null, gridCoord);
    }

    /// <summary>
    /// Get the sprite at a grid coordinate.
    /// </summary>
    /// <param name="gridCoord">The tile location.</param>
    /// <returns>The sprite at the given location, null if the coord is out of bounds or the tile doesn't exist.</returns>
    public Sprite? GetTile(Point2 gridCoord) {
        return Grid.Get(gridCoord);
    }
}