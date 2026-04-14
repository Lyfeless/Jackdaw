using System.Numerics;

namespace Jackdaw;

/// <summary>
/// Component responsible for handling collisions with other colliders.
/// Any ticking instances inside the game root tree automatically handle collisions checked through the manager. <br/>
/// Not visible by default.
/// </summary>
public class CollisionComponent : Component {
    /// <summary>
    /// The current collider.
    /// </summary>
    public Collider Collider;

    /// <param name="game">The current game instance.</param>
    /// <param name="collider">The collider to use for collision checking.</param>
    public CollisionComponent(Game game, Collider collider) : base(game) {
        Collider = collider;
        Visible = false;
    }

    protected override void EnterTree() {
        Game.Collision.Add(this);
    }

    protected override void ExitTree() {
        Game.Collision.Remove(this);
    }
}