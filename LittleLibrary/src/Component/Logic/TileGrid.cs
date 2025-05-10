using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class TileGridComponent(LittleGame game, Vector2 position, Point2 gridSize, Vector2 tileSize) : Component(game) {
    Vector2 Position = position;
    GridCollider collider = new(position, gridSize, tileSize);
    GridRendererComponent renderer = new(game, position, gridSize, tileSize);

    //! FIXME (Alex): Don't run update or render for these, just handle adding/removing the components when the controller (this) changes parenting
}