using System.Numerics;
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
/// <param name="lineWeight">The width of the rendered line.</param>
public class DisplayLine(Line line, float lineWeight)
    : DisplayObject, IDisplayObjectLine, IDisplayObjectOutline {
    public Line Line { get; set; } = line;
    public float LineWeight { get; set; } = lineWeight;

    public override RectInt Bounds => Line.Bounds.Int();

    /// <summary>
    /// A renderable single line.
    /// </summary>
    /// <param name="start">The start position of the line.</param>
    /// <param name="end">The end position of the line.</param>
    /// <param name="lineWeight">The width of the rendered line.</param>
    public DisplayLine(Vector2 start, Vector2 end, float lineWeight)
        : this(new Line(start, end), lineWeight) { }

    /// <summary>
    /// A renderable single line.
    /// </summary>
    /// <param name="start">The start position of the line.</param>
    /// <param name="end">The end position of the line.</param>
    /// <param name="lineWeight">The width of the rendered line.</param>
    public DisplayLine(Point2 start, Point2 end, float lineWeight)
        : this(new Line(start, end), lineWeight) { }

    /// <summary>
    /// A renderable single line.
    /// </summary>
    /// <param name="startX">The x start position of the line.</param>
    /// <param name="startY">The y start position of the line.</param>
    /// <param name="endX">The x end position of the line.</param>
    /// <param name="endY">The y end position of the line.</param>
    /// <param name="lineWeight">The width of the rendered line.</param>
    public DisplayLine(float startX, float startY, float endX, float endY, float lineWeight)
        : this(new Line(startX, startY, endX, endY), lineWeight) { }

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
    : DisplayObject, IDisplayObjectLine, IDisplayObjectDashedOutline {
    public Line Line { get; set; } = line;
    public float LineWeight { get; set; } = lineWeight;
    public float DashLength { get; set; } = dashLength;
    public float OffsetPercent { get; set; } = 0;

    public override RectInt Bounds => Line.Bounds.Int();

    /// <summary>
    /// A renderable single dashed line.
    /// </summary>
    /// <param name="start">The start position of the line.</param>
    /// <param name="end">The end position of the line.</param>
    /// <param name="lineWeight">The width of the rendered line.</param>
    /// <param name="dashLength">The length of every segment and gap pair along the line.</param>
    public DisplayLineDashed(Vector2 start, Vector2 end, float lineWeight, float dashLength)
        : this(new Line(start, end), lineWeight, dashLength) { }

    /// <summary>
    /// A renderable single dashed line.
    /// </summary>
    /// <param name="start">The start position of the line.</param>
    /// <param name="end">The end position of the line.</param>
    /// <param name="lineWeight">The width of the rendered line.</param>
    /// <param name="dashLength">The length of every segment and gap pair along the line.</param>
    public DisplayLineDashed(Point2 start, Point2 end, float lineWeight, float dashLength)
        : this(new Line(start, end), lineWeight, dashLength) { }

    /// <summary>
    /// A renderable single dashed line.
    /// </summary>
    /// <param name="startX">The x start position of the line.</param>
    /// <param name="startY">The y start position of the line.</param>
    /// <param name="endX">The x end position of the line.</param>
    /// <param name="endY">The y end position of the line.</param>
    /// <param name="lineWeight">The width of the rendered line.</param>
    /// <param name="dashLength">The length of every segment and gap pair along the line.</param>
    public DisplayLineDashed(float startX, float startY, float endX, float endY, float lineWeight, float dashLength)
        : this(new Line(startX, startY, endX, endY), lineWeight, dashLength) { }

    public override void Render(Batcher batcher) {
        batcher.LineDashed(Line, LineWeight, Color, DashLength, OffsetPercent);
    }
}