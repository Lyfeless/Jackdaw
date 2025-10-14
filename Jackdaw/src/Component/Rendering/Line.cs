using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A simple line renderer.
/// </summary>
public class LineComponent : Component {
    Line line;
    Rect bounds;

    /// <summary>
    /// The line to render.
    /// </summary>
    public Line Line {
        get => line;
        set {
            line = value;
            bounds = line.Bounds;
        }
    }

    /// <summary>
    /// The width of the rendered line.
    /// </summary>
    public int LineWeight;

    /// <summary>
    /// The line color.
    /// </summary>
    public Color Color;

    /// <summary>
    /// A simple line renderer.
    /// </summary>
    /// <param name="game">The game instance.</param>
    /// <param name="point1">The line start position.</param>
    /// <param name="point2">The line end position.</param>
    /// <param name="lineWeight">The width to render line with.</param>
    /// <param name="color">The color to render the line with.</param>
    public LineComponent(Game game, Vector2 point1, Vector2 point2, int lineWeight, Color color) : base(game) {
        line = new(point1, point2);
        bounds = line.Bounds;
        LineWeight = lineWeight;
        Color = color;
    }

    protected override void Render(Batcher batcher) {
        if (!Game.Window.BoundsInPixels().Overlaps(CalcExtra.TransformRect(bounds, Actor.Position.GlobalDisplayMatrix))) { return; }
        batcher.Line(line.From, line.To, LineWeight, Color);
    }
}