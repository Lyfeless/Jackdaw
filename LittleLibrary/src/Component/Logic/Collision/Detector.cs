namespace LittleLib;

public class CollisionDetectorComponent(LittleGame game, Collider collider) : Component(game) {
    Collider Collider = collider;

    public override void EnterTree() {
        Game.Collision.Add(this);
    }

    public override void ExitTree() {
        Game.Collision.Remove(this);
    }
}