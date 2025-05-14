using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class GridRendererComponent(LittleGame game, Vector2 position, Grid<Sprite> grid) : Component(game) {
    RenderablePosition Position = new(position);
    Grid<Sprite> Grid = grid;

    public GridRendererComponent(LittleGame game, Vector2 position, Point2 gridSize, Vector2 tileSize)
        : this(game, position, new(gridSize, tileSize)) { }

    public override void Render(Batcher batcher) {

    }
}