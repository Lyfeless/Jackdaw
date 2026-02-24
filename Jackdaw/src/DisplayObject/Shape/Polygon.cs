using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Display configuration for rendering a polygon.
/// </summary>
public interface IDisplayObjectPolygon {
    /// <summary>
    /// The polygon to render.
    /// </summary>
    public Polygon Polygon { get; set; }
}

/// <summary>
/// A renderable filled polygon.
/// </summary>
/// <param name="polygon">The polygon to render.</param>
public class DisplayPolygon(Polygon polygon)
    : DisplayObject, IDisplayObjectPolygon {
    public Polygon Polygon { get; set; } = polygon;

    public override RectInt Bounds => Polygon.Bounds.Int();

    /// <summary>
    /// A renderable filled polygon.
    /// </summary>
    /// <param name="points">The points making up the polygon.</param>
    public DisplayPolygon(Vector2[] points)
        : this(new Polygon(points)) { }

    /// <summary>
    /// A renderable filled polygon.
    /// </summary>
    /// <param name="points">The points making up the polygon.</param>
    public DisplayPolygon(Point2[] points)
        : this(new Polygon([.. points.Select(e => (Vector2)e)])) { }

    public override void Render(Batcher batcher) {
        TriangulationEnumerator triangleEnumeration = Polygon.Triangles.GetEnumerator();
        while (triangleEnumeration.MoveNext()) {
            batcher.Triangle(triangleEnumeration.Current, Color);
        }
    }
}

/// <summary>
/// A renderable polygon outline.
/// </summary>
/// <param name="polygon">The polygon to render.</param>
/// <param name="lineWeight">The width of the rendered lines.</param>
public class DisplayPolygonLines(Polygon polygon, float lineWeight)
    : DisplayObject, IDisplayObjectPolygon, IDisplayObjectOutline {
    public Polygon Polygon { get; set; } = polygon;
    public float LineWeight { get; set; } = lineWeight;

    public override RectInt Bounds => Polygon.Bounds.Int();

    /// <summary>
    /// A renderable polygon outline.
    /// </summary>
    /// <param name="points">The points making up the polygon.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    public DisplayPolygonLines(Vector2[] points, float lineWeight)
        : this(new Polygon(points), lineWeight) { }

    /// <summary>
    /// A renderable polygon outline.
    /// </summary>
    /// <param name="points">The points making up the polygon.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    public DisplayPolygonLines(Point2[] points, float lineWeight)
        : this(new Polygon([.. points.Select(e => (Vector2)e)]), lineWeight) { }

    public override void Render(Batcher batcher) {
        Polygon.LineEnumerator lineEnumeration = Polygon.Edges.GetEnumerator();
        while (lineEnumeration.MoveNext()) {
            batcher.Line(lineEnumeration.Current, LineWeight, Color);
        }
    }
}

/// <summary>
/// A renderable dashed polygon outline.
/// </summary>
/// <param name="polygon">The polygon to render.</param>
/// <param name="lineWeight">The width of the rendered lines.</param>
/// <param name="dashLength">The length of every segment and gap pair along the line.</param>
public class DisplayPolygonLinesDashed(Polygon polygon, float lineWeight, float dashLength)
    : DisplayObject, IDisplayObjectPolygon, IDisplayObjectDashedOutline {
    public Polygon Polygon { get; set; } = polygon;
    public float LineWeight { get; set; } = lineWeight;
    public float DashLength { get; set; } = dashLength;
    public float OffsetPercent { get; set; } = 0;

    public override RectInt Bounds => Polygon.Bounds.Int();

    /// <summary>
    /// A renderable dashed polygon outline.
    /// </summary>
    /// <param name="points">The points making up the polygon.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    /// <param name="dashLength">The length of every segment and gap pair along the line.</param>
    public DisplayPolygonLinesDashed(Vector2[] points, float lineWeight, float dashLength)
        : this(new Polygon(points), lineWeight, dashLength) { }

    /// <summary>
    /// A renderable dashed polygon outline.
    /// </summary>
    /// <param name="points">The points making up the polygon.</param>
    /// <param name="lineWeight">The width of the rendered lines.</param>
    /// <param name="dashLength">The length of every segment and gap pair along the line.</param>
    public DisplayPolygonLinesDashed(Point2[] points, float lineWeight, float dashLength)
        : this(new Polygon([.. points.Select(e => (Vector2)e)]), lineWeight, dashLength) { }

    public override void Render(Batcher batcher) {
        if (Polygon.Count < 2) { return; }
        Vector2 center = Polygon.Center;
        float halfWeight = LineWeight / 2;
        for (int i = 0; i < Polygon.Count; ++i) {
            Vector2 a = Polygon[i];
            Vector2 b = Polygon[(i + 1) % Polygon.Count];
            Vector2 scaledA = ScalePoint(a - center, halfWeight);
            Vector2 scaledB = ScalePoint(b - center, halfWeight);
            batcher.LineDashed(scaledA, scaledB, LineWeight, Color, DashLength, OffsetPercent);
        }
    }

    static Vector2 ScalePoint(Vector2 point, float amount) {
        float length = point.Length();
        float lengthReduced = MathF.Max(length - amount, 0);
        float scale = lengthReduced / length;
        return point * scale;
    }
}