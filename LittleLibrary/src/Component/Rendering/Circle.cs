using Foster.Framework;

namespace LittleLib;

public class CircleComponent(LittleGame game, Circle circle, int steps, Color color) : Component(game) {
    Circle Circle = circle;
    int Steps = steps;
    Color Color = color;

    public override void Render(Batcher batcher) {
        batcher.Circle(Circle, Steps, Color);
    }
}