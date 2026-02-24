using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Display configuration for rendering a triangle.
/// </summary>
public interface IDisplayObjectTriangle {
    /// <summary>
    /// The triangle to render.
    /// </summary>
    public Triangle Triangle { get; set; }
}

/// <summary>
/// A renderable filled triangle.
/// </summary>
/// <param name="triangle">The triangle to render.</param>
public class DisplayTriangle(Triangle triangle)
    : DisplayObject, IDisplayObjectTriangle {
    public Triangle Triangle { get; set; } = triangle;

    public override RectInt Bounds => Triangle.Bounds.Int();

    /// <summary>
    /// A renderable filled triangle.
    /// </summary>
    /// <param name="a">The triangle's first point.</param>
    /// <param name="b">The triangle's second point.</param>
    /// <param name="c">The triangle's third point.</param>
    public DisplayTriangle(Vector2 a, Vector2 b, Vector2 c)
        : this(new(a, b, c)) { }

    /// <summary>
    /// A renderable filled triangle.
    /// </summary>
    /// <param name="ax">The x position of the triangle's first point.</param>
    /// <param name="ay">The y position of the triangle's first point.</param>
    /// <param name="bx">The x position of the triangle's second point.</param>
    /// <param name="by">The y position of the triangle's second point.</param>
    /// <param name="cx">The x position of the triangle's third point.</param>
    /// <param name="cy">The y position of the triangle's third point.</param>
    public DisplayTriangle(float ax, float ay, float bx, float by, float cx, float cy)
        : this(new(ax, ay, bx, by, cx, cy)) { }

    public override void Render(Batcher batcher) {
        batcher.Triangle(Triangle, Color);
    }
}

/// <summary>
/// A renderable triangle outline.
/// </summary>
/// <param name="triangle">The triangle to render.</param>
/// <param name="lineWeight">The width of the rendered lines.</param>
public class DisplayTriangleLines(Triangle triangle, float lineWeight)
    : DisplayObject, IDisplayObjectTriangle, IDisplayObjectOutline {
    public Triangle Triangle { get; set; } = triangle;
    public float LineWeight { get; set; } = lineWeight;

    public override RectInt Bounds => Triangle.Bounds.Int();

    /// <summary>
    /// A renderable triangle outline.
    /// </summary>
    /// <param name="a">The triangle's first point.</param>
    /// <param name="b">The triangle's second point.</param>
    /// <param name="c">The triangle's third point.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    public DisplayTriangleLines(Vector2 a, Vector2 b, Vector2 c, float lineWeight)
        : this(new(a, b, c), lineWeight) { }

    /// <summary>
    /// A renderable triangle outline.
    /// </summary>
    /// <param name="ax">The x position of the triangle's first point.</param>
    /// <param name="ay">The y position of the triangle's first point.</param>
    /// <param name="bx">The x position of the triangle's second point.</param>
    /// <param name="by">The y position of the triangle's second point.</param>
    /// <param name="cx">The x position of the triangle's third point.</param>
    /// <param name="cy">The y position of the triangle's third point.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    public DisplayTriangleLines(float ax, float ay, float bx, float by, float cx, float cy, float lineWeight)
        : this(new(ax, ay, bx, by, cx, cy), lineWeight) { }

    public override void Render(Batcher batcher) {
        Vector2 center = Triangle.Center;
        float halfWeight = LineWeight / 2;
        Vector2 scaledA = ScalePoint(Triangle.A - center, halfWeight) + center;
        Vector2 scaledB = ScalePoint(Triangle.B - center, halfWeight) + center;
        Vector2 scaledC = ScalePoint(Triangle.C - center, halfWeight) + center;
        batcher.Line(scaledA, scaledB, LineWeight, Color);
        batcher.Line(scaledB, scaledC, LineWeight, Color);
        batcher.Line(scaledC, scaledA, LineWeight, Color);
    }

    static Vector2 ScalePoint(Vector2 point, float amount) {
        float length = point.Length();
        float lengthReduced = MathF.Max(length - amount, 0);
        float scale = lengthReduced / length;
        return point * scale;
    }
}

/// <summary>
/// A renderable dashed triangle outline.
/// </summary>
/// <param name="triangle">The triangle to render.</param>
/// <param name="lineWeight">The width of the rendered lines.</param>
/// <param name="dashLength">The length of every segment and gap pair along the line.</param>
public class DisplayTriangleLinesDashed(Triangle triangle, float lineWeight, float dashLength)
    : DisplayObject, IDisplayObjectTriangle, IDisplayObjectOutline, IDisplayObjectDashedOutline {
    public Triangle Triangle { get; set; } = triangle;
    public float LineWeight { get; set; } = lineWeight;
    public float DashLength { get; set; } = dashLength;
    public float OffsetPercent { get; set; } = 0;

    public override RectInt Bounds => Triangle.Bounds.Int();

    /// <summary>
    /// A renderable dashed triangle outline.
    /// </summary>
    /// <param name="a">The triangle's first point.</param>
    /// <param name="b">The triangle's second point.</param>
    /// <param name="c">The triangle's third point.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    /// <param name="dashLength">The length of every segment and gap pair along the line.</param>
    public DisplayTriangleLinesDashed(Vector2 a, Vector2 b, Vector2 c, float lineWeight, float dashLength)
        : this(new(a, b, c), lineWeight, dashLength) { }

    /// <summary>
    /// A renderable dashed triangle outline.
    /// </summary>
    /// <param name="ax">The x position of the triangle's first point.</param>
    /// <param name="ay">The y position of the triangle's first point.</param>
    /// <param name="bx">The x position of the triangle's second point.</param>
    /// <param name="by">The y position of the triangle's second point.</param>
    /// <param name="cx">The x position of the triangle's third point.</param>
    /// <param name="cy">The y position of the triangle's third point.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    /// <param name="dashLength">The length of every segment and gap pair along the line.</param>
    public DisplayTriangleLinesDashed(float ax, float ay, float bx, float by, float cx, float cy, float lineWeight, float dashLength)
        : this(new(ax, ay, bx, by, cx, cy), lineWeight, dashLength) { }

    public override void Render(Batcher batcher) {
        Vector2 center = Triangle.Center;
        float halfWeight = LineWeight / 2;
        Vector2 scaledA = ScalePoint(Triangle.A - center, halfWeight) + center;
        Vector2 scaledB = ScalePoint(Triangle.B - center, halfWeight) + center;
        Vector2 scaledC = ScalePoint(Triangle.C - center, halfWeight) + center;
        batcher.LineDashed(scaledA, scaledB, LineWeight, Color, DashLength, OffsetPercent);
        batcher.LineDashed(scaledB, scaledC, LineWeight, Color, DashLength, OffsetPercent);
        batcher.LineDashed(scaledC, scaledA, LineWeight, Color, DashLength, OffsetPercent);
    }

    static Vector2 ScalePoint(Vector2 point, float amount) {
        float length = point.Length();
        float lengthReduced = MathF.Max(length - amount, 0);
        float scale = lengthReduced / length;
        return point * scale;
    }
}