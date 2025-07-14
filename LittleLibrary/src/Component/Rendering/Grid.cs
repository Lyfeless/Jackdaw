using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class GridRendererComponent(LittleGame game, Point2 position, Grid<Sprite> grid, Point2 tileSize) : Component(game), ISpatialGrid<Sprite?, Sprite?> {
    Grid<Sprite> Grid = grid;

    Point2 position = position;
    public Vector2 Position {
        get => position;
        set => position = (Point2)value;
    }

    Point2 tileSize = tileSize;
    public Vector2 TileSize {
        get => tileSize;
        set => tileSize = (Point2)value;
    }

    public GridRendererComponent(LittleGame game, Point2 position, Point2 gridSize, Point2 tileSize)
        : this(game, position, new Grid<Sprite>(gridSize), tileSize) { }

    public override void Render(Batcher batcher) {
        //! FIXME (Alex): NOT DONE! Needs culling
        for (int x = 0; x < Grid.Size.X; ++x) {
            for (int y = 0; y < Grid.Size.Y; ++y) {
                Sprite? sprite = Grid.Get(x, y);
                if (sprite == null) { continue; }
                batcher.PushMatrix((TileSize * new Point2(x, y)) + Position);
                sprite.Render(batcher);
                batcher.PopMatrix();
            }
        }
    }

    public void SetTile(Sprite? sprite, Point2 gridCoord) {
        Grid.Set(sprite, gridCoord);
    }
    public void AddTileStack(Sprite? sprite, Point2 gridCoord) {
        if (sprite == null) { return; }
        Sprite? current = Grid.Get(gridCoord);
        if (current == null) { Grid.Set(sprite, gridCoord); return; }
        if (current is SpriteStack currentStack) { Grid.Set(new SpriteStack([.. currentStack.Sprites, sprite]), gridCoord); return; }
        Grid.Set(new SpriteStack(current, sprite), gridCoord);
    }
    public void RemoveTileStack(Point2 gridCoord) {
        Sprite? current = Grid.Get(gridCoord);
        if (current == null) { return; }
        if (current is not SpriteStack currentStack) { Grid.Set(null, gridCoord); return; }
        Grid.Set(new SpriteStack(currentStack.Sprites[..^1]), gridCoord);
    }

    public void ClearTile(Point2 gridCoord) {
        Grid.Set(null, gridCoord);
    }

    public Sprite? GetTile(Point2 gridCoord) {
        return Grid.Get(gridCoord);
    }
}