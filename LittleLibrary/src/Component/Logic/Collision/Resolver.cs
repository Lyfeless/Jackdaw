namespace LittleLib;

public class CollisionResolverComponent(LittleGame game) : Component(game) {
    public CollisionDetectorComponent Detector;

    public CollisionResolverComponent(LittleGame game, CollisionDetectorComponent detector) : this(game) {
        Detector = detector;
    }

    public override void EnterTree() {
        Game.Collision.Add(this);
    }

    public override void ExitTree() {
        Game.Collision.Remove(this);
    }
}