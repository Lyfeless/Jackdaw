using System.Numerics;
using Foster.Framework;

namespace LittleLib;

public class CircleComponent(LittleGame game, Circle circle, int steps, Color color) : Component(game) {
    public Circle Circle = circle;
    readonly int Steps = steps;
    public Color Color = color;

    public override void Render(Batcher batcher) {
        //! FIXME (Alex): Verify culling is correct
        if (!Game.Viewspace.Bounds.Overlaps(new Rect(Actor.GlobalPosition + Circle.Position - new Vector2(Circle.Radius), new(Circle.Radius * 2)))) { return; }
        batcher.Circle(Circle, Steps, Color);
    }
}