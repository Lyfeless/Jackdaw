using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

public interface IDisplayObjectPolygon {
    public Polygon Polygon { get; set; }
}

public class DisplayPolygon(Polygon polygon)
    : DisplayObject, IDisplayObjectPolygon {
    public Polygon Polygon { get; set; } = polygon;

    public override RectInt Bounds => Polygon.Bounds.Int();

    public override void Render(Batcher batcher) {
        TriangulationEnumerator triangleEnumeration = Polygon.Triangles.GetEnumerator();
        while (triangleEnumeration.MoveNext()) {
            batcher.Triangle(triangleEnumeration.Current, Color);
        }
    }
}

public class DisplayPolygonLines(Polygon polygon, float lineWeight)
    : DisplayObject, IDisplayObjectPolygon, IDisplayObjectLines {
    public Polygon Polygon { get; set; } = polygon;
    public float LineWeight { get; set; } = lineWeight;

    public override RectInt Bounds => Polygon.Bounds.Int();

    public override void Render(Batcher batcher) {
        Polygon.LineEnumerator lineEnumeration = Polygon.Edges.GetEnumerator();
        while (lineEnumeration.MoveNext()) {
            batcher.Line(lineEnumeration.Current, LineWeight, Color);
        }
    }
}

public class DisplayPolygonLinesDashed(Polygon polygon, float lineWeight, float dashLength)
    : DisplayObject, IDisplayObjectPolygon, IDisplayObjectDashedLines {
    public Polygon Polygon { get; set; } = polygon;
    public float LineWeight { get; set; } = lineWeight;
    public float DashLength { get; set; } = dashLength;
    public float OffsetPercent { get; set; } = 0;

    public override RectInt Bounds => Polygon.Bounds.Int();

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