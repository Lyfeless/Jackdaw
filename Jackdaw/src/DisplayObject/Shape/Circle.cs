using Foster.Framework;

namespace Jackdaw;

public interface IDisplayObjectCircle {
    public Circle Circle { get; set; }
    public int Resolution { get; set; }
}

public interface IDisplayObjectSemiCircle : IDisplayObjectCircle {
    public float StartRadians { get; set; }
    public float EndRadians { get; set; }
}

public class DisplayCircle(Circle circle)
    : DisplayObject, IDisplayObjectCircle {
    public Circle Circle { get; set; } = circle;
    public int Resolution { get; set; } = 16;

    public override RectInt Bounds => Circle.Bounds.Int();

    public override void Render(Batcher batcher) {
        batcher.Circle(Circle, Resolution, Color);
    }
}

public class DisplayCircleLines(Circle circle, float lineWeight)
    : DisplayObject, IDisplayObjectCircle, IDisplayObjectLines {
    public Circle Circle { get; set; } = circle;
    public int Resolution { get; set; } = 16;
    public float LineWeight { get; set; } = lineWeight;

    public override RectInt Bounds => Circle.Bounds.Int();

    public override void Render(Batcher batcher) {
        batcher.CircleLine(Circle, LineWeight, Resolution, Color);
    }
}

public class DisplayCircleLinesDashed(Circle circle, float lineWeight, float dashLength)
    : DisplayObject, IDisplayObjectCircle, IDisplayObjectDashedLines {
    public Circle Circle { get; set; } = circle;
    public int Resolution { get; set; } = 16;
    public float LineWeight { get; set; } = lineWeight;
    public float DashLength { get; set; } = dashLength;
    public float OffsetPercent { get; set; } = 0;

    public override RectInt Bounds => Circle.Bounds.Int();

    public override void Render(Batcher batcher) {
        batcher.CircleDashed(Circle, LineWeight, Resolution, Color, DashLength, OffsetPercent);
    }
}

public class DisplaySemiCircle(Circle circle, float startRadians, float endRadians)
    : DisplayObject, IDisplayObjectSemiCircle, IDisplayObjectCircle {
    public Circle Circle { get; set; } = circle;
    public int Resolution { get; set; } = 16;
    public float StartRadians { get; set; } = startRadians;
    public float EndRadians { get; set; } = endRadians;

    public override RectInt Bounds => Circle.Bounds.Int();

    public override void Render(Batcher batcher) {
        batcher.SemiCircle(Circle.Position, StartRadians, EndRadians, Circle.Radius, Resolution, Color);
    }
}

public class DisplaySemiCircleLines(Circle circle, float startRadians, float endRadians, float lineWeight)
    : DisplayObject, IDisplayObjectSemiCircle, IDisplayObjectLines {
    public Circle Circle { get; set; } = circle;
    public int Resolution { get; set; } = 16;
    public float StartRadians { get; set; } = startRadians;
    public float EndRadians { get; set; } = endRadians;
    public float LineWeight { get; set; } = lineWeight;

    public override RectInt Bounds => Circle.Bounds.Int();

    public override void Render(Batcher batcher) {
        batcher.SemiCircleLine(Circle.Position, StartRadians, EndRadians, Circle.Radius, Resolution, LineWeight, Color);
    }
}