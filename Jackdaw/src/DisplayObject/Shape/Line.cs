using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Display configuration for rendering a single line.
/// </summary>
public interface IDisplayObjectLine {
    /// <summary>
    /// The line to render.
    /// </summary>
    public Line Line { get; set; }
}

/// <summary>
/// A renderable single line.
/// </summary>
/// <param name="line">The line to render.</param>
/// <param name="lineWeight"></param>
public class DisplayLine(Line line, float lineWeight)
    : DisplayObject, IDisplayObjectLine, IDisplayObjectLines {
    public Line Line { get; set; } = line;
    public float LineWeight { get; set; } = lineWeight;

    public override RectInt Bounds => Line.Bounds.Int();

    public override void Render(Batcher batcher) {
        batcher.Line(Line, LineWeight, Color);
    }
}

/// <summary>
/// A renderable single dashed line.
/// </summary>
/// <param name="line">The line to render.</param>
/// <param name="lineWeight">The width of the rendered lines.</param>
/// <param name="dashLength">The length of every segment and gap pair along the line.</param>
public class DisplayLineDashed(Line line, float lineWeight, float dashLength)
    : DisplayObject, IDisplayObjectLine, IDisplayObjectDashedLines {
    public Line Line { get; set; } = line;
    public float LineWeight { get; set; } = lineWeight;
    public float DashLength { get; set; } = dashLength;
    public float OffsetPercent { get; set; } = 0;

    public override RectInt Bounds => Line.Bounds.Int();

    public override void Render(Batcher batcher) {
        batcher.LineDashed(Line, LineWeight, Color, DashLength, OffsetPercent);
    }
}