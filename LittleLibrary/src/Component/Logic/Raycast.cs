using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class RaycastComponent(LittleGame game, Vector2 position, Vector2 direction) : Component(game) {
    public Ray Ray = new(position, direction);

    public TagContainer Mask = new();

    public CollisionManager.SweptCollisionResult Collisions;

    public override void Update() {
        Collisions = Game.Collision.GetRayCollision(this);
    }
}