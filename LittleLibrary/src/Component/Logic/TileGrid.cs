using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class TileGridComponent(LittleGame game, Vector2 position, Point2 gridSize, Vector2 tileSize) : Component(game) {
    Vector2 Position = position;
    CollisionDetectorComponent Collider = new(game, new GridCollider(position, gridSize, tileSize));
    GridRendererComponent Renderer = new(game, position, gridSize, tileSize);

    Vector2 oldPosition = position;

    public override void Added() {
        Actor.Components.Add(Collider);
        Actor.Components.Add(Renderer);
    }

    public override void Removed() {
        Actor.Components.Remove(Collider);
        Actor.Components.Remove(Renderer);
    }

    public override void Update() {
        if (oldPosition != Position) {
            //! FIXME (Alex): Update positions
        }
    }
}