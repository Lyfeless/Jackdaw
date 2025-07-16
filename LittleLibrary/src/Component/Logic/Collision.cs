using System.Numerics;

namespace LittleLib;

public class CollisionComponent(LittleGame game, Collider collider) : Component(game) {
    public record struct CollisionInfo(CollisionComponent Other, Vector2 Pushout);

    public Collider Collider = collider;

    public CollisionResolver? Resolver;
    public VelocityComponent? Velocity;

    public readonly List<CollisionInfo> Collisions = [];
    public bool Collided => Collisions.Count > 0;

    public override void EnterTree() {
        Game.Collision.Add(this);
    }

    public override void ExitTree() {
        Game.Collision.Remove(this);
    }
}

public struct CollisionResolver() {
    public float Mass = 100;
    public float PushoutPercent = 1;
}