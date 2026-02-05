using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

public interface IDisplayObjectTriangle {
    public Triangle Triangle { get; set; }
}

public class DisplayTriangle(Triangle triangle)
    : DisplayObject, IDisplayObjectTriangle {
    public Triangle Triangle { get; set; } = triangle;

    public override RectInt Bounds => Triangle.Bounds.Int();

    public override void Render(Batcher batcher) {
        batcher.Triangle(Triangle, Color);
    }
}

public class DisplayTriangleLines(Triangle triangle, float lineWeight)
    : DisplayObject, IDisplayObjectTriangle, IDisplayObjectLines {
    public Triangle Triangle { get; set; } = triangle;
    public float LineWeight { get; set; } = lineWeight;

    public override RectInt Bounds => Triangle.Bounds.Int();

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

public class DisplayTriangleLinesDashed(Triangle triangle, float lineWeight, float dashLength)
    : DisplayObject, IDisplayObjectTriangle, IDisplayObjectLines, IDisplayObjectDashedLines {
    public Triangle Triangle { get; set; } = triangle;
    public float LineWeight { get; set; } = lineWeight;
    public float DashLength { get; set; } = dashLength;
    public float OffsetPercent { get; set; } = 0;

    public override RectInt Bounds => Triangle.Bounds.Int();

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