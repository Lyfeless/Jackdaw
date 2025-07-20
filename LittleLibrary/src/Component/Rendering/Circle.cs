using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class CircleComponent(LittleGame game, Circle circle, int steps, Color color) : Component(game) {
    Circle Circle = circle;
    int Steps = steps;
    Color Color = color;

    public override void Render(Batcher batcher) {
        //! FIXME (Alex): Verify culling is correct
        if (!Game.Viewspace.Bounds.Overlaps(new Rect(Actor.GlobalPosition + Circle.Position - new Vector2(Circle.Radius), new(Circle.Radius * 2)))) { return; }
        batcher.Circle(Circle, Steps, Color);
    }
}