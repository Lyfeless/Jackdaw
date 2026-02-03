using Foster.Framework;

namespace Jackdaw;

public class DisplayLine(Line line, float lineWeight)
    : DisplayObject, IDisplayObjectLines {
    public Line Line = line;
    public float LineWeight { get; set; } = lineWeight;

    public override RectInt Bounds => Line.Bounds.Int();

    public override void Render(Batcher batcher) {
        batcher.Line(Line, LineWeight, Color);
    }
}

public class DisplayLineDashed(Line line, float lineWeight, float dashLength)
    : DisplayObject, IDisplayObjectDashedLines {
    public Line Line = line;
    public float LineWeight { get; set; } = lineWeight;
    public float DashLength { get; set; } = dashLength;
    public float OffsetPercent { get; set; } = 0;

    public override RectInt Bounds => Line.Bounds.Int();

    public override void Render(Batcher batcher) {
        batcher.LineDashed(Line, LineWeight, Color, DashLength, OffsetPercent);
    }
}