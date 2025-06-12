namespace LittleLib;

public class CollisionResolverComponent(LittleGame game) : Component(game) {
    public CollisionDetectorComponent Detector;

    public float Mass = 1;
    public float PushoutPercent = 1;

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