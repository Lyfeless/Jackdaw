using System.Numerics;

namespace Jackdaw;

/// <summary>
/// Component responsible for handling collisions with other colliders.
/// Any instances inside the game root tree automatically handle collisions checked through the manager.
/// </summary>
/// <param name="game">The current game instance.</param>
/// <param name="collider">The collider to use for collision checking.</param>
public class CollisionComponent(Game game, Collider collider) : Component(game) {
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