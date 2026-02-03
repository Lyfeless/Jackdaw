using System.Numerics;
using Foster.Framework;

namespace Jackdaw;

public interface IDisplayObjectQuad {
    public Quad Quad { get; set; }
}

public class DisplayQuad(Quad quad)
: DisplayObject, IDisplayObjectQuad {
    public Quad Quad { get; set; } = quad;

    public override RectInt Bounds => Quad.Bounds.Int();

    public override void Render(Batcher batcher) {
        batcher.Quad(Quad, Color);
    }
}

public class DisplayQuadLines(Quad quad, float lineWeight)
    : DisplayObject, IDisplayObjectQuad, IDisplayObjectLines {
    public Quad Quad { get; set; } = quad;
    public float LineWeight { get; set; } = lineWeight;


    public override RectInt Bounds => Quad.Bounds.Int();

    public override void Render(Batcher batcher) {
        batcher.QuadLine(Quad, LineWeight, Color);
    }
}

public class DisplayQuadLinesDashed(Quad quad, float lineWeight, float dashLength)
    : DisplayObject, IDisplayObjectQuad, IDisplayObjectDashedLines {
    public Quad Quad { get; set; } = quad;
    public float LineWeight { get; set; } = lineWeight;
    public float DashLength { get; set; } = dashLength;
    public float OffsetPercent { get; set; } = 0;

    public override RectInt Bounds => Quad.Bounds.Int();

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