using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Display configuration for rendering polygon with 4 points.
/// </summary>
public interface IDisplayObjectQuad {
    /// <summary>
    /// The quad to render.
    /// </summary>
    public Quad Quad { get; set; }
}

/// <summary>
/// A renderable filled quad.
/// </summary>
/// <param name="quad">The quad to render.</param>
public class DisplayQuad(Quad quad)
: DisplayObject, IDisplayObjectQuad {
    public Quad Quad { get; set; } = quad;

    public override RectInt Bounds => Quad.Bounds.Int();

    /// <summary>
    /// A renderable filled quad.
    /// </summary>
    /// <param name="a">The quad's first point.</param>
    /// <param name="b">The quad's second point.</param>
    /// <param name="c">The quad's third point.</param>
    /// <param name="d">The quad's fourth point.</param>
    public DisplayQuad(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        : this(new(a, b, c, d)) { }

    /// <summary>
    /// A renderable filled quad.
    /// </summary>
    /// <param name="a">The quad's first point.</param>
    /// <param name="b">The quad's second point.</param>
    /// <param name="c">The quad's third point.</param>
    /// <param name="d">The quad's fourth point.</param>
    public DisplayQuad(Point2 a, Point2 b, Point2 c, Point2 d)
        : this(new(a, b, c, d)) { }

    /// <summary>
    /// A renderable filled quad.
    /// </summary>
    /// <param name="ax">The x position of the quad's first point.</param>
    /// <param name="ay">The y position of the quad's first point.</param>
    /// <param name="bx">The x position of the quad's second point.</param>
    /// <param name="by">The y position of the quad's second point.</param>
    /// <param name="cx">The x position of the quad's third point.</param>
    /// <param name="cy">The y position of the quad's third point.</param>
    /// <param name="dx">The x position of the quad's fourth point.</param>
    /// <param name="dy">The y position of the quad's fourth point.</param>
    public DisplayQuad(float ax, float ay, float bx, float by, float cx, float cy, float dx, float dy)
        : this(new(new(ax, ay), new(bx, by), new(cx, cy), new(dx, dy))) { }

    public override void Render(Batcher batcher) {
        batcher.Quad(Quad, Color);
    }
}

/// <summary>
/// A renderable quad outline.
/// </summary>
/// <param name="quad">The quad to render.</param>
/// <param name="lineWeight">The width of the rendered lines.</param>
public class DisplayQuadLines(Quad quad, float lineWeight)
    : DisplayObject, IDisplayObjectQuad, IDisplayObjectOutline {
    public Quad Quad { get; set; } = quad;
    public float LineWeight { get; set; } = lineWeight;

    public override RectInt Bounds => Quad.Bounds.Int();

    /// <summary>
    /// A renderable quad outline.
    /// </summary>
    /// <param name="a">The quad's first point.</param>
    /// <param name="b">The quad's second point.</param>
    /// <param name="c">The quad's third point.</param>
    /// <param name="d">The quad's fourth point.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    public DisplayQuadLines(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float lineWeight)
        : this(new(a, b, c, d), lineWeight) { }

    /// <summary>
    /// A renderable quad outline.
    /// </summary>
    /// <param name="a">The quad's first point.</param>
    /// <param name="b">The quad's second point.</param>
    /// <param name="c">The quad's third point.</param>
    /// <param name="d">The quad's fourth point.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    public DisplayQuadLines(Point2 a, Point2 b, Point2 c, Point2 d, float lineWeight)
        : this(new(a, b, c, d), lineWeight) { }

    /// <summary>
    /// A renderable quad outline.
    /// </summary>
    /// <param name="ax">The x position of the quad's first point.</param>
    /// <param name="ay">The y position of the quad's first point.</param>
    /// <param name="bx">The x position of the quad's second point.</param>
    /// <param name="by">The y position of the quad's second point.</param>
    /// <param name="cx">The x position of the quad's third point.</param>
    /// <param name="cy">The y position of the quad's third point.</param>
    /// <param name="dx">The x position of the quad's fourth point.</param>
    /// <param name="dy">The y position of the quad's fourth point.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    public DisplayQuadLines(float ax, float ay, float bx, float by, float cx, float cy, float dx, float dy, float lineWeight)
        : this(new(new(ax, ay), new(bx, by), new(cx, cy), new(dx, dy)), lineWeight) { }

    public override void Render(Batcher batcher) {
        batcher.QuadLine(Quad, LineWeight, Color);
    }
}

/// <summary>
/// A renderable dashed quad outline.
/// </summary>
/// <param name="quad">The quad to render.</param>
/// <param name="lineWeight">The width of the rendered lines.</param>
/// <param name="dashLength">The length of every segment and gap pair along the line.</param>
public class DisplayQuadLinesDashed(Quad quad, float lineWeight, float dashLength)
    : DisplayObject, IDisplayObjectQuad, IDisplayObjectDashedOutline {
    public Quad Quad { get; set; } = quad;
    public float LineWeight { get; set; } = lineWeight;
    public float DashLength { get; set; } = dashLength;
    public float OffsetPercent { get; set; } = 0;

    public override RectInt Bounds => Quad.Bounds.Int();

    /// <summary>
    /// A renderable dashed quad outline.
    /// </summary>
    /// <param name="a">The quad's first point.</param>
    /// <param name="b">The quad's second point.</param>
    /// <param name="c">The quad's third point.</param>
    /// <param name="d">The quad's fourth point.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    /// <param name="dashLength">The length of every segment and gap pair along the line.</param>
    public DisplayQuadLinesDashed(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float lineWeight, float dashLength)
        : this(new(a, b, c, d), lineWeight, dashLength) { }

    /// <summary>
    /// A renderable dashed quad outline.
    /// </summary>
    /// <param name="a">The quad's first point.</param>
    /// <param name="b">The quad's second point.</param>
    /// <param name="c">The quad's third point.</param>
    /// <param name="d">The quad's fourth point.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    /// <param name="dashLength">The length of every segment and gap pair along the line.</param>
    public DisplayQuadLinesDashed(Point2 a, Point2 b, Point2 c, Point2 d, float lineWeight, float dashLength)
        : this(new(a, b, c, d), lineWeight, dashLength) { }

    /// <summary>
    /// A renderable dashed quad outline.
    /// </summary>
    /// <param name="ax">The x position of the quad's first point.</param>
    /// <param name="ay">The y position of the quad's first point.</param>
    /// <param name="bx">The x position of the quad's second point.</param>
    /// <param name="by">The y position of the quad's second point.</param>
    /// <param name="cx">The x position of the quad's third point.</param>
    /// <param name="cy">The y position of the quad's third point.</param>
    /// <param name="dx">The x position of the quad's fourth point.</param>
    /// <param name="dy">The y position of the quad's fourth point.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    /// <param name="dashLength">The length of every segment and gap pair along the line.</param>
    public DisplayQuadLinesDashed(float ax, float ay, float bx, float by, float cx, float cy, float dx, float dy, float lineWeight, float dashLength)
        : this(new(new(ax, ay), new(bx, by), new(cx, cy), new(dx, dy)), lineWeight, dashLength) { }

    public override void Render(Batcher batcher) {
        Vector2 center = Quad.Center;
        float halfWeight = LineWeight / 2;
        Vector2 scaledA = ScalePoint(Quad.A - center, halfWeight);
        Vector2 scaledB = ScalePoint(Quad.B - center, halfWeight);
        Vector2 scaledC = ScalePoint(Quad.C - center, halfWeight);
        Vector2 scaledD = ScalePoint(Quad.D - center, halfWeight);
        batcher.LineDashed(scaledA, scaledB, LineWeight, Color, DashLength, OffsetPercent);
        batcher.LineDashed(scaledB, scaledC, LineWeight, Color, DashLength, OffsetPercent);
        batcher.LineDashed(scaledC, scaledD, LineWeight, Color, DashLength, OffsetPercent);
        batcher.LineDashed(scaledD, scaledA, LineWeight, Color, DashLength, OffsetPercent);
    }

    static Vector2 ScalePoint(Vector2 point, float amount) {
        float length = point.Length();
        float lengthReduced = MathF.Max(length - amount, 0);
        float scale = lengthReduced / length;
        return point * scale;
    }
}