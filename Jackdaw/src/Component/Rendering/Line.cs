using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// A simple line renderer.
/// </summary>
public class LineComponent : Component {
    Line line;

    /// <summary>
    /// The line to render.
    /// </summary>
    public Line Line {
        get => line;
        set {
            line = value;
            Bounds = line.Bounds;
        }
    }

    /// <summary>
    /// The rectangular region the line occupies.
    /// </summary>
    public Rect Bounds { get; private set; }

    /// <summary>
    /// The width of the rendered line.
    /// </summary>
    public int Weight;

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
    /// <param name="weight">The width to render line with.</param>
    /// <param name="color">The color to render the line with.</param>
    public LineComponent(Game game, Vector2 point1, Vector2 point2, int weight, Color color)
        : this(game, new Line(point1, point2), weight, color) { }

    /// <summary>
    /// A simple line renderer.
    /// </summary>
    /// <param name="game">The game instance.</param>
    /// <param name="line">The line to render.</param>
    /// <param name="weight">The width to render line with.</param>
    /// <param name="color">The color to render the line with.</param>
    public LineComponent(Game game, Line line, int weight, Color color) : base(game) {
        Line = line;
        Weight = weight;
        Color = color;
    }

    protected override void Render(Batcher batcher) {
        if (!Game.Window.BoundsInPixels().Overlaps(CalcExtra.TransformRect(Bounds, Actor.Transform.GlobalDisplayMatrix))) { return; }
        batcher.Line(line.From, line.To, Weight, Color);
    }
}