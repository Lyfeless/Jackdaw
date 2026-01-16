using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A simple circle renderer.
/// </summary>
/// <param name="game">The game instance.</param>
/// <param name="circle">The circle to render.</param>
/// <param name="steps">The number of segments to construct the shape with, higher steps makes a higher quailty circle.</param>
/// <param name="color">The color to render the circle with.</param>
public class CircleRenderComponent(Game game, Circle circle, int steps, Color color) : Component(game) {
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

    /// <summary>
    /// The circle's render resolution.
    /// More steps will result in a smoother result but will take longer to render.
    /// </summary>
    public int Steps = steps;

    /// <summary>
    /// The rectangular region the circle occupies.
    /// </summary>
    public Rect Bounds { get; private set; } = circle.Bounds;

    protected override void Render(Batcher batcher) {
        if (!Game.Window.BoundsInPixels().Overlaps(Bounds.TransformAABB(Actor.Transform.GlobalDisplayMatrix))) { return; }
        batcher.Circle(Circle, Steps, Color);
    }

    void SetBounds() { Bounds = circle.Bounds; }
}