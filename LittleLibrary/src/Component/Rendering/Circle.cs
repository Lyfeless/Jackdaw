using System.Numerics;
using Foster.Framework;

namespace LittleLib;

//! FIXME (Alex): Should this be a sprite?

/// <summary>
/// A simple circle renderer.
/// </summary>
/// <param name="game">The game instance.</param>
/// <param name="circle">The circle to render.</param>
/// <param name="steps">The number of segments to construct the shape with, higher steps makes a higher quailty circle.</param>
/// <param name="color">The color to render the circle with.</param>
public class CircleComponent(LittleGame game, Circle circle, int steps, Color color) : Component(game) {
    /// <summary>
    /// The rendering circle.
    /// </summary>
    public Circle Circle = circle;

    /// <summary>
    /// The circle color.
    /// </summary>
    public Color Color = color;

    readonly int Steps = steps;

    protected override void Render(Batcher batcher) {
        //! FIXME (Alex): Verify culling is correct
        //! FIXME (Alex): Disabled because of viewport changes
        // if (!Game.Viewspace.Bounds.Overlaps(new Rect(Actor.GlobalPosition + Circle.Position - new Vector2(Circle.Radius), new(Circle.Radius * 2)))) { return; }
        batcher.Circle(Circle, Steps, Color);
    }
}