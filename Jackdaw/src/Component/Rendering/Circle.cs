using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A simple circle renderer.
/// </summary>
/// <param name="game">The game instance.</param>
/// <param name="circle">The circle to render.</param>
/// <param name="steps">The number of segments to construct the shape with, higher steps makes a higher quailty circle.</param>
/// <param name="color">The color to render the circle with.</param>
public class CircleComponent(Game game, Circle circle, int steps, Color color) : Component(game) {
    Circle circle = circle;

    /// <summary>
    /// The rendering circle.
    /// </summary>
    public Circle Circle {
        get => circle;
        set {
            circle = value;
            SetBounds();
        }
    }

    /// <summary>
    /// The circle color.
    /// </summary>
    public Color Color = color;

    readonly int Steps = steps;

    public Rect Bounds { get; private set; } = GetBounds(circle);

    protected override void Render(Batcher batcher) {
        if (!Game.Window.BoundsInPixels().Overlaps(CalcExtra.TransformRect(Bounds, Actor.Position.GlobalDisplayMatrix))) { return; }
        batcher.Circle(Circle, Steps, Color);
    }

    void SetBounds() { Bounds = GetBounds(circle); }
    static Rect GetBounds(Circle circle) => Rect.Centered(circle.Position, circle.Radius * 2, circle.Radius * 2);
}