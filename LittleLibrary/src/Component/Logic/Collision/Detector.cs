using System.Numerics;

namespace LittleLib;

public class CollisionDetectorComponent(LittleGame game, Collider collider) : Component(game) {
    public Collider Collider = collider;

    public TagContainer Tag = new();
    public TagContainer Mask = new();

    public record struct CollisionInfo(CollisionDetectorComponent Other, Vector2 Pushout);
    public readonly List<CollisionInfo> Collisions = [];
    public bool Collided => Collisions.Count > 0;

    public override void EnterTree() {
        Game.Collision.Add(this);
    }

    public override void ExitTree() {
        Game.Collision.Remove(this);
    }
}