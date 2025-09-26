using System.Numerics;

namespace LittleLib;

/// <summary>
/// Component responsible for handling collisions with other colliders.
/// Any instances inside the game root tree automatically handle collisions checked through the manager.
/// </summary>
/// <param name="game">The current game instance.</param>
/// <param name="collider">The collider to use for collision checking.</param>
public class CollisionComponent(LittleGame game, Collider collider) : Component(game) {
    public record struct CollisionInfo(CollisionComponent Other, Vector2 Pushout);

    /// <summary>
    /// The current collider.
    /// </summary>
    public Collider Collider = collider;

    protected override void EnterTree() {
        Game.Collision.Add(this);
    }

    protected override void ExitTree() {
        Game.Collision.Remove(this);
    }
}