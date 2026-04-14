using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Continuous raycast check. Collisions object updates with collision results every tick. <br/>
/// Not visible by default.
/// </summary>
public class RaycastComponent : Component {
    public Ray Ray;

    /// <summary>
    /// Mask for what colliders the ray should interact with. Leave empty to interact with everything.
    /// </summary>
    public TagContainer Mask = new();

    /// <summary>
    /// Collision result for the tick.
    /// </summary>
    public CollisionResult Collisions;

    /// <param name="game">The current game instance.</param>
    /// <param name="position">The ray origin.</param>
    /// <param name="direction">The ray direction vector.</param>
    public RaycastComponent(Game game, Vector2 position, Vector2 direction) : base(game) {
        Ray = new(position, direction);
        Visible = true;
    }

    protected override void Update() {
        Collisions = Game.Collision.GetRayCollision(this);
    }
}