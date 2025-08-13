using System.Numerics;
using Foster.Framework;

namespace LittleLib;

/// <summary>
/// Continuous raycast check. Collisions object updates with collision results every tick.
/// </summary>
/// <param name="game">The current game instance.</param>
/// <param name="position">The ray origin.</param>
/// <param name="direction">The ray direction vector.</param>
public class RaycastComponent(LittleGame game, Vector2 position, Vector2 direction) : Component(game) {
    public Ray Ray = new(position, direction);

    /// <summary>
    /// Mask for what colliders the ray should interact with. Leave empty to interact with everything.
    /// </summary>
    public TagContainer Mask = new();

    /// <summary>
    /// Collision result for the tick.
    /// </summary>
    public SweptCollisionInfo Collisions;

    public override void Update() {
        Collisions = Game.Collision.GetRayCollision(this);
    }
}